using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
    
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView , VisualElement.UxmlTraits> { }

    private Editor editor;
    public InspectorView()
    {
    }

    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        
        Add(container);
    }
}
