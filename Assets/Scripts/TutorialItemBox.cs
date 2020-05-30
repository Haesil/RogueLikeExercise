using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialItemBox : MonoBehaviour
{
    // 튜토리얼에서 결정된 아이템을 지급하기 위한 아이템 박스
    // 아이템 박스 클래스와 거의 흡사하다.

    public ItemDatabase itemDB;
    public Item item;

    bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        itemDB = FindObjectOfType<ItemDatabase>();
        item = itemDB.items[0];
        item.itemPoint = 50;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer && Input.GetButtonDown("Interaction"))
        {
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "";
            FindObjectOfType<Inventory>().PushItem(item);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "아이템을 획득한다 : Space";
            isPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "";
            isPlayer = false;
        }

    }
}
