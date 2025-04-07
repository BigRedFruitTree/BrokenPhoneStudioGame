using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class BossManager : MonoBehaviour
{

    public GameManager gm;

    public GameObject bossSpawn;
    public GameObject bossObject;

    public PlayerController player;
    public NavMeshAgent bossAgent;

    public Animator animator;

    public Rigidbody RB;

    [Header("Stats")]
    public float health = 200f;
    public float maxHealth = 200f;
    public int damage = 5;
    public bool canTakeDamage = true;
    public bool canEat = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        bossAgent = GetComponent<NavMeshAgent>();

        bossObject.transform.position = bossSpawn.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
       if(gm.GameOn == true && gm.GameOver == false)
       {
         if (health <= 0)
         {
            Destroy(bossObject);
         }
       }


    }

    public void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true && player.weapon == 2)
        {
            canTakeDamage = false;
            health -= 1;
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true && player.weapon == 5)
        {
            canTakeDamage = false;
            health -= 2;
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            canTakeDamage = false;
            health--;
            StartCoroutine("WaitDamage");

        }
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyCorpse" && gm.GameOn == true)
        {
            StartCoroutine("WaitEat");
        }
    }

    IEnumerator WaitEat()
    {
        canEat = true;
        yield return new WaitForSeconds(5f);
        canEat = false;
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
        canTakeDamage = true;
    }
}
