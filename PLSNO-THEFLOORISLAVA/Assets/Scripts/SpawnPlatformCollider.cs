using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnPlatformCollider : NetworkBehaviour
{
    public GameObject[] platforms;
    public Transform spawnPos;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(collision.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("InstantiatePlatform"))
        {
            Debug.Log("Entro");
            IntantiatePlatformServerRpc();
        }
        Destroy(collision.gameObject);





        //Debug.Log("Entro");
        //spawnPos.gameObject.transform.SetParent(null);
        //spawnPos.transform.localScale = new Vector3(1, 1, 1);
        
        ////prueba.gameObject.transform.SetParent(null);
        //this.gameObject.SetActive(false);
        
    }

    [ServerRpc]
    private void IntantiatePlatformServerRpc()
    {
        Debug.Log("Entro al instantiateServer");
        IntantiatePlatformClientRpc();
    }  
    
    [ClientRpc]
    private void IntantiatePlatformClientRpc()
    {
        Debug.Log("Entro al instantiateClient");
        GameObject prueba = Instantiate(platforms[Random.Range(0, 1)], spawnPos.position, Quaternion.identity);
        //prueba.gameObject.transform.SetParent(null);
    }
}
