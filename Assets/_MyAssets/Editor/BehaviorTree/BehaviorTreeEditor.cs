using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

public class BehaviorTreeEditor : EditorWindow
{
    private BehaviorTreeView _treeView;
    private InspectorView _inspectorView;
    private IMGUIContainer _blackboardView;

    private SerializedObject _treeObject;
    private SerializedProperty _blackboardProperty;
    
    [MenuItem("DMW Tools/BehaviorTreeEditor", false, 30)]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_MyAssets/Editor/BehaviorTree/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_MyAssets/Editor/BehaviorTree/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviorTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _blackboardView = root.Q<IMGUIContainer>();
        _blackboardView.onGUIHandler = () =>
        {
            if (_treeObject != null)
            {
                _treeObject.Update();
                EditorGUILayout.PropertyField(_blackboardProperty);
                _treeObject.ApplyModifiedProperties();
            }
        };
        _treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                // OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    private void OnSelectionChange()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                EnemyBase runner = Selection.activeGameObject.GetComponent<EnemyBase>();
                if (runner)
                {
                    tree = runner.Tree;
                }
            }
        }

        if (Application.isPlaying)
        {
            if (tree)
            {
                _treeView.PopulateView(tree);
            }
        }
        else if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            _treeView.PopulateView(tree);
        }

        if (tree != null)
        {
            _treeObject = new SerializedObject(tree);
            _blackboardProperty = _treeObject.FindProperty("blackboard");
        }
    }

    void OnNodeSelectionChanged(NodeView node)
    {
        _inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        _treeView?.UpdateNodeStates();
    }
}
