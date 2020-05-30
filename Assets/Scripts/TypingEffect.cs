using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingEffect : MonoBehaviour
{
    // 튜토리얼과 엔딩에서 대사가 타이핑쳐지듯 출력되게하는 함수
    // Resources폴더에서 txt파일을 읽어들여 라인 별로 차례차례 출력해야함.
    // 필요한 멤버 변수
    // - 타이핑 효과가 주어질 문자열
    // - 읽어들은 텍스트 파일을 줄별로 구분하여 저장할 문자열 배열
    // - 상호작용 키를 통해 타이핑효과를 취소하고 모든 문자열을 출력하거나, 다음 줄로 넘어가기 위한 플래그
    // 구현해야할 함수
    // - 한글자씩 출력하는 함수(코루틴을 이용)
    // - 다음 줄을 넘겨주거나 모든 대사가 끝난 후 이벤트를 처리할 함수
    public string txt;
    public string[] strs;
    public bool isEvent = false;

    Text m_text;
    TextAsset data;
    int flag = 0;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
        m_text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (isEvent && Input.GetButtonDown("Interaction"))
        {
            SetFlag();
        }
        if (flag == 1)
        {

            StopCoroutine("Printing");
            m_text.text = txt;
        }
        if (flag == 2)
        {
            txt = "";
            flag = 0;
            Type();
        }
    }

    IEnumerator Printing()
    {
        
        for (int i = 0; i < txt.Length; i++)
        {
            m_text.text += txt[i];
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine("Printing");
        flag = 1;
    }

    public void StartTyping()
    {
        m_text = GetComponent<Text>();
        m_text.text = "";
        StartCoroutine("Printing");

    }

    public void SetFlag()
    {
        flag = (flag + 1) % 3;
    }

    public void Tutorial1On()
    {
        data = Resources.Load("tutorial1", typeof(TextAsset)) as TextAsset;

        strs = data.text.Split('\n');
        Type();
    }

    public void Tutorial2On()
    {
        data = Resources.Load("tutorial2", typeof(TextAsset)) as TextAsset;

        strs = data.text.Split('\n');
        Type();
    }

    public void Tutorial3On()
    {
        data = Resources.Load("tutorial3", typeof(TextAsset)) as TextAsset;

        strs = data.text.Split('\n');
        Type();
    }

    public void EndingOn()
    {
        data = Resources.Load("ending", typeof(TextAsset)) as TextAsset;

        isEvent = true;
        strs = data.text.Split('\n');
        Type();
    }

    public void Type()
    {
        if (index >= strs.Length)
        {
            strs = null;
            GetComponent<Text>().text = "";
            gameObject.transform.Find("Image").gameObject.SetActive(false);
            isEvent = false;
            if(FindObjectOfType<Player>())
                FindObjectOfType<Player>().m_move = true;
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            }
            index = 0;
            flag = 0;
        }
        else
        {
            txt = strs[index];
            
            index++;
            StartTyping();

        }
    }
}
