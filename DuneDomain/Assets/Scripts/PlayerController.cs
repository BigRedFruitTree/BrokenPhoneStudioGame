using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public GameObject PlayerObject;
    private Rigidbody myRB;
    public Camera playerCam;
    Transform cameraHolder;

    [Header("Player Stats")]
    public int health = 5;
    public int maxHealth = 5;
    public int healthRestore = 5;

    [Header("Movement Settings")]
    public float speed = 10.0f;
    public float jumpHeight = 10.0f;
    public float groundDetectDistance = 1.5f;
    public bool sprintMode = false;
    public bool isGrounded = true;
    public float stamina = 150f;

    [Header("User Settings")]
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public bool GameOver = false;

    [Header("Input System")]
    public InputActionAsset playerCntrols;
    public InputHandler inputHandler;



   
    // Start is called before the first frame update
    void Start()
    {
        inputHandler = InputHandler.Instance;
        myRB = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        cameraHolder = transform.GetChild(0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        playerCam.transform.position = cameraHolder.position;

        Vector3 temp = myRB.velocity;

        float verticalMove = Input.GetAxisRaw("Vertical");
        float horizontalMove = Input.GetAxisRaw("Horizontal");

    
        temp.x = verticalMove * speed;
        temp.z = horizontalMove * speed;
        

        myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);

        if (health < 0)
            health = 0;

        if (health == 0)
            GameOver = true;
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(10f);
    }
}