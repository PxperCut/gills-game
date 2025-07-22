using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Camera Settings")]
    
    public float minFOV=10f;
    public float maxFOV=1f;

    private float CurrentCamScale = 10f;
    private float CurrentFOV = 40f;

    private float FOV
    {
        get
        {
            return CinemachineCamera.Lens.FieldOfView;
        }

        set
        {
            CinemachineCamera.Lens.FieldOfView = value;
        }
    }

    //PLAYER
    [Header("Player")]
    public CharacterController Character;
    public Transform Player;
    private float turnsmoothtime = 0.1f;
    private float turnsmoothvelocity;
    public CinemachineOrbitalFollow OrbitalFollow;
    public CinemachineCamera CinemachineCamera;
    public Camera PlayerCamera;
    private bool IsGrounded;

    [Header("Ground")]
    public Transform Groundcheck;
    public LayerMask groundmask;

    //MOVEMENT

    [Header("Movement")]
    [SerializeField] private Vector3 Velocity;
    [SerializeField] private float Gravity = -9.81f;
    [SerializeField] private float GravityMultiplier = 2.3f;
    private float MovementSpeed = 7f;
    [SerializeField] private float JumpPower;
    [SerializeField] private float WalkingSpeed;
    [SerializeField] private float SprintingSpeed;
    [SerializeField] private float CrouchingSpeed;
    private bool IsSprinting;
    private bool IsCrouching;

    //INPUTS

    [Header("Controls")]
    [SerializeField] private InputActionReference moveinput;
    [SerializeField] private InputActionReference jumpinput;
    [SerializeField] private InputActionReference sprintinput;
    [SerializeField] private InputActionReference crouchinput;
    [SerializeField] private InputActionReference zoomoutinput;
    [SerializeField] private InputActionReference zoomininput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //start
        Cursor.lockState = CursorLockMode.Locked;

        //Debug.Log("Graphics Level: " + QualitySettings.GetQualityLevel());
        //Debug.Log("Shadow Resolution: " + QualitySettings.shadowResolution);
        //Debug.Log("AntiAliasing: " + QualitySettings.antiAliasing);
        //Debug.Log("VSync: " + QualitySettings.vSyncCount);
    }
    // Update is called once per frame
    void Update()
    {
        //Gravity Shit
        
        Velocity.y += Gravity*GravityMultiplier * Time.deltaTime;

        Character.Move(Velocity * Time.deltaTime);
        IsGrounded = Physics.CheckSphere(Groundcheck.position, 0.4f, groundmask);

        if (IsGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }

        Vector3 move = moveinput.action.ReadValue<Vector3>();
        
        if (move.magnitude > 1f)
            move = move.normalized;

        if (move.magnitude >= 0.1f)
        {
            float targetangle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + PlayerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetangle, ref turnsmoothvelocity, turnsmoothtime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 movedir = Quaternion.Euler(0f, targetangle, 0f) * Vector3.forward;

            Character.Move(movedir * move.magnitude * MovementSpeed * Time.deltaTime);
        }

        //camera
        if (zoomininput.action.IsPressed())
        {
            CurrentCamScale += .1f;
        }
        if (zoomoutinput.action.IsPressed())
        {
            CurrentCamScale -= .1f;
        }

        CurrentCamScale-=Input.GetAxis("Mouse ScrollWheel")*4;

        CurrentCamScale = Mathf.Clamp(CurrentCamScale, minFOV, maxFOV);
        OrbitalFollow.Radius=Mathf.Lerp(OrbitalFollow.Radius, CurrentCamScale, 0.1f);

        //Sprinting
        if (IsSprinting && !IsCrouching && Character.velocity.magnitude > 0.1f)
        {
            MovementSpeed = SprintingSpeed;
            FOV = Mathf.Lerp(FOV, CurrentFOV + 10f, 0.02f);
        }
        else
        {
            if (!IsCrouching)
            {
                MovementSpeed = WalkingSpeed;
            }
            FOV = Mathf.Lerp(FOV, CurrentFOV, 0.02f);
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
            if (context.performed&&IsGrounded)
            {
                IsCrouching = false;
                Velocity.y = Mathf.Sqrt(JumpPower * -2 * Gravity);
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