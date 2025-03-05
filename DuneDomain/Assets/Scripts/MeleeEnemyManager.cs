using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MeleeEnemyManager : MonoBehaviour
{
    public GameManager gm;

    public GameObject EnemyObject;

    public PlayerController player;
    public NavMeshAgent EnemyAgent;

    [Header("Stats")]
    public float health = 10f;
    public float maxHealth = 10f;
    public int damage = 2;
    public bool canTakeDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        EnemyAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {
            if (health <= 0)
            {
                Destroy(EnemyObject);
            }
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.health--;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot" && canTakeDamage == true)
        {
            canTakeDamage = false;
            health--;
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true)
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
