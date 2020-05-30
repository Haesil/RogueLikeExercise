using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tutorial : MonoBehaviour
{
    // 튜토리얼에서 플레이어가 일정한 위치에 도달했을 때 UI를 띄워 TypingEffect클래스를 통해 대사를 출력하는 클래스
    

    GameObject canvas;
   

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (gameObject.tag == "Tutorial1")
            {
                FindObjectOfType<TypingEffect>().isEvent = true;
                FindObjectOfType<Player>().m_move = false;
                canvas.transform.Find("Tutorial").Find("Image").gameObject.SetActive(true);
                FindObjectOfType<TypingEffect>().Tutorial1On();
                Destroy(gameObject);
            }
            else if (gameObject.tag == "Tutorial2")
            {
                FindObjectOfType<TypingEffect>().isEvent = true;
                FindObjectOfType<Player>().m_move = false;
                canvas.transform.Find("Tutorial").Find("Image").gameObject.SetActive(true);
                FindObjectOfType<TypingEffect>().Tutorial2On();
                Destroy(gameObject);
            }
            else if (gameObject.tag == "Tutorial3")
            {
                FindObjectOfType<TypingEffect>().isEvent = true;
                FindObjectOfType<Player>().m_move = false;
                canvas.transform.Find("Tutorial").Find("Image").gameObject.SetActive(true);
                FindObjectOfType<TypingEffect>().Tutorial3On();
                Destroy(gameObject);
            }
        }
    }
    
}
