using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowMonsterController : MonoBehaviourPun, IPunObservable
{

    public float speed;
    public Animator anim;
    public Text playerNameText;


    private void Start()
    {
        if (base.photonView.IsMine)
        {
            playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(base.photonView.IsMine)
        {
            UpdateMovement();
            UpdateAttack();
        }
    }

    void UpdateMovement()
    {
        int dir = 0;

        if(Input.GetKey(KeyCode.UpArrow))
        {
            dir = 1;
            
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            dir = -1;
        }

        if(dir == 0)
        {
            anim.SetBool("Run Forward", false);
        }
        else
        {
            anim.SetBool("Run Forward", true);
        }

        transform.position += Vector3.forward * speed * dir * Time.deltaTime;
    }

    void UpdateAttack()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            throwSnowball();
        }
    }

    void throwSnowball()
    {
        anim.SetTrigger("Attack 01");
        Vector3 snowballSpawnOffset = new Vector3(0.5f, 0.5f, 0);
        GameObject snowball = PhotonNetwork.Instantiate("Snowball", transform.position + snowballSpawnOffset, transform.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    //private void OnEnable()
    //{
    //    PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    //}

    //private void OnDisable()
    //{
    //    PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    //}

    //private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
    //{
    //    if(obj.Code == 0)
    //    {

    //    }
    //}
}
