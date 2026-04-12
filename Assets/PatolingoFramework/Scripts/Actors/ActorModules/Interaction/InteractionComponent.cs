using System;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    [SerializeField] private float interactableRange;
    [SerializeField] private LayerMask interactableLayer;

    public event Func<Vector3> whereToGetOrientation;
    public event Func<Vector3> whereToGetOrigin;

    private bool isHittingInteractable;
    private RaycastHit hit;

    [Space]

    [SerializeField] private InteractionUI ui;
    
    public Vector3 Orientation => GetOrientation();
    public Vector3 Origin => GetOrigin();
    
    private Vector3 GetOrientation()
    {
        if(whereToGetOrientation == null)
        {
            return transform.forward;
        }

        return whereToGetOrientation();
    }
    private Vector3 GetOrigin()
    {
        if (whereToGetOrigin == null)
        {
            return transform.position;
        }

        return whereToGetOrigin();
    }

    private void Update()
    {
        isHittingInteractable = Physics.Raycast(Origin, Orientation.normalized, out hit, interactableRange, interactableLayer);

        ui?.SetCanInteract(isHittingInteractable);
    }

    public void TriggerInteracting()
    {
        if(isHittingInteractable)
        {
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();

            if(interactable != null) interactable.TriggerInteraction();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Origin, Origin + Orientation * interactableRange);
    }
}
