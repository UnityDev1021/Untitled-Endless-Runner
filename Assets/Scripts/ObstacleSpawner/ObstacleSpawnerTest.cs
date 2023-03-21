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
            obstacleGroupIndex = ChooseObstacleGroup();

            SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].firstObstacleIndex);
            SetObstaclePosition(ref obstacleGroups[obstacleGroupIndex].secondObstacleindex);
            ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].firstObstacleIndex].tag, tempSpawnPos, Quaternion.identity);

            tempSpawnPos = new Vector3(tempSpawnPos.x + obstacleGroups[obstacleGroupIndex].distance, tempSpawnPos.y);
            ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleGroups[obstacleGroupIndex].secondObstacleindex].tag, tempSpawnPos, Quaternion.identity);

            Invoke("SpawnObstacle", spawnTime);
            //Debug.Log($"Spawning Obstacle : {enemyUnitTags[enemyUnitIndex]}");
        }

        private byte ChooseObstacleGroup()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(0, obstacleGroups.Length);

            return obstacleUnitIndex;
        }

        private void SetObstaclePosition(ref byte obstacleUnitIndex)
        {
            //Check where to spawn for different objects
            if (obstacleUnitIndex < enemyUnitStats.Length - 3)
            {
                //byte randomSpawnIndex = (byte)Random.Range(0, spawnPointsAbove.Length);              //Last point is for those obstacles that spawn on the ground
                byte randomSpawnIndex = 0;
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f, spawnPointsAbove[randomSpawnIndex].y + mainCamera.transform.position.y, 0f);
            }
            else if (obstacleUnitIndex == 6)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f, spawnPointsBelow[0].y, 0f);
            else if (obstacleUnitIndex == 7)
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f, spawnPointsBelow[1].y, 0f);
            else
                tempSpawnPos = new Vector3(mainCamera.transform.position.x + 12f, spawnPointsBelow[2].y, 0f);


        }
    }
}