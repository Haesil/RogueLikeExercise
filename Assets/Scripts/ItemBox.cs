using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    // 랜덤한 아이템을 가지고 있는 클래스
    // 플레이어가 근접할 경우 UI를 띄우고 상호작용 버튼을 누를경우 아이템을 획득해야함
    // 필요한 멤버 변수
    //  - 아이템 오브젝트 변수
    

    public ItemDatabase itemDB;
    public Item item;

    bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        itemDB = FindObjectOfType<ItemDatabase>();
        int x = Random.Range(0, 100);
        int tmp;
        if (x < 30)
            tmp = 0;
        else if (x < 60)
            tmp = 1;
        else
            tmp = 2;
        item = itemDB.items[tmp];
        switch(tmp)
        {
            case 0:
                item.itemPoint = Random.Range(30, 51);
                break;
            case 1:
                item.itemPoint = Random.Range(25, 41);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayer && Input.GetButtonDown("Interaction"))
        {
            // 가까이 온 상태에서 상호작용 키를 누를경우 인벤토리에 추가함
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "";
            FindObjectOfType<Inventory>().PushItem(item);
            FindObjectOfType<ObjectController>().PushObject(gameObject, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            // 가까이 왔을때 UI를 띄워줌
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "아이템을 획득한다 : Space";
            isPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // 멀어지면 UI를 끔
            GameObject.Find("Canvas").transform.Find("Text").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>().text = "";
            isPlayer = false;
        }
        
    }
}
