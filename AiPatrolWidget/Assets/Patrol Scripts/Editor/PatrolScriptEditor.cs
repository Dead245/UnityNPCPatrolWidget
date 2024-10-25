using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace PatrolScripts
{

    [CustomEditor(typeof(NPCPatrol))]
    public class PatrolScriptEditor :Editor
    {
        public VisualTreeAsset visualTree;

        private void OnEnable()
        {
            string path = "Assets/UXML/PatrolScriptVisualTree.uxml";
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        }
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            visualTree.CloneTree(root);


            SerializedObject serObj = new SerializedObject(target);
            root.Bind(serObj);
            return root;

        }
    }
}
