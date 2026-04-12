using GraphProcessor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphWindow : BaseGraphWindow
{
    [MenuItem("Tools/Dialogue Graph")]
    public static void Open()
    {
        var window = GetWindow<DialogueGraphWindow>();

        // abre um painel pra selecionar o asset de grafo
        var graph = EditorUtility.OpenFilePanel("Open Dialogue Graph", "Assets", "asset");
        if (string.IsNullOrEmpty(graph)) return;

        // converte o caminho absoluto pro caminho relativo do projeto
        graph = "Assets" + graph.Substring(Application.dataPath.Length);

        var graphAsset = AssetDatabase.LoadAssetAtPath<DialogueGraph>(graph);
        if (graphAsset != null)
            window.InitializeGraph(graphAsset);
    }

    protected override void InitializeWindow(BaseGraph graph)
    {
        titleContent = new GUIContent("Dialogue Graph");

        var graphView = new DialogueGraphView(this);
        rootView.Add(graphView);
    }
}

public class DialogueGraphView : BaseGraphView
{
    public DialogueGraphView(EditorWindow window) : base(window)
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/PatolingoFramework/DialogueGraph.uss"
        );

        if (styleSheet != null)
            styleSheets.Add(styleSheet);
    }
}