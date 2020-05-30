using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    // 몬스터의 공격에 관련된 메서드를 관리하는 클래스
    // 몬스터가 공격할때 콜라이더를 켜고 꺼야함. (공격하지 않을때 무기에 닿을 경우 공격당하면 안됨.)
    // 현재 착용한 무기의 종류에 따라 공격 속도 달라짐.
    // 필요한 멤버 변수
    // - 현재 무기의 종류를 나타낼 enum 변수
    // - 무기 장착시 생성할 오브젝트 (무기의 종류가 적어 저장해둠. 종류가 많아질 경우 Resources폴더에서 불러올 계획)
    // - 장착과 해제할때 정보를 전달받거나 전달할 인벤토리 변수
    // - 공격력을 저장할 변수
    // 구현해야할 함수
    // - 공격 모션을 시작할때 무기나 손의 콜라이더를 켜주는 함수
    // - 공격 모션이 끝날때 무기나 손의 콜라이더를 꺼주는 함수
    // - 무기를 장착하는 함수
    // - 무기를 해제하는 함수

    public enum Weapon { hand, sword, club };
    public Weapon curWeapon = Weapon.hand;
    public GameObject m_monster;
    public GameObject m_sword;
    public GameObject m_club;
    public int m_attackPoint = 10;
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AttackTriggerOn()
    {
        
        if (curWeapon == Weapon.hand)
        {
            col.enabled = true;
        }
        else
        {
            gameObject.GetComponentInChildren<EquipMent>().AttackTriggerOn();
        }

    }

    void AttackTriggerOff()
    {
        if (curWeapon == Weapon.hand)
            col.enabled = false;
        else
        {
            gameObject.GetComponentInChildren<EquipMent>().AttackTriggerOn();
        }

    }

    public void WeaponChange(int weaponType)
    {
        curWeapon = Weapon.hand;
        curWeapon += weaponType;
        switch (weaponType)
        {
            case 0:
                m_monster.SendMessage("SetAttackSpeed", 1.0);
                break;
            case 1:
                m_monster.SendMessage("SetAttackSpeed", 0.8);
                m_sword.SetActive(true);
                break;
            case 2:
                m_monster.SendMessage("SetAttackSpeed", 0.5);
                m_club.SetActive(true);
                break;
        }
    }

    public void AttackPointChange(int attackPoint)
    {
        m_attackPoint = attackPoint;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.SendMessage("BeHit", m_attackPoint);
        }

    }
}
