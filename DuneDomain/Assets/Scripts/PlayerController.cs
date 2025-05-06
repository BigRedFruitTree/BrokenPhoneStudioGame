using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;
using JetBrains.Annotations;

public class PlayerController : MonoBehaviour
{
    //Sword = 1
    //Bow = 2
    //Hammer = 3
    //Spear & Shield = 4
    //Crossbow = 5

    [Header("Player Stats")]
    public int health = 10;
    public int maxHealth = 10;
    public int weapon = 0;
    public bool canAttack = true;
    public bool canAttack2 = true;
    public int arrowSpeed;
    public bool canMove = true;
    public float drawSpeed;
    public bool canDash = true;
    public bool canTakeDamage = true;
    public bool attacking = false;
    public bool isCooldownOver = true;
    public bool canBlock = true;
    public bool isBlocking = false;
    public bool canUnblock = false;
    public bool isPaused = false;
    public float stringTimer;
    public int whichAttack = 1;
    public bool isCharging = false;
    public bool canCharge = false;
    public bool isGoingtoCharge = false;
    public int chargeLevel = 0;
    public float stringCooldown = 0f;
    public bool stringCount = false;
    public bool canRotate = false;
    public bool dashingEnd = false;
    public bool recovering = false;

    [Header("Movement Settings")]
    public float speed = 7f;
    public float stamina = 50f;
    public bool isDashing = false;
    public float verticalMove;
    public float horizontalMove;

    [Header("Input System")]
    public InputActionAsset playerCntrols;
    public InputHandler inputHandler;

    [Header("Animator Stuff")]
    public Animator playerAnimator;

    [Header("Misc Refrences")]
    public GameObject playerObject;
    public GameObject playerRotationHolder;
    private Rigidbody myRB;
    public CinemachineVirtualCamera playerCam;
    public GameObject cameraHolder;
    public GameManager gm;
    public GameObject sword;
    public GameObject hammer;
    public GameObject spear;
    public GameObject shield;
    public GameObject bow;
    public GameObject crossbow;
    public GameObject arrow;
    public GameObject arrowSpawnB;
    public GameObject arrowSpawnC;
    public Canvas Pausemenu;
    public MeleeEnemyManager enemyScriptM;
    public RangedEnemyManager enemyScriptR;
    public AudioSource AudioSource;
    public AudioClip Slash;
    public AudioClip Woosh;
    public AudioClip stabsound;
    public AudioClip hammersound;
    public AudioClip hammerchargesound;
    public AudioClip shieldblock;


