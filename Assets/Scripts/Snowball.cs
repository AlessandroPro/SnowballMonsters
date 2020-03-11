﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Snowball : MonoBehaviourPun
{
    public float speed;
    public GameObject poofPefab;

    // Update is called once per frame
    void Update()
    {
        if (base.photonView.IsMine)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(base.photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}