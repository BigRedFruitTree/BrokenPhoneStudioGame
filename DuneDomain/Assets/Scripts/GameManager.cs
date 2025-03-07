using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Player Stuff")]
    public PlayerController playerController;
    public GameObject playerObject;

    [Header("Boss Stuff")]
    public float timer;
    public float timer2;
    public int rounds;
    public NavMeshAgent bossAgent;
    public GameObject bossObject;
    public GameObject bossSpawn;

    [Header("Enemy Stuff")]
    public MeleeEnemyManager enemyScript;
    public GameObject enemyObject;
    public float spawnRange;
    public GameObject enemyPrefab;
    public GameObject[] enemyNumber;
    public bool enemyCanMove = false;

    [Header("GameManager Stuff")]
    public bool GameOn = false;
    public bool GameOver = false;
    public int weapon = 0;
    public GameObject weaponScreen;
    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        timer = 3000f;
        timer2 = 2000f;
        rounds = 0;
        spawnRange = 20f;
        enemyObject = enemyPrefab;
        enemyNumber = GameObject.FindGameObjectsWithTag("MeleeEnemy");

        if (SceneManager.GetActiveScene().buildIndex > 0)
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
       if(GameOn == true && GameOver == false && started == true)
       {
            while (enemyNumber.Length > 14) 
            {
               Destroy(GameObject.FindGameObjectWithTag("MeleeEnemy"));
            }
            
            if(enemyNumber.Length == enemyNumber.Length/2)
            {
                enemyCanMove = false;
            }
            else
            {
                enemyCanMove = true;
            }
 
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
              if(enemyNumber.Length < 15)
              {
                 SpawnEnemies(rounds);
              }
              bossObject.transform.position = bossSpawn.transform.position;
              rounds++;
              bossObject.SetActive(false);   
              timer = 3000f;
              timer2 = 2000f + rounds;
           }
          
       }
    }

    public void SpawnEnemies(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
             Instantiate(enemyPrefab, GenerateSpawnPos(), enemyPrefab.transform.rotation);
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

    public void StartGame(int SceneID)
    {
        SceneManager.LoadScene(SceneID);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
    }
}
