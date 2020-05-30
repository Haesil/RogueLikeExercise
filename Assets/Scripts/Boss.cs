using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    // 보스의 정보와 행동을 관리하는 클래스
    // 플레이어를 자동으로 추적하고 따라다니며 일정 거리 안으로 다가올 경우 패턴을 이용하여 공격함.
    // 랜덤으로 두가지 패턴중 한가지를 선택하여 발동해야 함. (1번 패턴 45%, 2번패턴 55%)
    // 필요한 멤버 변수
    //  - 자동으로 플레이어를 추적하기위해 필요한 NavMeshAgent오브젝트 변수
    //  - 애니메이션을 작동시키기 위한 애니메이터 오브젝트 변수
    //  - 쫓아갈 타겟 오브젝트 변수
    //  - 폭발 패턴 범위를 표시하기 위한 게임오브젝트 변수와 폭발 데미지 처리를 위한 게임 오브젝트 변수
    //  - 현재 체력, 가득 찬 체력
    //  - 쫓아갈 거리, 공격 거리, 일정 이상 다가갔을때 멈출 거리
    //  - 보스의 패턴 발동 타이밍을 계산할 timer로 사용할 변수
    // 구현할 함수
    //  - 몬스터가 이동하는 함수 (이동할 Vector3을 애니메이터에 전달)
    //  - 피격 함수
    //  - 폭발 공격 함수
    //  - 불화살 공격 함수
    //  - 사망 함수 (사망 후 포탈 오픈)


    enum state { idle, boom, firearrow }
    state curState;
    int arrowNum;
    float timer;
    bool m_alive = true;
    bool m_attack = false;
    bool m_move = true;
    bool isHPbar = false;
    GameObject cylinder;
    GameObject damage;
    GameObject m_target;
    Vector3 moveDir;
    float m_turnAmount;
    NavMeshAgent m_agent;
    Animator m_animator;
    ObjectController objController;

    public int m_currentHP;
    public int m_fullHP;
    public float m_traceDistance;
    public float m_stopDistance;
    public GameObject portal;
    public GameObject m_shotPrefab;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        curState = state.idle;
        arrowNum = 0;
        timer = 0.0f;
        damage = transform.Find("Damage").gameObject;
        cylinder = transform.Find("Cylinder").gameObject;
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        objController = FindObjectOfType<ObjectController>();

        m_traceDistance = 30.0f;
        m_stopDistance = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_alive && m_currentHP <= 0)
        {
            // 사망 판정
            m_alive = false;
            m_animator.SetBool("Alive", false);
            m_animator.SetTrigger("Dead");
            if (isHPbar)
            {
                GameObject.Find("Canvas").transform.Find("BossHPBar").gameObject.SetActive(false);
                isHPbar = false;
            }
        }
        if (m_alive && m_target != null)
        {
            // 타겟이 정해져 있을때
            float distance = Vector3.Distance(m_target.transform.position, transform.position);

            RaycastHit hit;
            if (distance < m_traceDistance)
            {
                if(!isHPbar)
                {
                    GameObject.Find("Canvas").transform.Find("BossHPBar").gameObject.SetActive(true);
                    isHPbar = true;
                }
                // 타겟까지의 거리가 추격거리 안에 있을때
                if (distance <= m_stopDistance)
                {
                    m_move = false;
                    m_attack = true;
                }
                else if (Physics.Raycast(transform.position, (m_target.transform.position - transform.position).normalized, out hit, m_traceDistance))
                {
                    // 몬스터가 플레이어를 볼수 있는지 판정
                    if (hit.transform.name == "Player")
                    {
                        if (m_target != null)
                        {
                            //볼 수 있을때 몬스터의 목적지를 타겟의 위치로 설정
                            m_move = true;
                            m_attack = true;
                        }

                    }
                    else
                    {
                        // 못볼 경우 움직이지 않도록 함
                        m_move = false;
                        m_attack = false;
                    }

                }
            }
            else
            {
                // 추격거리 밖에 있을 경우
                m_move = false;
                m_attack = false;
            }
        }

        if (m_move)
        {
            m_agent.isStopped = false;
            m_agent.SetDestination(m_target.transform.position);
            moveDir = m_agent.desiredVelocity;
            Move();
        }
        else
        {
            m_agent.SetDestination(transform.position);
            moveDir = m_agent.desiredVelocity;
            Move();
            m_agent.isStopped = true;
        }
            
        

        if(m_alive && m_attack)
        {
            timer += Time.deltaTime;
            switch (curState)
            {
                case state.idle:
                    if (timer >= 4.0f)
                    {
                        int x = Random.Range(0, 100);
                        if (x < 45)
                        {
                            m_animator.SetTrigger("Explosion");
                            curState = state.boom;
                            cylinder.SetActive(true);
                        }
                        else
                        {
                            m_animator.SetTrigger("Firearrow");
                            curState = state.firearrow;
                        }
                        timer = 0;
                    }
                    break;
                case state.boom:
                    if (timer >= 2.0f)
                    {
                        Boom();
                        timer = 0;
                    }
                    break;
                case state.firearrow:
                    if (timer >= 0.5f)
                    {
                        //불화살 소환
                        arrowNum++;
                        Firearrow();
                        timer = 0;
                    }
                    if (arrowNum >= 5)
                    {
                        curState = state.idle;
                        arrowNum = 0;
                    }
                    break;
            }
        }
        

    }
    void Boom()
    {
        //폭발 이펙트 켜기
        GameObject obj = Instantiate(explosion, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        damage.SetActive(true);
        cylinder.SetActive(false);
        curState = state.idle;
    }

    void Firearrow()
    {
        GameObject firearrow = objController.PopObject(0);
        firearrow.SetActive(true);
        firearrow.transform.position = transform.position + new Vector3(0.0f, 12.0f, 0.0f);
        firearrow.transform.LookAt(m_target.transform);
        //GameObject firearrow = Instantiate(m_shotPrefab, transform.position + new Vector3(0.0f, 12.0f, 0.0f), Quaternion.identity);
        firearrow.transform.LookAt(m_target.transform);
        
    }

    void SetTarget()
    {
        m_target = GameObject.FindWithTag("Player");
    }

    public void Move()
    {
        // 이동 Vector3를 받아 애니메이터에 전달하는 함수
        moveDir = transform.InverseTransformDirection(moveDir);
        //m_turnAmount = moveDir.x;
        m_turnAmount = Mathf.Atan2(moveDir.x, moveDir.z);
        m_animator.SetFloat("Forward", moveDir.z, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
    }

    void BeHit(int damage)
    {
        // 피격함수
        m_currentHP -= damage;
    }

    public void DeadFunction()
    {
        Instantiate(portal, transform.position, Quaternion.identity);
        if (FindObjectOfType<ObjectController>()) // 오브젝트 풀에서 가져온 경우 반납
            FindObjectOfType<ObjectController>().PushObject(gameObject, 0);
        else // 아닌경우 파괴
            Destroy(gameObject);
    }

    
}
