using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueGraph))]
public class DialogueGraphEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Open Dialogue Graph"))
        {
            var window = EditorWindow.GetWindow<DialogueGraphWindow>();
            window.InitializeGraph(target as DialogueGraph);
        }
    }
}