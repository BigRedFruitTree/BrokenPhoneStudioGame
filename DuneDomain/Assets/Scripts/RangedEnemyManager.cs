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

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyBow = enemyObject.transform.GetChild(0).gameObject;
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(5f, 7f);
        timer2 = 5f;
        health = 5 + gm.rounds;
        maxHealth = 5 + gm.rounds;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        speed = 10f + gm.rounds;
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
                Vector3 lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else if (gm.rangedEnemyMovePattern == 1 && gm.GameOn == true && canWalk == true && dead == false)
            {
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }

            if (gm.rangedEnemyMovePattern == 2 && gm.GameOn == true && canRotate == true && dead == false)
            {
                Vector3 lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = awayRotation;
            }
            else if (gm.rangedEnemyMovePattern == 1 && gm.GameOn == true && canRotate == true && dead == false)
            {
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = awayRotation;
            }

            if (timer > 0f)
            {
                enemyBow.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                timer -= 1 * Time.deltaTime;
            }

            if (timer <= 0f && timer2 > 0f)
            {
                canWalk = false;
                enemyBow.transform.eulerAngles = new Vector3(90f, enemyObject.transform.eulerAngles.y, enemyObject.transform.eulerAngles.z);
                timer = 0f;
                timer2 -= 1 * Time.deltaTime;
            }
           
            if (timer2 <= 0f && canAttack == false)
            {
                timer2 = 0f;
                doneAttacking = false;
                canAttack = true;
            }

            if (doneAttacking == false && canAttack == true && timer <= 0f && timer2 <= 0f)
            {
                attacking = true;
                arrowSummon = Instantiate(arrow, enemyBow.transform.position, enemyBow.transform.rotation);
                arrowSummon.GetComponent<Rigidbody>().AddForce(arrowSummon.transform.up * 1000);
                arrow.SetActive(true);
                Destroy(arrowSummon, 2f);
                attacking = false;
                doneAttacking = true;
                canWalk = true;
                timer2 = 5f;
                canAttack = false;
                timer = Random.Range(3f, 5f);
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
        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true)
        {
            canTakeDamage = false;
            health--;
            StartCoroutine(nameof(WaitDamage));

        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            canTakeDamage = false;
            health--;
            StartCoroutine(nameof(WaitDamage));

        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
        canTakeDamage = true;
    }
}
