using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{ 
    // 엔딩씬 대화창 뜨게하는 클래스

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<TypingEffect>().EndingOn();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
