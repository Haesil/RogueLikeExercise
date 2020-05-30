using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 플레이어의 정보와 행동을 관리하는 클래스
    // wasd를 이용하여 움직이고 뒤 키를 누를 경우 뒷걸음질 쳐야함.
    // 대쉬 기능이 있어야함.
    // 필요 경험치를 얻을 경우 레벨업 해야함
    // 필요한 멤버 변수
    //  - 다음 씬이 로드될 경우 현재의 플레이어의 정보를 유지하기 위해 정보를 전달할 게임 매니저 오브젝트 변수
    //  - 애니메이션을 작동시키기 위한 애니메이터 오브젝트 변수
    //  - 현재 체력, 가득 찬 체력
    //  - 현재 경험치 량, 레벨업을 위해 필요한 경험치량을 저장할 변수
    // 구현할 함수
    //  - 키보드에서 입력받은 만큼 이동하는 함수 (이동할 Vector3을 애니메이터에 전달)
    //  - 피격 함수
    //  - 공격 함수
    //  - 사망 함수

    public GameManager m_manager;
    public GameObject m_equip;
    public int m_currentHP;
    public int m_fullHP;
    public int playerLevel = 1;
    public int exp;
    public int maxExp;
    public bool m_move;

    private Animator m_animator;
    private Rigidbody m_rigidbody;
    private bool m_dash;
    private bool m_attack;
    private bool m_hit;
    private bool m_alive;
    private float m_turnAmount;
    private Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        m_manager = FindObjectOfType<GameManager>();
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_dash = true;
        m_attack = true;
        m_hit = true;
        m_alive = true;
        m_move = true;
        playerLevel = m_manager.playerLevel;
        exp = m_manager.exp;
        maxExp = 90 + playerLevel * 10;
    }

    // Update is called once per frame
    void Update()
    {
        float h = 0;
        float v = 0;
        if (m_move)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        if(exp >= maxExp)
        {
            playerLevel++;
            exp = exp - maxExp;
            m_fullHP += 50;
            m_currentHP = m_fullHP;
            m_manager.playerCurHP = m_currentHP;
            m_manager.playerFullHP = m_fullHP;
            m_manager.playerLevel = playerLevel;
            m_manager.exp = exp;
        }

        moveDir = v * Vector3.forward + h * Vector3.right;
        if (moveDir.magnitude > 1f)
        {
            moveDir.Normalize();
        }

        if (!FindObjectOfType<Inventory>().showInventory && m_attack && Input.GetButtonDown("Attack"))
        {
            // 인벤토리를 켰을 때 공격이 불가능하도록 함
            m_attack = false;
            StartCoroutine(CoolDown(0.4f, 0));
            Attack();
        }

        if(!FindObjectOfType<Inventory>().showInventory && m_dash && Input.GetButtonDown("Dash"))
        {
            // 인벤토리를 켰을 때 대쉬가 불가능하도록 함
            m_dash = false;
            StartCoroutine(CoolDown(2.0f, 1));
            m_hit = false;
            StartCoroutine(CoolDown(0.5f, 2));
            m_rigidbody.AddForce(transform.forward * 5000f);
        }

        if(m_alive&&m_currentHP <= 0)
        {
            // 사망 판정
            m_alive = false;
            m_animator.SetBool("Alive", false);
            m_animator.SetTrigger("Dead");
            GameObject.Find("Canvas").transform.Find("Gameover").gameObject.SetActive(true);
        }
        
        Move();
    }

    public void Move()
    {
        // 이동 Vector를 애니메이터에 전달
        m_turnAmount = Mathf.Atan2(moveDir.x, moveDir.z);
        if (moveDir.z > 0)
        {
            m_animator.SetBool("BackMoving", false);
        }
        else if (moveDir.z < 0)
        {
            // 뒤로 이동시 뒤로 걷도록함
            m_animator.SetBool("BackMoving", true);
            transform.Rotate(Vector3.up * moveDir.x * 80.0f * Time.deltaTime);
        }
        else
        {
            m_animator.SetBool("BackMoving", false);
        }
        m_animator.SetFloat("Forward", moveDir.z, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
    }
    
    
    public void Heal(int hp)
    {
        // 인벤토리에서 체력 회복약을 먹을 경우 체력을 회복하는 함수
        m_currentHP += hp;
        if (m_currentHP > m_fullHP)
            m_currentHP = m_fullHP;
        m_manager.playerCurHP = m_currentHP;
    }

    public void DeadFunction()
    {
        // 죽을 경우 안내 UI 띄움
        GameObject.Find("Canvas").transform.Find("Restart").gameObject.SetActive(true);
    }

    public void Attack()
    {
        // 공격 함수, 공격 애니메이션 수행할때 콜라이더를 켜는 함수
        m_animator.SetTrigger("Attack");
        m_equip.SendMessage("AttackTriggerOn");
    }

    public void AttackTriggerOff()
    {
        // 공격 애니메이션이 끝나고 콜라이더를 꺼주는 함수
        m_equip.SendMessage("AttackTriggerOff");
    }

    void BeHit(int damage)
    {
        // 피격 함수, 한번 피격하면 3초간 피격이 불가능하도록 코루틴으로 구현
        if(m_hit)
        {
            m_hit = false;
            StartCoroutine(CoolDown(3.0f, 2));
            m_animator.SetTrigger("Hit");
            m_currentHP -= damage;
            m_manager.playerCurHP = m_currentHP;
        }
    }

    void SetAttackSpeed(float speed)
    {
        // 공격 속도 변경
        m_animator.SetFloat("AttackSpeed", speed);
    }
    
    public void expUP(int e)
    {
        // 경험치를 획득하는 함수
        exp += e;
        m_manager.exp = exp;
    }

    IEnumerator CoolDown(float cooltime, int condition)
    {
        //쿨다운 코루틴
        while (cooltime > 0.0f)
        {
            cooltime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        switch (condition)
        {
            case 0:
                m_attack = true;
                break;
            case 1:
                m_dash = true;
                break;
            case 2:
                m_hit = true;
                break;
        }
    }

}
