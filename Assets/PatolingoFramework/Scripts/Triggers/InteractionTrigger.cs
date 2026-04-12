using UnityEngine;

public class InteractionTrigger : BaseTrigger, IInteractable
{
    public void TriggerInteraction()
    {
        Invoke();
    }

    public string GetInteractionName()
    {
        return "Interact";
    }
}
