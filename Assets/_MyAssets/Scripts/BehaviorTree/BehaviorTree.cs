using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviorTree : ScriptableObject
{
    public Node rootNode;
    public Node.ENodeState treeState = Node.ENodeState.Running;
    public List<Node> nodes = new List<Node>();
    public Blackboard blackboard = new Blackboard();
    
    public Node.ENodeState Update()
    {
        if (rootNode.state == Node.ENodeState.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

#if UNITY_EDITOR
    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        node.OnCreate();
        
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (Create Node)");

        Undo.RecordObject(this, "Behavior Tree (Create Node)");
        nodes.Add(node);

        // Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (Create Node)");
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behavior Tree (Delete Node)");
        nodes.Remove(node);

        // AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behavior Tree (Add Child)");
            decorator.child = child;
            EditorUtility.SetDirty(decorator);
        }

        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behavior Tree (Add Child)");
            root.child = child;
            EditorUtility.SetDirty(root);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behavior Tree (Add Child)");
            composite.children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behavior Tree (Remove Child)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }

        RootNode root = parent as RootNode;
        if (root)
        {
            Undo.RecordObject(root, "Behavior Tree (Remove Child)");
            root.child = null;
            EditorUtility.SetDirty(root);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behavior Tree (Remove Child)");
            composite.children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }
#endif
    
    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }

        RootNode root = parent as RootNode;
        if (root && root.child != null)
        {
            children.Add(root.child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.children;
        }

        return children;
    }

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(n => Traverse(n, visiter));
        }
    }

    public BehaviorTree Clone()
    {
        BehaviorTree tree = Instantiate(this);
        tree.rootNode = rootNode.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.rootNode, n => { tree.nodes.Add(n); });

        return tree;
    }

    public void Bind(EnemyBase enemyBase)
    {
        Traverse(rootNode, n =>
        {
            n.agent = enemyBase;
            n.blackboard = blackboard;
        });
    }
}
