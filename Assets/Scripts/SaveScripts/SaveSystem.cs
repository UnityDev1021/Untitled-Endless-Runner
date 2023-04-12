using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Untitled_Endless_Runner
{
    public class SaveSystem : MonoBehaviour
    {
        public static void SaveHighScore(PlayerData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/UER_Player.data";
            FileStream stream;

            if (!File.Exists(path))
            {
                stream = new FileStream(path, FileMode.Create);
            }
            else
            {
                stream = new FileStream(path, FileMode.Open);
                stream.SetLength(0);
            }

            formatter.Serialize(stream, data);

            stream.Close();

            //Debug.Log("Data Saved");
        }

        public static PlayerData LoadHighScore()
        {
            string path = Application.persistentDataPath + "/UER_Player.data";
            //Debug.Log($"Path : {path}");

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                PlayerData pData = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                //foreach (PlayerData data in pData)            //works good
                {
                    //Debug.Log("Loaded Data : " + data.name + ", score : " + data.score);
                }

                return pData;
            }
            else
            {
                Debug.Log("File Does Not Exist");

                return null;
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string name = "Player";
        public int score;

        public PlayerData(int pScore, string pName = "Player")
        {
            name = pName;
            score = pScore;
        }
    }
}