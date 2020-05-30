using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectController : MonoBehaviour
{
    // 오브젝트의 생성 및 삭제를 관리하는 클래스
    // 여러개의 오브젝트 컨트롤러가 생성되면 안되기 때문에 싱글톤으로 구현
    // 몬스터, 아이템, 맵 생성 등을 처리함
    // 오브젝트 풀을 이용하여 생성 삭제에 드는 비용을 최소화
    // 필요한 멤버 변수
    //  - 들어올 오브젝트접근시 사용할 index용 enum
    //     -> enum으로 접근하는 경우보다 가독성은 떨어지지만 편의를 위해 정수로 대체
    //  - 들어올 오브젝트의 종류의 수
    //  - 풀링할 오브젝트
    //  - 각 오브젝트 종류를 저장할 수
    //  - 오브젝트를 저장할 스택
    //  - 맵 생성을 위한 변수들 (맵 크기, 벽을 생성할 확률, 맵)
    //  - 벽 Color를 바꾸기 위한 Player의 hp
    //  - 맵 생성 후 빈 공간에 플레이어를 생성하기 위한 플레이어 프리펩
    //  - 몬스터와 아이템 생성했는지 확인하는 bool변수
    //  - 몬스터와 아이템 생성할 개수
    // 구현해야 할 함수
    //  - 오브젝트 풀에서 꺼내는 함수
    //  - 오브젝트 풀에 넣는 함수
    //  - 셀룰러 오토마타를 사용하여 씬이 로드될때마다 새로운 랜덤 맵을 생성하는 함수
    //  - 랜덤 맵이 클리어가 가능한지 (플레이어의 위치에서 포탈까지 도달할 수 있는가) 판별하는 함수
    //  - 플레이어, 포탈 등 오브젝트를 생성하는 함수


    public static ObjectController instance;
    public GameManager m_manager;
    public int objectKind;                      // 들어올 오브젝트의 종류의 수
    public GameObject[] objectList;
    public int[] objectNum;
    public Stack<GameObject>[] objectStack;

    public int width;
    public int height;
    [Range(0, 100)]
    public int fillPercent;
    // 0 - 빈 공간, 1 - 벽, 2 - Player 위치, 3 - 몬스터 위치, 4 - 아이템 박스 위치, 5 - 포탈 위치
    public int[,] m_map;
    int[,] m_check;
    
    public GameObject player;
    public GameObject portal;
    public GameObject boss;

    private bool isSummoned;
    public int monsterNum;
    public int itemNum;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            m_manager = FindObjectOfType<GameManager>();
            objectStack = new Stack<GameObject>[objectKind];
            isSummoned = false;
            for (int i = 0; i < objectKind; i++)
            {
                objectStack[i] = new Stack<GameObject>();
                for (int j = 0; j < objectNum[i]; j++)
                {
                    GameObject obj = Instantiate(objectList[i]);
                    obj.transform.SetParent(transform);
                    obj.SetActive(false);
                    objectStack[i].Push(obj);
                }
            }
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_manager.curScene == GameManager.Scene.dungeon && !isSummoned && m_map != null)
        {
            // 맵이 생성된 후 1회에 한정하여 몬스터와 아이템 생성
            isSummoned = true;
            
            for (int i = 0; i < monsterNum; i++)
            {
                MonsterInstantiate();
            }
            for(int i = 0; i < itemNum; i++)
            {
                ItemInstantiate();
            }

            // 몬스터가 자동으로 플레이어를 쫓기 위하여 맵을 Bake함
            GameObject.Find("Floor").SendMessage("RuntimeBake");
        }
        else if(m_manager.curScene == GameManager.Scene.boss && !isSummoned)
        {
            //보스 씬일 경우 보스와 플레이어만 생성
            isSummoned = true;
            PlayerInstantiate(50, 30);
            BossInstantiate();
            GameObject.Find("Floor").SendMessage("RuntimeBake");
        }
        
    }


    public void GenerateMap()
    {
        // Cellular Automata를 이용하여 랜덤 맵을 생성하는 함수
        m_map = new int[width, height];
        if (fillPercent == 0)
        {
            return;
        }
            
        if (m_map != null)
            FillMap();


        for (int i = 0; i < 7; i++)
            ChangeMap();

        MapCheck();
    }

    void FillMap()
    {
        // fillPercent의 확률로 각 좌표에 벽을 생성하는 함수
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    m_map[i, j] = 1;
                }
                else
                {
                    m_map[i, j] = (Random.Range(0, 100) < fillPercent) ? 1 : 0;
                }
            }
        }
    }

    int GetWallCount(int x, int y)
    {
        // 둘러싸인 좌표들 중 벽이 몇개인지 계산하는 함수
        int wallcount = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < width && j >= 0 && j < height)
                {
                    if (i != x || j != y)
                        wallcount += m_map[i, j];
                }
                else
                    wallcount++;
            }
        }
        return wallcount;
    }

    void ChangeMap()
    {
        // 주변에 둘러쌓인 벽의 개수가 4개 이상일 경우 벽으로 치환하는 함수
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int count = GetWallCount(i, j);
                if (count > 4)
                {
                    m_map[i, j] = 1;
                }
                else if (count < 4)
                {
                    m_map[i, j] = 0;
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    // 맵 확인용 기즈모 생성
    //    if (m_map != null)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                Gizmos.color = (m_map[x, y] == 1) ? Color.black : Color.white;
    //                Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}

    public GameObject PopObject(int x)
    {
        // 오브젝트 풀에서 오브젝트를 내보내는 함수
        if(objectStack[x].Count <= 0)
        {
            // 오브젝트 풀에 오브젝트가 모자랄 경우 예외처리
            for (int j = 0; j < objectNum[x]; j++)
            {
                GameObject tmp = Instantiate(objectList[0]);
                tmp.SetActive(false);
                objectStack[x].Push(tmp);
            }
        }
        GameObject obj = objectStack[x].Pop();
        return obj;
    }

    public void PushObject(GameObject obj, int objectKind)
    {
        //오브젝트 풀에 오브젝트를 다시 넣는 함수
        obj.SetActive(false);
        objectStack[objectKind].Push(obj);
    }

    public void PlayerInstantiate(int x, int y)
    {
        //생성할 좌표를 받아서 플레이어를 생성하는 함수
        GameObject obj = Instantiate(player, new Vector3(-50 + x, 0, -50 + y), Quaternion.identity);
        obj.name = "Player";

        obj.GetComponent<Player>().m_currentHP = m_manager.playerCurHP;
        obj.GetComponent<Player>().m_fullHP = m_manager.playerFullHP;

        GameObject.Find("MainCamera").SendMessage("TargetOn");
        if (m_manager.curScene == GameManager.Scene.dungeon)
            GameObject.Find("MinimapCamera").SendMessage("TargetOn");
    }
    
    public void MonsterInstantiate()
    {
        // 랜덤한 위치에 몬스터를 생성하는 마술
        int x;
        int y;
        while (true)
        {

            x = UnityEngine.Random.Range(0, 100);
            y = UnityEngine.Random.Range(0, 100);
            if (m_map[x, y] == 0 && m_map[x - 1, y] == 0 && m_map[x, y - 1] == 0 && m_map[x - 1, y - 1] == 0 &&
                m_map[x + 1, y] == 0 && m_map[x, y + 1] == 0 && m_map[x + 1, y + 1] == 0)
                break;
        }
        GameObject obj = PopObject(0);
        obj.transform.position = new Vector3(-50 + x, 0, -50 + y);
        m_map[x, y] = 3;
        obj.GetComponent<Monster>().SetTarget();
        obj.GetComponent<Monster>().m_currentHP = 150;
        obj.GetComponent<Monster>().m_fullHP = 150;
        obj.SetActive(true);
        obj.GetComponent<NavMeshAgent>().enabled = true;

    }

    public void ItemInstantiate()
    {
        // 랜덤한 위치에 아이템을 생성하는 마술
        int x;
        int y;
        while (true)
        {

            x = UnityEngine.Random.Range(0, 100);
            y = UnityEngine.Random.Range(0, 100);
            if (m_map[x, y] == 0 && m_map[x - 1, y] == 0 && m_map[x, y - 1] == 0 && m_map[x - 1, y - 1] == 0 &&
                m_map[x + 1, y] == 0 && m_map[x, y + 1] == 0 && m_map[x + 1, y + 1] == 0)
                break;
        }
        GameObject obj = PopObject(1);
        obj.transform.position = new Vector3(-50 + x, 0, -50 + y);
        m_map[x, y] = 4;
        obj.SetActive(true);
    }

    public void PortalInstantiate(int x, int y)
    {
        //생성할 좌표를 받아 포탈을 생성하는 함수.   
        GameObject obj = Instantiate(portal, new Vector3(-50 + x, 0, -50 + y), Quaternion.identity);
    }

    public void MapCheck()
    {
        //랜덤하게 플레이어와 포탈의 위치를 결정하고 FindPath 함수를 통해
        //플레이어 위치에서 포탈까지 도달할 수 있는지 확인하는 함수.
        m_check = new int[width, height];
        for(int i =0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                m_check[i,j] = m_map[i,j];
            }
        }
        int x; int y;
        int player_x = -1; int player_y = -1;
        int portal_x = -1; int portal_y = -1;
        do
        {
            if(player_x> 0)
            {
                m_check[player_x, player_y] = 0;
                m_check[portal_x, portal_y] = 0;
            }
            while (true)
            {

                x = UnityEngine.Random.Range(0, 100);
                y = UnityEngine.Random.Range(0, 100);
                if (m_map[x, y] == 0)
                    break;
            }
            m_check[x, y] = 2;

            player_x = x;
            player_y = y;
            while (true)
            {

                x = UnityEngine.Random.Range(0, 100);
                y = UnityEngine.Random.Range(0, 100);
                if (m_map[x, y] == 0)
                    break;
            }
            m_check[x, y] = 5;
            portal_x = x;
            portal_y = y;
        } while (!FindPath(player_x, player_y));
        PlayerInstantiate(player_x, player_y);
        PortalInstantiate(portal_x, portal_y);
    }

    bool FindPath(int x, int y)
    {
        // BFS를 통해 입력받은 좌표에서 포탈 위치까지 도달할 수 있는지 확인하는 함수
        // 0 - 통로, 1 - 벽, 2 - 방문
        Queue<int[]> queue = new Queue<int[]>();
        int[] dirX = { -1, 1, 0, 0 };
        int[] dirY = { 0, 0, -1, 1 };
        int[] start = { x, y };
        queue.Enqueue(start);
        m_check[x, y] = 2;
        while(queue.Count>0)
        {
            int[] tmp = queue.Dequeue();
            if (m_map[tmp[0], tmp[1]] == 5)
                return true;
            for(int i = 0; i < 4; i++)
            {
                int[] next = { dirX[i] + tmp[0], dirY[i] + tmp[1] };

                if (next[0] < 0|| next[0] >= width || next[1] < 0 || next[1] >= height)
                {
                    continue;
                }

                if(m_check[next[0], next[1]] == 0)
                {
                    queue.Enqueue(next);

                    m_check[next[0], next[1]] = 2;
                }
                if(m_check[next[0], next[1]] == 5)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void BossInstantiate()
    {
        GameObject obj  =Instantiate(boss, new Vector3(0, 0, 35), Quaternion.identity);
        obj.SendMessage("SetTarget");
        
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
