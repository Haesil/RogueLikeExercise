using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRange : MonoBehaviour
{
    // 보스의 광역 폭발 기술이 맞았는지 판정하는 클래스
    float timer = 0;
    bool isDamaged = false;
    private void OnEnable()
    {
        timer = 0;
        isDamaged = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.5f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isDamaged&&other.tag == "Player")
        {
            isDamaged = true;
            other.SendMessage("BeHit", 60);
        }
    }
    
}
