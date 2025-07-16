using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine.Analytics;

public class PlayerMovement : MonoBehaviour
{
    public float minFOV;
    public float maxFOV;
    public float ScrollSensitivity;
    public float FOV;
    
    public GlobalVariables GlobalVariables;

    [Header("Player")]
    public CharacterController Character;
    public Transform Player;
    public float turnsmoothtime = 0.1f;
    private float turnsmoothvelocity;
    public Camera PlayerCamera;
    public float MouseSensitivity;
    private bool IsGrounded;
    private float GroundDistance;

    [Header("Ground")]
    public Transform Groundcheck;
    public LayerMask groundmask;

    [Header("Gravity")]
    [SerializeField] private Vector3 Velocity;
    [SerializeField] private float Gravity = 9.81f;
    [SerializeField] private float GravityMultiplier = 2.3f;
    [SerializeField] private float MovementSpeed = 7f;
    [SerializeField] private float JumpPower;
    [SerializeField] private float WalkingSpeed;
    [SerializeField] private float SprintingSpeed;
    [SerializeField] private float CrouchingSpeed;
    private bool IsSprinting;
    private bool IsCrouching;

    [Header("Controls")]
    [SerializeField] private InputActionReference moveinput, lookinput, jumpinput, sprintinput, crouchinput, zoominput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        //SET STARTING PHYSICS HERE
        JumpPower = 6f;
        CrouchingSpeed = 1.5f;
        WalkingSpeed = 4f;
        SprintingSpeed = 6;
        GravityMultiplier = 1.5f;
        MovementSpeed = WalkingSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        //Gravity Shit
        
        Character.Move(Velocity * Time.deltaTime);
        IsGrounded = Physics.CheckSphere(Groundcheck.position, 0.6f, groundmask);

        if (IsGrounded && Velocity.y > 0f)
        {
            Debug.Log("Landed");
            Velocity.y = -2f;
        }
         Velocity.y -= Gravity * Time.deltaTime;

        //---Movement---
        Vector3 move = moveinput.action.ReadValue<Vector3>().normalized;

        if (move.magnitude>=1f)//move here
        {
            float targetangle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + PlayerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref turnsmoothvelocity, turnsmoothtime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 movedir = Quaternion.Euler(0f, targetangle, 0f) * Vector3.forward;

            Character.Move(movedir * MovementSpeed * Time.deltaTime);
        }
        //camera

        //Sprinting
        if (IsSprinting && !IsCrouching && Character.velocity.magnitude > 0.1f)
        {
            MovementSpeed = SprintingSpeed;
            PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, 70, 0.02f);
        }
        else
        {
            if (!IsCrouching)
            {
                MovementSpeed = WalkingSpeed;
            }
            PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, 60, 0.02f);
        }

        //Crouching
        if (IsCrouching)
        {
            MovementSpeed = CrouchingSpeed;
        }
        else
        {
            if (!IsSprinting)
            {
                MovementSpeed = WalkingSpeed;
            }
        }
        PlayerCamera.enabled = true;
    }

    //Hey!! subscribe to events here v
    private void OnEnable()
    {
        jumpinput.action.performed += JumpShit;

        crouchinput.action.performed += CrouchShit;

        sprintinput.action.started += context => { IsSprinting = true; };
        sprintinput.action.canceled += context => { IsSprinting = false; };
    }

    //Remember to unsubscribe from those events when you destroy the player thingy here vv
    private void OnDisable()
    {
        jumpinput.action.performed -= JumpShit;

        sprintinput.action.started -= context => { IsSprinting = true; };
        sprintinput.action.canceled -= context => { IsSprinting = false; };
    }


    //Jump
    private void JumpShit(InputAction.CallbackContext context)
    {
        {
            if (context.performed)
            {
                IsCrouching = false;
                Velocity.y = JumpPower;
            }
        }
    }

    //Crouch

    private void CrouchShit(InputAction.CallbackContext context)
    {
        if (IsGrounded)
        {
            if (context.performed)
            {
                IsCrouching = !IsCrouching;
                print("Crouching");
                if (IsCrouching)
                {
                    MovementSpeed = CrouchingSpeed;
                }
                else
                {
                    MovementSpeed = WalkingSpeed;
                }
            }
        }
    }
}