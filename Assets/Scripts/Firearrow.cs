using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearrow : MonoBehaviour
{
    // 보스가 소환할 불화살을 구현한 클래스
    // 필요한 기능
    // - 생성시 주어진 방향으로 날아가야함.
    // - 플레이어에게 맞을 경우 데미지를 줌.
    // - 충돌 시 오브젝트 풀에 반납.
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 20f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.SendMessage("BeHit", 10);
        }
        FindObjectOfType<ObjectController>().PushObject(gameObject, 0);
    }
}
