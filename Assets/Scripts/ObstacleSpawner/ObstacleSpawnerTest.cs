//#define TEST_MODE

using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ObstacleSpawnerTest : MonoBehaviour
    {
        //[Header("Test Variables")]
        //[SerializeField] private bool enableSpawn;

        [Space]
        [SerializeField] private Camera mainCamera;
        private float parallaxEffect = 0.1f, startPosX;
        //[SerializeField] private Vector3[] spawnPointsAbove, spawnPointsBelow;              //Why Vector3??
        [SerializeField] private float[] spawnPointsAbove, spawnPointsBelow, comboSpawnPoints, headSpawnPoints;              //Why Vector3??
        [SerializeField] private float initialSpawnTime, timeAtSpawn;
        [SerializeField] private ObstacleStat[] enemyUnitStats;
        private Vector3 tempSpawnPos;
        [SerializeField] private byte obstacleGroupIndex = 0;

        //Obstacle Spawn Groups
        [SerializeField] private ObstacleGroup[] obstacleGroups;
        [SerializeField] private bool spawnEnabled = true;                  //Serialize for test

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnPause_ResumeClicked += ToggleSpawn;
            localGameLogic.OnPlayerHealthOver += ResetStats;
            localGameLogic.OnGameplayContinued += ContinueMainGameplay;

            spawnEnabled = true;
            Invoke(nameof(SpawnObstacle), initialSpawnTime);
            startPosX = transform.position.x;
        }

        private void OnDisable()
        {
            localGameLogic.OnPause_ResumeClicked -= ToggleSpawn;
            localGameLogic.OnPlayerHealthOver -= ResetStats;
            localGameLogic.OnGameplayContinued -= ContinueMainGameplay;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Invoke(nameof(SpawnObstacle), initialSpawnTime);
            //startPosX = transform.position.x;
        }

        private void ResetStats()
        {
            transform.localPosition = Vector3.zero;
            ToggleSpawn(false);
            Debug.Log($"Calling For Reset : {spawnEnabled}");
        }

        public void SpawnObstacle()
        {
            //enableSpawn = false;
            timeAtSpawn = Time.unscaledTime;

#if TEST_MODE
            obstacleGroupIndex = 0;
#else
            obstacleGroupIndex = ChooseObstacleGroup();                   //Uncomment for Real Gameplay
#endif
            for (int i = 0; i < obstacleGroups[obstacleGroupIndex].obstaclesIndex.Length; i++)
            {
                SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].obstaclesIndex[i], ref obstacleGroups[obstacleGroupIndex].disX[i]);
                GameObject tempObstacle = ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].obstaclesIndex[i]].tag, tempSpawnPos, Quaternion.identity);

                //if (obstacleGroups[obstacleGroupIndex].obstacleGroupType[i] != 0)
                tempObstacle.GetComponent<BaseObstacleController>().AssignGroupTypes(obstacleGroups[obstacleGroupIndex].obstacleGroupType[i], tempSpawnPos.y);

                //if (enemyUnitStats[obstacleGroups[obstacleGroupIndex].obstaclesIndex[i]].tag.CompareTo(ObstacleTag.MetalPlate_Spike) == 0)
                //    tempObstacle.GetComponent<BlockController>().AssignIndex((byte)i);

                tempObstacle.SetActive(true);
                //Debug.Log($"Object : {tempObstacle.name}, status : {tempObstacle.activeSelf}");
            }

            //SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].firstObstacleIndex);
            //ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].firstObstacleIndex].tag, tempSpawnPos, Quaternion.identity);

            if (spawnEnabled)
                Invoke(nameof(SpawnObstacle), obstacleGroups[obstacleGroupIndex].spawnNextAfter);
            //Debug.Log($"Spawning Obstacle : {obstacleGroups[obstacleGroupIndex].name}, Spawn After : {obstacleGroups[obstacleGroupIndex].spawnNextAfter}");
        }

        private void ContinueMainGameplay()
        {
            Invoke(nameof(InvokeToggleSpawn), 1.5f);
        }

        private void InvokeToggleSpawn()
        {
            ToggleSpawn(true);
        }

        private void ToggleSpawn(bool toggleValue)
        {
            //As this is called multiple times. i.e. at restart is twice called
            if (GameManager.instance.gameStarted)
            {
                spawnEnabled = toggleValue;
                //Debug.Log($"Time Now : {Time.unscaledTime}");

                //In case the next obstacle is in the process of spawning and gets stopped as the spawn variable is not enabled
                //Invoke the spawn func with the time left before the pause button was clicked
                if (spawnEnabled)
                    Invoke(nameof(SpawnObstacle), obstacleGroups[obstacleGroupIndex].spawnNextAfter - timeAtSpawn);
                else
                {
                    //Get the time difference between the invoke time and the time went when the pause button was clicked
                    timeAtSpawn = Time.unscaledTime - timeAtSpawn;
                    CancelInvoke(nameof(SpawnObstacle));
                }
            }
            else
                this.enabled = false;               //If Called again during Restart

            //Debug.Log($"Toggle Spawn status : {spawnEnabled}");
        }

        private void FixedUpdate()
        {
            //To Move along with the ground
            transform.position = new Vector3(startPosX + (mainCamera.transform.position.x * parallaxEffect), transform.position.y, transform.position.z);
        }

        private byte ChooseObstacleGroup()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(1, obstacleGroups.Length);           //0 would be for Test

            return obstacleUnitIndex;
        }

        private void SetObstaclePosition(ref byte obstacleUnitIndex, ref float addDisX, float addDisY = 0f)
        {
            //Check where to spawn for different objects
            if (obstacleUnitIndex < 2 || obstacleUnitIndex >= 11)
            {
                byte randomSpawnIndex = (byte)Random.Range(0, spawnPointsAbove.Length);              //Last point is for those obstacles that spawn on the ground
                //byte randomSpawnIndex = 0;                            //Uncomment for test
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    spawnPointsAbove[randomSpawnIndex] + mainCamera.transform.position.y, 0f);
                //Debug.Log($"Chose Random Point : {randomSpawnIndex}");
            }

            else if (obstacleUnitIndex == 2 || obstacleUnitIndex == 3)
            {
                //byte randomSpawnIndex = (byte)Random.Range(0, spawnPointsAbove.Length);              //Last point is for those obstacles that spawn on the ground
                //byte randomSpawnIndex = 2;                    //Does not matter as the y-position will be replaced in the further steps
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    headSpawnPoints[1] + addDisY, 0f);                
            }

            else if (obstacleUnitIndex == 4)
            {
                byte randomSpawnIndex = (byte)Random.Range(3, 5);              //Last point is for those obstacles that spawn on the ground
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    spawnPointsAbove[randomSpawnIndex] + addDisY, 0f);                
            }

            else if (obstacleUnitIndex == 5)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    spawnPointsBelow[0] + addDisY, 0f);

            else if (obstacleUnitIndex == 6)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    spawnPointsBelow[1] + addDisY, 0f);

            else if (obstacleUnitIndex == 7)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    spawnPointsBelow[2] + addDisY, 0f);

            else if (obstacleUnitIndex == 8)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    spawnPointsBelow[3] + addDisY, 0f);

            else if (obstacleUnitIndex == 9)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + addDisX, 
                    comboSpawnPoints[0] + addDisY, 0f);

            //Debug.Log($"obstacleUnitIndex : {obstacleUnitIndex} ,tempSpawnPos : {tempSpawnPos}, addDisX : {addDisX}, addDisY : {addDisY}");
        }
    }
}