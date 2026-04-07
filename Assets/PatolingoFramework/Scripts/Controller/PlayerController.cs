using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : ActorController, InputSystem_Actions.IPlayerActions
{
    [SerializeField]
    private Actor startWithActor;

    [Space]

    [Header("InteractionSystem")]
    private InteractionComponent interactionComponent;

    private CameraHandler _cameraHandler;

    private Vector3 moveInput;
    private bool sprintInput;


    #region Unity Methods
    private void Start()
    {
        InputManager.Instance.SubscribeToGameplay(this);

        InitializeCamera();
        InitializeActor();
        InitializeInteractComponent();
    }

    private void Update()
    {
        UpdateLocomotion();
        UpdateCamera();
    }
    #endregion

    #region Initialization
    private void InitializeActor()
    {
        if (startWithActor != null)
            if (startWithActor.gameObject.scene.IsValid())
            {
                PossessActor(startWithActor);
            }
            else
            {
                PossessActor(Instantiate(startWithActor, transform.position, Quaternion.identity));
            }

        controllingActor._bindedCamera = _cameraHandler;
    }
    private void InitializeCamera()
    {
        if (TryGetComponent(out CameraHandler existingCameraHandler))
        {
            _cameraHandler = existingCameraHandler;
        }
        else
            _cameraHandler = CameraHandler.CreateOrFindCameraHandler();

        _cameraHandler.viewOffset = new Vector3(0f, .5f, 0f);

        _cameraHandler.bRotateParentYAxis = true;

        _cameraHandler.isHeadbobTickEnabled += () => (controllingActor.IsMoving() && controllingActor.IsGrounded());
        _cameraHandler.headbobIntervalMultiplier += () => controllingActor.IsSprinting() ? 2f : 1f;
    }
    private void InitializeInteractComponent()
    {
        interactionComponent = controllingActor.GetComponent<InteractionComponent>();

        if (interactionComponent != null)
        {
            interactionComponent.whereToGetOrientation += () =>
            {
                return _cameraHandler.controllingCamera.transform.forward;
            };
            interactionComponent.whereToGetOrigin += () =>
            {
                return _cameraHandler.transform.position;
            };
        }
    }
    #endregion

    #region ActorController Overrides
    public override void PossessActor(Actor actor)
    {
        base.PossessActor(actor);

        if (_cameraHandler != null)
        {
            Vector3 actorForward = controllingActor != null ? controllingActor.transform.forward : Vector3.forward;

            _cameraHandler.SetPitchAndYaw(Quaternion.LookRotation(actorForward, Vector3.up));
            _cameraHandler.followTransform = controllingActor?.transform;
        }
    }
    public override void Dispose()
    {
        if (_disposed) return;

        base.Dispose();

        if(InputManager.InstanceExists)
            InputManager.Instance?.UnsubscribeFromGameplay(this);
    }
    #endregion

    private void UpdateLocomotion()
    {
        controllingActor?.LocomotionModule?.SetMovementInput(new LocomotionInput(moveInput, sprintInput));
    }

    private void UpdateCamera()
    {
        controllingActor?.LocomotionModule?.SetOrientation(new OrientationContext(controllingActor.transform));
    }


    


    #region InputSystem_Actions.IPlayerActions

    public void OnAttack(InputAction.CallbackContext context)
    {
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
            interactionComponent.TriggerInteracting();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        float pitch = context.ReadValue<Vector2>().y;
        float yaw = context.ReadValue<Vector2>().x;

        _cameraHandler.AddPitchInput(-pitch);
        _cameraHandler.AddYawInput(yaw);

        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputDir = context.ReadValue<Vector2>();
        moveInput = new Vector3(inputDir.x, 0f, inputDir.y);
    }

    public void OnNext(InputAction.CallbackContext context)
    {
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprintInput = context.ReadValueAsButton();
    }

    #endregion
}
