using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Player Stuff")]
    public PlayerController playerController;

    [Header("Boss Stuff")]
    public float timer;
    public float timer2;
    public int rounds;
    public NavMeshAgent bossAgent;
    public GameObject bossObject;
    public GameObject bossSpawn;
  
    [Header("Enemy Stuff")]
    public NavMeshAgent EnemyAgent;
    public GameObject EnemyObject;
    public float spawnRange = 40f;
    public GameObject enemyPrefab;
    
    [Header("GameManager Stuff")]
    public bool GameOn = false;
    public bool GameOver = false;
    public int weapon = 0;
    public GameObject weaponScreen;
    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        timer = 300f;
        timer2 = 600f;
        rounds = 0;
        spawnRange = 40f;
    }

    // Update is called once per frame
    void Update()
    {
       if(GameOn == true && GameOver == false && started == true)
       {
           
           if (timer > 0f)
           {
              StartCoroutine("Wait");
              timer--;
           }
           if (timer <= 0f)
           {
               timer = 0f;
               bossObject.SetActive(true);
               bossAgent.destination = playerController.transform.position;
               StartCoroutine("Wait");
               timer2--;
           }

           if (timer2 <= 0f)
           {
              bossObject.transform.position = bossSpawn.transform.position;
              rounds++;
              bossObject.SetActive(false);   
              timer = 120f;
              timer2 = 600f + rounds;
           }
           if (timer <= 0f)
           {
                Instantiate(enemyPrefab, GenerateSpawnPos(), enemyPrefab.transform.rotation);
                EnemyObject.SetActive(true);
                EnemyAgent.destination = playerController.transform.position;
           }
       }
    }

    public Vector3 GenerateSpawnPos()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 1.5f, spawnPosZ);
        return randomPos;

    }

    public void ChooseWeapon(int chosenWeapon)
    {
        if(chosenWeapon == 1)
        {
            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
            started = true;
        }
        if(chosenWeapon == 2)
        {
            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
            started = true;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
    }
}
