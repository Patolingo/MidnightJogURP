using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI contentText;

    private CanvasGroup canvasGroup;

    private TMP_TypewriterEffect contentTypewriterEffect;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if(contentText != null && contentText.TryGetComponent(out TMP_TypewriterEffect typewriterEffect)) contentTypewriterEffect = typewriterEffect;
    }


    public void SetVisible(bool visible)
    {
        canvasGroup.DOKill(true);
        canvasGroup.DOFade((visible ? 1f : 0f), .5f);
    }


    public void SetField(string name, string content)
    {
        nameText.text = name;
        contentText.text = content;
    }

    public bool TryToSkip()
    {
        if(contentTypewriterEffect != null && contentTypewriterEffect.isTypewriting)
        {
            Debug.Log("Is typewriting");
            contentTypewriterEffect.EndTypewriting();
            return false;
        }

        return true;
    }

    public bool IsDialogueFullyDisplayed()
    {
        if (contentTypewriterEffect != null && contentTypewriterEffect.isTypewriting)
        {
            return false;
        }

        return true;
    }


    public void HideField()
    {
        nameText.text = "";
        contentText.text = "";
    }

}
