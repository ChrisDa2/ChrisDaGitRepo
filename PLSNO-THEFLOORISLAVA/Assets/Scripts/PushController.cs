using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PushController : NetworkBehaviour
{
    [Header("Push Parameters")]
    public bool canPush = true;
    public bool pushing = false;

    public List<GameObject> players = new List<GameObject>();

    private void Update()
    {
        if (!IsOwner) return;
        else
        {
            PushingLocal();
        }
    }

    private void PushingLocal()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPush)
        {
            Debug.Log("Ieeeee");
            pushing = true;
            canPush = false;

            PushClientRpc();
            PushServerRpc();
            Invoke(nameof(CanPushAgain), 1f);
            Invoke(nameof(DeactivatePush), 1f);
        }
    }

    [ClientRpc]
    private void PushClientRpc()
    {
        Debug.Log("Entro");
        pushing = true;
        canPush = false;

        GoPush();
        
        Invoke(nameof(CanPushAgain), 1f);
        Invoke(nameof(DeactivatePush), 1f);
    }

    [ServerRpc]
    private void PushServerRpc()
    {
        Debug.Log("Entro");
        pushing = true;
        canPush = false;

        GoPush();

        Invoke(nameof(CanPushAgain), 1f);
        Invoke(nameof(DeactivatePush), 1f);
    }

    private void GoPush()
    {
        Debug.Log("GoPush");
        if (pushing)
        {
            Debug.Log("GoPush1");
            for (int i = 0; i < players.Count; i++)
            {
                Debug.Log("GoPush2");
                players[i].GetComponent<Rigidbody2D>().AddForce(transform.right * 10, ForceMode2D.Impulse);
            }
        }
    }

    private void DeactivatePush()
    {
        pushing = false;
    }

    private void CanPushAgain()
    {
        canPush = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            if (collision.GetComponent<PlayerMovement>() && !players.Contains(collision.gameObject))
            {
                players.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            if (collision.GetComponent<PlayerMovement>())
            {
                players.Remove(collision.gameObject);
            }
        }
    }
}
