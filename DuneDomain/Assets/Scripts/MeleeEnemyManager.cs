using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeleeEnemyManager : MonoBehaviour
{
    public GameManager gm;
    public PlayerController player;
    public GameObject playerObject;

    [Header("Enemy Ref's")]
    public GameObject enemyObject;
    public NavMeshAgent agent;
    public Rigidbody enemyRidigbody;
    public GameObject enemySword;
    public GameObject corpsePrefab;

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public float speed;
    public bool canTakeDamage = true;
    public bool dead = false;
    public float timer;
    public bool attacking = false;
    public bool canAttack = true;
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        enemySword = enemyObject.transform.GetChild(0).gameObject;
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(2f, 4f);
        health = 5;
        maxHealth = 5;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        speed = 13f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {
            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            enemySword.SetActive(true);
            if (gm.meleeEnemyMovePattern == 2 && gm.GameOn == true && canMove == true && dead == false)
            {
                Vector3 lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = awayRotation;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else if (gm.meleeEnemyMovePattern == 1 && gm.GameOn == true && canMove == true && dead == false)
            {
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = awayRotation;
                enemyRidigbody.AddForce(lookDirection * speed);
            }

            if (gm.GameOn == true && dead == false && distance < 10)
            {
                if (timer > 0)
                {
                    timer -= 1 * Time.deltaTime;
                }

                if (timer <= 0)
                {
                    timer = 0;
                    canAttack = true;
                    attacking = true;
                }

                if (attacking == true && canAttack == true)
                {
                    enemySword.transform.eulerAngles = new Vector3(90f, enemyObject.transform.eulerAngles.y, enemyObject.transform.eulerAngles.z);
                    StartCoroutine("WaitAttack");
                }
            }
            else
            {
                attacking = false;
                canAttack = false;
                enemySword.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                canMove = true;
                timer = Random.Range(2f, 4f);
            }

            if (health <= 0 && dead == false)
            {
                Destroy(enemyObject);
                Instantiate(corpsePrefab, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y - 0.59f, enemyObject.transform.position.z), Quaternion.Euler(90, 0, 0));
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
    IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(1f);
        attacking = false;
        canAttack = false;
        enemySword.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        canMove = true;
        timer = Random.Range(2f, 4f);
    }
}
