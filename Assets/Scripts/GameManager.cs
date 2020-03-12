using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject winnerUI;
    public Text winnerText;

    public GameObject player1SpawnPosition;
    public GameObject player2SpawnPosition;

    private bool gameOver = false;

    private GameObject player1;
    private GameObject player2;

    public static bool returnToLobby = false;

    // Start Method
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");
            return;
        }

        if (PhotonNetwork.IsMasterClient) 
        {
            Debug.Log("Instantiating Player 1");

            player1 = PhotonNetwork.Instantiate("SnowMonster", player1SpawnPosition.transform.position, player1SpawnPosition.transform.rotation, 0);
        }
        else 
        {
            player2 = PhotonNetwork.Instantiate("SnowMonster", player2SpawnPosition.transform.position, player2SpawnPosition.transform.rotation, 0);
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        returnToLobby = true;
        PhotonNetwork.LoadLevel("Launcher");
    }

    public override void OnPlayerLeftRoom(Player other)
    {
         Debug.Log("OnPlayerLeftRoom() " + other.NickName);
         PhotonNetwork.LeaveRoom();
    }


    public void QuitRoom()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        if(gameOver == false)
        {
            gameOver = true;
            winnerUI.SetActive(true);

            SnowMonsterController player = null;

            if (player1)
            {
                player = player1.GetComponent<SnowMonsterController>();
            }
            else if(player2)
            {
                player = player2.GetComponent<SnowMonsterController>();
            }


            if (player.isDead)
            {
                winnerText.text = "You Lose!";
            }
            else
            {
                winnerText.text = "You Win!";
            }
            
            StartCoroutine(BackToLobby());
        }
    }

    IEnumerator BackToLobby()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.LeaveRoom();
    }
}

