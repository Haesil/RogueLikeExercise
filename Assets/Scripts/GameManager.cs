using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 전체적인 게임의 흐름을 제어하는 오브젝트
    // 전체 게임에서 단 한개의 오브젝트만 존재해야하기때문에 싱글톤으로 구현함
    // 필요한 멤버 변수
    // - 현재의 씬 상태를 저장할 enum변수
    // - 모든 씬에서 생성될 플레이어에게 전달할 플레이어의 정보
    // 구현해야할 함수
    // 각 state에 따라 다음 씬을 로드할 함수.
    public static GameManager instance;

    public enum Scene { start, tutorial, dungeon, boss };
    public Scene curScene = Scene.start;
    public int playerLevel;
    public int exp;
    public int playerCurHP;
    public int playerFullHP;
    public int level;

    bool isCursor;
    bool flag;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            level = 0;
            playerLevel = 1;
            exp = 0;
            playerCurHP = 150;
            playerFullHP = 150;
            isCursor = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (curScene == Scene.start && Input.GetButtonDown("Interaction"))
        {
            curScene = Scene.tutorial;
            SceneManager.LoadScene("TutorialScene");
        }
        if(Input.GetButtonDown("Cursor"))
        {
            flag = true;
            isCursor = !isCursor;
        }
        if(flag)
        {
            flag = false;
            if (isCursor)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
    }

    public void LoadScene()
    {
        //현재 씬에 따라 다음 씬을 호출하는 함수
        if(curScene == Scene.tutorial)
        {
            //튜토리얼에서 얻은 것을 초기화하고 던전 씬을 호출
            curScene = Scene.dungeon;
            playerLevel = 1;
            exp = 0;
            playerCurHP = 150;
            playerFullHP = 150;
            FindObjectOfType<Inventory>().InventoryClear();
            SceneManager.LoadScene("DungeonScene");
        }
        else if(curScene == Scene.dungeon && level < 3)
        {
            // 던전 방을 3번 방문하지 않은 경우
            level = level + 1;
            SceneManager.LoadScene("DungeonScene");
        }
        else if (curScene == Scene.dungeon && level >= 3)
        {
            // 던전 방을 3번 방문한 경우 보스로 진입
            curScene = Scene.boss;
            SceneManager.LoadScene("BossScene");
        }
        else
        {
            // 보스를 잡고 등장한 포탈을 이용할 경우
            SceneManager.LoadScene("Ending");
        }
    }

    public void CursorActivated()
    {
        flag = true;
        isCursor = true;
    }

    public void CursorDisabled()
    {
        flag = true;
        isCursor = false;
    }

    public void QuitUIOpen()
    {
        Time.timeScale = 0;
        GameObject.Find("Canvas").transform.Find("QuitMenu").gameObject.SetActive(true);
        CursorActivated();
    }

    public void QuitUIClose()
    {
        Time.timeScale = 1;
        GameObject.Find("Canvas").transform.Find("QuitMenu").gameObject.SetActive(false);
        CursorDisabled();
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GameOver()
    {
        // 플레이어 사망 후 재시작 버튼 클릭 시 호출
        level = 0;
        playerLevel = 1;
        exp = 0;
        playerCurHP = 150;
        playerFullHP = 150;
        curScene = Scene.tutorial;
        SceneManager.LoadScene("TutorialScene");
    }
}
