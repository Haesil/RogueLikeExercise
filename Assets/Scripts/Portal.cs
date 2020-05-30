using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    // 플레이어가 근접하여 상호작용 버튼을 누르면 다음 씬을 로드하게 하는 클래스
    // 플레이어가 근접하면 안내 UI를 띄우고 멀어지면 안내 UI를 끔.

    bool isPlayer;
    // Start is called before the first frame update
    void Start()
    {
        isPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayer && Input.GetButtonDown("Interaction"))
        {
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "";
            FindObjectOfType<GameManager>().LoadScene();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "다음 층으로 이동한다 : Space";
            isPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "";
            isPlayer = false;
        }
    }
    public void LoadScene()
    {
        FindObjectOfType<GameManager>().LoadScene();
    }
}
