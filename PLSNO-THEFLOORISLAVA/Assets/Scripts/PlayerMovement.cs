using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rig;
    //public MeshRenderer m_MeshRenderer;
    public SpriteRenderer spriteRenderer;
    public Sprite[] prueba;

    [Header("Movement Parameters")]
    [SerializeField]private int moveSpeed;

    [Header("Jump Parameters")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private Vector2 vecGravity;

    [SerializeField]private int jumpForce;

    [SerializeField]private float fallMultiplier;
    [SerializeField]private float jumpTime;
    [SerializeField]private float jumpMultiplier;
    private float jumpCounter;

    private bool isJumping;

    public GameObject cam;
    public GameObject bounds;

    void Start()
    {
        if(IsOwner)
        {
            Spawn();
        }
        
        
    }
    public override void OnNetworkSpawn() //Change the sprite of the player
    {
        base.OnNetworkSpawn();
        spriteRenderer.sprite = prueba[OwnerClientId];
    }

    // Update is called once per frame
    private void Spawn() //Spawn on the different positions
    {
        //Hacer un array de posiciones, que cada uno salga dependiendo de su ownerClientId en una posicion
        transform.position = new Vector3(0,0,0);
        //GameObject.Find("Main Camera").SetActive(false);
        //cam.transform.parent = null;
        //cam.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        else
        {
            Movement();
            Jump();
        }
    }

    private void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        //if (moveHorizontal < 0) //Looking left
        //{
        //    GetComponent<SpriteRenderer>().flipX = true;
        //    transform.Translate(moveHorizontal * moveSpeed * transform.right * Time.deltaTime, Space.World);
        //}
        //if (moveHorizontal > 0) //Looking right
        //{
        //    GetComponent<SpriteRenderer>().flipX = false;
        //    transform.Translate(moveHorizontal * moveSpeed * transform.right * Time.deltaTime, Space.World);
        //}


        if (moveHorizontal < 0) //Looking left
        {
            transform.Translate(moveHorizontal * moveSpeed * -transform.right * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            cam.transform.position = new Vector3(0, 0, 20);
            cam.transform.rotation = Quaternion.Euler(0, -180, 0);

        }
        if (moveHorizontal > 0) //Looking right
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.Translate(moveHorizontal * moveSpeed * transform.right * Time.deltaTime, Space.World);
            
            cam.transform.position = new Vector3(0, 0, -20);
            cam.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    #region Jump
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rig.velocity = new Vector2(rig.velocity.x, jumpForce);
            isJumping = true;
            jumpCounter = 0;
        }
            
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
