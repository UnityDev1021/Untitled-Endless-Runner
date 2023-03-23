using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ObstacleSpawnerTest : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3[] spawnPointsAbove, spawnPointsBelow;              //Why Vector3??
        [SerializeField] private float spawnTime;
        [SerializeField] private ObstacleStat[] enemyUnitStats;
        private Vector3 tempSpawnPos;
        [SerializeField] private byte obstacleGroupIndex = 0;

        //Obstacle Spawn Groups
        [SerializeField] private ObstacleGroup[] obstacleGroups;

        // Start is called before the first frame update
        void Start()
        {
            Invoke("SpawnObstacle", spawnTime);
        }

        public void SpawnObstacle()
        {
            //obstacleGroupIndex = ChooseObstacleGroup();
            for (int i = 0; i < obstacleGroups[obstacleGroupIndex].obstaclesIndex.Length; i++)
            {
                SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].obstaclesIndex[i], obstacleGroups[obstacleGroupIndex].distance[i]);
                ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].obstaclesIndex[i]].tag, tempSpawnPos, Quaternion.identity);
            }

            //SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].firstObstacleIndex);
            //ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].firstObstacleIndex].tag, tempSpawnPos, Quaternion.identity);

            Invoke("SpawnObstacle", spawnTime);
            //Debug.Log($"Spawning Obstacle : {enemyUnitTags[enemyUnitIndex]}");
        }

        private byte ChooseObstacleGroup()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(0, obstacleGroups.Length);

            return obstacleUnitIndex;
        }

        private void SetObstaclePosition(ref byte obstacleUnitIndex, float additionalDistance = 0f)
        {
            //Check where to spawn for different objects
            if (obstacleUnitIndex < enemyUnitStats.Length - 3)
            {
                byte randomSpawnIndex = (byte)Random.Range(0, spawnPointsAbove.Length);              //Last point is for those obstacles that spawn on the ground
                //byte randomSpawnIndex = 0;                            //Uncomment for test
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + additionalDistance, spawnPointsAbove[randomSpawnIndex].y + mainCamera.transform.position.y, 0f);
                //Debug.Log($"Chose Random Point : {randomSpawnIndex}");
            }
            else if (obstacleUnitIndex == 6)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + additionalDistance, spawnPointsBelow[0].y, 0f);
            else if (obstacleUnitIndex == 7)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + additionalDistance, spawnPointsBelow[1].y, 0f);
            else
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f + additionalDistance, spawnPointsBelow[2].y, 0f);

            //Debug.Log($"obstacleUnitIndex : {obstacleUnitIndex} ,tempSpawnPos : {tempSpawnPos}");
        }
    }
}