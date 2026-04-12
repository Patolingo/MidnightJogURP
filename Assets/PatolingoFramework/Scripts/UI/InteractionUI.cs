using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private Image interactableUIHit;
    [SerializeField] private Sprite canInteractSprite;
    [SerializeField] private Sprite canNotInteractSprite;

    private void Start()
    {
        SetCanInteract(false);
    }

    public void SetCanInteract(bool canInteract)
    {
        interactableUIHit.sprite = canInteract ? canInteractSprite : canNotInteractSprite;
    }
}
