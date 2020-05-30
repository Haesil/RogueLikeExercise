using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 카메라의 이동을 관리하는 클래스
    // 플레이어가 이동할 경우 카메라가 따라가야함
    // 플레이어가 회전할 경우 카메라가 플레이어를 기준으로 회전해야함.
    // 필요한 멤버 변수
    //  - 카메라가 바라볼 타겟 오브젝트 변수
    //  - 타겟과 떨어져 있을 오프셋 변수
    //  - 이동이 자연스럽게 느껴지도록 주어지는 딜레이 시간 변수

    public GameObject target;

    private float distance;
    private float height;
    private float DelayTime;
    

    // Start is called before the first frame update
    void Start()
    {
        
        distance = 4.0f;
        height = 2.8f;
        DelayTime = 5.0f;
        
    }
    
    void LateUpdate()
    {
        if (target != null)
        {
            
            float rotY = Mathf.LerpAngle(transform.eulerAngles.y, target.transform.eulerAngles.y, DelayTime * Time.deltaTime);

            Quaternion rot = Quaternion.Euler(0, rotY, 0);

            transform.position = target.transform.position - (rot * Vector3.forward * distance) + (Vector3.up * height);
            transform.LookAt(target.transform);
            transform.eulerAngles = new Vector3(5, transform.eulerAngles.y, transform.eulerAngles.z);
        }

    }

    void TargetOn()
    {
        target = GameObject.Find("Player");
    }
}
