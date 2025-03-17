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

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public float speed;
    public bool canTakeDamage = true;
    public bool dead = false;
    public float timer;
    public float timer2;
    public bool attacking = false;
    public bool canAttack = true;
    public bool canWalk = true;
    public bool canRotate = true;


    // Start is called before the first frame update
    void Start()
    {
        enemyBow = enemyObject.transform.GetChild(0).gameObject;
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(2f, 4f);
        timer2 = 50f;
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
            enemyBow.SetActive(true);
            if (gm.rangedEnemyMovePattern == 2 && gm.GameOn == true && canWalk == true)
            {
                Vector3 lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                enemyRidigbody.AddForce(-lookDirection * speed);
            }
            else if (gm.rangedEnemyMovePattern == 1 && gm.GameOn == true && canWalk == true)
            {
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }

            if (gm.rangedEnemyMovePattern == 2 && gm.GameOn == true && canRotate == true)
            {
                Vector3 lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(-lookDirection);
                enemyObject.transform.rotation = awayRotation;
            }
            else if (gm.rangedEnemyMovePattern == 1 && gm.GameOn == true && canRotate == true)
            {
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = awayRotation;
            }

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
                enemyBow.transform.eulerAngles = new Vector3(90f, enemyObject.transform.eulerAngles.y, enemyObject.transform.eulerAngles.z);
                StartCoroutine(nameof(WaitAttack));
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
        enemyBow.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        canWalk = true;
        timer = Random.Range(2f, 4f);
    }
}
