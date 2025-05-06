using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
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
        if (other.gameObject.name == "Shield" && playerScript.isBlocking == true)
        {
            Destroy(arrow);
        }

        if (other.gameObject.tag == "RangedEnemy")
        { 
            Destroy(arrow);
        }

        if (other.gameObject.tag == "MeleeEnemy")
        {
            Destroy(arrow);
        }

        if (other.gameObject.tag == "Environment")
        {
            Destroy(arrow);
        }

        if (other.gameObject.name == "Boss")
        {
            Destroy(arrow);
        }

    }
}
