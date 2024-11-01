using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;


namespace PatrolScripts
{

    [CustomEditor(typeof(NPCPatrol))]
    public class PatrolScriptEditor : Editor
    {
        public VisualTreeAsset visualTree;
        public VisualElement previewContainer;
        public ListView NPCList;

        SerializedProperty npcProp;
        GameObject selectedGameObject = null;

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

            npcProp = serializedObject.FindProperty("npcList");

            //Preload/Cache the Asset Previews.
            for (int i = 0; i < npcProp.arraySize; i++) {
                // Get the specific item the is selected.
                SerializedProperty element = npcProp.GetArrayElementAtIndex(i);
                //Get the GameObject from the field called "npcObj" from the item in the element
                selectedGameObject = element.FindPropertyRelative("npcObj").objectReferenceValue as GameObject;
                //Make it preload into the cache
                if (selectedGameObject != null)
                {
                    Texture2D assetPreview = AssetPreview.GetAssetPreview(selectedGameObject);
                }
            }

            root.Bind(serObj);

            NPCList = (ListView)root.Q("NPCListView");
            NPCList.selectedIndicesChanged += NPCListIndeceChange;
            previewContainer = root.Q("NpcDisplayContainer");
         
            return root;
        }

        private void NPCListIndeceChange(System.Collections.Generic.IEnumerable<int> obj)
        {
            //Selected Index is -1 if dragging an item, and will cause errors
            if (NPCList.selectedIndex == -1) { return; }

            // Get the specific item the is selected.
            SerializedProperty element = npcProp.GetArrayElementAtIndex(NPCList.selectedIndex);
            //Get the GameObject from the field called "npcObj" from the item in the element
            selectedGameObject = element.FindPropertyRelative("npcObj").objectReferenceValue as GameObject;

            if (selectedGameObject != null)
            {
                //May take time for asset image to load for the first time...
                Texture2D assetPreview = AssetPreview.GetAssetPreview(selectedGameObject);
                previewContainer.style.backgroundImage = assetPreview;
            }
            else {
                //Loads error symbol onto background if either nothing is selected, or an item with an empty npcObj is selected.
                previewContainer.style.backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/com.unity.collab-proxy/Editor/PlasticSCM/Assets/Images/d_iconconflicted@2x.png");
            }
        }
    }


}
