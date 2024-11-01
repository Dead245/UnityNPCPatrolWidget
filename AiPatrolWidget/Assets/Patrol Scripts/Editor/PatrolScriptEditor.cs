using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Search;


namespace PatrolScripts
{

    [CustomEditor(typeof(NPCPatrol))]
    public class PatrolScriptEditor :Editor
    {
        public VisualTreeAsset visualTree;
        public VisualElement previewContainer;
        public ListView NPCList;

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

            NPCList = (ListView)root.Q("NPCListView");
            NPCList.selectedIndicesChanged += NPCListIndeceChange;
            previewContainer = root.Q("NpcDisplayContainer");
         
            return root;
        }

        private void NPCListIndeceChange(System.Collections.Generic.IEnumerable<int> obj)
        {
            Debug.Log(NPCList.selectedIndex);
            //Why can't I get the visualElement I click on???
        }
    }


}
