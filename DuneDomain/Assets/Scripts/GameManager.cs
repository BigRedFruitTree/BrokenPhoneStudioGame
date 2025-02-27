using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Stuff")]
    public PlayerController playerControler;
    
    public float Time = 0f;
    public bool GameOn = false;
    public bool GameOver = false;
    [SerializeField] public int weapon = 0;
    public GameObject weaponScreen;

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
            playerControler.sword.SetActive(true);
            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
        }
        if(chosenWeapon == 2)
        {
            playerControler.bow.SetActive(true);
            weaponScreen.SetActive(false);
            weapon = chosenWeapon;
            GameOn = true;
        }
    }
}
