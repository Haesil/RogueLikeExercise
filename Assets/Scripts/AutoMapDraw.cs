using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMapDraw : MonoBehaviour
{
    // 오브젝트 컨트롤러 클래스에서 맵을 받아 맵을 그리는 클래스
    // 씬 로드시 맵 자동 생성

    public GameObject wall;
    public ObjectController objcontroller;
    public NavMeshSurface surface;
    int[,] m_map;
    int width;
    int height;

    // Start is called before the first frame update
    void Start()
    {
        objcontroller = FindObjectOfType<ObjectController>();
        objcontroller.GenerateMap();
        width = objcontroller.width;
        height = objcontroller.height;
        m_map = new int[width, height];
        m_map = objcontroller.m_map;
        if(m_map != null)
            DrawMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void DrawMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (m_map[x, y] == 1)
                {
                    GameObject obj = Instantiate(wall);
                    obj.transform.position = new Vector3(-50 + x, 5, -50 + y);
                    obj.transform.SetParent(transform);
                }
            }
        }
    }

    void RuntimeBake()
    {
        surface.BuildNavMesh();
    }
}
