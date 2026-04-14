using GraphProcessor;
using System;
using System.Collections;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : StaticInstance<DialogueManager>
{
    [SerializeField] private DialogueUI dialogueUI;

    private bool inputTriggered;
    private Coroutine conversationRoutine;
    private DialogueGraphExecutor executor;
    
    public event Action OnConversationEnded;


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
            StopCoroutine(conversationRoutine);

        conversationRoutine = StartCoroutine(EConversation(conversationData));
    }

    private IEnumerator EConversation(ConversationData conversationData)
    {
        if (conversationData == null) yield break;

        if(conversationData.pausePlayerOnDialogue)
            PlayerController.instance.Block(this);

        executor = new DialogueGraphExecutor(conversationData.graph);

        executor.Start();

        dialogueUI.ClearLine();
        dialogueUI.ClearAnswers();
        dialogueUI.SetDisplayVisible(true);

        yield return new WaitForEndOfFrame();

        Debug.Log(executor.Graph);

        while(executor.GetCurrent() != null)
        {
            Debug.Log("Running Node");
            yield return ProcessNode(executor.GetCurrent());
        }

        dialogueUI.SetDisplayVisible(false);
        dialogueUI.ClearAnswers();

        if (conversationData.pausePlayerOnDialogue)
        {
            PlayerController.instance.ChangeCursorVisible(false);
            PlayerController.instance.Unblock(this);
        }

        OnConversationEnded?.Invoke();
    }

    private IEnumerator ProcessNode(BaseNode node)
    {
        if (node is LineNode lineNode)
            yield return ProcessLineNode(lineNode);
        else if (node is StartNode startNode)
            yield return ProcessStartNode(startNode);
        else if (node is EndNode endNode)
            yield return ProcessEndNode(endNode);
        else
            executor.Advance();
    }

    private IEnumerator ProcessStartNode(StartNode startNode)
    {
        Debug.Log("Entered StartNode");

        startNode.OnEnterConversation.Call();
        executor.Advance();
        yield break;
    }
    private IEnumerator ProcessEndNode(EndNode endNode)
    {
        Debug.Log("Entered EndNode");

        endNode.OnExitConversation.Call();
        executor.Advance();
        yield break;
    }
    private IEnumerator ProcessLineNode(LineNode lineNode)
    {
        Debug.Log("Entered LineNode");

        yield return new WaitForSeconds(lineNode.delayToStart);

        lineNode.OnEnterLine.Call();

        dialogueUI.DisplayLine(
            lineNode.whoIsTalking.GetLocalizedString(),
            lineNode.content.GetLocalizedString()
        );

        yield return new WaitForSeconds(0.16f);

        if (lineNode.HasChoices)
        {
            PlayerController.instance.ChangeCursorVisible(true);
            dialogueUI.DisplayAnswers(lineNode.GetAnswers());

            int selectedIndex = -1;
            dialogueUI.OnChoiceSelected = (i) => selectedIndex = i;
            yield return new WaitUntil(() => selectedIndex >= 0);

            lineNode.onSelectedCallbacks[selectedIndex].Call();
            executor.AdvanceFromChoice(selectedIndex);

            dialogueUI.ClearAnswers();

        }
        else
        {
            yield return WaitForInput();
            dialogueUI.ClearLine();
            executor.Advance();
        }

        lineNode.OnExitLine.Call();
        yield return new WaitForSeconds(lineNode.delayToEnd);
    }

    private IEnumerator WaitForInput()
    {
        inputTriggered = false;
        while (true)
        {
            yield return null;
            if (!inputTriggered) continue;

            Debug.Log("Skipping");

            if (dialogueUI.TryToSkip())
                break;

            inputTriggered = false;
        }
    }

    public void DialogueInputTrigger()
    {
        inputTriggered = true;
    }

    public void SelectAnswer(int index)
    {
        dialogueUI.OnChoiceSelected?.Invoke(index);
        PlayerController.instance.ChangeCursorVisible(false);
    }

}
