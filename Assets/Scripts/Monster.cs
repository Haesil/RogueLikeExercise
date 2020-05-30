using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    // 몬스터의 정보와 행동을 관리하는 클래스
    // 플레이어를 자동으로 추적하고 따라다니며 일정 거리 안으로 다가올 경우 공격해야함.
    // 랜덤으로 무기와 공격력이 결정되어야 함.
    // 필요한 멤버 변수
    //  - 자동으로 플레이어를 추적하기위해 필요한 NavMeshAgent오브젝트 변수
    //  - 애니메이션을 작동시키기 위한 애니메이터 오브젝트 변수
    //  - 쫓아갈 타겟 오브젝트 변수
    //  - 랜덤으로 변할 장비 오브젝트 변수
    //  - 현재 체력, 가득 찬 체력
    //  - 쫓아갈 거리, 공격 거리, 일정 이상 다가갔을때 멈출 거리
    // 구현할 함수
    //  - 몬스터가 이동하는 함수 (이동할 Vector3을 애니메이터에 전달)
    //  - 피격 함수
    //  - 공격 함수
    //  - 사망 함수

    private NavMeshAgent m_agent;
    private Animator m_animator;
    private bool m_attack = true;
    private bool m_alive = true;
    
    private Vector3 moveDir;
    private float m_turnAmount;

    public GameObject m_target;
    public GameObject m_equip;
    public int m_currentHP;
    public int m_fullHP;
    public float m_traceDistance;
    public float m_stopDistance;
    public float m_attackDistance;

    // Start is called before the first frame update
    void Start()
    {
        int tmp = Random.Range(0, 100);
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        if (tmp < 60&& tmp > 10)
        {
            m_equip.SendMessage("WeaponChange", 1);
            m_equip.SendMessage("AttackPointChange", Random.Range(30, 40));
        }
        else if (tmp >= 60)
        {
            m_equip.SendMessage("WeaponChange", 2);
            m_equip.SendMessage("AttackPointChange", Random.Range(15, 23));
        }

        SetAttackSpeed(0.5f);

        m_traceDistance = 20.0f;
        m_stopDistance = 2.0f;
        m_attackDistance = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_alive&&m_currentHP <= 0)
        {
            // 사망 판정
            AttackTriggerOff();
            m_alive = false;
            m_animator.SetBool("Alive", false);
            m_animator.SetTrigger("Dead");
        }
        if (m_target != null)
        {
            // 타겟이 정해져 있을때
            float distance = Vector3.Distance(m_target.transform.position, transform.position);

            Vector3 direction = (m_target.transform.position - transform.position).normalized;
            RaycastHit hit;
            if (distance < m_traceDistance)
            {
                // 타겟까지의 거리가 추격거리 안에 있을때
                if (distance <= m_stopDistance)
                {
                    // 타겟이 가까이 있을때 정지하도록 하는 함수.
                    m_agent.SetDestination(transform.position);
                    if (m_attack && distance < m_attackDistance)
                    {
                        // 공격거리 이내에 존재할 경우 공격
                        // 공격이 2.5초마다 발생하도록 코루틴 사용
                        m_attack = false;
                        StartCoroutine(CoolDown(2.5f));
                        Attack();
                    }
                }
                else if (Physics.Raycast(transform.position, direction, out hit, m_traceDistance))
                {
                    // 몬스터가 플레이어를 볼수 있는지 판정
                    if (hit.transform.name == "Player")
                    {
                        if (m_target != null)
                        {
                            //볼 수 있을때 몬스터의 목적지를 타겟의 위치로 설정
                            m_agent.SetDestination(m_target.transform.position);
                        }
                        if (m_attack && distance < m_attackDistance)
                        {
                            // 공격거리 이내에 존재할 경우 공격
                            // 공격이 2.5초마다 발생하도록 코루틴 사용
                            m_attack = false;
                            StartCoroutine(CoolDown(2.5f));
                            Attack();
                        }
                    }
                    else
                    {
                        // 못볼 경우 움직이지 않도록 함
                        m_agent.SetDestination(transform.position);
                    }
                }
            }
            else
            {
                // 추격거리 밖에 있을 경우
                m_agent.SetDestination(transform.position);
            }
        }
        moveDir = m_agent.desiredVelocity;
        Move();
    }

    public void Attack()
    {
        // 공격 애니메이션 시작, 공격 판정을 위해 콜라이더를 켜는 함수
        m_animator.SetTrigger("Attack");
        m_equip.SendMessage("AttackTriggerOn");
    }

    public void AttackTriggerOff()
    {
        // 공격 모션이 끝날때 콜라이더를 끄는 함수
        m_equip.SendMessage("AttackTriggerOff");
    }

    void SetAttackSpeed(float speed)
    {
        //공격속도를 설정하는 함수
        if(m_animator != null)
            m_animator.SetFloat("AttackSpeed", speed);
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
        AttackTriggerOff();
        m_animator.SetTrigger("Hit");
        m_currentHP -= damage;
    }

    public void SetTarget()
    {
        // 타겟을 플레이어로 설정하는 함수
        m_target = GameObject.FindWithTag("Player");
    }
    
    public void DeadFunction()
    {
        // 사망시 동작
        FindObjectOfType<Player>().expUP(30);
        if (FindObjectOfType<ObjectController>()) // 오브젝트 풀에서 가져온 경우 반납
            FindObjectOfType<ObjectController>().PushObject(gameObject, 0);
        else // 아닌경우 파괴
            Destroy(gameObject);
    }

    IEnumerator CoolDown(float cooltime)
    {
        //쿨다운 코루틴
        while (cooltime > 1.0f)
        {
            cooltime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        m_attack = true;
    }

}
