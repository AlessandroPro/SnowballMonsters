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
    public bool isDead = false;


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
        if(base.photonView.IsMine && !isDead)
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
            anim.SetTrigger("Attack 01");
        }
    }

    public void throwSnowball()
    {
        if(base.photonView.IsMine)
        {
            Vector3 snowballSpawnOffset = new Vector3(transform.forward.x, 0.5f, 0);
            GameObject snowball = PhotonNetwork.Instantiate("Snowball", transform.position + snowballSpawnOffset, transform.rotation);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PhotonNetwork.LocalPlayer.NickName);
            stream.SendNext(isDead);
        }
        else
        {
            playerNameText.text = (string)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
        }
    }

    void die()
    {
        anim.SetTrigger("Die");
        isDead = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Snowball"))
        {
            if(base.photonView.IsMine && !isDead)
            {
                die();
                collision.transform.GetComponent<PhotonView>().RPC("DestroySelf", RpcTarget.All);
            }
        }
    }

    public void endGame()
    {
        FindObjectOfType<GameManager>().GameOver();
    }
}
