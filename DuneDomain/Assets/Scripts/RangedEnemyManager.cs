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
    public ParticleSystem bloodParticle;
    public AudioSource audioSource;
    public AudioClip deathSFX;
    private ParticleSystem.MainModule main;
    public BoxCollider nearestWall;

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public bool canTakeDamage = true;
    public bool dead = false;
    public float timer;
    public float timer2;
    public bool attacking = false;
    public bool canAttack = true;
    public bool canWalk = true;
    public bool canMove = true;
    public bool doneAttacking = true;
    public Vector3 lookDirection;
    public float minBSize = 0.1f;
    public float maxBSize = 1.0f;
    public float minBLifetime = 0.5f;
    public float maxBLifetime = 2.0f;
    public bool canRotate;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        model = enemyObject.transform.GetChild(0).gameObject;
        animator = model.GetComponent<Animator>();
        arrowSpawner = model.transform.GetChild(0).gameObject;
        bloodParticle = enemyObject.transform.Find("BloodParticleR").GetComponentInChildren<ParticleSystem>();
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(7f, 9f);
        timer2 = 5f;
        health = 30 + gm.rounds;
        maxHealth = 30 + gm.rounds;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        model.SetActive(true);
        canRotate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {
            main = bloodParticle.main;

            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            if (gm.enemyMovementPattern == 2 && canMove == true && dead == false && attacking == false && animator.GetBool("attacking") == false)
            {
                nearestWall = GetNearestWall();
                animator.SetBool("moving", true);
                agent.destination = nearestWall.transform.position;
            }
            else if (gm.enemyMovementPattern == 1 && canMove == true && dead == false && attacking == false && animator.GetBool("attacking") == false && animator.GetBool("charging") == false)
            {
                animator.SetBool("moving", true);
                agent.destination = playerObject.transform.position;
            }
            else
            {
                animator.SetBool("moving", false);
            }

            if (gm.enemyMovementPattern == 2 && canRotate == true && dead == false)
            {
                lookDirection = (nearestWall.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }
            else if (gm.enemyMovementPattern == 1 && canRotate == true && dead == false)
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
                agent.speed = 0;
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
                bloodParticle.Stop();
                dead = true;
                audioSource.clip = deathSFX;
                audioSource.Play();
                Destroy(enemyObject);
                Instantiate(corpsePrefab, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y, enemyObject.transform.position.z), Quaternion.Euler(0, 0, 0));
            }

            if (health < 10 && health > 0 && dead == false)
            {
                main.loop = true;
                bloodParticle.Play();
            }

            if (maxHealth > 100)
            {
                maxHealth = 100;
                health = 100;
            }

        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot" && canTakeDamage == true && gm.GameOn == true)
        {
            UpdateParticleSystem();
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
            UpdateParticleSystem();
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
            UpdateParticleSystem();
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
            UpdateParticleSystem();
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
    void UpdateParticleSystem()
    {
        float healthPercent = (float)health / maxHealth;
        float effectStrength = 1.5f - healthPercent;
        main.startSize = Mathf.Lerp(minBSize, maxBSize, effectStrength);
        main.startLifetime = Mathf.Lerp(minBLifetime, maxBLifetime, effectStrength);
    }

    public BoxCollider GetNearestWall()
    {
        BoxCollider nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var wall in gm.walls)
        {
            float distance = Vector3.Distance(enemyObject.transform.position, wall.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = wall.GetComponent<BoxCollider>();
            }
        }

        return nearestTarget;
    }

    IEnumerator WaitDamage()
    {
        bloodParticle.Play();
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }
    IEnumerator Attack()
    {
        canAttack = false;
        doneAttacking = false;
        agent.speed = 0;
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
        agent.speed = 3;
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
