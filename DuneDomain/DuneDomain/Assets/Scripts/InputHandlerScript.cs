using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    [Header("Input Action Stuff")]
    [SerializeField] private InputActionAsset playerCtrls;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string move = "Move";
   

    [SerializeField] public InputAction moveAction;

    public Vector2 MoveInput {get; private set;}

    public static InputHandler Instance {get; private set;}


    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
        
       moveAction = playerCtrls.FindActionMap(actionMapName).FindAction(move);
       RegisterInputActions();
    }

    public void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;
    }

    public void OnEnable()
    {
        moveAction.Enable();
    }

    public void OnDisable()
    {
        moveAction.Disable();
    }
}