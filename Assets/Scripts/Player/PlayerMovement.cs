using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

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
    [SerializeField] private InputActionReference lookinput, jumpinput, sprintinput, crouchinput, zoominput;
    public Vector3 moveinput;
    private Vector3 airspeed;

    private bool wasFalling = false;

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
        IsGrounded = Physics.CheckSphere(Groundcheck.position, 0.6f, groundmask);

        if (IsGrounded && wasFalling == true && Velocity.y > -0.1f)
        {
            wasFalling = false;
            print("Landed");
        }
        else if (!IsGrounded && Velocity.y < 0)
        {
            wasFalling = true;
        }

        if (IsGrounded && Velocity.y > 0)
        {
            Velocity.y = 4f;
        }
        else
        {
            Velocity.y += Gravity * GravityMultiplier * Time.deltaTime;
        }

        //---Movement---
        Vector3 move = new Vector3(moveinput.x * MovementSpeed, 0f, moveinput.z * MovementSpeed).normalized;

        if (IsGrounded)//move here
        {
            float targetangle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + PlayerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref turnsmoothvelocity, turnsmoothtime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 movedir = Quaternion.Euler(0f, targetangle, 0f) * Vector3.forward;

            Character.Move(movedir * MovementSpeed * Time.deltaTime - Velocity * Time.deltaTime);
            airspeed = Character.velocity;
            airspeed.y = 0;
        }
        else//if the character isn't grounded, we limit their air control
        {
            float airControlFactor = 0.4f;
            Vector3 airControl = move * MovementSpeed * airControlFactor;
            airspeed = Vector3.Lerp(airspeed, Vector3.zero, Time.deltaTime * 2);
            Character.Move((airspeed + airControl) * Time.deltaTime - Velocity * Time.deltaTime);
        }

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
                Velocity.y = -JumpPower;
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

    void OnMove(InputValue value)
    {
        moveinput = value.Get<Vector3>();
    }
}