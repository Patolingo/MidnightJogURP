using UnityEngine;
using UnityEngine.Events;

public abstract class BaseTrigger : MonoBehaviour
{
    public bool destroyOnTrigger = false;

    [SerializeField] protected UnityEvent onTrigger;

    public virtual void Invoke()
    {
        onTrigger?.Invoke();

        if(destroyOnTrigger)
            Destroy(gameObject);
    }
}
