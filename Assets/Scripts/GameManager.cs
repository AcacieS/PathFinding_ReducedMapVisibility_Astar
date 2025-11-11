
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool ResetTest = false;
    [Header(" ====== Testing =======")]
    public bool isTestingSpawn = false;
    public bool isTestingRVGMap = false;
    public bool isTestingRVGStartGoal = false;
    public bool isTestingHeuristic = false;
    [SerializeField] private Cost costInCase;

    [Header("Other")]
    
    [SerializeField] private GameObject ground;

    [Header("Obstacle")]
    [SerializeField] private int minObstacle = 8;
    [SerializeField] private int maxObstacle = 13;
    
    [SerializeField] private float minDistance = 1.0f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private GameObject[] obstaclesType;
    private List<GameObject> obstacles = new List<GameObject>();
    private static int randomNbObstacles = 0;
    private List<Vector3> failedAttempts = new List<Vector3>();
    


    [Header("MOBILE AGENT")]

    [SerializeField] MobileAgentInfo[] MobileAgentInfos;
    [SerializeField] private GameObject UI;
    private static MobileAgent mobileAgent;



    [Header("RUG")]
    [SerializeField] private RUG RUG;
    private static int id = 0;
    public string GetTypeRVG()
    {
        if (isTestingHeuristic)
        {
            return "heuristic";
        }
        return "naive";
        
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // ===================================================== START =================================
    void Start()
    {
        if (ResetTest)
        {
            isTestingSpawn = false;
            isTestingRVGMap = false;
            isTestingRVGStartGoal = false;
            minObstacle = 8;
            maxObstacle = 13;
        }
        SpawnLevel();
        PathLengthTest.ShowLogLocation();
    }

    // ===================================================== INPUT KEYBOARD =================================
    void Update()
    {
        ShowUI();
        TestSpawn();
    }
    private void TestSpawn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnLevel();
        }
    }
    private void ShowUI()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (UI.activeSelf)
            {
                UI.SetActive(false);
            }
            else
            {
                UI.SetActive(true);
            }
        }

    }


    // ===================================================== SPAWN ==========================================
    
    public void SpawnLevel()
    {
        if (Cost.InstanceC == null)
        {
            costInCase.SetCostInstance();
            Debug.LogError("why is it null");
        }
        Cost.InstanceC.SpawnCost();
        SpawnObstacles();
        SpawnMobileAgent(MobileAgentInfos[Random.Range(0, MobileAgentInfos.Length)]);
    }

    // ---------------- OBSTACLES -------------

    private void SpawnObstacles()
    {
        //RESET OBSTACLES
        if (obstacles.Count != 0)
        {
            Debug.Log("RESET OBSTACLE");
            foreach (GameObject obstacle in obstacles)
            {
                Destroy(obstacle);
            }
            obstacles = new List<GameObject>();
            failedAttempts = new List<Vector3>();
        }
        Debug.Log("NEW OBSTACLE");

        //SPAWN NEW OBSTACLES
        
        randomNbObstacles = Random.Range(minObstacle, maxObstacle);
        for (int i = 0; i < randomNbObstacles; i++)
        {
            int randomObstacleType = Random.Range(0, 2);
            GameObject currentObstacle = obstaclesType[randomObstacleType];
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            Vector3 pos = RandomSpawnPos();
            Vector3 newPos = new Vector3(pos.x, pos.y + 1f, pos.z);
            GameObject newObstacle = Instantiate(currentObstacle, newPos, rotation);
            
            obstacles.Add(newObstacle);
        }
    }
    
    // ---------------- MOBILE AGENT -------------
    public void SpawnMobileAgent(MobileAgentInfo info)
    {
        if (mobileAgent != null)
        {
            Destroy(mobileAgent.GetGoal());
            Destroy(mobileAgent.GetObj());
        }

        GameObject mobileAgentObj = SpawnOBJ(info, info.obj);
        GameObject goal = SpawnOBJ(info, info.goal_pref);

        mobileAgent = new MobileAgent(info, mobileAgentObj, goal);

        if (!isTestingSpawn)
        {
            SpawnRVG();
        }
    }

    private GameObject SpawnOBJ(MobileAgentInfo info, GameObject prefab)
    {
        Quaternion rotation;
        if(prefab == info.obj)
        {
            rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        Vector3 spawnPos = RandomSpawnPos();
        spawnPos = new Vector3(spawnPos.x, ground.transform.position.y + 1f, spawnPos.z);
        GameObject mobileAgentObj = Instantiate(prefab, spawnPos, rotation);
        mobileAgentObj.transform.localScale = new Vector3(info.size, info.size, info.size);

        return mobileAgentObj;
    }
    private string RVGType = "naive";
    
    // ---------------- RVG -------------
    public void SpawnRVG(string type = "naive")
    {
        id = 0;

        if(type == "naive")
        {
            Debug.Log("RVG Test: naive");
            isTestingHeuristic = false;
            mobileAgent.ResetStart(); 
            Vertex Vstart = new Vertex(gameObject, mobileAgent.GetObj().transform);
            Vertex Vend = new Vertex(gameObject, mobileAgent.GetGoal().transform);
            RUG.BuildRUG(obstacles, Vstart, Vend);
        }
        else
        {
            Debug.Log("RVG Test: heuristic");
            isTestingHeuristic = true;
            mobileAgent.ResetStart(); 
            Vertex Vstart = new Vertex(gameObject, mobileAgent.GetObj().transform);
            Vertex Vend = new Vertex(gameObject, mobileAgent.GetGoal().transform);
            RUG.BuildRUG(obstacles, Vstart, Vend);
        }
        
    }
    // ---------------- RANDOM POS -------------
    private Vector3 RandomSpawnPos(){

        BoxCollider groundCollider = ground.GetComponent<BoxCollider>();
        Vector3 groundSize = Vector3.Scale(groundCollider.size, ground.transform.localScale);
        Vector3 groundCenter = groundCollider.transform.position;
        Vector3 spawnPos = Vector3.zero;

        bool valid = false;
        int attempts = 0;
        int maxAttempts = 10000;

        while (!valid && attempts < maxAttempts)
        {
            float randomX = Random.Range(
                groundCenter.x - groundSize.x / 2f + 2.5f,
                groundCenter.x + groundSize.x / 2f - 2.5f
            );
            float randomZ = Random.Range(
                groundCenter.z - groundSize.z / 2f + 2f,
                groundCenter.z + groundSize.z / 2f - 2f
            );

            spawnPos = new Vector3(randomX, groundCenter.y, randomZ);

            bool collides = Physics.CheckSphere(spawnPos, minDistance, obstacleMask);

            if (!collides)
            {
                valid = true;
            }else
            {
                failedAttempts.Add(spawnPos);
            }

            attempts++;
        }

        if (!valid)
        {
            Debug.LogWarning("Could not find valid spawn position!");
        }

        return spawnPos;

    }


    
    // ===================================================== GET FUNCTION =================================
    public int GetId()
    {
        int currentId = id;
        id++;
        return currentId; 
    }
    public MobileAgent GetCurrentMA()
    {
        return mobileAgent;
    }
    // ==================================================== GIZMOS ========================================
    private void OnDrawGizmos()
    {
        //if (!Application.isPlaying) return;
        Gizmos.color = Color.black;
        if (failedAttempts != null && isTestingSpawn)
        {
            Gizmos.color = Color.red;
            foreach (Vector3 pos in failedAttempts)
            {
                Gizmos.DrawSphere(pos + Vector3.up * 0.1f, 0.1f);
            }
        }
        
        Gizmos.color = Color.green;
        // if (mobileAgent != null)
        // {
        //     Gizmos.DrawWireSphere(mobileAgent.GetObj().transform.position, minDistance);
        // }

        for (int i = 0; i < obstacles.Count; i++)
        {
            Gizmos.DrawWireSphere(obstacles[i].transform.position, minDistance);
        }
    }
    // ===================================================== OTHER FUNCTION =================================
    
    
}