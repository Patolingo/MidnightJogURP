using System;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueField dialogueField;
    [SerializeField] private AnswerField answerField;

    public Action<int> OnChoiceSelected;

    public void SetDisplayVisible(bool visible)
    {
        dialogueField.SetVisible(visible);
    }

    public void DisplayLine(string name, string line)
    {
        dialogueField.SetField(name, line);
        Debug.Log(line);
    }


    public void DisplayAnswers(string[] answerLines)
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
