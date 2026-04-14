using GraphProcessor;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


[NodeCustomEditor(typeof(StartNode))]
public class StartNodeView : BaseNodeView
{
    public override void Enable()
    {
        AddToClassList("start-node");

        base.Enable();
    }
}

[NodeCustomEditor(typeof(LineNode))]
public class LineNodeView : BaseNodeView
{
    LineNode lineNode;
    VisualElement choicesContainer;
    SerializedObject serializedGraph; // mantém vivo aqui
    bool isUpdating = false;

    public override void Enable()
    {
        lineNode = nodeTarget as LineNode;
        AddToClassList("line-node");

        base.Enable();

        serializedGraph = new SerializedObject(owner.graph); // cria uma vez

        choicesContainer = new VisualElement();
        controlsContainer.Add(choicesContainer);

        owner.graph.onGraphChanges += OnGraphChanged;
        RebuildChoiceFields();
    }

    public override void Disable()
    {
        owner.graph.onGraphChanges -= OnGraphChanged;
        serializedGraph?.Dispose();
    }

    void OnGraphChanged(GraphChanges changes)
    {
        if (changes.nodeChanged == lineNode)
        {
            serializedGraph.Update(); // atualiza o objeto serializado
            lineNode.UpdateAllPorts();
            //RefreshPorts();
            RebuildChoiceFields();
        }
    }

    void RebuildChoiceFields()
    {
        // desvincula todos os PropertyFields antes de limpar
        choicesContainer.Unbind();
        choicesContainer.Clear();

        serializedGraph = new SerializedObject(owner.graph);

        var nodeIndex = owner.graph.nodes.IndexOf(lineNode);
        var nodeProp = serializedGraph.FindProperty("nodes").GetArrayElementAtIndex(nodeIndex);
        var contentsProp = nodeProp.FindPropertyRelative(nameof(LineNode.choiceContents));

        for (int i = 0; i < lineNode.choiceContents.Count; i++)
        {
            var contentField = new PropertyField(contentsProp.GetArrayElementAtIndex(i), $"Choice {i}");
            contentField.Bind(serializedGraph);

            contentField.RegisterCallback<MouseDownEvent>(e => e.StopPropagation());
            contentField.RegisterCallback<MouseMoveEvent>(e => e.StopPropagation());
            contentField.RegisterCallback<MouseUpEvent>(e => e.StopPropagation());

            contentField.RegisterCallback<FocusOutEvent>(evt =>
            {
                schedule.Execute(() => ForceUpdate()).StartingIn(0);
            });

            choicesContainer.Add(contentField);
        }

        choicesContainer.Add(new Button(() => AddChoice()) { text = "Add Choice" });

        if (lineNode.choiceContents.Count > 0)
            choicesContainer.Add(new Button(() => RemoveChoice()) { text = "Remove Choice" });
    }

    void AddChoice()
    {
        lineNode.choiceContents.Add(new LocalizedString());
        lineNode.onSelectedCallbacks.Add(Message.Empty);
        schedule.Execute(() => ForceUpdate()).StartingIn(0);
    }

    void RemoveChoice()
    {
        if (lineNode.choiceContents.Count == 0) return;

        // desvincula imediatamente antes de modificar a lista
        choicesContainer.Unbind();
        choicesContainer.Clear();

        int last = lineNode.choiceContents.Count - 1;
        lineNode.choiceContents.RemoveAt(last);
        lineNode.onSelectedCallbacks.RemoveAt(last);
        schedule.Execute(() => ForceUpdate()).StartingIn(0);
    }



    void ForceUpdate()
    {
        if (isUpdating) return;
        isUpdating = true;

        EditorUtility.SetDirty(owner.graph);
        owner.graph.NotifyNodeChanged(lineNode);

        isUpdating = false;
    }
}