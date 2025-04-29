using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RangedEnemyManager : MonoBehaviour
{
    public GameManager gm;
    public PlayerController player;
    public GameObject playerObject;

    [Header("Enemy Ref's")]
    public GameObject enemyObject;
    public NavMeshAgent agent;
    public Rigidbody enemyRidigbody;
    public GameObject enemyBow;
    public GameObject corpsePrefab;
    public GameObject arrow;
    private GameObject arrowSummon;
    public GameObject arrowSpawner;

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public float speed;
    public bool canTakeDamage = true;
    public bool dead = false;
    public float timer;
    public float timer2;
    public bool attacking = false;
    public bool canAttack = false;
    public bool canWalk = true;
    public bool canRotate = true;
    public bool doneAttacking = true;
    public Vector3 lookDirection;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyBow = enemyObject.transform.GetChild(0).gameObject;
        arrowSpawner = enemyObject.transform.GetChild(1).gameObject;
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(7f, 9f);
        timer2 = 5f;
        health = 30 + gm.rounds;
        maxHealth = 30 + gm.rounds;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        speed = 4f + gm.rounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {
            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            enemyBow.SetActive(true);
            if (gm.rangedEnemyMovePattern == 2 && gm.GameOn == true && canWalk == true && dead == false || distance < 5 && gm.GameOn == true && canWalk == true && dead == false)
            {
                lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else if (gm.rangedEnemyMovePattern == 1 && gm.GameOn == true && canWalk == true && dead == false)
            {
                lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }

            if (gm.rangedEnemyMovePattern == 2 && gm.GameOn == true && canRotate == true && dead == false)
            {
                lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }
            else if (gm.rangedEnemyMovePattern == 1 && gm.GameOn == true && canRotate == true && dead == false)
            {
                lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }

            if (timer > 0f && gm.rangedEnemyMovePattern == 1)
            {
                enemyBow.transform.eulerAngles = new Vector3(0f, enemyObject.transform.eulerAngles.y, 0f);
                timer -= 1 * Time.deltaTime;
            }

            if (timer <= 0f && timer2 > 0f && gm.rangedEnemyMovePattern == 1)
            {
                canWalk = false;
                enemyBow.transform.eulerAngles = new Vector3(0f, enemyObject.transform.eulerAngles.y, enemyObject.transform.eulerAngles.z);
                timer = 0f;
                timer2 -= 1 * Time.deltaTime;
            }
           
            if (timer2 <= 0f && canAttack == false && gm.rangedEnemyMovePattern == 1)
            {
                timer2 = 0f;
                doneAttacking = false;
                canAttack = true;
            }

            if (doneAttacking == false && canAttack == true && timer <= 0f && timer2 <= 0f && gm.rangedEnemyMovePattern == 1)
            {
                attacking = true;
                arrowSummon = Instantiate(arrow, arrowSpawner.transform.position, arrowSpawner.transform.rotation);
                arrowSummon.transform.Rotate(180f, 0f, 0f);
                arrowSummon.GetComponent<Rigidbody>().AddForce(enemyBow.transform.forward * 2000);
                arrow.SetActive(true);
                Destroy(arrowSummon, 2f);
                attacking = false;
                doneAttacking = true;
                canWalk = true;
                timer2 = 5f;
                canAttack = false;
                timer = Random.Range(7f, 9f);
            }

            if (health <= 0 && dead == false)
            {
                Destroy(enemyObject);
                Instantiate(corpsePrefab, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y - 0.59f, enemyObject.transform.position.z), Quaternion.Euler(90f, 0f, 0f));
                dead = true;
            }

        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true && player.weapon == 2)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
            canTakeDamage = false;
            health -= 6;
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true && player.weapon == 5)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2500);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2500);
            }
            canTakeDamage = false;
            health -= 8;
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
            canTakeDamage = false;
            health -= 7;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Hammer" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2500);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2500);
            }
            canTakeDamage = false;
            health -= 10;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Hammer" && canTakeDamage == true && gm.GameOn == true && player.attacking == true && player.chargeLevel == 1)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 3000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 3000);
            }
            canTakeDamage = false;
            health -= 12;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Hammer" && canTakeDamage == true && gm.GameOn == true && player.attacking == true && player.chargeLevel == 2)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 3500);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 3500);
            }
            canTakeDamage = false;
            health -= 15;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Hammer" && canTakeDamage == true && gm.GameOn == true && player.attacking == true && player.chargeLevel == 3)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 4000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 4000);
            }
            canTakeDamage = false;
            health -= 20;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Spear" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
            canTakeDamage = false;
            health -= 5;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Shield" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (gm.meleeEnemyMovePattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
            canTakeDamage = false;
            health -= 4;
            StartCoroutine("WaitDamage");
        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }
}
