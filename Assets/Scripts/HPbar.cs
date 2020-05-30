using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    // 보스의 체력을 슬라이더를 통하여 보여주는 클래스
    
    public Slider hp;
    public Boss m_boss;
    public bool isBoss;

    // Start is called before the first frame update
    void Start()
    {
        isBoss = false; 
    }

    private void OnEnable()
    {
        isBoss = false;
        hp.value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBoss)
        {
            //보스가 뒤돌때 체력바가 차오르는 연출
            if (hp.value >= 1.0f)
                isBoss = true;
            hp.value += 0.5f* Time.deltaTime;
        }
        else
        {
            // 보스와 체력 연동
            if (m_boss == null)
                m_boss = FindObjectOfType<Boss>();
            else
                hp.value = ((float)m_boss.m_currentHP / m_boss.m_fullHP);
        }
    }
}
