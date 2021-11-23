using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

[ExecuteInEditMode]
 public class Node_Editor : EditorWindow
{
    [MenuItem("Tool/NodeConnection")]
    public static void ShowWindow()
    {
        // Opens the window, otherwise focuses it if it’s already open.
        var window = GetWindow<Node_Editor>();

        // Adds a title to the window.
        window.titleContent = new GUIContent("Node Conection");

        // Sets a minimum size to the window.
        window.minSize = new Vector2(250, 50);
    }

    private void OnEnable()
    {
        // Reference to the root of the window.
        var root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Scripts/Editor/NodeTool.uxml");
        VisualElement toolFromUXML = visualTree.CloneTree();
        root.Add(toolFromUXML);

        // 6
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its
        // children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>
            ("Assets/Scripts/Editor/NodeStyle.uss");
        root.styleSheets.Add(styleSheet);


        Button connect = rootVisualElement.Q<Button>("btnConnect");
        Button delete = rootVisualElement.Q<Button>("btnDelete");
        Button deleteAll = rootVisualElement.Q<Button>("btnDeleteAll");
        Button updateGizmos = rootVisualElement.Q<Button>("btnUpdateGizmos");

        connect.clickable.clicked += () =>
        {
            if (Selection.activeGameObject == null)
                return;

            var selection = Selection.gameObjects.ToList();

            foreach (GameObject go in selection)
            {
                foreach (var g in selection)
                {
                    if (g.GetInstanceID() != go.GetInstanceID() && !g.GetComponent<Node>().neighbours.Contains(go.GetComponent<Node>()))
                        g.GetComponent<Node>().neighbours.Add(go.GetComponent<Node>());
                }
            }
        };

        delete.clickable.clicked += () =>
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Node>() == null)
                return;

            Selection.activeGameObject.GetComponent<Node>().neighbours.Clear();
        };

        deleteAll.clickable.clicked += () =>
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Node>() == null)
                return;

            var select = Selection.gameObjects;

            foreach (var item in select)
            {
                item.GetComponent<Node>().neighbours.Clear();
            }
        };

        updateGizmos.clickable.clicked += () =>
         {
             foreach (var g in Selection.gameObjects)
             {
                 if (g.GetComponent<Node>() != null)
                 {
                     foreach (var item in g.GetComponent<Node>().neighbours)
                     {
                         Debug.DrawLine(g.transform.position, item.gameObject.transform.position, Color.green);
                     }
                 }
             }
         };
    }
}
