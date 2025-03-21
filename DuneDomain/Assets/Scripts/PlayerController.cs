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

    [Header("Movement Settings")]
    public float speed = 7f;
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
    public GameObject shieldHolder;
    public GameObject sword;
    public GameObject hammer;
    public GameObject spear;
    public GameObject shield;
    public GameObject bow;
    public GameObject crossbow;
    public GameObject arrow;
    public Canvas Pausemenu;
    public MeleeEnemyManager enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        speed = 7f;
        drawSpeed = 200f;
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
        if (gm.GameOn == true && gm.GameOver == false && canMove == true && gm.started == true)
        {

            weapon = gm.weapon;

            playerCam.transform.position = cameraHolder.transform.position;

            Vector3 temp = myRB.velocity;

            float verticalMove = Input.GetAxisRaw("Vertical");
            float horizontalMove = Input.GetAxisRaw("Horizontal");

            temp.x = verticalMove * speed;
            temp.z = horizontalMove * speed;

            myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);

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
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 90f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 45f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 145f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -90f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -45f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, -45f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, -145f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, -145f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 0f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
                if (playerRotationHolder.transform.rotation == Quaternion.Euler(0f, 180f, 0f) && canMove == true)
                {
                    weaponHolder.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }

            if (Input.GetMouseButtonDown(0) && gm.started == true && gm.GameOn == true && isDashing == false && attacking == false || attacking == true && gm.started == true && gm.GameOn == true && isDashing == false || Input.GetMouseButton(0) && gm.started == true && gm.GameOn == true && isDashing == false && weapon == 2 || Input.GetMouseButton(0) && gm.started == true && gm.GameOn == true && isDashing == false && weapon == 5)
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

            if (Input.GetMouseButtonDown(0) && canAttack == true && weapon > 0 && gm.started == true && isDashing == false && attacking == false)
            {
                if (weapon == 1)
                {
                    attacking = true;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("SwordCoolDown");
                }

                if (weapon == 3)
                {
                    isDashing = false;
                    attacking = true;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("hammerCoolDown");
                }

                if (weapon == 4)
                {
                    isDashing = false;
                    attacking = true;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("SpearCoolDown");
                }
            }
            

            if (Input.GetMouseButton(0) && isCooldownOver == true && weapon > 0 && gm.started == true && isDashing == false)
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

            if (Input.GetMouseButtonUp(0) && weapon > 0 && gm.started == true && isDashing == false)
            {
                if (weapon == 2 && drawSpeed <= 0f)
                {
                    attacking = true;
                    arrowSpeed = 2000;
                    arrow.SetActive(true);
                    GameObject arrowSummon = Instantiate(arrow, bow.transform.position, bow.transform.rotation);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(arrowSummon.transform.up * arrowSpeed);
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
                    arrowSpeed = 4000;
                    arrow.SetActive(true);
                    GameObject arrowSummon = Instantiate(arrow, bow.transform.position, bow.transform.rotation);
                    arrowSummon.GetComponent<Rigidbody>().AddForce(arrowSummon.transform.up * arrowSpeed);
                    Destroy(arrowSummon, 2f);
                    canAttack = false;
                    drawSpeed = 200f;
                    myRB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    StartCoroutine("CrossbowCoolDown");
                }
            }
            
            if(Input.GetMouseButtonDown(1) && isBlocking == false && weapon > 0 && gm.started == true && isDashing == false)
            {
                if (weapon == 3)
                {
                    isDashing = false;
                    attacking = true;
                    myRB.constraints = RigidbodyConstraints.FreezeAll;
                    canAttack = false;
                    canMove = true;
                    StartCoroutine("hammerCoolDown");
                }
            }

            if (Input.GetMouseButtonDown(1) && isBlocking == false && weapon > 0 && gm.started == true && isDashing == false)
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

            if (Input.GetMouseButtonDown(1) && isBlocking == false && weapon == 4)
            {
                canBlock = false;
                isBlocking = true;
                canMove = false;
                canUnblock = false;
                StartCoroutine("UnblockCoolDown");
                canUnblock = true;
            }

            if (Input.GetMouseButtonDown(1) && isBlocking == true && canUnblock == true && weapon == 4)
            {
                isBlocking = false;
                canMove = true;
                canUnblock = false;
                StartCoroutine("ShieldCoolDown");
            }

            if (Input.GetKeyDown(KeyCode.E) && gm.started == true && stamina >= 5)
            {
                canTakeDamage = false;
                isDashing = true;
                stamina -= 10;
                myRB.AddForce(playerRotationHolder.transform.forward * 10000f, ForceMode.Force);
                StartCoroutine("WaitDash");
                StartCoroutine("WaitDamage");
            }

            if (!Input.GetKeyDown(KeyCode.E) && gm.started == true && gm.GameOn == true && isDashing == false)
            {
                if (stamina <= 10 && stamina < 10.0001)
                {
                    StartCoroutine("WaitStamina");
                }

                if (stamina > 10)
                    stamina = 10;
                

                if (stamina <= 0)
                    stamina = 0;


                if (health < 0)
                    health = 0;

                if (health == 0)
                    gm.GameOver = true;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    gm.PauseGame();
                }

                if (health > 0)
                {
                    StartCoroutine(nameof(WaitCheckNearestEnemy));
                }
            }
        }
    }
   
    public void OnCollisionStay(Collision collision)
    {
       if (collision.gameObject.tag == "MeleeEnemy" && canTakeDamage == true && gm.GameOn == true)
       {
           canTakeDamage = false;
           health--;
           StartCoroutine("WaitDamage");
       }
    }

    public void OnTriggerStay(Collider other)
    {
       if (other.gameObject.tag == "EnemySword" && canTakeDamage == true && gm.GameOn == true && enemyScript.attacking == true)
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

    public void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.name == "Boss" && gm.GameOn == true && canTakeDamage == true)
       {
           canTakeDamage = false;
           health--;
           StartCoroutine("WaitDamage");
       }
    }

    public MeleeEnemyManager GetNearestTarget()
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

    IEnumerator WaitDraw()
    {
         yield return new WaitForSeconds(1f);
         drawSpeed--;
    }

    IEnumerator SwordCoolDown()
    {
         yield return new WaitForSeconds(1f);
         canAttack = true;
         attacking = false;
    }

    IEnumerator hammerCoolDown()
    {
        yield return new WaitForSeconds(3f);
        canAttack = true;
        attacking = false;
    }

    IEnumerator SpearCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
        attacking = false;
    }

    IEnumerator BowCoolDown()
    {
         isCooldownOver = false;
         yield return new WaitForSeconds(2f);
         drawSpeed = 100f;
         canAttack = true;
         attacking = false;
         isCooldownOver = true;
    }

    IEnumerator CrossbowCoolDown()
    {
        isCooldownOver = false;
        yield return new WaitForSeconds(5f);
        drawSpeed = 200f;
        canAttack = true;
        attacking = false;
        isCooldownOver = true;
    }

    IEnumerator ShieldCoolDown()
    {
        yield return new WaitForSeconds(1f);
        canBlock = true;
        isBlocking = false;
    }

    IEnumerator UnblockCoolDown()
    {
        yield return new WaitForSeconds(1f);
    }

    IEnumerator WaitDash()
    {
         yield return new WaitForSeconds(1f);
         isDashing = false;
    }

    IEnumerator WaitStamina()
    {
         yield return new WaitForSeconds(2f);
         stamina += 0.1f;
    }
     
    IEnumerator WaitCheckNearestEnemy()
    {
         yield return new WaitForSeconds(0.1f);
         enemyScript = GetNearestTarget();
    }   
}