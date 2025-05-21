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
    public GameObject corpsePrefab;
    public GameObject arrow;
    private GameObject arrowSummon;
    public GameObject arrowSpawner;
    public Animator animator;
    public GameObject model;

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
    public bool doneAttacking = true;
    public Vector3 lookDirection;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        model = enemyObject.transform.GetChild(0).gameObject;
        animator = model.GetComponent<Animator>();
        arrowSpawner = model.transform.GetChild(0).gameObject;
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(7f, 9f);
        timer2 = 5f;
        health = 30 + gm.rounds;
        maxHealth = 30 + gm.rounds;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        speed = 7f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {

            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            if (gm.enemyMovementPattern == 2 && gm.GameOn == true && canWalk == true && dead == false || distance < 5 && gm.GameOn == true && canWalk == true && dead == false)
            {
                animator.SetBool("moving", true);
                lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else if (gm.enemyMovementPattern == 1 && gm.GameOn == true && canWalk == true && dead == false)
            {
                animator.SetBool("moving", true);
                lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else
            {
                animator.SetBool("moving", false);
            }

            if (gm.enemyMovementPattern == 2 && gm.GameOn == true && canRotate == true && dead == false)
            {
                lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }
            else if (gm.enemyMovementPattern == 1 && gm.GameOn == true && canRotate == true && dead == false)
            {
                lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }

            if (timer > 0f && gm.enemyMovementPattern == 1)
            {
                timer -= 1 * Time.deltaTime;
            }

            if (timer <= 0f && timer2 > 0f && gm.enemyMovementPattern == 1)
            {
                animator.SetBool("charging", true);
                canWalk = false;
                timer = 0f;
                timer2 -= 1 * Time.deltaTime;
            }

            if (doneAttacking == true && canAttack == true && timer <= 0f && timer2 <= 0f && gm.enemyMovementPattern == 1)
            {
                timer2 = 0f;
                StartCoroutine("Attack");
            }

            if (gm.enemyMovementPattern == 2)
            {
                animator.SetBool("attacking", false);
                animator.SetBool("charging", false);
                attacking = false;
                canAttack = false;
                doneAttacking = false;
                canWalk = true;
                timer2 = 5f;
                timer = Random.Range(7f, 9f);
            }

            if (gm.started == false || gm.GameOn == false)
            {
                animator.SetBool("attacking", true);
                attacking = false;
                canAttack = true;
                doneAttacking = true;
                canWalk = true;
                timer2 = 5f;
                timer = Random.Range(7f, 9f);
            }

            if (health <= 0 && dead == false)
            {
                Destroy(enemyObject);
                Instantiate(corpsePrefab, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y - 1f, enemyObject.transform.position.z), Quaternion.Euler(0, 0, -90));
                dead = true;
            }

            if (maxHealth > 99)
            {
                maxHealth = 99;
                health = 99;
            }

        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true)
        {
            if (gm.enemyMovementPattern == 2 && gm.weapon == 5)
            {
                enemyRidigbody.AddForce(lookDirection * 2500);
            }
            else if (gm.enemyMovementPattern == 1 && gm.weapon == 5)
            {
                enemyRidigbody.AddForce(-lookDirection * 2500);
            }
            if (gm.enemyMovementPattern == 2 && gm.weapon == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else if (gm.enemyMovementPattern == 1 && gm.weapon == 2)
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
            canTakeDamage = false;
            if (gm.weapon == 5)
            {
                health -= 12;
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
            if (gm.enemyMovementPattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
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
            if (gm.enemyMovementPattern == 2)
            {
                if (player.chargeLevel == 1)
                {
                    enemyRidigbody.AddForce(lookDirection * 3000);
                }
                else if (player.chargeLevel == 2)
                {
                    enemyRidigbody.AddForce(lookDirection * 3500);
                }
                else if (player.chargeLevel == 3)
                {
                    enemyRidigbody.AddForce(lookDirection * 4000);
                }
                else
                {
                    enemyRidigbody.AddForce(lookDirection * 2500);
                }
            }
            else
            {
                if (player.chargeLevel == 1)
                {
                    enemyRidigbody.AddForce(-lookDirection * 3000);
                }
                else if (player.chargeLevel == 2)
                {
                    enemyRidigbody.AddForce(-lookDirection * 3500);
                }
                else if (player.chargeLevel == 3)
                {
                    enemyRidigbody.AddForce(-lookDirection * 4000);
                }
                else
                {
                    enemyRidigbody.AddForce(-lookDirection * 2500);
                }
            }

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

        if (other.gameObject.name == "Spear" && canTakeDamage == true && gm.GameOn == true && player.attacking == true || other.gameObject.name == "Shield" && canTakeDamage == true && gm.GameOn == true && player.attacking == true)
        {
            if (gm.enemyMovementPattern == 2)
            {
                enemyRidigbody.AddForce(lookDirection * 2000);
            }
            else
            {
                enemyRidigbody.AddForce(-lookDirection * 2000);
            }
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

        if (other.gameObject.tag == "Wall" && gm.enemyMovementPattern == 2 && gm.GameOn == true)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }

    IEnumerator Attack()
    {
        canAttack = false;
        doneAttacking = false;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("charging", false);
        animator.SetBool("attacking", true);
        attacking = true;
        arrowSummon = Instantiate(arrow, arrowSpawner.transform.position, arrowSpawner.transform.rotation);
        arrowSummon.transform.Rotate(90f, 90f, 0f);
        arrowSummon.GetComponent<Rigidbody>().AddForce(-arrowSpawner.transform.right * 2000);
        arrow.SetActive(true);
        Destroy(arrowSummon, 2f);
        yield return new WaitForSeconds(0.1f);
        attacking = false;
        animator.SetBool("attacking", false);
        doneAttacking = true;
        canWalk = true;
        timer2 = 5f;
        timer = Random.Range(7f, 9f);
        yield return new WaitForSeconds(2f);
        canAttack = true;
        
    }
}
