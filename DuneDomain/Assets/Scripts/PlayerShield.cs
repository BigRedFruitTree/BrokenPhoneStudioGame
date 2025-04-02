using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public PlayerController playerController;
    public GameManager gm;
    public MeleeEnemyManager enemyScript;
    public bool blockedAttackM = false;
    public bool blockedAttackB = false;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyScript = playerController.enemyScriptM;
    }

    public void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.tag == "EnemySword" && playerController.isBlocking == true && gm.GameOn == true && gm.started == true && enemyScript.attacking == true)
        {
            blockedAttackM = true;
        }
        else
        {
            blockedAttackM = false;
        }

        if (other.gameObject.name == "AttackObject" && playerController.isBlocking == true && gm.GameOn == true && gm.started == true && enemyScript.attacking == true)
        {
            blockedAttackB = true;
        }
        else
        {
            blockedAttackB = false;
        }
    }
}
