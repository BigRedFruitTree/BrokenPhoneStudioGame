using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController playerController;
    public GameManager gm;

    [Header("Input Action Stuff")]
    [SerializeField] private InputActionAsset playerCtrls;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string move = "Move";
    [SerializeField] private string primarySword = "Sword";
    [SerializeField] private string primaryHammer = "PrimaryHammer";
    [SerializeField] private string secondaryHammer = "SecondaryHammer";
    [SerializeField] private string primarySpAnSh = "PrimarySpAnSh";
    [SerializeField] private string secondarySpAnSh = "SecondarySpAnSh";
    [SerializeField] private string primaryBow = "Bow";
    [SerializeField] private string primaryCrBo = "CrossBow";
    [SerializeField] private string roll = "Roll";

    [SerializeField] public InputAction moveAction;
    [SerializeField] public InputAction primaryActionSword;
    [SerializeField] public InputAction primaryActionHammer;
    [SerializeField] public InputAction secondaryActionHammer;
    [SerializeField] public InputAction primaryActionSpAnSh;
    [SerializeField] public InputAction secondaryActionSpAnSh;
    [SerializeField] public InputAction primaryActionBow;
    [SerializeField] public InputAction primaryActionCrossB;
    [SerializeField] public InputAction rollAction;
 
    public Vector2 MoveInput {get; private set;}
    public bool PrimaryInputSword { get; private set; }
    public bool PrimaryInputHammer { get; private set; }
    public bool SecondaryInputHammer { get; private set; }
    public bool PrimaryInputSpAnSh { get; private set; }
    public bool SecondaryInputSpAnSh { get; private set; }
    public bool PrimaryInputBow { get; private set; }
    public bool PrimaryInputCrossB { get; private set; }
    public bool RollInput { get; private set; }


    public static InputHandler Instance {get; private set;}


    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        moveAction = playerCtrls.FindActionMap(actionMapName).FindAction(move);
        primaryActionSword = playerCtrls.FindActionMap(actionMapName).FindAction(primarySword);
        primaryActionHammer = playerCtrls.FindActionMap(actionMapName).FindAction(primaryHammer);
        secondaryActionHammer = playerCtrls.FindActionMap(actionMapName).FindAction(secondaryHammer);
        primaryActionSpAnSh = playerCtrls.FindActionMap(actionMapName).FindAction(primarySpAnSh);
        secondaryActionSpAnSh = playerCtrls.FindActionMap(actionMapName).FindAction(secondarySpAnSh);
        primaryActionBow = playerCtrls.FindActionMap(actionMapName).FindAction(primaryBow);
        primaryActionCrossB = playerCtrls.FindActionMap(actionMapName).FindAction(primaryCrBo);
        rollAction = playerCtrls.FindActionMap(actionMapName).FindAction(roll);
        RegisterInputActions();
    }
    public void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        primaryActionSword.performed += context => PrimaryInputSword = true;
        primaryActionSword.canceled += context => PrimaryInputSword = false;

        /*primaryActionH.performed += context => PrimaryInputH = true;
        primaryActionH.canceled += context => PrimaryInputH = false;

        secondaryAction.performed += context => SecondaryInput = true;
        secondaryAction.canceled += context => SecondaryInput = false;

        rollAction.performed += context => RollInput = true;
        rollAction.canceled += context => RollInput = true;*/
    }

    public void OnEnable()
    {
        moveAction.Enable();
        primaryActionSword.Enable();
        /*primaryActionH.Enable();
        secondaryAction.Enable();
        rollAction.Enable();*/
    }

    public void OnDisable()
    {
        moveAction.Disable();
        primaryActionSword.Disable();
        /*primaryActionH.Disable();
        secondaryAction.Disable();
        rollAction.Disable();*/
    }
}