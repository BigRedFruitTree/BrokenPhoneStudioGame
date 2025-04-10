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
    public bool chargeBoxActive = false;
    public int chargeLevel = 0;

    [Header("Movement Settings")]
    public float speed = 7f;
    public float stamina = 50f;
    public bool isDashing = false;
    public float verticalMove;
    public float horizontalMove;

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
    public GameObject shieldHolder;
    public GameObject blockHolder;
    public GameObject sword;
    public GameObject hammer;
    public GameObject spear;
    public GameObject shield;
    public GameObject bow;
    public GameObject crossbow;
    public GameObject arrow;
    public GameObject arrowSpawnB;
    public GameObject arrowSpawnC;
    public GameObject chargeHurtBox;
    public Canvas Pausemenu;
    public MeleeEnemyManager enemyScriptM;
    public RangedEnemyManager enemyScriptR;

    // Start is called before the first frame update
    void Start()
    {
        stringTimer = 20;
        chargeHurtBox.transform.localScale = new Vector3(2f, 1f, 2f);
        speed = 7f;
        drawSpeed = 200f;
        stamina = 10;
        health = 100;
        maxHealth = 100;
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
        if (gm.GameOn == true && gm.GameOver == false && gm.started == true && isPaused == false)
        {

            weapon = gm.weapon;

            playerCam.transform.position = cameraHolder.transform.position;

            Vector3 temp = myRB.velocity;

            verticalMove = Input.GetAxisRaw("Vertical");
            horizontalMove = Input.GetAxisRaw("Horizontal");
            

            temp.x = verticalMove * speed;
            temp.z = horizontalMove * speed;

            if(isDashing == false && attacking == false && isBlocking == false)
            {
               myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
            } 

            if (weapon == 1)
            {
                sword.SetActive(true);
                bow.SetActive(false);
                crossbow.SetActive(false);
                hammer.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }
            if (weapon == 2)
            {
                bow.SetActive(true);
                sword.SetActive(false);
                crossbow.SetActive(false);
                hammer.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }
            if (weapon == 3)
            {
                hammer.SetActive(true);
                sword.SetActive(false);
                bow.SetActive(false);
                crossbow.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }
            if (weapon == 4)
            {
                spear.SetActive(true);
                shield.SetActive(true);
                sword.SetActive(false);
                bow.SetActive(false);
                crossbow.SetActive(false);
                hammer.SetActive(false);
            }
            if (weapon == 5)
            {
                crossbow.SetActive(true);
                sword.SetActive(false);
                bow.SetActive(false);
                hammer.SetActive(false);
                spear.SetActive(false);
                shield.SetActive(false);
            }

            sword.transform.position = weaponHolder.transform.position;
            sword.transform.rotation = weaponHolder.transform.rotation;
            bow.transform.position = weaponHolder.transform.position;
            bow.transform.rotation = weaponHolder.transform.rotation;
            crossbow.transform.position = weaponHolder.transform.position;
            crossbow.transform.rotation = weaponHolder.transform.rotation;
            hammer.transform.position = weaponHolder.transform.position;
            hammer.transform.rotation = weaponHolder.transform.rotation;
            spear.transform.position = weaponHolder.transform.position;
            spear.transform.rotation = weaponHolder.transform.rotation;
            shield.transform.position = shieldHolder.transform.position;
            shield.transform.rotation = shieldHolder.transform.rotation;

            if (horizontalMove > 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            if (horizontalMove > 0 && verticalMove > 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            }
            if (horizontalMove > 0 && verticalMove < 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 145f, 0f);
            }
            if (horizontalMove < 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            if (horizontalMove < 0 && verticalMove > 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -45f, 0f);
            }
            if (horizontalMove < 0 && verticalMove < 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, -145f, 0f);
            }
            if (verticalMove > 0 && horizontalMove == 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (verticalMove < 0 && horizontalMove == 0 && canMove == true)
            {
                playerRotationHolder.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            if (!Input.GetMouseButtonDown(0) && attacking == false)
            {
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 90f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 45f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 145f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -90f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -45f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, -45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -145f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, -145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 0f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }

            if (Input.GetMouseButtonDown(0) && isDashing == false && attacking == false && canAttack == true && weapon == 1 || Input.GetMouseButtonDown(0) && isDashing == false && attacking == false && canAttack == true && weapon == 3 || Input.GetMouseButtonDown(0) && isDashing == false && attacking == false && canAttack == true && weapon == 4)
            {

                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 90f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 45f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 145f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -90f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -45f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -145f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 0f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
                }
            }

            if (Input.GetMouseButtonUp(1) && isDashing == false && weapon == 3)
            {

                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 90f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 45f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 145f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -90f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -45f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -145f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 0f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
                }
            }

            if (Input.GetMouseButton(0) && isDashing == false && attacking == false && weapon == 5)
            {

                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 90f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 45f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 145f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -90f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -45f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -145f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, -145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 0f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 180f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
                }
            }

            if (Input.GetMouseButtonDown(1) && weapon == 4 && isDashing == false && isBlocking == false || isBlocking == true && isDashing == false || Input.GetMouseButton(1) && isDashing == false && weapon == 4)
            {
                shield.transform.position = blockHolder.transform.position;
                shield.transform.rotation = blockHolder.transform.rotation;
            }

            if (Input.GetMouseButtonDown(0) && canAttack == true && weapon > 0 && isDashing == false && attacking == false && isBlocking == false)
            {
                if (weapon == 1)
                {
                    if (whichAttack == 1)
                    {
                        whichAttack = 1;
                        myRB.velocity += playerRotationHolder.transform.forward * 30f;
                        attacking = true;
                        canAttack = false;
                        canMove = false;
                        while (stringTimer > 0)
                        {
                            stringTimer -= Time.deltaTime;
                        }
                        if (Input.GetMouseButtonDown(0) && stringTimer > 0)
                        {
                            whichAttack = 2;
                            myRB.velocity += playerRotationHolder.transform.forward * 30f;
                            attacking = true;
                            canAttack = false;
                            canMove = false;
                        }
                        else
                        {
                            StartCoroutine("SwordCoolDown");
                        }
                    }
                }

                if (weapon == 3)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 20f;
                    isDashing = false;
                    attacking = true;
                    canAttack = false;
                    canMove = false;
                    StartCoroutine("HammerCoolDownBase");
                }

                if (weapon == 4)
                {
                    myRB.velocity += playerRotationHolder.transform.forward * 40f;
                    isDashing = false;
                    attacking = true;
                    canAttack = false;
                    canMove = false;
                    StartCoroutine("SpearCoolDown");
                }
            }
            

            if (Input.GetMouseButton(0) && isCooldownOver == true && weapon > 0 && isDashing == false && isBlocking == false)
            {
                if (weapon == 2)
                {
                    myRB.constraints = RigidbodyConstraints.FreezeAll;
                    isDashing = false;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("WaitDraw");
                }

                if (weapon == 5)
                {
                    myRB.constraints = RigidbodyConstraints.FreezeAll;
                    isDashing = false;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("WaitDraw");
                }
            }

            if (Input.GetMouseButtonUp(0) && weapon > 0 && isDashing == false && isBlocking == false)
            {
                if (weapon == 2 && drawSpeed <= 0f)
                {
                    attacking = true;
                    arrowSpeed = 2500;
                    arrow.SetActive(true);
                    GameObject arrowSummon = Instantiate(arrow, arrowSpawnB.transform.position, arrowSpawnB.transform.rotation);
                    arrowSummon.transform.Rotate(180f, 0f, 0f);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(bow.transform.forward * arrowSpeed);
                    Destroy(arrowSummon, 2f);
                    canAttack = false;
                    drawSpeed = 100f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }
                if (weapon == 2 && drawSpeed > 0f)
                {
                    attacking = true;
                    drawSpeed = 100f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }

                if (weapon == 5 && drawSpeed > 0f)
                {
                    attacking = true;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("BowCoolDown");
                }
                if (weapon == 5 && drawSpeed <= 0)
                {
                    attacking = true;
                    arrowSpeed = 4500;
                    arrow.SetActive(true);
                    GameObject arrowSummon = Instantiate(arrow, arrowSpawnC.transform.position, arrowSpawnC.transform.rotation);
                    arrowSummon.transform.Rotate(90f, 0f, 0f);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(crossbow.transform.up * arrowSpeed);
                    Destroy(arrowSummon, 2f);
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("CrossbowCoolDown");
                }
            }

            if (Input.GetMouseButtonUp(1) && weapon == 3 && isDashing == false && isBlocking == false)
            {
                if (weapon == 3 && drawSpeed >= 133.34f)
                {
                    chargeHurtBox.SetActive(true);
                    chargeLevel = 1;
                    isCharging = false;
                    chargeBoxActive = true;
                    chargeHurtBox.transform.localScale = new Vector3(2f, 1f, 2f);
                    attacking = true;
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("HammerCoolDown1");
                }
                if (weapon == 3 && drawSpeed > 66.67f && drawSpeed < 133.34)
                {
                    chargeHurtBox.SetActive(true);
                    chargeLevel = 2;
                    isCharging = false;
                    chargeBoxActive = true;
                    chargeHurtBox.transform.localScale = new Vector3(4f, 1f, 2f);
                    attacking = true;
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("HammerCoolDown2");
                }
                if (weapon == 3 && drawSpeed <= 66.67f)
                {
                    chargeHurtBox.SetActive(true);
                    chargeLevel = 3;
                    isCharging = false;
                    chargeBoxActive = true;
                    chargeHurtBox.transform.localScale = new Vector3(6f, 1f, 2f);
                    attacking = true;
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("HammerCoolDown3");
                }
            }
            
            if (Input.GetMouseButtonDown(0) && isBlocking == false && weapon > 0 && gm.started == true && isDashing == false)
            {
                if (weapon == 4)
                {
                    isDashing = false;
                    attacking = true;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("SpearCoolDown");
                }
            }

            if (Input.GetMouseButton(1) && canMove == true && isBlocking == false && weapon == 4)
            {
                myRB.constraints = RigidbodyConstraints.FreezeAll;
                canBlock = false;
                isBlocking = true;
                canMove = true;
                canUnblock = true;
            }

            if (Input.GetMouseButton(1) && canMove == true && isBlocking == false && weapon == 3)
            { 
                isCharging = true;
                myRB.constraints = RigidbodyConstraints.FreezeAll;
                isDashing = false;
                canAttack = false;
                canMove = true;
                StartCoroutine("WaitDraw");
            }

            if (Input.GetMouseButtonUp(1) && isBlocking == true && canUnblock == true && canBlock == false && weapon == 4)
            {
                myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                isBlocking = false;
                canMove = true;
                canBlock = true;
                canUnblock = false;
            }        

            if(isDashing == true && attacking == false && isBlocking == false)
            {
                myRB.velocity += playerRotationHolder.transform.forward * 1.5f;
            }

            if (Input.GetKeyDown(KeyCode.E) && stamina > 2 && isBlocking == false && canDash == true)
            {
                canTakeDamage = false;
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
           health--;
           StartCoroutine("WaitDamage"); 
       }
       if (other.gameObject.tag == "EnemyShot" && canTakeDamage == true && gm.GameOn == true)
       {
           canTakeDamage = false;
           health--;
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

    IEnumerator WaitDraw()
    {
         yield return new WaitForSeconds(1f);
         drawSpeed--;
    }

    IEnumerator WaitStringTime()
    {
        yield return new WaitForSeconds(1f);
        stringTimer -= Time.deltaTime;
    }

    IEnumerator SwordCoolDown()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
        attacking = false;
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }

    IEnumerator HammerCoolDownBase()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
        attacking = false;
        yield return new WaitForSeconds(2.7f);
        canAttack = true;
    }

    IEnumerator HammerCoolDown1()
    {
        yield return new WaitForSeconds(1f);
        chargeHurtBox.SetActive(false);
        chargeLevel = 0;
        canMove = true;
        attacking = false;
        chargeBoxActive = false;
        chargeHurtBox.transform.localScale = new Vector3(2f, 1f, 2f);
        yield return new WaitForSeconds(2.7f);
        canAttack = true;
    }

    IEnumerator HammerCoolDown2()
    {
        yield return new WaitForSeconds(1f);
        chargeHurtBox.SetActive(false);
        chargeLevel = 0;
        canMove = true;
        attacking = false;
        chargeBoxActive = false;
        chargeHurtBox.transform.localScale = new Vector3(2f, 1f, 2f);
        yield return new WaitForSeconds(3f);
        canAttack = true;
    }

    IEnumerator HammerCoolDown3()
    {
        yield return new WaitForSeconds(1f);
        chargeHurtBox.SetActive(false);
        chargeLevel = 0;
        canMove = true;
        attacking = false;
        chargeBoxActive = false;
        chargeHurtBox.transform.localScale = new Vector3(2f, 1f, 2f);
        yield return new WaitForSeconds(3.7f);
        canAttack = true;
    }

    IEnumerator SpearCoolDown()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
        attacking = false;
        yield return new WaitForSeconds(1.2f);
        canAttack = true;
    }

    IEnumerator BowCoolDown()
    {
        isCooldownOver = false;
        attacking = false;
        yield return new WaitForSeconds(2f);
        drawSpeed = 100f;
        canAttack = true;
        isCooldownOver = true;
    }

    IEnumerator CrossbowCoolDown()
    {
        isCooldownOver = false;
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