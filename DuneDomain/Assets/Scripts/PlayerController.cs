using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


public class PlayerController : MonoBehaviour
{ 

    [Header("Player Stats")]
    public int health = 10;
    public int maxHealth = 10;
    public int weapon = 0;
    public bool canAttack = true;
    public int arrowSpeed;
    public bool canMove = true;
    public float drawSpeed;
    public bool canDash = true;
    public bool canTakeDamage = true;

    [Header("Movement Settings")]
    public float speed = 10.0f;
    public float stamina = 50f;
    public bool isDashing = false;

    [Header("Input System")]
    public InputActionAsset playerCntrols;
    public InputHandler inputHandler;

    [Header("Misc Refrences")]
    public GameObject playerObject;
    public GameObject playerRotationHolder;
    private Rigidbody myRB;
    public CinemachineVirtualCamera playerCam;
    public GameObject cameraHolder;
    public GameManager gm;
    public GameObject weaponHolder;
    public GameObject sword;
    public GameObject bow;
    public GameObject arrow;
    RaycastHit raycast;


    // Start is called before the first frame update
    void Start()
    {
        stamina = 10;
        health = 10;
        maxHealth = 10;
        inputHandler = InputHandler.Instance;
        myRB = GetComponent<Rigidbody>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        canMove = true;
        canDash = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gm.GameOn == true && gm.GameOver == false && canMove == true)
        {
            weapon = gm.weapon;

            playerCam.transform.position = cameraHolder.transform.position;

            Vector3 temp = myRB.velocity;

            float verticalMove = Input.GetAxisRaw("Vertical");
            float horizontalMove = Input.GetAxisRaw("Horizontal");

            temp.x = verticalMove * speed;
            temp.z = horizontalMove * speed;

            myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);

            if(horizontalMove > 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }
            if(horizontalMove > 0 && verticalMove > 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }
            if(horizontalMove > 0 && verticalMove < 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 145f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }

            if(horizontalMove < 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }
            if(horizontalMove < 0 && verticalMove > 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -45f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }
            if(horizontalMove < 0 && verticalMove < 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -145f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }
            if(verticalMove > 0 && horizontalMove == 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }
            if(verticalMove < 0 && horizontalMove == 0 && canMove == true) 
            {
               playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
               sword.transform.position = weaponHolder.transform.position;
               bow.transform.position = weaponHolder.transform.position;
               sword.transform.rotation = weaponHolder.transform.rotation;
               bow.transform.rotation = weaponHolder.transform.rotation;
            }

            if (Input.GetMouseButtonDown(0) && canAttack == true && weapon > 0 && gm.started == true && isDashing == false)
            {
               if(weapon == 1)
               {
                   
                   canMove = false;
                   canAttack = false;
                   sword.SetActive(true);
                   sword.transform.position = weaponHolder.transform.position;
                   canMove = true;
                   StartCoroutine("WaitForWeapons");
                   StartCoroutine("AttackCoolDown");
               }
               if (weapon == 2)
               {
                  drawSpeed = 60f;
                  bow.SetActive(true);
               }
            }

            if (Input.GetMouseButton(0) && canAttack == true && weapon > 0 && gm.started == true && isDashing == false)
            {
               if (weapon == 2)
               {
                  myRB.constraints = RigidbodyConstraints.FreezeAll;
                  StartCoroutine("WaitDraw");
               }
            }

            if (Input.GetMouseButtonUp(0) && drawSpeed <= 0 && weapon > 0 && gm.started == true && isDashing == false)
            {
               if (weapon == 2)
               {
                   arrowSpeed = 2000;
                   arrow.SetActive(true);
                   GameObject arrowSummon = Instantiate(arrow, bow.transform.position, bow.transform.rotation);
                   arrowSummon.GetComponent<Rigidbody>().AddForce(arrowSummon.transform.up * arrowSpeed);
                   Destroy(arrowSummon, 2f);
                   canAttack = false;
                   drawSpeed = 60f;
                   myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                   StartCoroutine("WaitForWeapons");
                   StartCoroutine("AttackCoolDown");
               }
            }

            if (Input.GetKeyDown(KeyCode.E) && gm.started == true && stamina >= 5)
            {
                canTakeDamage = false;
                isDashing = true;
                stamina -= 10;
                //canDash = false;
                myRB.AddForce(playerRotationHolder.transform.forward * 10000f, ForceMode.Force);
                StartCoroutine("WaitDash");
                StartCoroutine("WaitDamage");
            }
            
            if (!Input.GetKeyDown(KeyCode.E) && gm.started == true && gm.GameOn == true && isDashing == false)
            {
                if(stamina <= 10 && stamina < 10.0001)
                {
                    StartCoroutine("WaitStamina");
                }

                if(stamina > 10)
                  stamina = 10;
            }
            
            if (stamina <= 0)
                stamina = 0;


            if (health < 0)
                health = 0;

            if (health == 0)
                gm.GameOver = true;

        }

    }

    IEnumerator WaitForWeapons()
    {
        yield return new WaitForSeconds(1f);
        sword.SetActive(false);
        bow.SetActive(false);
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
    }

    IEnumerator WaitDraw()
    {
        yield return new WaitForSeconds(2f);
        drawSpeed--;
    }

    IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(3f);
        canAttack = true;
    }

    IEnumerator WaitDash()
    {
        yield return new WaitForSeconds(1f);
        isDashing = false;
        //canDash = true;
    }

    IEnumerator WaitStamina()
    {
        yield return new WaitForSeconds(2f);
        stamina += 0.1f;
    }
}