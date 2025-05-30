using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.ParticleSystem;

public class MeleeEnemyManager : MonoBehaviour
{
    public GameManager gm;
    public PlayerController player;
    public GameObject playerObject;

    [Header("Enemy Ref's")]
    public GameObject enemyObject;
    public NavMeshAgent agent;
    public Rigidbody enemyRidigbody;
    public GameObject corpsePrefab;
    public Animator animator;
    public GameObject model;
    public ParticleSystem bloodParticle;
    private ParticleSystem.MainModule main;
    public AudioSource audioSource;
    public AudioClip deathSFX;

    [Header("Stats")]
    public int health;
    public int maxHealth;
    public float speed;
    public bool canTakeDamage = true;
    public bool dead = false;
    public float timer;
    public bool attacking = false;
    public bool canAttack = true;
    public bool canMove = true;
    public bool canRotate = true;
    public Vector3 lookDirection;
    public float minBSize = 0.1f;
    public float maxBSize = 1.0f;
    public float minBLifetime = 0.5f;
    public float maxBLifetime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        model = enemyObject.transform.GetChild(0).gameObject;
        bloodParticle = enemyObject.transform.Find("BloodParticleM").GetComponentInChildren<ParticleSystem>();
        animator = model.GetComponent<Animator>();
        agent = enemyObject.GetComponent<NavMeshAgent>();
        timer = Random.Range(3f, 5f);
        health = 50 + gm.rounds;
        maxHealth = 50 + gm.rounds;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Player");
        enemyRidigbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        speed = 7f;
        model.SetActive(true);
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
                animator.SetBool("moving", true);
                lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }
            else if (gm.enemyMovementPattern == 1 && canMove == true && dead == false && attacking == false && animator.GetBool("attacking") == false)
            {
                animator.SetBool("moving", true);
                lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                enemyRidigbody.AddForce(lookDirection * speed);
            }

            if (gm.enemyMovementPattern == 2 && canRotate == true && dead == false)
            {
                lookDirection = (enemyObject.transform.position - playerObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }
            else if (gm.enemyMovementPattern == 1 && canRotate == true && dead == false)
            {
                lookDirection = (playerObject.transform.position - enemyObject.transform.position).normalized;
                Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
                enemyObject.transform.rotation = Quaternion.Euler(enemyObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, enemyObject.transform.rotation.eulerAngles.z);
            }

            if (gm.GameOn == true && dead == false && distance < 10 && gm.enemyMovementPattern == 1)
            {
                if (timer > 0f)
                {
                    timer -= 1 * Time.deltaTime;
                }

                if (timer <= 0f)
                {
                    timer = 0f;
                }

                if (canAttack == true && canMove == true && timer <= 0f)
                {
                    StartCoroutine("WaitAttack");
                }
            }
            if (gm.GameOn == true && dead == false && gm.enemyMovementPattern == 2)
            {
                attacking = false;
                canAttack = false;
                canMove = true;
                timer = Random.Range(3f, 5f);
            }

            if (health <= 0 && dead == false)
            {
                bloodParticle.Stop();
                StartCoroutine("WaitDeath");
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
    IEnumerator WaitDamage()
    {
        bloodParticle.Play();
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }
    IEnumerator WaitDeath()
    {
        dead = true;
        audioSource.clip = deathSFX;
        audioSource.Play();
        Instantiate(corpsePrefab, new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y, enemyObject.transform.position.z), Quaternion.Euler(0, 0, 0));
        model.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Destroy(enemyObject);

    }
    IEnumerator WaitAttack()
    {
        canMove = false;
        canRotate = false;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", true);
        animator.SetInteger("whichAttack", 1);
        canAttack = true;
        attacking = true;
        enemyRidigbody.velocity += transform.forward * 20;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", false);
        animator.SetInteger("whichAttack", 0);
        attacking = false;
        canAttack = false;
        canRotate = false;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", true);
        animator.SetInteger("whichAttack", 2);
        canRotate = false;
        attacking = true;
        enemyRidigbody.velocity += transform.forward * 20;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", false);
        animator.SetInteger("whichAttack", 0);
        attacking = false;
        canAttack = false;
        canRotate = false;
        canMove = true;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", true);
        animator.SetInteger("whichAttack", 3);
        canRotate = false;
        attacking = true;
        enemyRidigbody.velocity += transform.forward * 20;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", false);
        animator.SetInteger("whichAttack", 0);
        attacking = false;
        canAttack = false;
        canRotate = false;
        canMove = false;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", true);
        animator.SetInteger("whichAttack", 4);
        canRotate = false;
        attacking = true;
        enemyRidigbody.velocity += transform.forward * 20;
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("attacking", false);
        animator.SetInteger("whichAttack", 0);
        attacking = false;
        canRotate = true;
        canMove = true;
        yield return new WaitForSeconds(2f);
        canAttack = true;
        timer = Random.Range(3f, 5f);
    }
}
