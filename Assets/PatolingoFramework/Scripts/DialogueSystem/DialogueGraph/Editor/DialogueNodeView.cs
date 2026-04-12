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
    public override void Enable()
    {
        AddToClassList("line-node");

        base.Enable();
    }
}

[NodeCustomEditor(typeof(BranchNode))]
public class BranchNodeView : BaseNodeView
{
    BranchNode branchNode;

    public override void Enable()
    {
        branchNode = nodeTarget as BranchNode;
        AddToClassList("branch-node");

        var addButton = new Button(() => AddChoice()) { text = "Add Choice" };
        var removeButton = new Button(() => RemoveChoice()) { text = "Remove Choice" };

        controlsContainer.Add(addButton);
        controlsContainer.Add(removeButton);

        owner.graph.onGraphChanges += OnGraphChanged;

        RebuildChoiceFields();
    }

    public override void Disable()
    {
        owner.graph.onGraphChanges -= OnGraphChanged;
    }

    void OnGraphChanged(GraphChanges changes)
    {
        if (changes.nodeChanged == branchNode)
        {
            branchNode.UpdateAllPorts();
            RefreshPorts();
            RebuildChoiceFields();
        }
    }

    void RebuildChoiceFields()
    {
        controlsContainer.Clear();

        var serializedGraph = new SerializedObject(owner.graph);
        var nodeIndex = owner.graph.nodes.IndexOf(branchNode);
        var nodeProp = serializedGraph.FindProperty("nodes").GetArrayElementAtIndex(nodeIndex);
        var contentsProp = nodeProp.FindPropertyRelative(nameof(BranchNode.choiceContents));

        for (int i = 0; i < branchNode.choiceContents.Count; i++)
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

            controlsContainer.Add(contentField);
        }

        controlsContainer.Add(new Button(() => AddChoice()) { text = "Add Choice" });
        controlsContainer.Add(new Button(() => RemoveChoice()) { text = "Remove Choice" });
    }

    void AddChoice()
    {
        branchNode.choiceContents.Add(new LocalizedString());
        schedule.Execute(() => ForceUpdate()).StartingIn(0);
    }

    void RemoveChoice()
    {
        if (branchNode.choiceContents.Count == 0) return;
        int last = branchNode.choiceContents.Count - 1;
        branchNode.choiceContents.RemoveAt(last);
        schedule.Execute(() => ForceUpdate()).StartingIn(0);
    }

    void ForceUpdate()
    {
        EditorUtility.SetDirty(owner.graph);
        owner.graph.NotifyNodeChanged(branchNode);
    }
}