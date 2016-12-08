using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoardManager : MonoBehaviour {

    public static BoardManager instance = null;

    [Tooltip("Board max length instantiation")]
    public int boardLength;
    private int boardLengthOrigin;
    [Tooltip("Board max row instantiation")]
    public int boardRow;
    private int boardRowTemp;

    [Tooltip("Land Prefabs, exactly safe land")]
    public GameObject[] landPrefabs;
    [Tooltip("Slippy land, make player automove down when touch it")]
    public GameObject[] slippyPrefabs;
    [Tooltip("Tree Land, You can't move through this")]
    public GameObject[] treePrefabs;
    [Tooltip("Obstacle land, be carefull!!")]
    public GameObject[] obstacleStaticPrefabs;
    [Tooltip("Enemy Land, where the enemy moves along")]
    public GameObject[] enemyLandPrefabs;
    [Tooltip("Enemy Prefabs, Yes the enemy!")]
    public GameObject[] enemyPrefabs;

    private List<GameObject> enemyInWorldArray = new List<GameObject>();

    [Tooltip("Coin, Collect this and you can buy more character > Cheers")]
    public GameObject coinPrefab;


    [Tooltip("Chance of safe land will instantiate")]
    [Range(0f, 1f)]
    public float safeLandPriority;
    [Tooltip("Toggle of slippy land, set false if you didn't want to instantiate slippy land")]
    public bool slippyLandActive = false;
    [Tooltip("Chance of slippy land will instantiate")]
    [Range(0f, .4f)]
    public float slippyLandPriority;
    [Tooltip("Toggle of enemy land, set false if you didn't want to instantiate enemy land")]
    public bool enemyLandActive = false;
    [Tooltip("Chance of enemy land will instantiate")]
    [Range(0f, .4f)]
    public float enemyLandPriority;
    [Tooltip("Chance of tree land will instantiate")]
    [Range(0f, 1f)]
    public float treePriority;
    [Tooltip("Chance of obstacle land will instantiate")]
    [Range(0f, 1f)]
    public float obstacleStaticPriority;
    [Tooltip("Chance of coin will instantiate")]
    [Range(0f, 1f)]
    public float coinPriority;

    private int slippyLandChain;
    private int slippyLandChainNotSafe;
    private int enemyLandChain;

    private List<Vector3> enemyLandPath = new List<Vector3>();

    private float randomizerForLand;
    private int initiateSpawnSafeLand;
    private Vector3 nextSafeLand = new Vector3();
    private Vector3 nextSlippyLand = new Vector3();
    private Vector3 nextEnemyLand = new Vector3();

    private bool hasSearchingSafeLand = false;

    public Dictionary<string, string> dictLand = new Dictionary<string, string>();
    private List<string> idLandList = new List<string>();
    private int indexForIdLand = 0;

    private int x, y, z;
    private bool tooglePosXandY = false;

    private Transform landGrup;

    private int landDestroyerIndex = 2;
    private bool isCreateNewLand = false;
    private MeshRenderer[] meshRendererLand;

    void InitBoard()
    {
        if (landGrup == null)
            landGrup = new GameObject("landGroup").transform;
        while (boardLength > 0)
        {
            Transform rowGrup = new GameObject("RowGroup : " + y.ToString()).transform;
            int i = 0;
            while (i < boardRowTemp)
            {
                float coinChance = Random.Range(0f, 1f);

                GameObject land = landPrefabs[Random.Range(0, landPrefabs.Length)] as GameObject;
                GameObject coinInstance = null;
                if (boardLength < initiateSpawnSafeLand)
                {
                    //    Debug.Log(nextSafeLand);
                    //Debug.Log("X : " +x +"Y : "+y+"Z : "+z);
                    //Debug.Log("hasBeen Searching : " + hasSearchingSafeLand);
                    if (x == nextSafeLand.x && z == nextSafeLand.z)         //Instantiate on safe land
                    {
                        //Debug.Log(rowGrup);
                        float randomSafeLand = Random.Range(0, safeLandPriority + slippyLandPriority + obstacleStaticPriority);
                        //if before this instantiate slippyland, continue to instantiate slippy land, until counter slippylandchain is zero
                        if (slippyLandChain > 0)
                        {
                            if (i == 0 && boardRowTemp == boardRow)
                            {
                                //slippyPrefabs[0] is for direction left
                                land = Instantiate(slippyPrefabs[0], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if(slippyLandChain > 1)
                                    nextSafeLand = new Vector3(x + 1, y - 1, z);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                idLandList.Add("slippy-left");
                            } //i == 0 equals with on right edge
                            else if (i == boardRowTemp - 1 && boardRowTemp == boardRow)
                            {
                                //slippyPrefabs[1] is for direction right
                                land = Instantiate(slippyPrefabs[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if(slippyLandChain > 1)
                                    nextSafeLand = new Vector3(x, y - 1, z + 1);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                idLandList.Add("slippy-right");
                            } // equals with on left edge
                            else
                            {
                                int random = Random.Range(0, slippyPrefabs.Length);
                                land = Instantiate(slippyPrefabs[random], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if (random == 0)
                                {
                                    if(slippyLandChain > 1)
                                        nextSafeLand = new Vector3(x + 1, y - 1, z);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                    idLandList.Add("slippy-left");
                                }
                                else
                                {
                                    if(slippyLandChain > 1)
                                        nextSafeLand = new Vector3(x, y - 1, z + 1);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                    idLandList.Add("slippy-right");
                                }
                            } //not on left or right edge
                            slippyLandChain -= 1;
                            if (slippyLandChain <= 0)
                                hasSearchingSafeLand = false;
                        }
                        else if (randomSafeLand <= safeLandPriority)
                        {
                            land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                            dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                            idLandList.Add("safe");
                        }
                        else if (randomSafeLand <= safeLandPriority + slippyLandPriority)
                        {
                            //Set For Slippy land chain
                            if (slippyLandChain <= 0)
                                slippyLandChain = Random.Range(3, 6);

                            //Check Edge
                            if (i == 0)
                            {
                                //slippyPrefabs[0] is for direction left
                                land = Instantiate(slippyPrefabs[0], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextSafeLand = new Vector3(x + 1, y - 1, z);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                idLandList.Add("slippy-left");
                            } //i == 0 equals with on right edge
                            else if (i == boardRowTemp - 1)
                            {
                                //slippyPrefabs[1] is for direction right
                                land = Instantiate(slippyPrefabs[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextSafeLand = new Vector3(x, y - 1, z + 1);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                idLandList.Add("slippy-right");
                            } // equals with on left edge
                            else
                            {
                                int random = Random.Range(0, slippyPrefabs.Length);
                                land = Instantiate(slippyPrefabs[random], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if (random == 0)
                                {
                                    nextSafeLand = new Vector3(x + 1, y - 1, z);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                    idLandList.Add("slippy-left");
                                }
                                else
                                {
                                    nextSafeLand = new Vector3(x, y - 1, z + 1);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                    idLandList.Add("slippy-right");
                                }
                            } //not on left or right edge
                            slippyLandChain -= 1;
                            hasSearchingSafeLand = true;
                        }
                        else if (randomSafeLand <= safeLandPriority + slippyLandPriority + obstacleStaticPriority)
                        {
                            land = Instantiate(obstacleStaticPrefabs[Random.Range(0, obstacleStaticPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                            dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "obstacle");
                            idLandList.Add("obstacle");
                        }
                        else
                        {
                            land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                            dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                            idLandList.Add("safe");
                        }
                        land.name = "THIS IS SAFE LAND";
                        if (coinChance <= coinPriority)
                            coinInstance = Instantiate(coinPrefab, new Vector3(x, y + 1, z), Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        //0 - safeLand, 1 - Tree, 2 - ObstacleStatis, 3 - ObstacleMoving
                        float randomLand = Random.Range(0f, randomizerForLand);
                        if (x == nextSlippyLand.x && z == nextSlippyLand.z)
                        {
                            int randomSlippy = Random.Range(0, 2); // 0 for left, 1 for right
                            if (slippyLandChainNotSafe <= 0)
                                slippyLandChainNotSafe = Random.Range(3, 6);

                            //Check Edge
                            if (i == 0 && boardRowTemp ==boardRow)
                            {
                                //slippyPrefabs[0] is for direction left
                                land = Instantiate(slippyPrefabs[0], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextSlippyLand = new Vector3(x + 1, y - 1, z);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                idLandList.Add("slippy-left");
                            } //i == 0 equals with on right edge
                            else if (i == boardRowTemp - 1 && boardRowTemp == boardRow)
                            {
                                //slippyPrefabs[1] is for direction right
                                land = Instantiate(slippyPrefabs[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextSlippyLand = new Vector3(x, y - 1, z + 1);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                idLandList.Add("slippy-right");
                            } // equals with on left edge
                            else
                            {
                                int random = Random.Range(0, slippyPrefabs.Length);
                                land = Instantiate(slippyPrefabs[random], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if (random == 0)
                                {
                                    nextSlippyLand = new Vector3(x + 1, y - 1, z);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                    idLandList.Add("slippy-left");
                                }
                                else
                                {
                                    nextSlippyLand = new Vector3(x, y - 1, z + 1);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                    idLandList.Add("slippy-right");
                                }
                            } //not on left or right edge
                            slippyLandChainNotSafe -= 1;
                            if (slippyLandChainNotSafe == 1)
                                nextSlippyLand = new Vector3(0, 0, 0);
                        }
                        else if (x == nextEnemyLand.x && z == nextEnemyLand.z)
                        {
                            int randomEnemy = Random.Range(0, 2); // 0 for left, 1 for right
                            if (enemyLandChain <= 0)
                                enemyLandChain = Random.Range(4, 8);

                            //Check Edge
                            if (i == 0)
                            {
                                //slippyPrefabs[0] is for direction left
                                land = Instantiate(enemyLandPrefabs[0], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextEnemyLand = new Vector3(x + 1, y - 1, z);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-left");
                                idLandList.Add("enemy-left");
                            } //i == 0 equals with on right edge
                            else if (i == boardRowTemp - 1)
                            {
                                //slippyPrefabs[1] is for direction right
                                land = Instantiate(enemyLandPrefabs[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextEnemyLand = new Vector3(x, y - 1, z + 1);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-right");
                                idLandList.Add("enemy-right");
                            } // equals with on left edge
                            else
                            {
                                land = Instantiate(enemyLandPrefabs[randomEnemy], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if (randomEnemy == 0)
                                {
                                    nextEnemyLand = new Vector3(x + 1, y - 1, z);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-left");
                                    idLandList.Add("enemy-left");
                                }
                                else if (randomEnemy == 1)
                                {
                                    nextEnemyLand = new Vector3(x, y - 1, z + 1);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-right");
                                    idLandList.Add("enemy-right");
                                }
                            } //not on left or right edge
                            enemyLandChain -= 1;
                            enemyLandPath.Add(new Vector3(x, y + 1, z));
                            if (enemyLandChain == 0)
                            {
                                nextEnemyLand = new Vector3(0, 0, 0);
                                GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                enemy.transform.SetParent(landGrup);
                                enemy.GetComponent<Enemy>().pathWalk = new List<Vector3>(enemyLandPath);
                                enemy.GetComponent<Enemy>().indexForMove = enemyLandPath.Count - 1;
                                enemyInWorldArray.Add(enemy);
                            }
                        }
                        else if (randomLand <= safeLandPriority)
                        {
                            land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                            dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                            idLandList.Add("safe");
                        }
                        else if (randomLand <= safeLandPriority + slippyLandPriority && slippyLandChainNotSafe <= 0)
                        {
                            int randomSlippy = Random.Range(0, 2); // 0 for left, 1 for right
                            if (slippyLandChainNotSafe <= 0)
                                slippyLandChainNotSafe = Random.Range(3, 6);

                            //Check Edge
                            if (i == 0)
                            {
                                //slippyPrefabs[0] is for direction left
                                land = Instantiate(slippyPrefabs[0], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextSlippyLand = new Vector3(x + 1, y - 1, z);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                idLandList.Add("slippy-left");
                            } //i == 0 equals with on right edge
                            else if (i == boardRowTemp - 1)
                            {
                                //slippyPrefabs[1] is for direction right
                                land = Instantiate(slippyPrefabs[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextSlippyLand = new Vector3(x, y - 1, z + 1);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                idLandList.Add("slippy-right");
                            } // equals with on left edge
                            else
                            {
                                int random = Random.Range(0, slippyPrefabs.Length);
                                land = Instantiate(slippyPrefabs[random], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if (random == 0)
                                {
                                    nextSlippyLand = new Vector3(x + 1, y - 1, z);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-left");
                                    idLandList.Add("slippy-left");
                                }
                                else
                                {
                                    nextSlippyLand = new Vector3(x, y - 1, z + 1);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "slippy-right");
                                    idLandList.Add("slippy-right");
                                }
                            } //not on left or right edge
                            slippyLandChainNotSafe -= 1;
                            slippyLandChainNotSafe -= 1;
                            if (slippyLandChainNotSafe == 0)
                                nextSlippyLand = new Vector3(0, 0, 0);
                        }
                        else if (randomLand <= safeLandPriority + slippyLandPriority + enemyLandPriority && enemyLandChain <= 0)
                        {
                            int randomEnemy = Random.Range(0, 2); // 0 for left, 1 for right
                            enemyLandPath.Clear();
                            enemyLandChain = Random.Range(4, 8);

                            //Check Edge
                            //Check Edge
                            if (i == 0)
                            {
                                //slippyPrefabs[0] is for direction left
                                land = Instantiate(enemyLandPrefabs[0], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextEnemyLand = new Vector3(x + 1, y - 1, z);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-left");
                                idLandList.Add("enemy-left");
                            } //i == 0 equals with on right edge
                            else if (i == boardRowTemp - 1)
                            {
                                //slippyPrefabs[1] is for direction right
                                land = Instantiate(enemyLandPrefabs[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                nextEnemyLand = new Vector3(x, y - 1, z + 1);
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-right");
                                idLandList.Add("enemy-right");
                            } // equals with on left edge
                            else
                            {
                                land = Instantiate(enemyLandPrefabs[randomEnemy], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                if (randomEnemy == 0)
                                {
                                    nextEnemyLand = new Vector3(x + 1, y - 1, z);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-left");
                                    idLandList.Add("enemy-left");
                                }
                                else if (randomEnemy == 1)
                                {
                                    nextEnemyLand = new Vector3(x, y - 1, z + 1);
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "enemy-right");
                                    idLandList.Add("enemy-right");
                                }
                            } //not on left or right edge
                            enemyLandChain -= 1;
                            enemyLandPath.Add(new Vector3(x, y + 1, z));
                            if (enemyLandChain == 0)
                                nextEnemyLand = new Vector3(0, 0, 0);

                            //land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                        }
                        //                            land = Instantiate(obstacleStaticPrefabs[Random.Range(0, obstacleStaticPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                        else if (randomLand <= safeLandPriority + slippyLandPriority + enemyLandPriority + treePriority)
                        {
                            //Debug.Log("-5" + idLandList[indexForIdLand - boardRow] + "-4" + idLandList[indexForIdLand - (boardRow - 1)]);
                            string checkRight = (x - 1).ToString() + (y + 1).ToString() + (z).ToString();
                            string checkLeft = (x).ToString() + (y + 1).ToString() + (z-1).ToString();
                            string rightValue;
                            string leftValue;

                            dictLand.TryGetValue(checkRight, out rightValue);
                            dictLand.TryGetValue(checkLeft, out leftValue);

                            //Debug.Log("Right : " + rightValue);
                            //Debug.Log("Left : " + leftValue);

                            if (leftValue != null && rightValue != null)
                            {
                                if ((leftValue.Equals("slippy-right") || leftValue.Equals("enemy-right")) && (rightValue.Equals("slippy-left") || rightValue.Equals("enemy-left")))
                                {
                                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                    idLandList.Add("safe");
                                }
                                else if(rightValue.Equals("slippy-left") || rightValue.Equals("enemy-left")) {
                                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                    idLandList.Add("safe");
                                }
                                else if (leftValue.Equals("slippy-right") || leftValue.Equals("enemy-right"))
                                {
                                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                    idLandList.Add("safe");
                                }
                                else
                                {
                                    land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                    idLandList.Add("tree");
                                }
                            }
                            else if (rightValue != null) {
                                if (rightValue.Equals("slippy-left") || rightValue.Equals("enemy-left")) {
                                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                    idLandList.Add("safe");
                                }
                                else
                                {
                                    land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                    idLandList.Add("tree");
                                }
                            }
                            else if (leftValue != null)
                            {
                                if (leftValue.Equals("slippy-right") || leftValue.Equals("enemy-right"))
                                {
                                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                    idLandList.Add("safe");
                                }
                                else
                                {
                                    land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                    idLandList.Add("tree");
                                }
                            }
                            else
                            {
                                land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                idLandList.Add("tree");
                            }

                            /*
                            if (boardRowTemp == boardRow)
                            {
                                if (i == 0)
                                {
                                    if (idLandList[indexForIdLand - (boardRowTemp - 1)] == "slippy-right" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "enemy-right")
                                    {
                                        land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                        dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                        idLandList.Add("safe");
                                    }
                                    else
                                    {
                                        land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                        dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                        idLandList.Add("tree");
                                    }
                                }
                                else if (i == boardRowTemp - 1)
                                {
                                    if (idLandList[indexForIdLand - (boardRowTemp - 1)] == "slippy-left" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "enemy-left")
                                    {
                                        land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                        dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                        idLandList.Add("safe");
                                    }
                                    else
                                    {
                                        land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                        dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                        idLandList.Add("tree");
                                    }
                                }
                                else
                                {
                                    if (idLandList[indexForIdLand - (boardRowTemp - 1)] == "slippy-right" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "enemy-right" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "slippy-left" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "enemy-left")
                                    {
                                        land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                        dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                        idLandList.Add("safe");
                                    }
                                    else
                                    {
                                        land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                        dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                        idLandList.Add("tree");
                                    }
                                }
                            }
                            else
                            {
                                if (idLandList[indexForIdLand - (boardRowTemp - 1)] == "slippy-right" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "enemy-right" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "slippy-left" || idLandList[indexForIdLand - (boardRowTemp - 1)] == "enemy-left")
                                {
                                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                                    idLandList.Add("safe");
                                }
                                else
                                {
                                    land = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "tree");
                                    idLandList.Add("tree");
                                }
                            }*/
                        }
                        else if (randomLand <= safeLandPriority + slippyLandPriority + enemyLandPriority + treePriority + obstacleStaticPriority)
                        {
                            land = Instantiate(obstacleStaticPrefabs[Random.Range(0, obstacleStaticPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                            dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "obstacle");
                            idLandList.Add("obstacle");

                        }
                        else
                        {
                            land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                            dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                            idLandList.Add("safe");

                        }


                        if (coinChance <= coinPriority && !(randomLand <= safeLandPriority + slippyLandPriority + enemyLandPriority + treePriority && randomLand >= safeLandPriority + slippyLandPriority + enemyLandPriority))
                            coinInstance = Instantiate(coinPrefab, new Vector3(x, y + 1, z), Quaternion.identity) as GameObject;

                    }
                }
                else
                {
                    land = Instantiate(landPrefabs[Random.Range(0, landPrefabs.Length)],
                        new Vector3(x, y, z),
                        Quaternion.identity) as GameObject;
                    dictLand.Add(x.ToString() + y.ToString() + z.ToString(), "safe");
                    idLandList.Add("safe");
                }   //for spawning safe land at 3 first row
                land.transform.SetParent(rowGrup);
                //Debug.Log(indexForIdLand + " : " + idLandList[indexForIdLand]);
                indexForIdLand += 1;
                if (coinInstance != null)
                    coinInstance.transform.SetParent(landGrup);
                x += 1;
                z -= 1;
                i++;
            }

            //LOGIC TO SET NEXT SAFE LAND
            //if (boardLength >= initiateSpawnSafeLand)
            //    nextSafeLand = InitiateSearchingForSafeLand(rowGrup);
            //else
            if(!hasSearchingSafeLand)
                nextSafeLand = SearchingForSafeLand(nextSafeLand, rowGrup);
            
            y -= 1;                         //change Line ke bawah
            x -= boardRowTemp;                  //Reset permulaan instantiate dari kanan
            z += boardRowTemp;                  //Reset permulaan instantiate dari kanan
            if (tooglePosXandY)
            {
                z += 1;                     //set the z position
                boardRowTemp = boardRow;              //back row to original
            }
            else if (!tooglePosXandY)
            {
                x += 1;                     //set the x position
                boardRowTemp = boardRow - 1;              //reduce row
            }
            rowGrup.SetParent(landGrup);
            tooglePosXandY = !tooglePosXandY;
            boardLength -= 1;
        }
    }

    Vector3 InitiateSearchingForSafeLand(Transform row)
    {
        List<Transform> tr = new List<Transform>();
        Transform[] trTemp = row.GetComponentsInChildren<Transform>();
        int index = 0;
        foreach (Transform t in trTemp)
        {
            if (t.tag == "SafeLand")
                tr.Add(t);
            index += 1;
        }
        Vector3 safland = tr[Random.Range(0, tr.Count - 1)].position;
        int rowCount = row.childCount;
        if(rowCount == boardRow - 1)
           return SetForNextSafeLand(safland, row);
        return SetForNextSafeLand(safland, safland == row.GetChild(0).position ? true : false);
    }
    Vector3 SearchingForSafeLand(Vector3 currentsafeland, Transform row)
    {
        int rowCount = row.childCount;
        if (currentsafeland == row.GetChild(0).position && row.childCount == boardRow)
            return SetForNextSafeLand(currentsafeland, currentsafeland == row.GetChild(0).position ? true : false);
        else if(currentsafeland == row.GetChild(row.childCount - 1).position && row.childCount == boardRow)
            return SetForNextSafeLand(currentsafeland, currentsafeland == row.GetChild(0).position ? true : false);
        else
            return SetForNextSafeLand(currentsafeland, row);
    }

    Vector3 SetForNextSafeLand(Vector3 currentSafeLand, Transform row)
    {
        Vector3 centerPos = new Vector3();          //priority for random land, if center has same priority left and right, if not, make it to imbang between left and right
        if (row.childCount % 2 == 1)
            centerPos = row.GetChild((row.childCount / 2) - 1).position;
        else
            centerPos = new Vector3(row.GetChild((row.childCount / 2) - 1).position.x + 1, row.GetChild((row.childCount / 2) - 1).position.y, row.GetChild((row.childCount / 2) - 1).position.z);
        int randomer = 0; //if 0 - 1 2left, if 3 - 4 5 right
        if (currentSafeLand.x > centerPos.x)
        {
            randomer = Random.Range(2, 6); // 2 - left, 3 4 5 right
            //Debug.Log("right");
        }
        else if (currentSafeLand.z > centerPos.z)
        {
            randomer = Random.Range(0, 4); // 0,1,2 left, 3 right
            //Debug.Log("left");
        }
        else
            randomer = Random.Range(1, 3); //1 left, 2 right
        if (randomer == 0 || randomer == 1 || randomer == 2)
            return new Vector3(currentSafeLand.x + 1, currentSafeLand.y - 1, currentSafeLand.z);
        else
            return new Vector3(currentSafeLand.x, currentSafeLand.y - 1, currentSafeLand.z + 1);
    }
    Vector3 SetForNextSafeLand(Vector3 currentSafeLand, bool ifRightTrueLeftFalse)
    {
        if (ifRightTrueLeftFalse)
            return new Vector3(currentSafeLand.x + 1, currentSafeLand.y - 1, currentSafeLand.z);
        return new Vector3(currentSafeLand.x, currentSafeLand.y - 1, currentSafeLand.z + 1);
    }

    void SpawnPlayer()
    {
        if (SaveLoad.savedGame.currentCharacter == "" || SaveLoad.savedGame.currentCharacter == null)
        {
            SaveLoad.SaveCharacterRecentUse("Bird");
            GameManager.instance.characterChoosen = Showcase.instance.characterPrefabs[0];
        }
        else
            GameManager.instance.characterChoosen = SaveLoad.getCurrentCharacter();
        GameManager.instance.instanceCharacter = Instantiate(GameManager.instance.characterChoosen, new Vector3(boardRow / 2, 1, -(boardRow / 2)), Quaternion.identity) as GameObject;
        GameManager.instance.instanceCharacter.name = GameManager.instance.characterChoosen.name;
        GameManager.instance.instanceCharacter.GetComponent<PlayerController>().enabled = true;
        GameManager.instance.instanceCharacter.transform.localScale = Vector3.one;
    }

    void setRandomizerLand()
    {
        if (landPrefabs.Length != 0)
            randomizerForLand += safeLandPriority;
        if (slippyLandActive)
            randomizerForLand += slippyLandPriority;
        if (enemyLandActive)
            randomizerForLand += slippyLandPriority;
        if (treePrefabs.Length != 0)
            randomizerForLand += treePriority;
        if (obstacleStaticPrefabs.Length != 0)
            randomizerForLand += obstacleStaticPriority;
    }

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
	}

    void Start()
    {
        nextSafeLand = new Vector3(boardRow / 2, 0, -(boardRow / 2));
        setRandomizerLand();
        initiateSpawnSafeLand = boardLength - 3;            //Save land is on 3 first collumn
        boardRowTemp = boardRow;
        boardLengthOrigin = boardLength;

        SpawnPlayer();
        InitBoard();
    }

    public void DestroyAndSpawnNewLand()
    {
        GameObject landToDestroy = GameObject.Find("RowGroup : " + landDestroyerIndex.ToString());
        if (landToDestroy != null)
        {
            meshRendererLand = landToDestroy.GetComponentsInChildren<MeshRenderer>();
            isCreateNewLand = true;
            iTween.MoveTo(landToDestroy, new Vector3(landToDestroy.transform.position.x, landToDestroy.transform.position.y - 1, landToDestroy.transform.position.z), 0.4f);
            StartCoroutine(DestroyLand(landToDestroy, 0.4f));
        }
        boardLength += 1;
        InitBoard();
        landDestroyerIndex -= 1;
    }

    void FadeOutMeshRenderer(MeshRenderer[] meshrenderer)
    {
        foreach (MeshRenderer mr in meshrenderer)
        {
            mr.material.color = Color.Lerp(mr.material.color, 
                new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, 0), 
                4 * Time.deltaTime);
        }
    }

    void FadeOutMeshRenderer(MeshRenderer[] meshrenderer, float time)
    {
        foreach (MeshRenderer mr in meshrenderer)
        {
            mr.material.color = Color.Lerp(mr.material.color,
                new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, 0),
                time * Time.deltaTime);
        }
    }

    IEnumerator DestroyLand(GameObject land, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(land.gameObject);
        isCreateNewLand = false;
    }
    IEnumerator DestroyCurrentLand(GameObject land, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(land.gameObject);
        isCreateNewLand = false;
    }

    // Update is called once per frame
    void Update () {
        if (isCreateNewLand)
        {
            //Fading Out Land
            FadeOutMeshRenderer(meshRendererLand);
        }
    }

    public void Restart()
    {
        Invoke("DestroyAllAndRestart", .8f);
    }

    void DestroyAllAndRestart()
    {
        Destroy(landGrup.gameObject);
        Destroy(GameManager.instance.instanceCharacter);
        foreach (GameObject enemy in enemyInWorldArray)
            Destroy(enemy);
        landGrup = new GameObject("landGroup").transform;

        GameManager.instance.score = 0;

        idLandList.Clear();
        dictLand.Clear();
        indexForIdLand = 0;
        hasSearchingSafeLand = false;

        boardRowTemp = boardRow;
        boardLength = boardLengthOrigin;
        landDestroyerIndex = 2;
        tooglePosXandY = false;
        nextSafeLand = new Vector3(boardRow / 2, 0, -(boardRow / 2));
        x = 0;y = 0;z = 0;
        ResetAllVariable();
        Invoke("InitiateBoardAgain", .3f);
    }

    void InitiateBoardAgain()
    {
        SpawnPlayer();
        Camera.main.GetComponent<CameraFollow>().setPosToOrigin();
        InitBoard();
    }

    void ResetAllVariable()
    {
        landGrup.name = "LandGroupOld";
        landGrup = new GameObject("landGroup").transform;
        boardRowTemp = boardRow;
        boardLength = boardLengthOrigin;
        landDestroyerIndex = 2;
        tooglePosXandY = false;
        slippyLandChain = 0;
        slippyLandChainNotSafe = 0;
    }
}
