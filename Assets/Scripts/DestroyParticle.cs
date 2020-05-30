using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    //폭파 효과가 자동으로 삭제되도록 하는 클래스
    public float deadLine;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, deadLine);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
