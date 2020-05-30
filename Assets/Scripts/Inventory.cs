using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 아이템 인벤토리를 구현한 클래스
    // 게임 전체에서 한개의 오브젝트만 존재해야하므로 싱글톤으로 구현
    // 획득한 아이템이 저장되어야 하고 사용될 경우 인벤토리에서 없어져야함
    // 'I'키를 누를 경우 UI가 켜져야함
    // 획득한 아이템 아이콘위에 마우스 커서를 올릴경우 위에 툴팁칸에 툴팁이 보여야함
    // 아이템 아이콘을 우클릭하면 착용되거나 사용되어야 함
    // 필요한 멤버변수
    //  - 아이템 정보를 받아올 ItemDatabase 변수
    //  - 아이템을 저장할 인벤토리 리스트
    //  - 아이템 슬롯을 표현할 리스트
    // 구현할 함수
    //  - 아이템을 저장하는 함수
    //  - 아이템을 빼내는 함수

    private ItemDatabase db;
    private bool showTooltip = false;
    private bool dragItem = false;
    private string tooltip;
    private Item draggedItem;
    private int prevIndex;

    public static Inventory instance;
    public List<Item> inventory;
    public List<Item> slots = new List<Item>();
    public GUISkin skin;
    public int slotX;
    public int slotY;
    public bool showInventory = false;
    public bool showQuitUI = false;

    public Item equiped;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            inventory = new List<Item>();
            db = FindObjectOfType<ItemDatabase>();
            equiped = new Item();
            for(int i = 0; i < slotX*slotY; i++)
            {
                inventory.Add(new Item());
                slots.Add(new Item());
            }
            
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            showInventory = !showInventory;
            if (showInventory)
                GameManager.instance.CursorActivated();
            else
                GameManager.instance.CursorDisabled();
        }
        if (showInventory && Input.GetButtonDown("Escape"))
        {
            showInventory = !showInventory;
            GameManager.instance.CursorDisabled();
        }
            
        else if (!showQuitUI && Input.GetButtonDown("Escape"))
        {
            showQuitUI = true;
            FindObjectOfType<GameManager>().QuitUIOpen();
            GameManager.instance.CursorActivated();
        }
        else if (showQuitUI && Input.GetButtonDown("Escape"))
        {
            showQuitUI = false;
            FindObjectOfType<GameManager>().QuitUIClose();
            GameManager.instance.CursorDisabled();
        }
        
    }

    public void PushItem(Item item)
    {
        for (int i = 0; i < slotX * slotX; i++)
        {
            if (inventory[i].itemName == null)
            {
                inventory[i] = item;
                return;
            }
        }
#if UNITY_EDITOR
        Debug.Log("인벤토리가 가득찼습니다!");
#endif
    }

    public Item PopItem(int idx)
    {
        Item item = inventory[idx];
        inventory[idx] = null;
        return item;
    }
    void OnGUI()
    {
        
        GUI.skin = skin;
        
        if (showInventory)
        {
            DrawInventory();
            if (showTooltip)
            {
                GUI.Box(new Rect(1300, 480, 455, 350), tooltip, skin.GetStyle("tooltip"));
            }
        }
        if (dragItem)
        {
            GUI.DrawTexture(new Rect(Event.current.mousePosition.x - 5, Event.current.mousePosition.y - 5, 50, 50), draggedItem.itemIcon);
        }
    }

    void DrawInventory()
    {
        // 인벤토리를 그리는 함수
        int k = 0;
        Event e = Event.current;
        Rect equipRect = new Rect(1300, 280, 200, 200);
        GUI.Box(new Rect(1260, 200, 465, 800), "inventory", skin.GetStyle("inventory background"));
        GUI.Box(equipRect, "", skin.GetStyle("equiped"));
        if(equiped.itemName != null)
        {
            // 장비한 아이템 슬롯을 그림
            GUI.Box(equipRect, equiped.itemIcon);
            string equipedTooltip = CreateTooltip(equiped);
            GUI.Box(new Rect(1530, 280, 200, 200), equipedTooltip, skin.GetStyle("tooltip"));
            if (equipRect.Contains(e.mousePosition))
            {
                if(e.isMouse && e.type == EventType.MouseDown && e.button == 1)
                {
                    instance.PushItem(equiped);
                    FindObjectOfType<Attack>().WeaponUnequip();
                    equiped = new Item();
                }
            }
        }
        for (int i = 0; i < slotX; i++)
        {
            for (int j = 0; j < slotY; j++)
            {
                //슬롯의 배경을 그림
                Rect slotRect = new Rect(j * 77 + 1300, i * 77 + 650, 75, 75);
                GUI.Box(slotRect, "", skin.GetStyle("slot background"));
                
                slots[k] = inventory[k];
                if (slots[k].itemName != null)
                {
                    // 인벤토리에 아이템이 있을 경우 아이콘을 그려줌
                    GUI.DrawTexture(slotRect, slots[k].itemIcon);
                    if (slotRect.Contains(e.mousePosition))
                    {
                        // 마우스 포인터가 슬롯 위에 올라올 경우 툴팁을 표시함
                        tooltip = CreateTooltip(slots[k]);
                        showTooltip = true;
                        if (e.button == 0 && e.type == EventType.MouseDrag && !dragItem)
                        {
                            //드래그를 시작하는 경우 아이템을 슬롯에서 빼서 임시로 드래그 아이템에 넣어둠
                            dragItem = true;
                            prevIndex = k;
                            draggedItem = slots[k];
                            inventory[k] = new Item();
                        }
                        if (e.type == EventType.MouseUp && dragItem)
                        {
                            // 드래그를 놓았을 경우 드래그 아이템에서 빼서 위치한 슬롯에 놓음
                            inventory[prevIndex] = inventory[k];
                            inventory[k] = draggedItem;
                            dragItem = false;
                            draggedItem = null;
                        }
                        if (e.isMouse && e.type == EventType.MouseDown && e.button == 1)
                        {
                            // 아이템 슬롯 위에서 우클릭 한 경우 아이템을 사용함
                            if (inventory[k].itemType == Item.ItemType.Using)
                            {
                                // 사용 아이템일 경우 아이템 아이디에 따라 사용 함수를 호출
                                switch (inventory[k].itemID)
                                {
                                    case 2:
                                        FindObjectOfType<Player>().Heal(inventory[k].itemPoint);
                                        break;
                                }
                                inventory[k] = new Item();
                                showTooltip = false;
                            }
                            else
                            {
                                // 무기인경우 착용 함수를 호출
                                if(equiped != null)
                                {
                                    instance.PushItem(equiped);
                                    FindObjectOfType<Attack>().WeaponUnequip();
                                    equiped = new Item();
                                }
                                FindObjectOfType<Attack>().WeaponEquip(inventory[k]);
                                equiped = inventory[k];
                                inventory[k] = new Item();
                                showTooltip = false;
                            }
                        }
                    }
                }
                else
                {
                    if (slotRect.Contains(e.mousePosition))
                    {
                        // 드래그한 아이템을 빈 슬롯에 놓을 경우
                        if (e.type == EventType.MouseUp && dragItem)
                        {
                            inventory[k] = draggedItem;
                            dragItem = false;
                            draggedItem = null;
                        }
                    }
                    else
                    {
                        // 드래그한 아이템 원상복귀
                        if (e.type == EventType.MouseUp && dragItem)
                        {
                            inventory[prevIndex] = draggedItem;
                            dragItem = false;
                            draggedItem = null;
                        }
                    }
                }
                if (tooltip == "")
                {
                    showTooltip = false;
                }
                k++;
            }
        }
    }
    string CreateTooltip(Item item)
    {
        // 툴팁을 생성하는 함수
        if (item.itemType == Item.ItemType.Weapon)
            tooltip = "아이템 이름: " + item.itemName + "\n아이템 데미지: " + item.itemPoint + "\n아이템 설명: \n" +item.itemDescription;
        else
            tooltip = "아이템 이름: " + item.itemName + "\n아이템 설명: \n" + item.itemDescription;

        return tooltip;
    }
    
    public void InventoryClear()
    {
        // 튜토리얼 끝나고 던전 씬을 호출할 때 인벤토리를 비우는 함수
        equiped = new Item();
        for (int i = 0; i < slotX * slotY; i++)
        {
            inventory.Add(new Item());
            slots.Add(new Item());
        }
    }

}



