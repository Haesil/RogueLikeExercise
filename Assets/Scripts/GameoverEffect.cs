using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameoverEffect : MonoBehaviour
{
    //게임 오버 화면을 연출하는 클래스
    Image img;
    float alpha;
    Color c;
    // Start is called before the first frame update
    void OnEnable()
    {
        img = GetComponent<Image>();
        alpha = 0;
        c = img.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (alpha <= 1.0f)
        {
            alpha += 0.005f;
            c.a = alpha;
            img.color = c;
        }
    }

    public void Restart()
    {
        FindObjectOfType<GameManager>().GameOver();
    }

    public void GameQuit()
    {
        FindObjectOfType<GameManager>().GameQuit();
    }
}
