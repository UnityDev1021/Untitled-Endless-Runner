using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3[] spawnPointsAbove, spawnPointsBelow;
        [SerializeField] private float[] spawnTime;
        [SerializeField] private ObstacleStat[] enemyUnitStats;
        private Vector3 tempSpawnPos;

        //Obstacle Spawn Groups
        private byte[][] obstacleGroups = new byte[2][] { new byte[]{ 0, 0 }, new byte[]{ 0, 1 } };

        // Start is called before the first frame update
        void Start()
        {
            Invoke("SpawnObstacle", spawnTime[0]);
        }

        public void SpawnObstacle()
        {
            byte obstacleUnitIndex = 0;
            //obstacleUnitIndex = ChooseObstacleIndex();
            obstacleUnitIndex = 6;                                              //Uncomment for test

            SetObstaclePosition(ref obstacleUnitIndex);
            ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleUnitIndex].tag, tempSpawnPos, Quaternion.identity);
            ObstaclePoolManager.instance.ReUseObstacle(enemyUnitStats[obstacleUnitIndex].tag, tempSpawnPos, Quaternion.identity);

            Invoke("SpawnObstacle", spawnTime[0]);
            //Debug.Log($"Spawning Obstacle : {enemyUnitTags[enemyUnitIndex]}");
        }

        private byte ChooseObstacleIndex()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(0, enemyUnitStats.Length);

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