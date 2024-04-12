using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SnapToFloor
{
    public class STFAbout : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset STFAboutUXML;
    
        [MenuItem("Window/SnapToFloor/About")]
        public static void Init()
        {
            STFAbout wnd = GetWindow<STFAbout>();
            wnd.titleContent = new GUIContent("About");
            wnd.minSize = new Vector2(350, 120);
            wnd.maxSize = new Vector2(350, 120);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
        
            // Import UXML
            VisualTreeAsset visualTree = STFAboutUXML;
            VisualElement container = visualTree.Instantiate();
            root.Add(container);
            
            // Set version
            Label versionText = root.Q<Label>("version-text");

            string packagePath = AssetDatabase.GUIDToAssetPath("389b485f86c44b8d92ef4fe6c1bcf38c");
            TextAsset packageJson = AssetDatabase.LoadAssetAtPath<TextAsset>(packagePath);
            
            PackageInfo info = JsonUtility.FromJson<PackageInfo>(packageJson.text);
            versionText.text = "Version " + info.version;
        }
        
        [Serializable]
        internal class PackageInfo
        {
            public string name;
            public string displayName;
            public string version;
            public string unity;
            public string description;
            public List<string> keywords;
            public string type;
        }
    }
}