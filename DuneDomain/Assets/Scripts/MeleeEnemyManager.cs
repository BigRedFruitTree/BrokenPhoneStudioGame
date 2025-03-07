using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeleeEnemyManager : MonoBehaviour
{
    public GameManager gm;

    public GameObject enemyObject;
    public Rigidbody enemyRidigbody;

    public PlayerController player;
    public GameObject playerObject;

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public float speed;
    public bool canTakeDamage = true;

    // Start is called before the first frame update
    void Start()
    {

        health = 5;
        maxHealth = 5;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        speed = 15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {
            if (gm.enemyCanMove == false)
            {
                Vector3 lookDirection = (playerObject.transform.position + enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else
            {
                Vector3 lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }

            if (health <= 0)
            {
                Destroy(enemyObject);
            }
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && player.canTakeDamage == true && gm.GameOn == true)
        {
            player.health--;
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

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true)
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