    // Start is called before the first frame update
    void Start()
    {
        playerAnimator.SetInteger("whichAttack", 0);
        playerAnimator.SetBool("attacking", false);
        playerAnimator.SetBool("IsCharging", false);
        playerAnimator.SetBool("isDashing", false);
        playerAnimator.SetBool("IsDrawing", false);
        playerAnimator.SetBool("isMoving", false);
        playerAnimator.SetInteger("weapon", 0);
        stringTimer = 2f;
        speed = 7f;
        drawSpeed = 50f;
        stamina = 10;
        health = 100;
        maxHealth = 100;
        inputHandler = InputHandler.Instance;
        myRB = GetComponent<Rigidbody>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        canMove = true;
        canRotate = true;
        canDash = true;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false && gm.started == true && isPaused == false)
        {

            weapon = gm.weapon;

            playerCam.transform.position = cameraHolder.transform.position;

            Vector3 temp = myRB.velocity;

            if (canMove == true && recovering == false)
            {
                verticalMove = Input.GetAxisRaw("Vertical");
                horizontalMove = Input.GetAxisRaw("Horizontal");
            }

            temp.x = verticalMove * speed;
            temp.z = horizontalMove * speed;

            if (isDashing == false && attacking == false && isBlocking == false)
            {
                myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
            }

            if (weapon == 1)
            {
                playerAnimator.SetInteger("weapon", 1);
                stringCooldown = 2f;
                sword.SetActive(true);
                bow.SetActive(false);
                crossbow.SetActive(false);
                hammer.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }
            if (weapon == 2)
            {
                playerAnimator.SetInteger("weapon", 2);
                bow.SetActive(true);
                sword.SetActive(false);
                crossbow.SetActive(false);
                hammer.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }
            if (weapon == 3)
            {
                playerAnimator.SetInteger("weapon", 3);
                stringCooldown = 2.7f;
                hammer.SetActive(true);
                sword.SetActive(false);
                bow.SetActive(false);
                crossbow.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }
            if (weapon == 4)
            {
                playerAnimator.SetInteger("weapon", 4);
                stringCooldown = 1.2f;
                spear.SetActive(true);
                shield.SetActive(true);
                sword.SetActive(false);
                bow.SetActive(false);
                crossbow.SetActive(false);
                hammer.SetActive(false);
            }
            if (weapon == 5)
            {
                playerAnimator.SetInteger("weapon", 5);
                crossbow.SetActive(true);
                sword.SetActive(false);
                bow.SetActive(false);
                hammer.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }

            if (horizontalMove > 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            if (horizontalMove > 0 && verticalMove > 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            }
            if (horizontalMove > 0 && verticalMove < 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 145f, 0f);
            }
            if (horizontalMove < 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            if (horizontalMove < 0 && verticalMove > 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -45f, 0f);
            }
            if (horizontalMove < 0 && verticalMove < 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -145f, 0f);
            }
            if (verticalMove > 0 && horizontalMove == 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (verticalMove < 0 && horizontalMove == 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", true);

                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            if (verticalMove == 0 && horizontalMove == 0 && canRotate == true && recovering == false)
            {
                if (canMove == true && recovering == false)
                    playerAnimator.SetBool("isMoving", false);

            }

            if (canMove == false)
                playerAnimator.SetBool("isMoving", false);

            if (Input.GetMouseButtonDown(0) && canAttack == true && weapon > 0 && isDashing == false && attacking == false && isBlocking == false && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                if (weapon == 1)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetInteger("whichAttack", 1);
                    whichAttack = 1;
                    if (!recovering)
                    {
                        myRB.velocity += playerRotationHolder.transform.forward * 30f;
                    }
                    attacking = true;
                    canAttack = false;
                    canMove = false;
                    canRotate = false;
                    stringCount = true;
                    StartCoroutine("SwordCoolDown");
                }

                if (weapon == 3)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetInteger("whichAttack", 1);
                    playerAnimator.SetBool("IsCharging", false);
                    canCharge = true;
                    isCharging = false;
                    whichAttack = 1;
                    if (!recovering)
                    {
                        myRB.velocity += playerRotationHolder.transform.forward * 20f;
                    }
                    attacking = true;
                    canAttack = false;
                    canMove = false;
                    canRotate = false;
                    AudioSource.clip = hammersound;
                    AudioSource.Play();
                    stringCount = true;
                    StartCoroutine("HammerCoolDownBase");
                }

                if (weapon == 4)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetInteger("whichAttack", 1);
                    whichAttack = 1;
                    myRB.velocity += playerRotationHolder.transform.forward * 40f;
                    attacking = true;
                    canAttack = false;
                    canMove = false;
                    canRotate = false;
                    stringCount = true;
                    AudioSource.clip = stabsound;
                    AudioSource.Play();
                    StartCoroutine("SpearCoolDown");
                }
            } 

            if (stringCount == true && stringTimer > 0f)
            {
                stringTimer -= Time.deltaTime;
                if (stringTimer <= 0f)
                {
                    stringTimer = 0f;
                    StartCoroutine("WaitEndString");
                }
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 1 && weapon == 1 && recovering == false && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                playerAnimator.SetInteger("whichAttack", 2);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 2;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 30f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                StartCoroutine("SwordCoolDown");  
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 2 && weapon == 1&& recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 3);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 3;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 30f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                StartCoroutine("SwordCoolDown");  
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 3 && weapon == 1 && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 4);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 4;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 40f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                StartCoroutine("SwordCoolDown"); 
                StartCoroutine("WaitEndString");  
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 1 && weapon == 3 && recovering == false && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 2);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 2;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 20f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                AudioSource.Play();
                StartCoroutine("HammerCoolDownBase");
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 2 && weapon == 3 && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 3);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 3;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 20f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                AudioSource.Play();
                StartCoroutine("HammerCoolDownBase");
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 3 && weapon == 3 && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 4);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 4;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 40f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                AudioSource.Play();
                StartCoroutine("HammerCoolDownBase");
                StartCoroutine("WaitEndString");
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 1 && weapon == 4 && recovering == false && recovering == false && playerAnimator.GetBool("recovering") == false)
            {

                playerAnimator.SetInteger("whichAttack", 2);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 2;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 30f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                AudioSource.Play();
                StartCoroutine("SpearCoolDown");
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 2 && weapon == 4 && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 3);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 3;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 30f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                AudioSource.Play();
                StartCoroutine("SpearCoolDown");
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 3 && weapon == 4 && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                playerAnimator.SetInteger("whichAttack", 4);
                playerAnimator.SetBool("attacking", true);
                whichAttack = 4;
                stringTimer = 2f;
                if (!recovering)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 40f;
                }
                attacking = true;
                canAttack2 = false;
                canMove = false;
                canRotate = false;
                stringCount = true;
                AudioSource.Play();
                StartCoroutine("SpearCoolDown");
                StartCoroutine("WaitEndString");
            }

            if (Input.GetMouseButton(0) && isCooldownOver == true && weapon > 0 && isDashing == false && isBlocking == false)
            {
                if (weapon == 2)
                {
                    playerAnimator.SetBool("IsDrawing", true);
                    myRB.constraints = RigidbodyConstraints.FreezeAll;
                    isDashing = false;
                    canAttack = false;
                    canMove = true;
                    canRotate = true;
                    StartCoroutine("WaitDraw");
                }

                if (weapon == 5)
                {
                    playerAnimator.SetBool("IsDrawing", true);
                    myRB.constraints = RigidbodyConstraints.FreezeAll;
                    isDashing = false;
                    canAttack = false;
                    canMove = true;
                    canRotate = true;
                    StartCoroutine("WaitDraw");
                }
            }

            if (Input.GetMouseButtonUp(0) && weapon > 0 && isDashing == false && isBlocking == false)
            {
                if (weapon == 2 && drawSpeed <= 0f)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    arrowSpeed = 2500;
                    arrow.SetActive(true);
                    GameObject arrowSummon = Instantiate(arrow, arrowSpawnB.transform.position, playerRotationHolder.transform.rotation);
                    arrowSummon.transform.Rotate(90f, 180f, 0f);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(playerRotationHolder.transform.forward * arrowSpeed);
                    canAttack = false;
                    drawSpeed = 50f;
                    Destroy(arrowSummon, 200f);
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }
                if (weapon == 2 && drawSpeed > 0f)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    drawSpeed = 50f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }

                if (weapon == 5 && drawSpeed > 0f)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    drawSpeed = 100f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("CrossbowCoolDown");
                }
                if (weapon == 5 && drawSpeed <= 0)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    arrowSpeed = 4500;
                    arrow.SetActive(true);
                    GameObject arrowSummon = Instantiate(arrow, arrowSpawnC.transform.position, playerRotationHolder.transform.rotation);
                    arrowSummon.transform.Rotate(90f, 180f, 0f);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(playerRotationHolder.transform.forward * arrowSpeed);
                    Destroy(arrowSummon, 2f);
                    canAttack = false;
                    drawSpeed = 100f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("CrossbowCoolDown");
                }
            }

            if (Input.GetMouseButtonUp(1) && weapon == 3 && isDashing == false && isBlocking == false)
            {
                if (weapon == 3 && drawSpeed >= 133.34f)
                {
                    playerAnimator.SetBool("IsCharging", false);
                    playerAnimator.SetBool("attacking", true);
                    chargeLevel = 1;
                    isCharging = false;
                    attacking = true;
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("HammerCoolDown1");
                }
                if (weapon == 3 && drawSpeed > 66.67f && drawSpeed < 133.34)
                {
                    playerAnimator.SetBool("IsCharging", false);
                    playerAnimator.SetBool("attacking", true);
                    chargeLevel = 2;
                    isCharging = false;
                    attacking = true;
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("HammerCoolDown2");
                }
                if (weapon == 3 && drawSpeed <= 66.67f)
                {
                    playerAnimator.SetBool("IsCharging", false);
                    playerAnimator.SetBool("attacking", true);
                    chargeLevel = 3;
                    isCharging = false;
                    attacking = true;
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("HammerCoolDown3");
                }
            }
            
            if (Input.GetMouseButton(1) && canMove == true && isBlocking == false && weapon == 4)
            {
                playerAnimator.SetBool("IsBlocking", true);
                myRB.constraints = RigidbodyConstraints.FreezeAll;
                canBlock = false;
                isBlocking = true;
                canMove = true;
                canUnblock = true;
            }

            if (Input.GetMouseButton(1) && canMove == true && isBlocking == false && weapon == 3)
            {
                playerAnimator.SetBool("IsCharging", true);
                isCharging = true;
                myRB.constraints = RigidbodyConstraints.FreezeAll;
                isDashing = false;
                canAttack = false;
                canMove = true;
                AudioSource.clip = hammerchargesound;
                AudioSource.Play();
                StartCoroutine("WaitDraw");
            }

            if (Input.GetMouseButtonUp(1) && isBlocking == true && canUnblock == true && canBlock == false && weapon == 4)
            {
                playerAnimator.SetBool("IsBlocking", false);
                myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                isBlocking = false;
                canMove = true;
                canBlock = true;
                canUnblock = false;
            }        

            if(isDashing == true && attacking == false && isBlocking == false)
            {
                myRB.velocity += playerRotationHolder.transform.forward * 2f;
            }

            if (dashingEnd == true && isDashing == false && attacking == false && isBlocking == false)
            {
                myRB.velocity += playerRotationHolder.transform.forward * 10f;
            }

            if (Input.GetKeyDown(KeyCode.F) && stamina > 2 && isBlocking == false && canDash == true && attacking == false && playerAnimator.GetBool("attacking") == false && playerAnimator.GetInteger("whichAttack") == 0)
            {
                Debug.Log("Pressed");
                playerAnimator.SetBool("isDashing", true);
                canTakeDamage = false;
                canMove = false;
                canRotate = false;
                canDash = false;
                isDashing = true;
                stamina -= 3;
                StartCoroutine("WaitDash");
                StartCoroutine("WaitDamage");
            }

            if (stamina < 0f)
                stamina = 0f;

            if (isDashing == false)
                stamina += 1f * Time.deltaTime;

            if (stamina > 10f)
                stamina = 10f;

            if (health < 0)
                health = 0;

            if (health == 0)
                gm.GameOver = true;

            if (health > 0)
            {
                StartCoroutine("WaitCheckNearestEnemy");
            }
        }
    }
   
    public void OnTriggerStay(Collider other)
    {
       if (other.gameObject.tag == "EnemySword" && canTakeDamage == true && gm.GameOn == true && enemyScriptM.attacking == true)
       {
           canTakeDamage = false;
           if (isBlocking == true)
           {
               health -= 5;
           }
           else
           {
               health -= 10;
           }
           StartCoroutine("WaitDamage"); 
       }
      
       if (other.gameObject.tag == "EnemyShot" && canTakeDamage == true && gm.GameOn == true)
       {
           canTakeDamage = false;
           if (isBlocking == true)
           {
                health -= 5;
           }
           else
           {
                health -= 10;
           }
           StartCoroutine("WaitDamage"); 
       }

       if (other.gameObject.name == "Handr" && canTakeDamage == true && gm.GameOn == true)
       {
           canTakeDamage = false;
           if (isBlocking == true)
           {
               health -= 10;
           }
           else
           {
               health -= 20;
           }
           StartCoroutine("WaitDamage");
       }

       if (other.gameObject.name == "Handl" && canTakeDamage == true && gm.GameOn == true)
       {
           canTakeDamage = false;
           if (isBlocking == true)
           {
               health -= 10;
           }
           else
           {
               health -= 20;
           }
           StartCoroutine("WaitDamage");
       }
       
       if (other.gameObject.name == "Tailblade" && canTakeDamage == true && gm.GameOn == true)
       {
           canTakeDamage = false;
           if (isBlocking == true)
           {
               health -= 15;
           }
           else
           {
               health -= 25;
           }
           StartCoroutine("WaitDamage");
       }
    }

    public MeleeEnemyManager GetNearestTargetM()
    {
       MeleeEnemyManager nearestTarget = null;
       float nearestDistance = Mathf.Infinity;

       foreach (var enemy in gm.meleeEnemyNumber)
       {
           float distance = Vector3.Distance(playerObject.transform.position, enemy.transform.position);
           if (distance < nearestDistance)
           {
                nearestDistance = distance;
               nearestTarget = enemy.GetComponent<MeleeEnemyManager>();
           }
       }

       return nearestTarget;
    }

    public RangedEnemyManager GetNearestTargetR()
    {
        RangedEnemyManager nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var enemy in gm.rangedEnemyNumber)
        {
            float distance = Vector3.Distance(playerObject.transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = enemy.GetComponent<RangedEnemyManager>();
            }
        }

        return nearestTarget;
    }

    IEnumerator WaitDamage()
    {
         yield return new WaitForSeconds(1f);
         canTakeDamage = true;
    }

    IEnumerator WaitEndString()
    {
        recovering = true;
        playerAnimator.SetBool("recovering", true);
        playerAnimator.SetBool("IsCharging", false);
        stringCount = false;
        stringTimer = 2f;
        yield return new WaitForSeconds(1f);
        playerAnimator.SetInteger("whichAttack", 0);
        myRB.constraints = RigidbodyConstraints.FreezeAll;
        whichAttack = 0;
        playerAnimator.SetBool("attacking", false);
        yield return new WaitForSeconds(stringCooldown);
        playerAnimator.SetBool("recovering", false);
        recovering = false;
        attacking = false;
        canAttack = true;
        canAttack2 = true;
        myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        canMove = true;
    }

    IEnumerator WaitDraw()
    {
         yield return new WaitForSeconds(1f);
         drawSpeed--;
    }

    IEnumerator SwordCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        myRB.constraints = RigidbodyConstraints.FreezeAll;
        playerAnimator.SetBool("attacking", false);
        yield return new WaitForSeconds(0.5f);
        attacking = false;
        yield return new WaitForSeconds(1f);
        myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        canRotate = true;
        canAttack2 = true;
    }

    IEnumerator HammerCoolDownBase()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("attacking", false);
        yield return new WaitForSeconds(0.5f);
        attacking = false;
        yield return new WaitForSeconds(1.5f);
        canRotate = true;
        canAttack2 = true;
    }

    IEnumerator HammerCoolDown1()
    {
        yield return new WaitForSeconds(1f);
        playerAnimator.SetBool("attacking", false);
        isCharging = false;
        chargeLevel = 0;
        canMove = true;
        canRotate = true;
        attacking = false;
        yield return new WaitForSeconds(2.7f);
        canAttack = true;
    }

    IEnumerator HammerCoolDown2()
    {
        yield return new WaitForSeconds(1f);
        playerAnimator.SetBool("attacking", false);
        isCharging = false;
        chargeLevel = 0;
        canMove = true;
        canRotate = true;
        attacking = false;
        yield return new WaitForSeconds(3f);
        canAttack = true;
    }

    IEnumerator HammerCoolDown3()
    {
        yield return new WaitForSeconds(1f);
        playerAnimator.SetBool("attacking", false);
        isCharging = false;
        chargeLevel = 0;
        canMove = true;
        canRotate = true;
        attacking = false;
        yield return new WaitForSeconds(3.7f);
        canAttack = true;
    }

    IEnumerator SpearCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("attacking", false);
        yield return new WaitForSeconds(0.5f);
        attacking = false;
        yield return new WaitForSeconds(1.5f);
        canRotate = true;
        canAttack2 = true;
    }

    IEnumerator BowCoolDown()
    {
        isCooldownOver = false;
        playerAnimator.SetBool("attacking", false);
        attacking = false;
        yield return new WaitForSeconds(2f);
        drawSpeed = 100f;
        canAttack = true;
        isCooldownOver = true;
    }

    IEnumerator CrossbowCoolDown()
    {
        isCooldownOver = false;
        playerAnimator.SetBool("attacking", false);
        attacking = false;
        yield return new WaitForSeconds(4f);
        drawSpeed = 200f;
        canAttack = true;
        isCooldownOver = true;
    }

    IEnumerator WaitDash()
    {
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        playerAnimator.SetBool("isDashing", false);
        dashingEnd = true;
        yield return new WaitForSeconds(0.3f);
        dashingEnd = false;
        canMove = true;
        canRotate = true;
        yield return new WaitForSeconds(1f);
        canDash = true;
    }
     
    IEnumerator WaitCheckNearestEnemy()
    {
         yield return new WaitForSeconds(0.1f);
         enemyScriptM = GetNearestTargetM();
         enemyScriptR = GetNearestTargetR();
    }   
}