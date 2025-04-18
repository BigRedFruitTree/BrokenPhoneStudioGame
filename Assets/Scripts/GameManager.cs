using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player Stuff")]
    public PlayerController playerController;
    public GameObject playerObject;
    public GameObject playerUiStuff;
    public Image pHealthBar;
    public Image pStaminaBar;
    

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
    public bool bossAttacking = false;
    public NavMeshAgent bossAgent;
    public GameObject bossObject;
    public GameObject bossSpawn;
    public GameObject bossattackObject;
    public BossManager bossScript;
    private GameObject currentTarget;
    public GameObject bossUiStuff;
    public Image bossBar;

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
    public GameObject winScreen;
    public GameObject pauseScreen;
    public GameObject weaponKeepButton;
    public GameObject weaponKeepTXT;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            bossObject.SetActive(false);
            startCycle = false;
            weaponScreen.SetActive(true);
            if (rounds <= 0)
            {
                weaponKeepButton.SetActive(false);
                weaponKeepTXT.SetActive(false);
            }

            timeUntilAppearance = 6000f;
            timeUntilEatPhase = 2000f;
            timeUntilAttack = Random.Range(200f, 300f);
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
        if (GameOn == true && GameOver == false && started == true && SceneManager.GetActiveScene().buildIndex > 0)
        {
            pHealthBar.fillAmount = Mathf.Clamp((float)playerController.health / (float)playerController.maxHealth, 0, 1);
            bossBar.fillAmount = Mathf.Clamp((float)bossScript.health / (float)50, 0, 1);
            pStaminaBar.fillAmount = Mathf.Clamp((float)playerController.stamina / (float)10, 0, 1);
            playerUiStuff.SetActive(true);
            float distance = Vector3.Distance(bossObject.transform.position, playerObject.transform.position);
            Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
            lookDirection.y = 0f;
            lookDirection.Normalize();
            Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
            if (bossScript.health <= 0)
            {
                winScreen.SetActive(true);
            }

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

            if (timeUntilAppearance <= 0f)
            {
                bossUiStuff.SetActive(true);
                if (timeUntilAttack > 0f && distance >= 10f)
                {
                    StartCoroutine("Wait");
                    timeUntilAttack--;
                }
                if (timeUntilAttack <= 0f && bossAttack == 0 && distance >= 10f)
                {
                    timeUntilAttack = 0f;
                    bossAttack = Random.Range(0, 4);
                }

                if (bossAttack == 1 && distance >= 10f)
                {
                    StartCoroutine("WaitAttack1");   
                }
                if (bossAttack == 2 && distance >= 10f)
                {
                    StartCoroutine("WaitAttack2");   
                }
                if (bossAttack == 3 && distance >= 10f)
                {
                    StartCoroutine("WaitAttack3");
                }

                timeUntilAppearance = 0f;
                bossObject.SetActive(true);
                if(distance <= 10f)
                {
                   bossAgent.ResetPath();
                   bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
                }
                else
                {
                   bossAgent.destination = playerObject.transform.position;
                }
                
                StartCoroutine("Wait");
                timeUntilEatPhase--;
            }

            if (timeUntilEatPhase <= 0f)
            {
                timeUntilEatPhase = 0f;
                if (enemyCorpseNumber.Length > 0)
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

            if (bossEating == false && timeUntilEatPhase <= 0f && enemyCorpseNumber.Length == 0)
            {
                if (rounds > 0 && startCycle == false)
                {
                    bossUiStuff.SetActive(false);
                    StartCoroutine("WaitBossAway");
                    StartCoroutine("WaitWeaponScreen");
                }
            }

            if (bossEating == false && timeUntilEatPhase <= 0f && startCycle == true && enemyCorpseNumber.Length == 0)
            {
                if (meleeEnemyNumber.Length < 15)
                {
                    SpawnMelee(rounds);
                }
                if (rangedEnemyNumber.Length < 15 && rounds >= 2)
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

            if (Input.GetKeyDown(KeyCode.Escape) && GameOn == true && playerController.health > 0 && bossScript.health > 0)
            {
                pauseScreen.SetActive(true);
                Time.timeScale = 0;
                GameOn = false;
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
        if (chosenWeapon == 1)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 2)
        {
            if (rounds > 1)
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
        if (weapon == 1)
        {
            if (rounds > 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (weapon == 2)
        {
            if (rounds > 1)
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

    public void ResumeGame()
    {
        GameOn = true;
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        started = true;
    }
    IEnumerator WaitAttack1()
    {
        bossattackObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        bossAttack = 0;
        timeUntilAttack = Random.Range(200f, 300f);
        bossattackObject.SetActive(false);
    }
    IEnumerator WaitAttack2()
    {
        bossattackObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        bossAttack = 0;
        timeUntilAttack = Random.Range(200f, 300f);
        bossattackObject.SetActive(false);
    }
    IEnumerator WaitAttack3()
    {
        bossattackObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        bossAttack = 0;
        timeUntilAttack = Random.Range(200f, 300f);
        bossattackObject.SetActive(false);
    }
    IEnumerator WaitBossAway()
    {
        yield return new WaitForSeconds(2f);
        bossObject.transform.position = bossSpawn.transform.position;
        bossObject.SetActive(false);
    }

    IEnumerator WaitForEating()
    {
        isProcessingTarget = true;
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
