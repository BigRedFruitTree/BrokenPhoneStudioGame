using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyManager : MonoBehaviour
{

    [Header("References")]
    public GameManager gm;
    public PlayerController player;
    public GameObject PlayerObject;
    public GameObject RangedEnemyObject;
    public GameObject RangedEnemySpawn;

    [Header("Prefabs")]
    public GameObject RangedCorpsePrefab;

    [Header("Stats")]
    public float Health = 5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
