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
    public float timeUntilAppearance;
    public float timeUntilEatPhase;
    public float timeUntilAttack;
    public int rounds;
    public bool bossEating = false;
    public bool startCycle;
    public bool canBossEat = false;
    public bool isProcessingTarget = false;
    public int bossAttack = 0;
    public NavMeshAgent bossAgent;
    public GameObject bossObject;
    public GameObject bossSpawn;
    public BossManager bossScript;
    private GameObject currentTarget;

    [Header("Enemy Stuff")]
    public float spawnRange;
    public MeleeEnemyManager enemyScript;
    public GameObject meleeEnemyObject;
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyObject;
    public GameObject rangedEnemyPrefab;
    public GameObject[] meleeEnemyNumber;
    public GameObject[] rangedEnemyNumber;
    public GameObject[] enemyCorpseNumber;
    public int meleeEnemyMovePattern = 0;
    public int rangedEnemyMovePattern = 0;

    [Header("GameManager Stuff")]
    public bool GameOn = false;
    public bool GameOver = false;
    public int weapon = 0;
    public bool started = false;
    public GameObject weaponScreen;
    public GameObject deathScreen;
    public GameObject weaponKeepButton;
    public GameObject weaponKeepTXT;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            startCycle = false;
            weaponScreen.SetActive(true);
            if(rounds <= 0)
            {
               weaponKeepButton.SetActive(false);
               weaponKeepTXT.SetActive(false);
            }
       
            timeUntilAppearance = 6000f;
            timeUntilEatPhase = 2000f;
            timeUntilAttack = Random.Range(5f, 10f);
            rounds = 1;
            spawnRange = 40f;
            meleeEnemyObject = meleeEnemyPrefab;
            rangedEnemyObject = rangedEnemyPrefab;
 
            if (enemyCorpseNumber == null || enemyCorpseNumber.Length == 0) return;

            SetNextTarget();

            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        
    }

    // Update is called once per frame
    void Update()
    {
       if(GameOn == true && GameOver == false && started == true)
       {
            meleeEnemyNumber = GameObject.FindGameObjectsWithTag("MeleeEnemy");
            enemyCorpseNumber = GameObject.FindGameObjectsWithTag("EnemyCorpse");
            rangedEnemyNumber = GameObject.FindGameObjectsWithTag("RangedEnemy");

            while (meleeEnemyNumber.Length > 14) 
            {
               Destroy(GameObject.FindGameObjectWithTag("MeleeEnemy"));
            }
            while (rangedEnemyNumber.Length > 14)
            {
               Destroy(GameObject.FindGameObjectWithTag("RangedEnemy"));
            }

            if (enemyCorpseNumber.Length >= rangedEnemyNumber.Length)
            {
                rangedEnemyMovePattern = 2;
            }
            else
            {
                rangedEnemyMovePattern = 1;
            }

            if (enemyCorpseNumber.Length >= meleeEnemyNumber.Length)
            {
                meleeEnemyMovePattern = 2;
            }
            else
            {
                meleeEnemyMovePattern = 1;
            }

            if (timeUntilAppearance > 0f)
            { 
              StartCoroutine("Wait");
              timeUntilAppearance--;
            }

            if (timeUntilAppearance <= 0f && timeUntilEatPhase > 0f)
            {
               if (timeUntilAttack > 0f)
               {

               }
               timeUntilAppearance = 0f;
               bossObject.SetActive(true);
               bossAgent.destination = playerController.transform.position;
               StartCoroutine("Wait");
               timeUntilEatPhase--;
            }

           if (timeUntilEatPhase <= 0f)
           {
              timeUntilEatPhase = 0f;
              if(enemyCorpseNumber.Length > 0)
              {
                 if (bossEating == false)
                 {
                     SetNextTarget();
                 }
                 if (!isProcessingTarget && currentTarget != null && !bossAgent.pathPending && bossAgent.remainingDistance <= bossAgent.stoppingDistance)
                 {
                     bossEating = true;
                 }
                 if (!isProcessingTarget && currentTarget != null && bossEating == true && !bossAgent.pathPending && bossAgent.remainingDistance <= bossAgent.stoppingDistance)
                 {
                     StartCoroutine("WaitForEating");
                     bossEating = false;
                 }
              }
              else
              {
                 bossEating = false;
              }
           }

           if(bossEating == false && timeUntilEatPhase <= 0f && enemyCorpseNumber.Length == 0)
           {
              if(rounds > 0 && startCycle == false)
              {
                 StartCoroutine("WaitBossAway");
                 StartCoroutine("WaitWeaponScreen");
              }
           }

           if(bossEating == false && timeUntilEatPhase <= 0f && startCycle == true && enemyCorpseNumber.Length == 0)
           {
              if(meleeEnemyNumber.Length < 15)
              {
                 SpawnMelee(rounds);
              } 
              if(rangedEnemyNumber.Length < 15 && rounds >= 2)
              {
                 SpawnRanged(rounds);
              }
              timeUntilAppearance = 6000f;
              timeUntilEatPhase = 2000f;
              startCycle = false;
             
           }

           if (playerController.health <= 0)
           {
                GameOn = false;
                GameOver = true;
                deathScreen.SetActive(true);
                Time.timeScale = 0;
           }

       }
    }

    public void SpawnMelee(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
             Instantiate(meleeEnemyPrefab, GenerateSpawnPos(), meleeEnemyPrefab.transform.rotation);
        }
    }
    public void SpawnRanged(int numberToSpawn)
    {
        
        for (int i = 0; i < numberToSpawn; i++)
        {
             Instantiate(rangedEnemyPrefab, GenerateSpawnPos(), rangedEnemyPrefab.transform.rotation);
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

            playerController.drawSpeed = 100f;
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

            playerController.drawSpeed = 200f;
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

            playerController.drawSpeed = 100f;
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

            playerController.drawSpeed = 200f;
            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }

    }

    public void LoadScene(int SceneID)
    {
        SceneManager.LoadScene(SceneID);
    }


    public void SetNextTarget()
    {
        if (enemyCorpseNumber.Length == 0) 
            return;

        currentTarget = GetNearestTarget();

        bossAgent.destination = currentTarget.transform.position;
    }

    public GameObject GetNearestTarget()
    {
        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var corpse in enemyCorpseNumber)
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

    IEnumerator WaitBossAway()
    {
        yield return new WaitForSeconds(2f);
        bossObject.transform.position = bossSpawn.transform.position;
        bossObject.SetActive(false);
    }

    IEnumerator WaitForEating()
    {   isProcessingTarget = true;
        yield return new WaitForSeconds(5f);
        if (currentTarget != null)
        {
           Destroy(currentTarget);
        }
        currentTarget = null;
        SetNextTarget();
        isProcessingTarget = false;
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
