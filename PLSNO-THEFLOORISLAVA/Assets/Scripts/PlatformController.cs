using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlatformController : NetworkBehaviour
{
    private float speed = 0.01f;
    //public GameObject colliderChecker;
    private void Start()
    {
        Debug.Log("Empiezo");
        //colliderChecker.gameObject.transform.SetParent(null);
        NetworkObject.Spawn();
    }
    private void Update()
    {
        MovePlatforms();
    }

    private void MovePlatforms()
    {
        MovePlatformsServerRpc();
    }
    [ServerRpc]
    private void MovePlatformsServerRpc()
    {
        MovePlatformsClientRpc();
    }
    [ClientRpc]
    private void MovePlatformsClientRpc()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            transform.position += new Vector3(0, -speed, 0);
        }
    }
}
