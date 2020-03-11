using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public GameObject winnerUI;

        public GameObject player1SpawnPosition;
        public GameObject player2SpawnPosition;

        private GameObject player1;
        private GameObject player2;

        // Start Method
        void Start()
        {
            if (!PhotonNetwork.IsConnected) // 1
            {
                SceneManager.LoadScene("Launcher");
                return;
            }

            if (PlayerManager.LocalPlayerInstance == null)
            {
                if (PhotonNetwork.IsMasterClient) // 2
                {
                    Debug.Log("Instantiating Player 1");

                    player1 = PhotonNetwork.Instantiate("SnowMonster", player1SpawnPosition.transform.position, player1SpawnPosition.transform.rotation, 0);
                }
                else 
                {
                    player2 = PhotonNetwork.Instantiate("SnowMonster", player2SpawnPosition.transform.position, player2SpawnPosition.transform.rotation, 0);
                }
            }
        }

        // Update Method
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //1
            {
                Application.Quit();
            }
        }

        // Photon Methods
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Launcher");
            }
        }

        // Helper Methods
        public void QuitRoom()
        {
            Application.Quit();
        }
    }
}

