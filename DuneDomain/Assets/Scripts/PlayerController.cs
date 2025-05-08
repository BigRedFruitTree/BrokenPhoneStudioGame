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
    public bool canAttack = true;
    public bool canAttack2 = true;
    public int arrowSpeed;
    public bool canMove = true;
    public float drawSpeed;
    public float maxdrawSpeed;
    public bool canDash = true;
    public bool canTakeDamage = true;
    public bool attacking = false;
    public bool isCooldownOver = true;
    public bool canBlock = true;
    public bool isBlocking = false;
    public bool canUnblock = false;
    public float stringTimer;
    public int whichAttack = 1;
    public bool isCharging = false;
    public bool canCharge = false;
    public bool isGoingtoCharge = false;
    public int chargeLevel = 0;
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
    public MeleeEnemyManager enemyScriptM;
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
        playerAnimator.SetBool("gm", false);
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
        if (gm.GameOn == true && gm.GameOver == false && gm.started == true)
        {
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

            if (Input.GetMouseButtonDown(0) && canAttack == true && gm.weapon != 2 && gm.weapon != 5 && isDashing == false && attacking == false && isBlocking == false && recovering == false && playerAnimator.GetBool("recovering") == false)
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
                if (gm.weapon == 1)
                {
                    AudioSource.clip = Slash;
                    AudioSource.Play();
                }
                else if (gm.weapon == 3)
                {
                    AudioSource.clip = hammersound;
                    AudioSource.Play();
                }
                else if (gm.weapon == 5)
                {
                    AudioSource.clip = stabsound;
                    AudioSource.Play();
                }
                StartCoroutine("AttackStringCoolDown");
                
                
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

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 1 && gm.weapon != 2 && gm.weapon != 5 && recovering == false && recovering == false && playerAnimator.GetBool("recovering") == false)
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
                if (gm.weapon == 1)
                {
                    AudioSource.clip = Slash;
                    AudioSource.Play();
                }
                else if (gm.weapon == 3)
                {
                    AudioSource.clip = hammersound;
                    AudioSource.Play();
                }
                else if (gm.weapon == 5)
                {
                    AudioSource.clip = stabsound;
                    AudioSource.Play();
                }
                StartCoroutine("AttackStringCoolDown");  
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 2 && gm.weapon != 2 && gm.weapon != 5 && recovering == false && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
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
                if (gm.weapon == 1)
                {
                    AudioSource.clip = Slash;
                    AudioSource.Play();
                }
                else if (gm.weapon == 3)
                {
                    AudioSource.clip = hammersound;
                    AudioSource.Play();
                }
                else if (gm.weapon == 5)
                {
                    AudioSource.clip = stabsound;
                    AudioSource.Play();
                }
                StartCoroutine("AttackStringCoolDown");  
            }

            if (Input.GetMouseButtonDown(0) && playerAnimator.GetBool("attacking") == false && canAttack == false && canAttack2 == true && attacking == false && stringTimer > 0 && whichAttack == 3 && gm.weapon != 2 && gm.weapon != 5 && recovering == false && recovering == false && playerAnimator.GetBool("recovering") == false)
            {
                myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
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
                if (gm.weapon == 1 || gm.weapon == 5)
                {
                    AudioSource.clip = Slash;
                    AudioSource.Play();
                }
                else if (gm.weapon == 3)
                {
                    AudioSource.clip = hammersound;
                    AudioSource.Play();
                }
                StartCoroutine("AttackStringCoolDown"); 
                StartCoroutine("WaitEndString");  
            }

            if (Input.GetMouseButton(0) && isCooldownOver == true && gm.weapon > 0 && isDashing == false && isBlocking == false)
            {
                if (gm.weapon == 2)
                {
                    playerAnimator.SetBool("IsDrawing", true);
                    myRB.constraints = RigidbodyConstraints.FreezeAll;
                    isDashing = false;
                    canAttack = false;
                    canMove = true;
                    canRotate = true;
                    StartCoroutine("WaitDraw");
                }

                if (gm.weapon == 5)
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

            if (Input.GetMouseButtonUp(0) && gm.weapon > 0 && isDashing == false && isBlocking == false)
            {
                if (gm.weapon == 2 && drawSpeed <= 0f)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    arrowSpeed = 2500;
                    arrow.SetActive(true);
                    AudioSource.clip = Woosh;
                    AudioSource.Play();
                    GameObject arrowSummon = Instantiate(arrow, arrowSpawnB.transform.position, playerRotationHolder.transform.rotation);
                    arrowSummon.transform.Rotate(90f, 180f, 0f);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(playerRotationHolder.transform.forward * arrowSpeed);
                    canAttack = false;
                    drawSpeed = 50f;
                    Destroy(arrowSummon, 200f);
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }
                if (gm.weapon == 2 && drawSpeed > 0f)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    drawSpeed = 50f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }

                if (gm.weapon == 5 && drawSpeed > 0f)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    drawSpeed = 100f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("CrossbowCoolDown");
                }
                if (gm.weapon == 5 && drawSpeed <= 0)
                {
                    playerAnimator.SetBool("attacking", true);
                    playerAnimator.SetBool("IsDrawing", false);
                    attacking = true;
                    AudioSource.clip = Woosh;
                    AudioSource.Play();
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

            if (Input.GetMouseButtonUp(1) && gm.weapon == 3 && isDashing == false && isBlocking == false)
            {
                AudioSource.Stop();
                if (gm.weapon == 3 && drawSpeed >= 133.34f)
                {
                    chargeLevel = 1;
                }
                if (gm.weapon == 3 && drawSpeed > 66.67f && drawSpeed < 133.34f)
                {
                    chargeLevel = 2;
                }
                if (gm.weapon == 3 && drawSpeed <= 66.67f)
                {
                    chargeLevel = 3;
                }
                playerAnimator.SetBool("IsCharging", false);
                playerAnimator.SetBool("attacking", true);
                isCharging = false;
                attacking = true;
                canAttack = false;
                drawSpeed = 200f;
                AudioSource.clip = hammersound;
                AudioSource.Play();
                myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                StartCoroutine("HammerCoolDownCharge");
            }
            
            if (Input.GetMouseButton(1) && canMove == true && isBlocking == false && gm.weapon == 4)
            {
                playerAnimator.SetBool("IsBlocking", true);
                myRB.constraints = RigidbodyConstraints.FreezeAll;
                canBlock = false;
                isBlocking = true;
                canMove = true;
                canUnblock = true;
            }

            if (Input.GetMouseButton(1) && canMove == true && isBlocking == false && gm.weapon == 3)
            {
                playerAnimator.SetBool("IsCharging", true);
                isCharging = true;
                myRB.constraints = RigidbodyConstraints.FreezeAll;
                isDashing = false;
                canAttack = false;
                canMove = true;
                StartCoroutine("WaitDraw");
            }

            if (Input.GetMouseButtonDown(1) && isCharging == true)
            {
                AudioSource.PlayOneShot(hammerchargesound);
            }

            if (Input.GetMouseButtonUp(1) && isBlocking == true && canUnblock == true && canBlock == false && gm.weapon == 4)
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

            if (Input.GetKeyDown(KeyCode.F) && stamina > 2 && isBlocking == false && canDash == true && attacking == false && playerAnimator.GetBool("attacking") == false && playerAnimator.GetInteger("whichAttack") == 0)
            {
                AudioSource.clip = Woosh;
                AudioSource.Play();
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

            if(health <= 0)
            {
                AudioSource.Stop();
                gm.GameOver = true;
                playerAnimator.SetInteger("whichAttack", 0);
                playerAnimator.SetBool("gm", false);
                playerAnimator.SetBool("attacking", false);
                playerAnimator.SetBool("IsCharging", false);
                playerAnimator.SetBool("isDashing", false);
                playerAnimator.SetBool("IsDrawing", false);
                playerAnimator.SetBool("isMoving", false);
                playerAnimator.SetInteger("weapon", 0);
            }

            if (health > 0)
            {
                if (stamina > 10f)
                    stamina = 10f;

                if (stamina < 0f)
                    stamina = 0f;

                if (isDashing == false)
                    stamina += 1f * Time.deltaTime;
                
                StartCoroutine("WaitCheckNearestEnemy");
            }
        }
    }
   
    public void OnTriggerEnter(Collider other)
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

       if (other.gameObject.name == "Handr" && canTakeDamage == true && gm.GameOn == true && gm.areHandsActive == true || other.gameObject.name == "Handl" && canTakeDamage == true && gm.GameOn == true && gm.areHandsActive == true || other.gameObject.name == "Head" && canTakeDamage == true && gm.GameOn == true && gm.isHeadActive == true)
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
       
       if (other.gameObject.name == "Tailblade" && canTakeDamage == true && gm.GameOn == true && gm.isTailActive == true || other.gameObject.name == "Jaw" && canTakeDamage == true && gm.GameOn == true && gm.isHeadActive == true)
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
        yield return new WaitForSeconds(1f);
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
         yield return new WaitForSeconds(0.5f);
         drawSpeed -= Time.deltaTime * 60;
    }

    IEnumerator AttackStringCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("attacking", false);
        myRB.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(0.3f);
        attacking = false;
        yield return new WaitForSeconds(0.5f);
        canRotate = true;
        canAttack2 = true;
    }
    IEnumerator HammerCoolDownCharge()
    {
        drawSpeed = 200f;
        yield return new WaitForSeconds(1f);
        playerAnimator.SetBool("attacking", false);
        isCharging = false;
        chargeLevel = 0;
        canMove = true;
        canRotate = true;
        drawSpeed = 200f;
        attacking = false;
        yield return new WaitForSeconds(2.7f);
        canAttack = true;
    }
    IEnumerator BowCoolDown()
    {
        drawSpeed = 50f;
        isCooldownOver = false;
        playerAnimator.SetBool("attacking", false);
        attacking = false;
        yield return new WaitForSeconds(2f);
        drawSpeed = 50f;
        canAttack = true;
        isCooldownOver = true;
    }

    IEnumerator CrossbowCoolDown()
    {
        drawSpeed = 100f;
        isCooldownOver = false;
        playerAnimator.SetBool("attacking", false);
        attacking = false;
        yield return new WaitForSeconds(4f);
        drawSpeed = 100f;
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
         yield return new WaitForSeconds(0.5f);
         enemyScriptM = GetNearestTargetM();
    }   
}