using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    public GameObject arrow;
    public PlayerController playerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Shield" && playerScript.isBlocking == true)
        {
            Destroy(arrow);
        }

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(arrow);
        }

        if (collision.gameObject.tag == "RangedEnemy")
        {
            Destroy(arrow);
        }

        if (collision.gameObject.tag == "MeleeEnemy")
        {
            Destroy(arrow);
        }

        if (collision.gameObject.tag == "Environment")
        {
            Destroy(arrow);
        }

        if (collision.gameObject.name == "Boss")
        {
            Destroy(arrow);
        }
    }
}
