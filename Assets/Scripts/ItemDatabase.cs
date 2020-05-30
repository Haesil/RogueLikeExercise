using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    // 게임에 사용될 아이템의 정보를 저장하는 클래스
    // 필요한 멤버 변수
    //  - 아이템의 정보를 저장할 리스트

    public List<Item> items = new List<Item>();
    // Start is called before the first frame update
    void Start()
    {
        items.Add(new Item("Sword", 0, "검", 0, "평범한 검. \n약하다.", Item.ItemType.Weapon));
        items.Add(new Item("Club", 1, "몽둥이", 0, "평범한 몽둥이. \n엄청 약하다.", Item.ItemType.Weapon));
        items.Add(new Item("Potion", 2, "포션", 20, "평범한 포션. \n체력을 약간 회복해준다.", Item.ItemType.Using));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
