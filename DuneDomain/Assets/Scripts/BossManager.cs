using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class BossManager : MonoBehaviour
{

    public GameManager gm;

    public GameObject bossSpawn;
    public GameObject bossObject;

    public PlayerController Player;
    public NavMeshAgent Agent;

    [Header("Stats")]
    public float health = 50f;
    public float maxHealth = 50f;
    public int damage = 5;
    public float timer;
    public float timer2;
    public int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        Agent = GetComponent<NavMeshAgent>();

        round = 0;
        bossObject.transform.position = bossSpawn.transform.position;

        timer = 60f;
        timer2 = 60f;
    }

    // Update is called once per frame
    void Update()
    {
       if(gm.GameOn == true && gm.GameOver == false)
       {
           
           if (timer > 0f)
           {
              StartCoroutine("Wait");
              timer--;
           }
           if (timer <= 0f)
           {
               timer = 0f;
               round++;
               bossObject.SetActive(true);
               Agent.destination = Player.transform.position;
               StartCoroutine("Wait");
               timer2--;

           }

           if (timer2 <= 0f)
           {
              bossObject.SetActive(false);   
              timer = 60f + round;
              timer2 = 60f;
           }
       }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.health--;
        }

        if (collision.gameObject.tag == "Shot")
        {
            health--;
        }
        if (health <= 0)
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
    }
}
