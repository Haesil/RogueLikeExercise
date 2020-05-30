using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMent : MonoBehaviour
{
    // 무기의 트리거 판정을 MonsterAttack 클래스나 Attack클래스에 전달하는 클래스


    CapsuleCollider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            transform.parent.GetComponent<Attack>().OnTriggerEnter(other);
        }
        else if(other.tag == "Player")
        {
            transform.parent.GetComponent<MonsterAttack>().OnTriggerEnter(other);
        }
    }
    public void AttackTriggerOn()
    {
        col.enabled = true;
    }
    public void AttackTriggerOff()
    {
        col.enabled = false;
    }
}
