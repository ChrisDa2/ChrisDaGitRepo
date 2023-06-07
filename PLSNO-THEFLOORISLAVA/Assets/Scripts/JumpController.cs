using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class JumpController : NetworkBehaviour
{
    private Rigidbody2D rig;

    [Header("Jump Parameters")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private Vector2 vecGravity;

    [SerializeField] private int jumpForce;

    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpMultiplier;
    private float jumpCounter;

    private bool isJumping;

    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Debug.Log("Entro en la condicion del space y suelo");
            ServerSaysJumpServerRpc();
        }
    }

    [ServerRpc]
    private void ServerSaysJumpServerRpc()
    {
        Debug.Log("Entro en el serversays");
        if (!IsServer)
        {
            Debug.Log("No soy server así que hago el jump");
            Debug.Log("Entro en el jump de dentro del serversays");

            rig.velocity = new Vector2(rig.velocity.x, jumpForce);
            isJumping = true;
            jumpCounter = 0;

            if (rig.velocity.y > 0 && isJumping)
            {
                jumpCounter += Time.deltaTime;
                if (jumpCounter > jumpTime) isJumping = false;

                float t = jumpCounter / jumpTime;
                float currentJumpM = jumpMultiplier;

                if (t > 0.5f)
                {
                    currentJumpM = jumpMultiplier * (1 - t);
                }

                rig.velocity += vecGravity * jumpMultiplier * Time.deltaTime;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                jumpCounter = 0;

                if (rig.velocity.y < 0)
                {
                    rig.velocity = new Vector2(rig.velocity.x, rig.velocity.y * .6f);
                }
            }

            if (rig.velocity.y < 0)
            {
                rig.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
            }
        }
        jumpLocallyClientServerRpc();
    }


    [ServerRpc]
    private void jumpLocallyClientServerRpc()
    {
        Debug.Log("Entro en el jumplocally");
        Jump();
    }
    #region Jump
    private void Jump()
    {
        Debug.Log("Entro en el jump normal");
        rig.velocity = new Vector2(rig.velocity.x, jumpForce);
        isJumping = true;
        jumpCounter = 0;

        if (rig.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime) isJumping = false;

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rig.velocity += vecGravity * jumpMultiplier * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            jumpCounter = 0;

            if (rig.velocity.y < 0)
            {
                rig.velocity = new Vector2(rig.velocity.x, rig.velocity.y * .6f);
            }
        }

        if (rig.velocity.y < 0)
        {
            rig.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.8f, 0.3f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
    #endregion
}
