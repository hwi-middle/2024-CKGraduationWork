using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;

    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits>
    {
    }

    private BehaviorTree tree;

    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer { maxScale = 2.0f });
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_MyAssets/Editor/BehaviorTree/BehaviorTreeEditor.uss");
        styleSheets.Add(styleSheet);
        
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    public void PopulateView(BehaviorTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        for (int index = 0; index < tree.nodes.Count; index++)
        {
            Node treeNode = tree.nodes[index];
            CreateNodeView(treeNode);
        }

        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });

    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewChange)
    {
        if (graphviewChange.elementsToRemove != null)
        {
            graphviewChange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphviewChange.edgesToCreate != null)
        {
            graphviewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.node, childView.node);
            });
        }

        if(graphviewChange.movedElements != null)
        {
            nodes.ForEach(n =>
            {
                NodeView view = n as NodeView;
                view.SortChildren();
            });
        }
        
        return graphviewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // base.BuildContextualMenu(evt);
        VisualElement contentViewContainer = ElementAt(1);
        Vector3 screenMousePosition = evt.localMousePosition;
        Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
        worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
        
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, worldMousePosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, worldMousePosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type, worldMousePosition));
            }
        }
    }

    private string RemoveNodeSuffix(string node)
    {
        return node.Replace("Node", "");
    }

    void CreateNode(System.Type type, Vector2 pos)
    {
        if (tree == null)
        {
            Debug.Log("Tree is null");
            return;
        }
        Debug.Log("CreateNode: " + type.Name);
        Node node = tree.CreateNode(type);
        
        node.position = pos;
        node.OnCreate();
        CreateNodeView(node);
    }

    void CreateNodeView(Node node)
    {
        Debug.Assert(node != null);
        NodeView nodeView = new NodeView(node);
        nodeView.onNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }
}
