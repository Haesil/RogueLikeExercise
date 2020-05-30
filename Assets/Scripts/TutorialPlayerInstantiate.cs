using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerInstantiate : MonoBehaviour
{
    // 튜토리얼에서 플레이어를 소환하는 클래스
    // UI 호출 함수가 포함됨

    public GameObject player;
    public GameManager m_manager;
    // Start is called before the first frame update
    void Start()
    {
        m_manager = FindObjectOfType<GameManager>();
        GameObject obj = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity);
        obj.name = "Player";
        obj.GetComponent<Player>().m_currentHP = m_manager.playerCurHP;
        obj.GetComponent<Player>().m_fullHP = m_manager.playerFullHP;
        GameObject.Find("Main Camera").SendMessage("TargetOn");
        FindObjectOfType<Monster>().SetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void QuitUIOpen()
    {
        FindObjectOfType<GameManager>().QuitUIOpen();
    }

    public void QuitUIClose()
    {
        FindObjectOfType<GameManager>().QuitUIClose();
    }

    public void GameQuit()
    {
        FindObjectOfType<GameManager>().GameQuit();
    }
}
