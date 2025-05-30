using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController playerController;
    public GameManager gm;
    public bool isDrawing = false;
    public bool blocking = false;
    public bool charging = false;

    [Header("Input Action Stuff")]
    [SerializeField] private InputActionAsset playerCtrls;
    [SerializeField] public InputActionMap currentActionMap;
    [SerializeField] private InputActionMap Player;
    [SerializeField] public InputActionMap UI;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string actionMapNameUI = "UI";
    [SerializeField] private string move = "Move";
    [SerializeField] private string primarySword = "Sword";
    [SerializeField] private string primaryHammer = "HammerBasic";
    [SerializeField] private string secondaryHammer = "HammerCharge";
    [SerializeField] private string secondaryHammerUp = "HammerChargeUp";
    [SerializeField] private string primarySpAnSh = "SpearAnShieldBasic";
    [SerializeField] private string secondarySpAnSh = "SpearAnShieldBlock";
    [SerializeField] private string secondarySpAnShUp = "SpearAnShieldBlockUp";
    [SerializeField] private string primaryBow = "Bow";
    [SerializeField] private string primaryBowUp = "BowUp";
    [SerializeField] private string primaryCrBo = "CrossBow";
    [SerializeField] private string primaryCrBoUp = "CrossBowUp";
    [SerializeField] private string roll = "Roll";
    [SerializeField] private string navigate = "Navigate";

    [SerializeField] public InputAction navigateAction;
    [SerializeField] public InputAction moveAction;
    [SerializeField] public InputAction primaryActionSword;
    [SerializeField] public InputAction primaryActionHammer;
    [SerializeField] public InputAction secondaryActionHammer;
    [SerializeField] public InputAction secondaryActionHammerUp;
    [SerializeField] public InputAction primaryActionSpAnSh;
    [SerializeField] public InputAction secondaryActionSpAnSh;
    [SerializeField] public InputAction secondaryActionSpAnShUp;
    [SerializeField] public InputAction primaryActionBow;
    [SerializeField] public InputAction primaryActionBowUp;
    [SerializeField] public InputAction primaryActionCrossB;
    [SerializeField] public InputAction primaryActionCrossBUp;
    [SerializeField] public InputAction rollAction;

    public Vector2 NavInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool PrimaryInputSword { get; private set; }
    public bool PrimaryInputHammer { get; private set; }
    public bool SecondaryInputHammer { get; private set; }
    public bool SecondaryInputHammerUp { get; private set; }
    public bool PrimaryInputSpAnSh { get; private set; }
    public bool SecondaryInputSpAnSh { get; private set; }
    public bool SecondaryInputSpAnShUp { get; private set; }
    public bool PrimaryInputBow { get; private set; }
    public bool PrimaryInputBowUp { get; private set; }
    public bool PrimaryInputCrossB { get; private set; }
    public bool PrimaryInputCrossBUp { get; private set; }
    public bool RollInput { get; private set; }

    public static InputHandler Instance { get; private set; }

    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Player = playerCtrls.FindActionMap(actionMapName);
        UI = playerCtrls.FindActionMap(actionMapNameUI);
        navigateAction = playerCtrls.FindActionMap(actionMapNameUI).FindAction(navigate);
        moveAction = playerCtrls.FindActionMap(actionMapName).FindAction(move);
        primaryActionSword = playerCtrls.FindActionMap(actionMapName).FindAction(primarySword);
        primaryActionHammer = playerCtrls.FindActionMap(actionMapName).FindAction(primaryHammer);
        secondaryActionHammer = playerCtrls.FindActionMap(actionMapName).FindAction(secondaryHammer);
        secondaryActionHammerUp = playerCtrls.FindActionMap(actionMapName).FindAction(secondaryHammerUp);
        primaryActionSpAnSh = playerCtrls.FindActionMap(actionMapName).FindAction(primarySpAnSh);
        secondaryActionSpAnSh = playerCtrls.FindActionMap(actionMapName).FindAction(secondarySpAnSh);
        secondaryActionSpAnShUp = playerCtrls.FindActionMap(actionMapName).FindAction(secondarySpAnShUp);
        primaryActionBow = playerCtrls.FindActionMap(actionMapName).FindAction(primaryBow);
        primaryActionBowUp = playerCtrls.FindActionMap(actionMapName).FindAction(primaryBowUp);
        primaryActionCrossB = playerCtrls.FindActionMap(actionMapName).FindAction(primaryCrBo);
        primaryActionCrossBUp = playerCtrls.FindActionMap(actionMapName).FindAction(primaryCrBoUp);
        rollAction = playerCtrls.FindActionMap(actionMapName).FindAction(roll);
        RegisterInputActions();
    }
    public void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        navigateAction.performed += context => NavInput = context.ReadValue<Vector2>();
        navigateAction.canceled += context => NavInput = Vector2.zero;

        primaryActionSword.performed += context => PrimaryInputSword = true;
        primaryActionSword.canceled += context => PrimaryInputSword = false;

        primaryActionHammer.performed += context => PrimaryInputHammer = true;
        primaryActionHammer.canceled += context => PrimaryInputHammer = false;

        secondaryActionHammer.performed += context => StartingToCharge();
        secondaryActionHammer.canceled += context => ChargingDone();

        secondaryActionHammerUp.performed += context => SecondaryInputHammerUp = true;
        secondaryActionHammerUp.canceled += context => SecondaryInputHammerUp = false;

        primaryActionSpAnSh.performed += context => PrimaryInputSpAnSh = true;
        primaryActionSpAnSh.canceled += context => PrimaryInputSpAnSh = false;

        secondaryActionSpAnSh.performed += context => StartingToBlock();
        secondaryActionSpAnSh.canceled += context => BlockingDone();

        secondaryActionSpAnShUp.performed += context => SecondaryInputSpAnShUp = true;
        secondaryActionSpAnShUp.canceled += context => SecondaryInputSpAnShUp = false;

        primaryActionBow.performed += context => StartingToDraw();
        primaryActionBow.canceled += context => DrawingDone();

        primaryActionBowUp.performed += context => PrimaryInputBowUp = true;
        primaryActionBowUp.canceled += context => PrimaryInputBowUp = false;

        primaryActionCrossB.performed += context => StartingToDraw();
        primaryActionCrossB.canceled += context => DrawingDone();

        primaryActionCrossBUp.performed += context => PrimaryInputCrossBUp = true;
        primaryActionCrossBUp.canceled += context => PrimaryInputCrossBUp = false;

        rollAction.performed += context => RollInput = true;
        rollAction.canceled += context => RollInput = true;
    }

    public void Update()
    {
        if (isDrawing == true)
        {
            playerController.DrawRangedWeapon();
        }
        else
        {

        }

        if (blocking == true)
        {
            playerController.Blocking();
        }
        else
        {

        }

        if (charging == true)
        {
            playerController.Charging();
        }
        else
        {

        }


    }

    public void OnEnable()
    {
        moveAction.Enable();
        navigateAction.Enable();
        primaryActionSword.Enable();
        primaryActionHammer.Enable();
        secondaryActionHammer.Enable();
        secondaryActionHammerUp.Enable();
        primaryActionSpAnSh.Enable();
        secondaryActionSpAnSh.Enable();
        secondaryActionSpAnShUp.Enable();
        primaryActionBow.Enable();
        primaryActionBowUp.Enable();
        primaryActionCrossB.Enable();
        primaryActionCrossBUp.Enable();
        rollAction.Enable();
    }

    public void OnDisable()
    {
        navigateAction.Disable();
        moveAction.Disable();
        primaryActionSword.Disable();
        primaryActionHammer.Disable();
        secondaryActionHammer.Disable();
        secondaryActionHammerUp.Disable();
        primaryActionSpAnSh.Disable();
        secondaryActionSpAnSh.Disable();
        secondaryActionSpAnShUp.Disable();
        primaryActionBow.Disable();
        primaryActionBowUp.Disable();
        primaryActionCrossB.Disable();
        primaryActionCrossBUp.Disable();
        rollAction.Disable();
    }

    public void StartingToDraw()
    {
        isDrawing = true;
    }

    public void DrawingDone()
    {
        isDrawing = false;
    }

    public void StartingToBlock()
    {
        blocking = true;
    }

    public void BlockingDone()
    {
        blocking = false;
    }

    public void StartingToCharge()
    {
        charging = true;
    }

    public void ChargingDone()
    {
        charging = false;
    }
}