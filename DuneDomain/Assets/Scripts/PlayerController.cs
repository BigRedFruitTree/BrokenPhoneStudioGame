using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    

    [Header("Player Stats")]
    public int health = 5;
    public int maxHealth = 5;
    public int healthRestore = 5;
    public int weapon = 0;

    [Header("Movement Settings")]
    public float speed = 10.0f;
    public float groundDetectDistance = 1.5f;
    public bool sprintMode = false;
    public bool isGrounded = true;
    public float stamina = 150f;

    [Header("Input System")]
    public InputActionAsset playerCntrols;
    public InputHandler inputHandler;

    [Header("Misc Refrences")]
    public GameObject PlayerObject;
    private Rigidbody myRB;
    public Camera playerCam;
    Transform cameraHolder;
    public GameManager gm;
    public GameObject weaponHolder;
    public GameObject sword;
    public GameObject bow;


    // Start is called before the first frame update
    void Start()
    {
        inputHandler = InputHandler.Instance;
        myRB = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        cameraHolder = transform.GetChild(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if(gm.GameOn == true && gm.GameOver == false)
        {
            weapon = gm.weapon;

            playerCam.transform.position = cameraHolder.position;

            Vector3 temp = myRB.velocity;

            float verticalMove = Input.GetAxisRaw("Vertical");
            float horizontalMove = Input.GetAxisRaw("Horizontal");

            temp.x = verticalMove * speed;
            temp.z = horizontalMove * speed;

            myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);

            if(horizontalMove > 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, 90f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, 90f);
            }
            if(horizontalMove > 0 && verticalMove > 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, -45f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, -45f);
            }
            if(horizontalMove > 0 && verticalMove < 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, 45f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, 45f);
            }

            if(horizontalMove < 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, 90f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, 90f);
            }
            if(horizontalMove < 0 && verticalMove > 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, 45f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, 45f);
            }
            if(horizontalMove < 0 && verticalMove < 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, -45f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, -45f);
            }
            if(verticalMove > 0 && horizontalMove == 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
            if(verticalMove < 0 && horizontalMove == 0) 
            {
               sword.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = Quaternion.Euler(90f, 0f, 180f);
               bow.transform.position = weaponHolder.transform.position;
               bow.transform.rotation = Quaternion.Euler(90f, 0f, 180f);
            }
              
            

            if (health < 0)
                health = 0;

            if (health == 0)
                gm.GameOver = true;

            
            weaponHolder.transform.position = transform.position + new Vector3(horizontalMove, 0,  verticalMove);

        }

    }
}