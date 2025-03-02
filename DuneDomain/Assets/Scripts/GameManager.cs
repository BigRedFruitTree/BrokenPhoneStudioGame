using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Stuff")]
    public PlayerController playerControler;
  
    public bool GameOn = false;
    public bool GameOver = false;
    public int weapon = 0;
    public GameObject weaponScreen;
    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ChooseWeapon(int chosenWeapon)
    {
        if(chosenWeapon == 1)
        {
            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
        if(chosenWeapon == 2)
        {
            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
            StartCoroutine("Wait");
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        started = true;
    }
}
