using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    GameManager gm;

    GameObject bossSpawn;
    public GameObject bossObject;

    public float health = 50f;
    public float maxHealth = 50f;
    public int damage = 5;
    public float timer = 60f;
    public float timer2 = 60f;
    public int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        round = 0;
        bossObject.transform.position = bossSpawn.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       if(gm.GameOn == true && gm.GameOver == false)
       {
           if(timer > 0f)
           {
              StartCoroutine("Wait");
              timer--;
           }
           if(timer <= 0f)
           {
               timer = 0f;
               round++;
               bossObject.SetActive(true);
               StartCoroutine("Wait");
               timer2--;
           }

           if(timer2 <= 0f)
           {
              bossObject.SetActive(false);   
              timer = 60f + round;
              timer2 = 60f;
           }

       }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
    }
}
