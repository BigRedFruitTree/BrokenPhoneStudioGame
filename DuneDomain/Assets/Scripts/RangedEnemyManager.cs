using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RangedEnemyManager : MonoBehaviour
{

    [Header("References")]
    public GameManager gm;
    public PlayerController Player;
    public GameObject PlayerObject;
    public GameObject RangedEnemyObject;
    public Rigidbody EnemyRigidBody;
    public NavMeshAgent Agent;

    [Header("Prefabs")]
    public GameObject RangedCorpsePrefab;

    [Header("Stats")]
    public int Health;
    public int MaxHealth;
    public float speed;
    public bool CanTakeDamage = true;
    public bool dead = false;


    // Start is called before the first frame update
    void Start()
    {
        Health = 5;
        MaxHealth = 5;
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        PlayerObject = GameObject.Find("Player");
        EnemyRigidBody = GetComponent<Rigidbody>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        speed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {
            if (gm.enemyMovePattern == 2 && dead == false)
            {
                Agent.destination = -Player.transform.position;
                Vector3 lookDirection = (-PlayerObject.transform.position - -RangedEnemyObject.transform.position).normalized;
                EnemyRigidBody.AddForce(lookDirection * speed);
            }
            else if (gm.enemyMovePattern == 1 && dead == false)
            {
                Agent.destination = Player.transform.position;
                Vector3 lookDirection = (PlayerObject.transform.position - RangedEnemyObject.transform.position).normalized;
                EnemyRigidBody.AddForce(lookDirection * speed);
            }

            if (Health <= 0 && dead == false)
            {
                Destroy(RangedEnemyObject);
                Instantiate(RangedCorpsePrefab, new Vector3(RangedEnemyObject.transform.position.x, RangedEnemyObject.transform.position.y - 0.59f, RangedEnemyObject.transform.position.z), Quaternion.Euler(90, 0, 0));
                dead = true;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot" && CanTakeDamage == true && gm.GameOn == true)
        {
            CanTakeDamage = false;
            Health--;
            StartCoroutine("WaitDamage");

        }

        if (other.gameObject.name == "Sword" && CanTakeDamage == true && gm.GameOn == true && Player.attacking == true)
        {
            CanTakeDamage = false;
            Health--;
            StartCoroutine("WaitDamage");

        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
        CanTakeDamage = true;
    }
}
