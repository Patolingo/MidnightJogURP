using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ZLinq;

public class Actor : BlockableMonoBehaviour
{
    [Header("Locomotion")]
    [SerializeField] protected LocomotionModule locomotionModule;
    public LocomotionModule LocomotionModule => locomotionModule;

    [Header("Tick-Handling")]
    public bool TickEnabled = true;

    private HashSet<ITick> _tickModules = new HashSet<ITick>();
    private HashSet<IFixedTick> _fixedTickModules = new HashSet<IFixedTick>();
    private HashSet<ILateTick> _lateTickModules = new HashSet<ILateTick>();

    private ActorController _controller;

    public CameraHandler _bindedCamera;

    private void Awake()
    {
        MonoBehaviour[] allMonoBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
        
        for(int i = 0;  i < allMonoBehaviours.Length; i++)
        {
            MonoBehaviour mono = allMonoBehaviours[i];
            if (mono is ITick tickModule)
                BindTick(tickModule);
            if(mono is IFixedTick fixedTickModule)
                BindFixedTick(fixedTickModule);
            if(mono is ILateTick lateTickModule)
                BindLateTick(lateTickModule);   
        }
    }
    private void Start()
    {
        if(locomotionModule == null)
        {
            locomotionModule = GetComponent<LocomotionModule>();
        }
    }
    private void Update()
    {
        if(TickEnabled)
            Tick(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if(TickEnabled)
            FixedTick(Time.fixedDeltaTime);
    }
    private void LateUpdate()
    {
        if(TickEnabled)
            LateTick(Time.deltaTime);
    }

    public void Possess(ActorController controller)
    {
        if(_controller != null)
        {
            if(controller == null) return;

            Debug.LogError($"Actor {name} is already possessed by {_controller.name}. Cannot be possessed by {controller.name}.");
            return;
        }

        _controller = controller;
    }

    public void Tick(float deltaTime)
    {
        if(!TickEnabled) return;

        foreach (ITick tickModule in _tickModules)
        {
            tickModule.Tick(deltaTime);
        }
    }
    public void FixedTick(float fixedDeltaTime)
    {
        if(!TickEnabled) return;
        foreach (IFixedTick fixedTickModule in _fixedTickModules)
        {
            fixedTickModule.FixedTick(fixedDeltaTime);
        }
    }
    public void LateTick(float deltaTime)
    {
        if(!TickEnabled) return;
        foreach (ILateTick lateTickModule in _lateTickModules)
        {
            lateTickModule.LateTick(deltaTime);
        }
    }


    #region Bind ITickers

    private void BindTick(ITick tick)
    {
        if(!_tickModules.Contains(tick))
        {
            _tickModules.Add(tick);
        }
    }
    private void UnbindTick(ITick tick)
    {
        if(_tickModules.Contains(tick))
        {
            _tickModules.Remove(tick);
        }
    }
    
    private void BindFixedTick(IFixedTick fixedTick)
    {
        if(!_fixedTickModules.Contains(fixedTick))
        {
            _fixedTickModules.Add(fixedTick);
        }
    }
    private void UnbindFixedTick(IFixedTick fixedTick)
    {
        if(_fixedTickModules.Contains(fixedTick))
        {
            _fixedTickModules.Remove(fixedTick);
        }
    }

    private void BindLateTick(ILateTick lateTick)
    {
        if(!_lateTickModules.Contains(lateTick))
        {
            _lateTickModules.Add(lateTick);
        }
    }
    private void UnbindLateTick(ILateTick lateTick)
    {
        if(_lateTickModules.Contains(lateTick))
        {
            _lateTickModules.Remove(lateTick);
        }
    }

    #endregion

    #region Utilities

    public virtual void LookAtObject(GameObject go)
    {
        LookAtObject(go, 3f);
    }
    public virtual void LookAtObject(GameObject go, float lookSpeed)
    {
        if (_bindedCamera == null) return;

        StartCoroutine(ELookAtObject(go, lookSpeed));
    }

    public virtual bool IsMoving()
    {
        if(locomotionModule != null)
        {
            return locomotionModule.IsMoving();
        }

        return false;
    }
    public virtual bool IsGrounded()
    {
        if(locomotionModule != null) return locomotionModule.IsGrounded();

        return true;
    }
    public virtual bool IsSprinting()
    {
        if (locomotionModule != null) return locomotionModule.IsSprinting();

        return false;
    }


    private IEnumerator ELookAtObject(GameObject go, float lookSpeed)
    {
        bool lookingToTarget = false;

        _bindedCamera.bInputEnabled = false;
        locomotionModule.bInputEnabled = false;

        _bindedCamera.LookAtObject(go, lookSpeed, () => lookingToTarget = true);

        while (lookingToTarget == false)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        _bindedCamera.bInputEnabled = true;
        locomotionModule.bInputEnabled = true;
    }
    #endregion
}
