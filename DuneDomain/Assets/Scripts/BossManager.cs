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

    [Header("Stats")]
    public float health = 50f;
    public float maxHealth = 50f;
    public int damage = 5;
    public bool canTakeDamage = true;

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
         if(gm.bossEating == true)
         {
             
         }

         if (health <= 0)
         {
            Destroy(bossObject);
         }
       }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && gm.GameOn == true)
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
