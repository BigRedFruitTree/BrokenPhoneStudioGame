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
    public float sleepDistance;
    public float bossDistance;
    public float bridgeDistance;
    public bool canRun;
    public bool canDash = true;
    public NavMeshAgent bossAgent;
    public GameObject bossObject;
    public GameObject bossSpawn;
    public GameObject bossSleepPoint;
    public BossManager bossScript;
    private GameObject currentTarget;
    public Rigidbody bossRigidBody;
    public GameObject bossUiStuff;
    public Image bossBar;
    public Animator bossanimator;
    public bool transitionAttack = false;
    public bool canAttack = true;

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
    public GameObject NotWaitingAlertScreen;
    public GameObject winScreen;
    public GameObject pauseScreen;
    public GameObject weaponKeepButton;
    public GameObject weaponKeepTXT;
    public GameObject TutorialStuff;
    public GameObject TutorialScreen2;
    public GameObject TutorialScreen3;
    public GameObject TutorialScreen4;
    public GameObject TutorialScreen5;
    public GameObject bridge;
    public bool canSpawnRocks = true;
    public GameObject DirectionalLight;
    public bool rotatetime = true;
    public float lightrotation = 60f;

    [Header("Game Map Stuff")]
    public GameObject Rock1Prefab;
    public GameObject Rock2Prefab;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            bossObject.SetActive(false);
            startCycle = false;
            TutorialStuff.SetActive(true);
            TutorialScreen2.SetActive(true);
            if (rounds <= 0)
            {
                weaponKeepButton.SetActive(false);
                weaponKeepTXT.SetActive(false);
            }

            timeUntilAppearance = 6000f;
            timeUntilEatPhase = 2000f;
            timeUntilAttack = Random.Range(200f, 300f);
            rounds = 1;
            spawnRange = 50f;
            meleeEnemyObject = meleeEnemyPrefab;
            rangedEnemyObject = rangedEnemyPrefab;

            if (enemyCorpseNumber == null || enemyCorpseNumber.Length == 0) return;

            SetNextTarget();
            if (rounds == 1 && sleepDistance > 4f)
            {
                bossanimator.SetBool("Isaggressive", false);
                bossAgent.speed = 3;
            }
            else
            {
                bossanimator.SetBool("Isaggressive", true);
                bossAgent.speed = 5;
            }

            playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        }
        else
        {
            weaponScreen.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOn == true && GameOver == false && started == true && SceneManager.GetActiveScene().buildIndex > 0)
        {
            pHealthBar.fillAmount = Mathf.Clamp((float)playerController.health / (float)playerController.maxHealth, 0, 1);
            bossBar.fillAmount = Mathf.Clamp((float)bossScript.health / (float)100, 0, 1);
            pStaminaBar.fillAmount = Mathf.Clamp((float)playerController.stamina / (float)10, 0, 1);
            playerUiStuff.SetActive(true);
            bossDistance = Vector3.Distance(bossObject.transform.position, playerObject.transform.position);
            bridgeDistance = Vector3.Distance(bridge.transform.position, playerObject.transform.position);
            sleepDistance = Vector3.Distance(bossSleepPoint.transform.position, bossObject.transform.position);
            if (bossScript.health <= 0)
            {
                winScreen.SetActive(true);
            }

            if (started == true && canSpawnRocks == true)
            {
                SpawnRock1atRandomPosition(5);
                SpawnRock2atRandomPosition(5);
                canSpawnRocks = false;
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

            if (rounds == 1 && timeUntilAppearance <= 0f && canRun == false)
            {
                bossObject.SetActive(true);
                if (sleepDistance > 4f)
                {
                    canRun = false;
                    bossAgent.speed = 3;
                    bossanimator.SetBool("Iswalking", true);
                    bossanimator.SetBool("Isaggressive", false);
                    bossAgent.destination = bossSleepPoint.transform.position;
                }
                if (sleepDistance <= 4f && bossanimator.GetBool("Isaggressive") == false)
                {
                    bossRigidBody.constraints = RigidbodyConstraints.FreezeAll;
                    bossanimator.SetBool("Iswalking", false);
                    bossanimator.SetBool("Issleeping", true);
                    NotWaitingAlertScreen.SetActive(true);
                    if (bridgeDistance <= 26)
                    {
                        StartCoroutine("WaitStart");
                    }
                }
            }


            if (rounds > 1 && timeUntilAppearance <= 0f && canRun == false)
            {
                bossObject.SetActive(true);
                bossAgent.speed = 5;
                canRun = true;
                bossanimator.SetBool("Isaggressive", true);
                bossanimator.SetBool("Iswalking", true);
                bossanimator.SetBool("Issleeping", false);
            }

            if (timeUntilAppearance <= 0f && canRun == true)
            {

                if (timeUntilAttack > 0f && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true)
                {
                    StartCoroutine("Wait");
                    timeUntilAttack--;
                }
                if (timeUntilAttack <= 0f && bossAttack == 0 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true)
                {
                    timeUntilAttack = 0f;
                    bossAttack = 1;//Random.Range(0, 4);
                }

                if (bossAttack == 1 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true)
                {
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack1");
                }
                if (bossAttack == 2 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true)
                {
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack2");
                }
                if (bossAttack == 3 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true)
                {
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack3");
                }

                timeUntilAppearance = 0f;
                if (bossDistance <= 16f && bossanimator.GetBool("Dodgeback") == false && canDash == true && bossanimator.GetBool("attacking") == false)
                {
                    bossAgent.ResetPath();
                    bossAgent.speed = 0;
                    StartCoroutine("WaitForWalking");
                }
                if (bossDistance > 16f && bossanimator.GetBool("Dodgeback") == false)
                {
                    bossAgent.destination = playerObject.transform.position;
                    bossanimator.SetBool("Isaggressive", true);
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
                    StartCoroutine("WaitAddRock1");

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
        // time stuff
        if (rotatetime == true && GameOn == true)
        {
            rotatetime = false;
            DirectionalLight.transform.rotation = Quaternion.Euler(lightrotation, -30, 0);
            lightrotation += 5;
            StartCoroutine("RotateWait");
        }
    }
    IEnumerator RotateWait()
    {
        yield return new WaitForSeconds(15f);
        rotatetime = true;
    }

    //This Generates a random spawn position for the 2 enemy types
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

    //This Generates a random position of rocks throughout the arena
    public void SpawnRock1atRandomPosition(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Instantiate(Rock1Prefab, RandomRockPosition(), Rock1Prefab.transform.rotation);
        }
    }
    public void SpawnRock2atRandomPosition(int numberToSpawn)
    {

        for (int i = 0; i < numberToSpawn; i++)
        {
            Instantiate(Rock2Prefab, RandomRockPosition(), Rock2Prefab.transform.rotation);
        }
    }

    public Vector3 RandomRockPosition()
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
            playerController.drawSpeed = 200f;
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
            playerController.drawSpeed = 200f;
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

    public void ContinueToTutorialScreen3()
    {
        TutorialScreen2.SetActive(false);
        TutorialScreen3.SetActive(true);
    }
    public void ContinueToTutorialScreen4()
    {
        TutorialScreen3.SetActive(false);
        TutorialScreen4.SetActive(true);
    }
    public void ContinueToTutorialScreen5()
    {
        TutorialScreen4.SetActive(false);
        TutorialScreen5.SetActive(true);
    }
    public void FinishButton()
    {
        TutorialStuff.SetActive(false);
        TutorialScreen5.SetActive(false);
        weaponScreen.SetActive(true);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        started = true;
    }

    IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(1f);
        bossRigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        bossAgent.speed = 5;
        Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
        lookDirection.y = 0f;
        lookDirection.Normalize();
        Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
        bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
        bossanimator.SetBool("Issleeping", false);
        bossanimator.SetBool("Isaggressive", true);
        NotWaitingAlertScreen.SetActive(false);
        yield return new WaitForSeconds(2f);
        canRun = true;
        bossanimator.SetBool("Iswalking", true);
    }
    IEnumerator WaitAttack1()
    {
        canAttack = false;
        int tempI = Random.Range(1, 4);
        if (tempI == 2)
        {
            Debug.Log("Its" + tempI);
            transitionAttack = true;
        }
        else
        {
            Debug.Log("Its" + tempI);
            transitionAttack = false;
        }
        bossAgent.ResetPath();
        bossAgent.speed = 0;
        Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
        lookDirection.y = 0f;
        lookDirection.Normalize();
        Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
        bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
        yield return new WaitForSeconds(2f);
        bossanimator.SetBool("attacking", false);
        if (transitionAttack == true)
        {
            bossAgent.speed = 0;
            bossAttack = 1;
            lookDirection = (playerObject.transform.position - bossObject.transform.position);
            lookDirection.y = 0f;
            lookDirection.Normalize();
            awayRotation = Quaternion.LookRotation(lookDirection);
            bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
            bossanimator.SetBool("attacking", true);
            bossanimator.SetBool("transitionAttack", true);
            yield return new WaitForSeconds(2.5f);
            bossAgent.speed = 5;
            bossAttack = 0;
            timeUntilAttack = Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            yield return new WaitForSeconds(2f);
            canAttack = true;
        }
        else
        {
            bossAgent.speed = 5;
            bossAttack = 0;
            timeUntilAttack = Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            yield return new WaitForSeconds(2f);
            canAttack = true;
        }
    }
    IEnumerator WaitAttack2()
    {
        canAttack = false;
        bossAgent.ResetPath();
        bossAgent.speed = 0;
        Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
        lookDirection.y = 0f;
        lookDirection.Normalize();
        Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
        bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
        yield return new WaitForSeconds(2f);
        bossAgent.speed = 5;
        bossAttack = 0;
        timeUntilAttack = Random.Range(100f, 150f);
        bossAgent.destination = playerObject.transform.position;
        bossanimator.SetBool("attacking", false);
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }
    IEnumerator WaitAttack3()
    {
        canAttack = false;
        bossAgent.ResetPath();
        bossAgent.speed = 0;
        Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
        lookDirection.y = 0f;
        lookDirection.Normalize();
        Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
        bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
        yield return new WaitForSeconds(2f);
        bossAgent.speed = 5;
        bossAttack = 0;
        timeUntilAttack = Random.Range(100f, 150f);
        bossAgent.destination = playerObject.transform.position;
        bossanimator.SetBool("attacking", false);
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }
    IEnumerator WaitBossAway()
    {
        yield return new WaitForSeconds(2f);
        bossObject.transform.position = bossSpawn.transform.position;
        bossObject.SetActive(false);
        canRun = false;
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
        yield return new WaitForSeconds(3f);
        GameOn = false;
        weaponScreen.SetActive(true);
        weaponKeepButton.SetActive(true);
        weaponKeepTXT.SetActive(true);
        rounds += 1;
    }

    IEnumerator WaitAddRock1()
    {
        yield return new WaitForSeconds(2f);
        SpawnRock1atRandomPosition(3);
    }

    IEnumerator WaitForWalking()
    {
        canDash = false;
        bossAgent.ResetPath();
        bossAgent.speed = 0;
        yield return new WaitForSeconds(0.5f);
        bossAgent.speed = 0;
        bossanimator.SetBool("Dodgeback", true);
        bossanimator.SetBool("Iswalking", false);
        yield return new WaitForSeconds(0.5f);
        bossAgent.speed = 0;
        Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
        lookDirection.y = 0f;
        lookDirection.Normalize();
        Quaternion awayRotation = Quaternion.LookRotation(lookDirection);
        bossObject.transform.rotation = Quaternion.Euler(bossObject.transform.rotation.eulerAngles.x, awayRotation.eulerAngles.y, bossObject.transform.rotation.eulerAngles.z);
        bossRigidBody.AddForce(-lookDirection * 6000000f);
        yield return new WaitForSeconds(1f);
        bossanimator.SetBool("Iswalking", true);
        bossanimator.SetBool("Dodgeback", false);
        bossAgent.destination = playerObject.transform.position;
        yield return new WaitForSeconds(1f);
        bossAgent.speed = 5;
        yield return new WaitForSeconds(2f);
        canDash = true;
    }
}
