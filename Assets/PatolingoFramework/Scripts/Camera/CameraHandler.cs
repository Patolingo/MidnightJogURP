using System;
using System.Collections;
using UnityEngine;

public class CameraHandler : MonoBehaviour, ICamera
{
    [SerializeField] private float _sensitivity = 1f;
    
    [SerializeField] private CursorLockMode _startCursorLockMode = CursorLockMode.Locked;

    [SerializeField]private float _pitchLimit = 90f;

    [Space]


    [Header("Headbob")]
    public bool bHeadbobEnabled = true;
    
    public float headbobInterval = 1f;
    public float headbobAmplitude = 1f;

    private float headbobTimer;

    private Vector3 _headbobPos;

    public Func<bool> isHeadbobTickEnabled;
    public Func<float> headbobIntervalMultiplier;

    [Space]
    
    [HideInInspector]public bool bRotateParentYAxis = false;

    public Transform followTransform;
    public Vector3 viewOffset;


    //Camera
    private Camera m_Camera;
    public Camera controllingCamera => m_Camera;


    //Input

    public bool bInputEnabled = true;

    private float _pitch;
    private float _yaw;


    public bool IsBoundToActor => followTransform != null;

    private void Awake()
    {
        InitializeCamera();
    }
 
    private void Update()
    {
        HandleHeadbob();

        SetCameraPosition();
    }
    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        m_Camera.transform.rotation = targetRotation;

        if (bRotateParentYAxis && followTransform != null)
        {
            followTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }
    }

    private void InitializeCamera()
    {
        Camera mainCamera = Camera.main;

        if (GetComponentInChildren<Camera>() != null)
        {
            m_Camera = GetComponentInChildren<Camera>();
        }
        else if (mainCamera == null || mainCamera.transform.parent != null)
        {
            GameObject cameraGameObject = new GameObject("Camera");
            m_Camera = cameraGameObject.AddComponent<Camera>();
        }
        else
        {
            m_Camera = mainCamera;
        }

        m_Camera.transform.SetParent(transform);
        m_Camera.transform.localPosition = Vector3.zero;
        Cursor.lockState = _startCursorLockMode;
    }

    public void AddPitchInput(float pitch) // Y Axis
    {
        if (!bInputEnabled) return;

        _pitch += pitch * _sensitivity;
        _pitch = Mathf.Clamp(_pitch, -_pitchLimit, _pitchLimit);
    }
    public void AddYawInput(float yaw) // X Axis
    {
        if (!bInputEnabled) return;
        _yaw += yaw * _sensitivity;
    }

    public void SetPitchAndYaw(Quaternion newRotation)
    {
        Vector3 eulerRotation = newRotation.eulerAngles;
        _pitch = eulerRotation.x;
        _yaw = eulerRotation.y;
    }

    public void LookAtObject(GameObject go, float lookAtSpeed, Action onView)
    {
        Vector3 origin = m_Camera.transform.position;
        Vector3 direction = (go.transform.position - origin).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 euler = lookRotation.eulerAngles;

        float targetPitch = NormalizeAngle(euler.x);
        float targetYaw = NormalizeAngle(euler.y);


        targetPitch = Mathf.Clamp(targetPitch, -_pitchLimit, _pitchLimit);
        
        StartCoroutine(ELookAtObject(targetPitch, targetYaw, lookAtSpeed, onView));
    }

    private IEnumerator ELookAtObject(float targetPitch, float targetYaw, float lookAtSpeed, Action onView)
    {
        while (Mathf.Abs(Mathf.DeltaAngle(_pitch, targetPitch)) > 0.1f ||
               Mathf.Abs(Mathf.DeltaAngle(_yaw, targetYaw)) > 0.1f)
        {
            _pitch = Mathf.LerpAngle(_pitch, targetPitch, lookAtSpeed * Time.deltaTime);
            _yaw = Mathf.LerpAngle(_yaw, targetYaw, lookAtSpeed * Time.deltaTime);

            yield return null;
        }

        _pitch = targetPitch;
        _yaw = targetYaw;

        onView?.Invoke();
    }



    private void SetCameraPosition()
    {
        Vector3 position = (followTransform != null) ? followTransform.position : Vector3.zero;

        transform.position = position + viewOffset + _headbobPos;
    }
    private void HandleHeadbob()
    {
        if (bHeadbobEnabled == false)
        {
            _headbobPos = Vector3.Lerp(_headbobPos, Vector3.zero, .2f);
            return;
        }

        if (isHeadbobTickEnabled != null && isHeadbobTickEnabled())
        {
            headbobTimer += Time.deltaTime * headbobInterval * (headbobIntervalMultiplier != null ? headbobIntervalMultiplier() : 1f);
        }
        else
        {
            headbobTimer = Mathf.Lerp(headbobTimer, 0f, .2f);
        }

        float yOffset = Mathf.Cos(headbobTimer) * headbobAmplitude;

        _headbobPos = new Vector3(0f, yOffset, 0f);
    }


    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    public void ChangeCursorLockState(CursorLockMode newLockMode)
    {
        Cursor.lockState = newLockMode;
    }


    public static CameraHandler CreateOrFindCameraHandler()
    {
        CameraHandler existingHandler = FindAnyObjectByType<CameraHandler>();
        if (existingHandler != null && existingHandler.IsBoundToActor == false)
        {
            return existingHandler;
        }

        GameObject handlerGameObject = new GameObject("CameraHandler");
        return handlerGameObject.AddComponent<CameraHandler>();
    }
}
