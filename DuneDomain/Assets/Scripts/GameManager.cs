using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using Cinemachine.Utility;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Player Stuff")]
    public PlayerController playerController;
    public Animator playerAnimator;
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
    private float sleepDistance;
    private float bossDistance;
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
    public bool areHandsActive = false;
    public bool isHeadActive = false;
    public bool isTailActive = false;

    [Header("Enemy Stuff")]
    public float spawnRange;
    public MeleeEnemyManager enemyScript;
    public GameObject meleeEnemyPrefab;
    public GameObject rangedEnemyPrefab;
    public GameObject[] meleeEnemyNumber;
    public GameObject[] rangedEnemyNumber;
    public GameObject[] enemyCorpseNumber;
    public int enemyMovementPattern = 0;

    [Header("GameManager Stuff")]
    public bool GameOn = false;
    public bool GameOver = false;
    public int weapon = 0;
    public bool started = false;
    public GameObject weaponScreen;
    public GameObject deathScreen;
    public GameObject NotWaitingAlertScreen;
    public GameObject winScreen;
    public GameObject TutorialStuff;
    public GameObject TutorialScreen3;
    public GameObject TutorialScreen4;
    public GameObject TutorialScreen5;
    public GameObject bridge;
    public bool canSpawnRocks = true;
    public GameObject Rock1Prefab;
    public AudioSource audioSource;
    public AudioClip bossThemeIntro;
    public AudioClip bossThemeLoop;
    public AudioClip cultistTheme;
    public AudioClip endSong;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            bossObject.SetActive(false);
            startCycle = false;
            TutorialStuff.SetActive(true);
            TutorialScreen3.SetActive(true);
            timeUntilAppearance = 6000f;
            timeUntilEatPhase = 2000f;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
            rounds = 1;
            spawnRange = 50f;

            if (enemyCorpseNumber == null || enemyCorpseNumber.Length == 0) return;

            SetNextTarget();
            bossanimator.SetBool("Isaggressive", false);
            bossAgent.speed = 3;

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
                Time.timeScale = 0;
                GameOn = false;
            }

            if (started == true && canSpawnRocks == true)
            {
                SpawnRock1atRandomPosition(5);
                canSpawnRocks = false;
            }

            meleeEnemyNumber = GameObject.FindGameObjectsWithTag("MeleeEnemy");
            enemyCorpseNumber = GameObject.FindGameObjectsWithTag("EnemyCorpse");
            rangedEnemyNumber = GameObject.FindGameObjectsWithTag("RangedEnemy");

            while (meleeEnemyNumber.Length > 9)
            {
                Destroy(GameObject.FindGameObjectWithTag("MeleeEnemy"));
            }
            while (rangedEnemyNumber.Length > 9)
            {
                Destroy(GameObject.FindGameObjectWithTag("RangedEnemy"));
            }

            if (enemyCorpseNumber.Length >= meleeEnemyNumber.Length)
            {
                enemyMovementPattern = 2;
            }
            else
            {
                enemyMovementPattern = 1;
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
                    if (bridgeDistance <= 29)
                    {
                        audioSource.clip = bossThemeIntro;
                        audioSource.Play();
                        StartCoroutine("WaitStart");
                    }
                }
            }
            else if (timeUntilAppearance <= 0f && canRun == false)
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

                if (timeUntilAttack > 0f && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true )
                {
                    StartCoroutine("Wait");
                    timeUntilAttack--;
                }

                if (timeUntilAttack <= 0f && bossAttack == 0 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true && timeUntilEatPhase > 0f)
                {
                    timeUntilAttack = 0f;
                    bossAttack = UnityEngine.Random.Range(4, 0);
                    Debug.Log("Hes doing number " + bossAttack);
                }

                if (bossAttack == 1 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true && timeUntilEatPhase > 0f)
                {
                    bossanimator.SetInteger("whichAttack", 1);
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack1");
                }
                if (bossAttack == 2 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true && timeUntilEatPhase > 0f)
                {
                    bossanimator.SetInteger("whichAttack", 2);
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack2");
                }
                if (bossAttack == 3 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true && timeUntilEatPhase > 0f)
                {
                    bossanimator.SetInteger("whichAttack", 3);
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack3");
                }
                if (bossAttack == 4 && bossDistance <= 30f && bossanimator.GetBool("Dodgeback") == false && canAttack == true && timeUntilEatPhase > 0f)
                {
                    bossanimator.SetInteger("whichAttack", 4);
                    bossAgent.ResetPath();
                    bossanimator.SetBool("attacking", true);
                    StartCoroutine("WaitAttack4");
                }

                timeUntilAppearance = 0f;
                if (bossDistance <= 16f && bossanimator.GetBool("Dodgeback") == false && canDash == true && bossanimator.GetBool("attacking") == false && timeUntilEatPhase > 0f && bossAttack <= 0)
                {
                    bossAgent.ResetPath();
                    bossAgent.speed = 0;
                    StartCoroutine("WaitForWalking");
                }
                if (bossDistance > 16f && bossanimator.GetBool("Dodgeback") == false && timeUntilEatPhase > 0f)
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
                        bossanimator.SetBool("Isaggressive", true);
                        bossanimator.SetBool("Iswalking", false);
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

            if (rounds >= 2 && bossEating == false && timeUntilEatPhase <= 0 && timeUntilAppearance <= 0)
            {
                audioSource.clip = bossThemeLoop;
                audioSource.Play();
            }
        }
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
        float spawnPosX = UnityEngine.Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
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
    public Vector3 RandomRockPosition()
    {
        float spawnPosX = UnityEngine.Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = UnityEngine.Random.Range(-spawnRange, spawnRange);
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
            playerController.sword.SetActive(true);
            playerController.bow.SetActive(false);
            playerController.crossbow.SetActive(false);
            playerController.spear.SetActive(false);
            playerController.shield.SetActive(false);
            playerController.hammer.SetActive(false);
            playerAnimator.SetInteger("weapon", 1);
            playerController.stringCooldown = 2f;
            audioSource.clip = cultistTheme;
            audioSource.Play();
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 2)
        {
            if (rounds > 1)
                startCycle = true;

            playerController.drawSpeed = 100f;
            playerAnimator.SetInteger("weapon", 2);
            playerController.bow.SetActive(true);
            playerController.sword.SetActive(false);
            playerController.crossbow.SetActive(false);
            playerController.spear.SetActive(false);
            playerController.shield.SetActive(false);
            playerController.hammer.SetActive(false);
            weaponScreen.SetActive(false);
            audioSource.clip = cultistTheme;
            audioSource.Play();
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 3)
        {
            if (rounds >= 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            playerAnimator.SetInteger("weapon", 3);
            playerController.stringCooldown = 2.7f;
            playerController.hammer.SetActive(true);
            playerController.bow.SetActive(false);
            playerController.sword.SetActive(false);
            playerController.crossbow.SetActive(false);
            playerController.spear.SetActive(false);
            playerController.shield.SetActive(false);
            playerController.drawSpeed = 200f;
            audioSource.clip = cultistTheme;
            audioSource.Play();
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 4)
        {
            if (rounds >= 1)
                startCycle = true;

            weaponScreen.SetActive(false);
            playerAnimator.SetInteger("weapon", 4);
            playerController.stringCooldown = 1.2f;
            playerController.spear.SetActive(true);
            playerController.shield.SetActive(true);
            playerController.bow.SetActive(false);
            playerController.sword.SetActive(false);
            playerController.crossbow.SetActive(false);
            playerController.hammer.SetActive(false);
            audioSource.clip = cultistTheme;
            audioSource.Play();
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if (chosenWeapon == 5)
        {
            if (rounds >= 1)
                startCycle = true;

            playerController.drawSpeed = 200f;
            playerAnimator.SetInteger("weapon", 5);
            playerController.crossbow.SetActive(true); 
            playerController.bow.SetActive(false);
            playerController.sword.SetActive(false);
            playerController.spear.SetActive(false);
            playerController.shield.SetActive(false);
            playerController.hammer.SetActive(false);
            weaponScreen.SetActive(false);
            audioSource.clip = cultistTheme;
            audioSource.Play();
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        playerAnimator.SetBool("gm", true);
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
        bossUiStuff.SetActive(true);
        canRun = true;
        bossanimator.SetBool("Iswalking", true);
    }
    IEnumerator WaitAttack1()
    {
        bossanimator.SetInteger("whichAttack", 1);
        areHandsActive = true;
        canAttack = false;
        int tempI = UnityEngine.Random.Range(1, 4);
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
        bossAgent.destination = playerObject.transform.position;
        yield return new WaitForSeconds(2f);
        bossanimator.SetBool("attacking", false);
        if (transitionAttack == true)
        {
            bossAgent.speed = 0;
            bossAttack = 1;
            bossAgent.destination = playerObject.transform.position;
            areHandsActive = false;
            isTailActive = true;
            bossanimator.SetBool("attacking", true);
            bossanimator.SetBool("transitionAttack", true);
            yield return new WaitForSeconds(2.5f);
            bossAgent.speed = 5;
            isTailActive = false;
            bossAttack = 0;
            timeUntilAttack = UnityEngine.Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            bossanimator.SetInteger("whichAttack", 0);
            yield return new WaitForSeconds(2f);
            canAttack = true;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
        }
        else
        {
            areHandsActive = false;
            bossAgent.speed = 5;
            bossAttack = 0;
            timeUntilAttack = UnityEngine.Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            bossanimator.SetInteger("whichAttack", 0);
            yield return new WaitForSeconds(2f);
            canAttack = true;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
        }
    }
    IEnumerator WaitAttack2()
    {
        bossanimator.SetInteger("whichAttack", 2);
        canAttack = false;
        int tempI = UnityEngine.Random.Range(4, 0);
        if (tempI == 2)
        {
            Debug.Log("Its " + tempI);
            transitionAttack = true;
        }
        else
        {
            Debug.Log("Its " + tempI);
            transitionAttack = false;
        }
        bossAgent.ResetPath();
        bossAgent.destination = playerObject.transform.position;
        bossAgent.speed = 6;
        isHeadActive = true;
        yield return new WaitForSeconds(2f);
        bossanimator.SetBool("attacking", false);
        if (transitionAttack == true)
        {
            bossAgent.speed = 0;
            bossAttack = 2;
            bossanimator.SetBool("attacking", true);
            bossanimator.SetBool("transitionAttack", true);
            areHandsActive = true;
            isHeadActive = false;
            yield return new WaitForSeconds(2.5f);
            bossAgent.speed = 5;
            bossAttack = 0;
            timeUntilAttack = UnityEngine.Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            bossanimator.SetInteger("whichAttack", 0);
            areHandsActive = false;
            yield return new WaitForSeconds(2f);
            canAttack = true;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
        }
        else
        {
            bossAgent.speed = 5;
            bossAttack = 0;
            timeUntilAttack = UnityEngine.Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            areHandsActive = false;
            bossanimator.SetInteger("whichAttack", 0);
            yield return new WaitForSeconds(2f);
            canAttack = true;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
        }
    }
    IEnumerator WaitAttack3()
    {
        bossanimator.SetInteger("whichAttack", 3);
        canAttack = false;
        int tempI = UnityEngine.Random.Range(4, 1);
        if (tempI == 2)
        {
            Debug.Log("Its " + tempI);
            transitionAttack = true;
        }
        else
        {
            Debug.Log("Its " + tempI);
            transitionAttack = false;
        }
        bossAgent.ResetPath();
        bossAgent.speed = 0;
        bossAgent.destination = playerObject.transform.position;
        isHeadActive = true;
        areHandsActive = true;
        yield return new WaitForSeconds(2f);
        bossanimator.SetBool("attacking", false);
        if (transitionAttack == true)
        {
            bossAgent.speed = 0;
            bossAttack = 3;
            bossAgent.destination = playerObject.transform.position;
            isHeadActive = true;
            areHandsActive = false;
            bossanimator.SetBool("attacking", true);
            bossanimator.SetBool("transitionAttack", true);
            yield return new WaitForSeconds(2.5f);
            bossAgent.speed = 5;
            isHeadActive = false;
            areHandsActive = false;
            bossAttack = 0;
            timeUntilAttack = UnityEngine.Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            bossanimator.SetInteger("whichAttack", 0);
            yield return new WaitForSeconds(2f);
            canAttack = true;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
        }
        else
        {
            bossAgent.speed = 5;
            bossAttack = 0;
            timeUntilAttack = UnityEngine.Random.Range(100f, 150f);
            bossAgent.destination = playerObject.transform.position;
            bossanimator.SetBool("attacking", false);
            bossanimator.SetBool("transitionAttack", false);
            bossanimator.SetInteger("whichAttack", 0);
            yield return new WaitForSeconds(2f);
            canAttack = true;
            timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
        }
    }
    IEnumerator WaitAttack4()
    {
        bossanimator.SetInteger("whichAttack", 4);
        isHeadActive = true;
        areHandsActive = true;
        canAttack = false;
        bossAgent.ResetPath();
        bossAgent.speed = 0;
        bossAgent.destination = playerObject.transform.position;
        yield return new WaitForSeconds(2f); 
        bossAgent.speed = 5;
        bossAttack = 4;
        bossAgent.destination = playerObject.transform.position;
        bossanimator.SetBool("attacking", true);
        yield return new WaitForSeconds(2f);
        bossanimator.SetBool("transitionAttack", true);
        yield return new WaitForSeconds(2.5f);
        bossAgent.speed = 5;
        bossanimator.SetBool("transitionAttack", false);
        bossanimator.SetBool("attacking", false);
        isHeadActive = false;
        areHandsActive= false;
        bossAttack = 0;
        yield return new WaitForSeconds(2.5f);
        canAttack = true;
        timeUntilAttack = UnityEngine.Random.Range(200f, 300f);
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
        bossanimator.SetBool("Isaggressive", true);
        bossanimator.SetBool("Iswalking", false);
        yield return new WaitForSeconds(5f);
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }
        currentTarget = null;
        SetNextTarget();
        bossanimator.SetBool("Isaggressive", true);
        bossanimator.SetBool("Iswalking", true);
        isProcessingTarget = false;
    }

    IEnumerator WaitWeaponScreen()
    {
        startCycle = true;
        yield return new WaitForSeconds(3f);
        playerAnimator.SetBool("gm", false);
        playerAnimator.SetInteger("whichAttack", 0);
        playerAnimator.SetBool("attacking", false);
        playerAnimator.SetBool("IsCharging", false);
        playerAnimator.SetBool("isDashing", false);
        playerAnimator.SetBool("IsDrawing", false);
        playerAnimator.SetBool("isMoving", false);
        GameOn = false;
        weaponScreen.SetActive(true);
        rounds += 1;
    }

    IEnumerator WaitForWalking()
    {
        canDash = false;
        bossAgent.speed = 0;
        yield return new WaitForSeconds(0.5f);
        bossAgent.speed = 0;
        bossanimator.SetBool("Dodgeback", true);
        bossanimator.SetBool("Iswalking", false);
        yield return new WaitForSeconds(0.5f);
        bossAgent.speed = 0;
        bossAgent.destination = playerObject.transform.position;
        Vector3 lookDirection = (playerObject.transform.position - bossObject.transform.position);
        lookDirection.y = 0f;
        lookDirection.Normalize();
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
