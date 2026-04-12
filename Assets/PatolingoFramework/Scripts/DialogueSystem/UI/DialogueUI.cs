using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueField dialogueField;
    [SerializeField] private AnswerField answerField;
    

    public void SetDisplayVisible(bool visible)
    {
        dialogueField.SetVisible(visible);
    }

    public void DisplayLine(ConversationLine line)
    {
        string nameStr = line.whoIsTalking.GetLocalizedString();
        string contentStr = line.content.GetLocalizedString();
        
        dialogueField.SetField(nameStr, contentStr);
    }


    public void DisplayAnswers(QuestionLine[] answerLines)
    {
        answerField.SetAnswers(answerLines);
    }
    public void ClearAnswers()
    {
        answerField.ClearAnswers();
    }


    public void ClearLine()
    {
        dialogueField.SetField("", "");
    }

    public bool TryToSkip()
    {
        return dialogueField.TryToSkip();
    }

    public bool IsDialogueFullyDisplayed()
    {
        return dialogueField.IsDialogueFullyDisplayed();
    }
}
