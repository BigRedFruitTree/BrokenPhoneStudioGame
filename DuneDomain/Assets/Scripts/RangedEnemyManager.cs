using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyManager : MonoBehaviour
{

    [Header("References")]
    public GameManager gm;
    public PlayerController Player;
    public GameObject PlayerObject;
    public GameObject RangedEnemyObject;
    public GameObject RangedEnemySpawn;
    public Rigidbody EnemyRigidBody;

    [Header("Prefabs")]
    public GameObject RangedCorpsePrefab;

    [Header("Stats")]
    public int Health;
    public int MaxHealth;
    public float Speed;
    public bool CanTakeDamage = true;
    public bool Dead = false;


    // Start is called before the first frame update
    void Start()
    {
        Health = 5;
        MaxHealth = 5;
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        PlayerObject = GameObject.Find("Player");
        EnemyRigidBody = GetComponent<Rigidbody>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        Speed = 12f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.GameOn == true && gm.GameOver == false)
        {

        }
    }
}
