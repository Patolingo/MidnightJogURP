using System;
using System.Collections;
using UnityEngine;

public class DialogueManager : StaticInstance<DialogueManager>
{
    [SerializeField] private DialogueUI dialogueUI;

    private bool inputTriggered;

    private Coroutine conversationRoutine;

    public string currentStep;

    public event Action OnConversationEnded;

    private QuestionLine[] currentQuestions;

    private bool showingLine = false;

    private void Start()
    {
        dialogueUI.SetDisplayVisible(false);
    }

    private void LateUpdate()
    {
        inputTriggered = false;
    }

    public void StartConversation(ConversationData conversationData)
    {
        if (conversationRoutine != null)
        {

            StopCoroutine(conversationRoutine);
            
        }

        conversationRoutine = StartCoroutine(EConversation(conversationData));
    }

    private IEnumerator EConversation(ConversationData conversationData)
    {
        if (conversationData == null) yield break;

        if(conversationData.dialogueType == ConversationData.DialogueType.CONVERSATION)
            PlayerController.instance.Block(this);

        int conversationLength = conversationData.Length;
        int currentIndex = 0;

        ConversationLine line = conversationData.GetLine(0);

        bool runningConversation = true;
        showingLine = false;

        bool lineIsQuestion = line.answers.Length > 0;
        bool displayingAnswes = false;

        dialogueUI.ClearLine();
        dialogueUI.ClearAnswers();


        dialogueUI.SetDisplayVisible(true);

        currentStep = "Initializing";

        while (runningConversation)
        {
            inputTriggered = false;
            line = conversationData.GetLine(currentIndex);
            lineIsQuestion = line.answers.Length > 0;
            displayingAnswes = false;

            yield return new WaitForSeconds(line.delayToStart);

            dialogueUI.DisplayLine(line);

            showingLine = true;

            currentIndex++;

            currentStep = "Showing Line";

            yield return new WaitForSeconds(.16f);

            while (showingLine)
            {
                currentStep = "Waiting Input";

                //Skip
                if (inputTriggered)
                {
                    if (dialogueUI.TryToSkip())
                    {
                        if (!lineIsQuestion)
                        {
                            showingLine = false;
                        }
                    }

                    
                    currentStep = "Line Kept";
                    inputTriggered = false;
                }


                //Display Answers / Questions
                if(dialogueUI.IsDialogueFullyDisplayed() && lineIsQuestion && conversationData.dialogueType != ConversationData.DialogueType.CONVERSATION)
                {
                    if (displayingAnswes == false)
                    {
                        currentQuestions = line.answers;

                        PlayerController.instance.ChangeCursorVisible(true);
                        dialogueUI.DisplayAnswers(line.answers);
                        displayingAnswes = true;
                    }
                }
                yield return null;
            }
            currentStep = "Advancing Conversation";

            if (currentIndex >= conversationLength)
            {
                currentStep = "Conversation Ended";
                runningConversation = false;
            }

            yield return new WaitForSeconds(line.delayToEnd);


            yield return null;
        }

        dialogueUI.SetDisplayVisible(false);
        dialogueUI.ClearAnswers();

        

        if (conversationData.dialogueType == ConversationData.DialogueType.CONVERSATION)
        {
            PlayerController.instance.ChangeCursorVisible(false);
            PlayerController.instance.Unblock(this);
        }

        OnConversationEnded?.Invoke();
    }

    public void DialogueInputTrigger()
    {
        inputTriggered = true;
    }


    public void SelectAnswer(int index)
    {
        QuestionLine selectedAnswer = currentQuestions[index];

        selectedAnswer.OnSelected.Call();

        if (selectedAnswer.resultConversation != null) StartConversation(selectedAnswer.resultConversation);
        else showingLine = false;
    }
}
