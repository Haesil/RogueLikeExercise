using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColorChange : MonoBehaviour
{
    // 플레이어의 체력에 따라 벽의 색과 점멸속도가 변화하도록 하는 클래스
    public Player m_player;
    Renderer ren;
    float bright;
    public bool flag;
    int m_HP;

    // Start is called before the first frame update
    void Start()
    {
        ren = GetComponent<Renderer>();
        bright = 0;
        flag = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_player == null)
            m_player = FindObjectOfType<Player>();
        else
        {
            m_HP = (m_player.m_currentHP * 10) / m_player.m_fullHP;
            
            if (flag)
                bright -= Time.deltaTime / (m_HP+1) * 2;
            else
                bright += Time.deltaTime / (m_HP + 1) * 2;
            if (bright < -0.8)
                flag = false;
            if (bright > 0)
                flag = true;
            ren.material.SetFloat("_Bright", bright);
            switch (m_HP)
            {
                case 10:
                    ren.material.SetFloat("_Green", 0.8f);
                    ren.material.SetFloat("_Red", 0.2f);
                    break;
                case 9:
                    ren.material.SetFloat("_Green", 0.8f);
                    ren.material.SetFloat("_Red", 0.2f);
                    break;
                case 8:
                    ren.material.SetFloat("_Green", 0.6f);
                    ren.material.SetFloat("_Red", 0.2f);
                    break;
                case 7:
                    ren.material.SetFloat("_Green", 0.5f);
                    ren.material.SetFloat("_Red", 0.2f);
                    break;
                case 6:
                    ren.material.SetFloat("_Green", 0.5f);
                    ren.material.SetFloat("_Red", 0.5f);
                    break;
                case 5:
                    ren.material.SetFloat("_Green", 0.5f);
                    ren.material.SetFloat("_Red", 1.0f);
                    break;
                case 4:
                    ren.material.SetFloat("_Green", 0.4f);
                    ren.material.SetFloat("_Red", 1.0f);
                    break;
                case 3:
                    ren.material.SetFloat("_Green", 0.3f);
                    ren.material.SetFloat("_Red", 1.0f);
                    break;
                case 2:
                    ren.material.SetFloat("_Green", 0.2f);
                    ren.material.SetFloat("_Red", 1.0f);
                    break;
                case 1:
                    ren.material.SetFloat("_Green", 0.1f);
                    ren.material.SetFloat("_Red", 1.0f);
                    break;
                case 0:
                    ren.material.SetFloat("_Green", 0.0f);
                    ren.material.SetFloat("_Red", 1.0f);
                    break;
            }
        }
        
    }
}
