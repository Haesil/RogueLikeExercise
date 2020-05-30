using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 

public class Item
{
    // 아이템 클래스
    // 필요한 멤버 변수
    //  - 아이템의 ID
    //  - 아이템 이름
    //  - 아이템의 능력치 (소모품일 경우 힐량, 무기일 경우 공격력)
    //  - 아이템 설명 (툴팁에 사용)
    //  - 아이템의 아이콘
    //  - 아이템의 타입 (소모품, 무기)

    public int itemID;
    public string itemName;
    public int itemPoint;
    public string itemDescription;
    public Texture2D itemIcon;
    public ItemType itemType;

    public enum ItemType
    {
        Weapon,
        Using
    }
    
    public Item()
    {

    }

    public Item(string img, int ID, string name, int point, string des,ItemType type)
    {
        itemID = ID;
        itemName = name;
        itemPoint = point;
        itemDescription = des;
        itemType = type;
        itemIcon = Resources.Load<Texture2D>(img);
    }
}
