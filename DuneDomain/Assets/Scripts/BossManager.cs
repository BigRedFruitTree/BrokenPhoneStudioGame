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
        health = 200f;
        maxHealth = 200f;
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

        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true)
        {
            canTakeDamage = false;
            if (gm.weapon == 5)
            {
                health -= 14;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            else
            {
                health -= 10;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        { 
            canTakeDamage = false;
            if (player.whichAttack == 4)
            {
                health -= 11;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            else
            {
                health -= 7;
                player.AudioSource.clip = player.Slash;
                player.AudioSource.Play();
            }
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Hammer" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (player.chargeLevel == 1)
            {
                health -= 10;
                player.AudioSource.clip = player.hammersound;
                player.AudioSource.Play();
            }
            else if (player.chargeLevel == 2)
            {
                health -= 12;
                player.AudioSource.clip = player.hammersound;
                player.AudioSource.Play();
            }
            else if (player.chargeLevel == 3)
            {
                health -= 15;
                player.AudioSource.clip = player.hammersound;
                player.AudioSource.Play();
            }
            else
            {
                if (player.whichAttack == 4)
                {
                    health -= 8;
                    player.AudioSource.clip = player.hammersound;
                    player.AudioSource.Play();
                }
                else
                {
                    health -= 7;
                    player.AudioSource.clip = player.hammersound;
                    player.AudioSource.Play();
                }
            }
            canTakeDamage = false;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Spear" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {  
            canTakeDamage = false;
            if (player.whichAttack == 4)
            {
                health -= 8;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            else
            {
                health -= 6;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            StartCoroutine("WaitDamage");
        }
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true)
        {
            canTakeDamage = false;
            if (gm.weapon == 5)
            {
                health -= 14;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            else
            {
                health -= 10;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Sword" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            canTakeDamage = false;
            if (player.whichAttack == 4)
            {
                health -= 11;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            else
            {
                health -= 7;
                player.AudioSource.clip = player.Slash;
                player.AudioSource.Play();
            }
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Hammer" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (player.chargeLevel == 1)
            {
                health -= 10;
                player.AudioSource.clip = player.hammersound;
                player.AudioSource.Play();
            }
            else if (player.chargeLevel == 2)
            {
                health -= 12;
                player.AudioSource.clip = player.hammersound;
                player.AudioSource.Play();
            }
            else if (player.chargeLevel == 3)
            {
                health -= 15;
                player.AudioSource.clip = player.hammersound;
                player.AudioSource.Play();
            }
            else
            {
                if (player.whichAttack == 4)
                {
                    health -= 8;
                    player.AudioSource.clip = player.hammersound;
                    player.AudioSource.Play();
                }
                else
                {
                    health -= 7;
                    player.AudioSource.clip = player.hammersound;
                    player.AudioSource.Play();
                }
            }
            canTakeDamage = false;
            StartCoroutine("WaitDamage");
        }

        if (other.gameObject.name == "Spear" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            canTakeDamage = false;
            if (player.whichAttack == 4)
            {
                health -= 8;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            else
            {
                health -= 6;
                player.AudioSource.clip = player.stabsound;
                player.AudioSource.Play();
            }
            StartCoroutine("WaitDamage");
        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }
}
