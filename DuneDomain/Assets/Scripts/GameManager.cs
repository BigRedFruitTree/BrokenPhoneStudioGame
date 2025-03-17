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
    public bool bossEating = false;
    public bool startCycle;
    private GameObject currentTarget;

    [Header("Enemy Stuff")]
    public float spawnRange;
    public MeleeEnemyManager enemyScript;
    public GameObject meleeEnemyObject;
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyObject;
    public GameObject rangedEnemyPrefab;
    public GameObject[] meleeEnemyNumber;
    public GameObject[] meleeEnemyCorpseNumber;
    public GameObject[] rangedEnemyNumber;
    public GameObject[] rangedEnemyCorpseNumber;
    public int meleeEnemyMovePattern = 0;
    public int rangedEnemyMovePattern = 0;

    [Header("GameManager Stuff")]
    public bool GameOn = false;
    public bool GameOver = false;
    public int weapon = 0;
    public GameObject weaponScreen;
    public GameObject weaponKeepButton;
    public GameObject weaponKeepTXT;
    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        startCycle = false;
        weaponScreen.SetActive(true);
        if(rounds <= 0)
        {
           weaponKeepButton.SetActive(false);
           weaponKeepTXT.SetActive(false);
        }
       
        timer = 6000f;
        timer2 = 2000f;
        rounds = 1;
        spawnRange = 20f;
        meleeEnemyObject = meleeEnemyPrefab;
        rangedEnemyObject = rangedEnemyPrefab;

        if (meleeEnemyCorpseNumber == null || meleeEnemyCorpseNumber.Length == 0) return;
        if (rangedEnemyCorpseNumber == null || rangedEnemyCorpseNumber.Length == 0) return;

        SetNextTarget();

        if (SceneManager.GetActiveScene().buildIndex > 0)
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
       if(GameOn == true && GameOver == false && started == true)
       {
            meleeEnemyCorpseNumber = GameObject.FindGameObjectsWithTag("MeleeEnemyCorpse");
            meleeEnemyNumber = GameObject.FindGameObjectsWithTag("MeleeEnemy");
            rangedEnemyCorpseNumber = GameObject.FindGameObjectsWithTag("RangedEnemyCorpse");
            rangedEnemyNumber = GameObject.FindGameObjectsWithTag("RangedEnemy");

            while (meleeEnemyNumber.Length > 14) 
            {
               Destroy(GameObject.FindGameObjectWithTag("MeleeEnemy"));
            }
            while (rangedEnemyNumber.Length > 14)
            {
               Destroy(GameObject.FindGameObjectWithTag("RangedEnemy"));
            }

            if (meleeEnemyCorpseNumber.Length >= meleeEnemyNumber.Length)
            {
                meleeEnemyMovePattern = 2;
            }
            else
            {
                meleeEnemyMovePattern = 1;
            }
            if (rangedEnemyCorpseNumber.Length >= rangedEnemyNumber.Length)
            {
                rangedEnemyMovePattern = 2;
            }
            else
            {
                rangedEnemyMovePattern = 1;
            }

           if (timer > 0f)
           { 
              StartCoroutine("Wait");
              timer--;
           }
           if (timer <= 0f && timer2 > 0f)
           {
               timer = 0f;
               bossObject.SetActive(true);
               bossAgent.destination = playerController.transform.position;
               StartCoroutine("Wait");
               timer2--;
           }

           if (timer2 <= 0f)
           {
              timer2 = 0f;
              if(meleeEnemyCorpseNumber.Length > 0 || rangedEnemyCorpseNumber.Length > 0)
              {
                 if(bossEating == false)
                 {
                    bossEating = true;
                    SetNextTarget();
                 }
                 
                 if (bossAgent.remainingDistance <= bossAgent.stoppingDistance)
                 {
                   StartCoroutine("WaitForEating");
                 }
              }
              else
              {
                 bossEating = false;
              }
           }

           if(bossEating == false && timer2 <= 0f && meleeEnemyCorpseNumber.Length == 0)
           {
              if(rounds > 0 && startCycle == false)
              {
                 bossObject.transform.position = bossSpawn.transform.position;
                 bossObject.SetActive(false);
                 StartCoroutine("WaitWeaponScreen");
              }
           }

           if(bossEating == false && timer2 <= 0f && meleeEnemyCorpseNumber.Length == 0 && startCycle == true)
           {
              if(meleeEnemyNumber.Length < 15)
              {
                 SpawnEnemies(rounds);
              } 
              timer = 6000f;
              timer2 = 2000f + rounds;
              startCycle = false;
             
           }

       }
    }

    public void SpawnEnemies(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
             Instantiate(meleeEnemyPrefab, GenerateSpawnPos(), meleeEnemyPrefab.transform.rotation);
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
            if(rounds > 1)
              startCycle = true;

            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if(chosenWeapon == 2)
        {
            if(rounds > 1)
             startCycle = true;

            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 3)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 4)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 5)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
    }

    public void KeepWeapon()
    {
        if(weapon == 1)
        {
            if(rounds > 1)
             startCycle = true;

            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }
        if(weapon == 2)
        {
            if(rounds > 1)
             startCycle = true;

            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (weapon == 3)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (weapon == 4)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (weapon == 5)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }

    }

    public void SetNextTarget()
    {
        if (meleeEnemyCorpseNumber.Length == 0) return;
        if (rangedEnemyCorpseNumber.Length == 0) return;

        currentTarget = GetNearestTarget();
        
        bossAgent.destination = currentTarget.transform.position;
    }



    public GameObject GetNearestTarget()
    {
        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var corpse in meleeEnemyCorpseNumber)
        {
            float distance = Vector3.Distance(bossObject.transform.position, corpse.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = corpse;
            }
        }
        foreach (var corpse in rangedEnemyCorpseNumber)
        {
            float distance = Vector3.Distance(bossObject.transform.position, corpse.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = corpse;
            }
        }

        return nearestTarget;
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
        yield return new WaitForSeconds(0.1f);
        started = true;
    }

    IEnumerator WaitForEating()
    {
        yield return new WaitForSeconds(5f);
        Destroy(currentTarget);
        SetNextTarget();
    }

    IEnumerator WaitWeaponScreen()
    {
        startCycle = true;
        yield return new WaitForSeconds(1f);
        GameOn = false;
        weaponScreen.SetActive(true);
        weaponKeepButton.SetActive(true);
        weaponKeepTXT.SetActive(true);
        rounds += 1;
    }
}
