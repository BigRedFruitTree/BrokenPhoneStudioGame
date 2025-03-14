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

    // Start is called before the first frame update
    void Start()
    {

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
            if (gm.enemyMovePattern == 2 && dead == false)
            {
                agent.destination = -player.transform.position;
                Vector3 lookDirection = (-playerObject.transform.position - -enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else if (gm.enemyMovePattern == 1 && dead == false)
            {
                agent.destination = player.transform.position;
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
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
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            canTakeDamage = false;
            health--;
            StartCoroutine("WaitDamage");

        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
        canTakeDamage = true;
    }
}
