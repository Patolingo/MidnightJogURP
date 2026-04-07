using UnityEngine;

public class CollisionTrigger : BaseTrigger
{

    public enum CollisionType
    {
        COLLISION,
        TRIGGER
    }

    [System.Flags]
    public enum ColTriggerType
    {
        ON_ENTER = 1,
        ON_STAY = 2 << 0,
        ON_EXIT = 2 << 1
    }

    public CollisionType collisionType;
    public ColTriggerType triggerType;
    public LayerMask allowedLayers;


    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (collisionType != CollisionType.TRIGGER) return;

        if (triggerType.HasFlag(ColTriggerType.ON_ENTER) == false) return;

        if ((allowedLayers & (1 << other.gameObject.layer)) != 0)
            Invoke();
    }
    private void OnTriggerStay(UnityEngine.Collider other)
    {
        if (collisionType != CollisionType.TRIGGER) return;

        if (triggerType.HasFlag(ColTriggerType.ON_STAY) == false) return;

        if ((allowedLayers & (1 << other.gameObject.layer)) != 0)
            Invoke();
    }
    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (collisionType != CollisionType.TRIGGER) return;

        if (triggerType.HasFlag(ColTriggerType.ON_EXIT) == false) return;

        if ((allowedLayers & (1 << other.gameObject.layer)) != 0)
            Invoke();
    }



    private void OnCollisionEnter(UnityEngine.Collision other)
    {
        if (collisionType != CollisionType.COLLISION) return;

        if (triggerType.HasFlag(ColTriggerType.ON_ENTER) == false) return;

        if ((allowedLayers & (1 << other.gameObject.layer)) != 0)
            Invoke();
    }
    private void OnCollisionStay(UnityEngine.Collision other)
    {
        if (collisionType != CollisionType.COLLISION) return;

        if (triggerType.HasFlag(ColTriggerType.ON_STAY) == false) return;

        if ((allowedLayers & (1 << other.gameObject.layer)) != 0)
            Invoke();
    }
    private void OnCollisionExit(UnityEngine.Collision other)
    {
        if (collisionType != CollisionType.COLLISION) return;

        if (triggerType.HasFlag(ColTriggerType.ON_EXIT) == false) return;

        if ((allowedLayers & (1 << other.gameObject.layer)) != 0)
            Invoke();
    }
}
