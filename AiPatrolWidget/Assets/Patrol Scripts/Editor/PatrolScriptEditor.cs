using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace PatrolScripts
{

    [CustomEditor(typeof(NPCPatrol))]
    public class PatrolScriptEditor :Editor
    {
        public VisualTreeAsset VisualTree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            VisualTree.CloneTree(root);


            SerializedObject serObj = new SerializedObject(target);
            root.Bind(serObj);
            return root;

        }
    }
}
