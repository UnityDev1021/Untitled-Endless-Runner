using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ObstacleSpawnerTest : MonoBehaviour
    {
        [Header("Test Variables")]
        [SerializeField] private bool enableSpawn;

        [Space]
        [SerializeField] private Camera mainCamera;
        private float parallaxEffect = 0.1f, startPosX;
        //[SerializeField] private Vector3[] spawnPointsAbove, spawnPointsBelow;              //Why Vector3??
        [SerializeField] private float[] spawnPointsAbove, spawnPointsBelow;              //Why Vector3??
        [SerializeField] private float initialSpawnTime;
        [SerializeField] private ObstacleStat[] enemyUnitStats;
        private Vector3 tempSpawnPos;
        [SerializeField] private byte obstacleGroupIndex = 0;

        //Obstacle Spawn Groups
        [SerializeField] private ObstacleGroup[] obstacleGroups;

        // Start is called before the first frame update
        void Start()
        {
            Invoke("SpawnObstacle", initialSpawnTime);
            startPosX = transform.position.x;
        }

        public void SpawnObstacle()
        {
            if (enableSpawn)
            {
                //enableSpawn = false;

                //obstacleGroupIndex = ChooseObstacleGroup();
                for (int i = 0; i < obstacleGroups[obstacleGroupIndex].obstaclesIndex.Length; i++)
                {
                    SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].obstaclesIndex[i], obstacleGroups[obstacleGroupIndex].disX[i]);
                    GameObject tempObstacle = ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].obstaclesIndex[i]].tag, tempSpawnPos, Quaternion.identity);

                    if (obstacleGroups[obstacleGroupIndex].obstacleGroupType[i] != 0)
                        tempObstacle.GetComponent<BaseObstacleController>().AssignGroupTypes();
                }

                //SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].firstObstacleIndex);
                //ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].firstObstacleIndex].tag, tempSpawnPos, Quaternion.identity);

                Invoke("SpawnObstacle", obstacleGroups[obstacleGroupIndex].spawnNextAfter);
                //Debug.Log($"Spawning Obstacle : {enemyUnitTags[enemyUnitIndex]}");
            }
        }

        private void FixedUpdate()
        {
            //To Move along with the ground
            transform.position = new Vector3(startPosX + (mainCamera.transform.position.x * parallaxEffect), transform.position.y, transform.position.z);
        }

        private byte ChooseObstacleGroup()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(0, obstacleGroups.Length);

            return obstacleUnitIndex;
        }

        private void SetObstaclePosition(ref byte obstacleUnitIndex, float addDisX = 0f, float addDisY = 0f)
        {
            //Check where to spawn for different objects
            if (obstacleUnitIndex < 5)
            {
                byte randomSpawnIndex = (byte)Random.Range(0, spawnPointsAbove.Length);              //Last point is for those obstacles that spawn on the ground
                //byte randomSpawnIndex = 0;                            //Uncomment for test
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, spawnPointsAbove[randomSpawnIndex] + mainCamera.transform.position.y, 0f);
                //Debug.Log($"Chose Random Point : {randomSpawnIndex}");
            }
            else if (obstacleUnitIndex == 5)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, spawnPointsBelow[0] + addDisY, 0f);
            else if (obstacleUnitIndex == 6)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, spawnPointsBelow[1], 0f);
            else if (obstacleUnitIndex == 7)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, spawnPointsBelow[2], 0f);
            else
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, spawnPointsBelow[3], 0f);

            //Debug.Log($"obstacleUnitIndex : {obstacleUnitIndex} ,tempSpawnPos : {tempSpawnPos}, addDisX : {addDisX}, addDisY : {addDisY}");
        }
    }
}