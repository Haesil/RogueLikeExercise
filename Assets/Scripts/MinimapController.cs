using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    // 던전의 미니맵을 보여주는 카메라를 조정하는 클래스
    // 카메라의 위치를 player 오브젝트 위에 고정해두고 RenderTexture로 보내 Raw Image로 만들어둔 미니맵에 띄움

    public GameObject target;

    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        distance = 14.0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.transform.position + new Vector3(0, distance, 0);
    }

    public void TargetOn()
    {
        target = GameObject.Find("Player");
    }
}
