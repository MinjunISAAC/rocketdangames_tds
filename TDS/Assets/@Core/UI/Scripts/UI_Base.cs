// ----- System
using System.Linq;

// ----- Unity
using UnityEngine;
using UnityEditor;

namespace UI
{
    public class UI_Base : MonoBehaviour
    {
        
    }

    // --------------------------------------------------
    // Editor Code
    // --------------------------------------------------
#if UNITY_EDITOR
    [CustomEditor(typeof(UI_Base), true)]
    public class UIBaseEditor : Editor
    {
        // --------------------------------------------------
        // Variables
        // -------------------------------------------------
        protected UI_Base _target = null;
        
        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------
        private void OnEnable() { _target = (UI_Base)target; }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if (GUILayout.Button("UI 바인드"))
                BindComponents();

            serializedObject.ApplyModifiedProperties();
        }

        // --------------------------------------------------
        // Functions - Normal
        // --------------------------------------------------
        private void BindComponents()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                {
                    string propertyName = iterator.name;
                    
                    if (propertyName.StartsWith("TMP_") || propertyName.StartsWith("_TMP_"))
                    {
                        var tmp = _target.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true)
                            .FirstOrDefault(x => x.gameObject.name == propertyName || 
                                               x.gameObject.name == propertyName.TrimStart('_'));
                        if (tmp != null)
                            iterator.objectReferenceValue = tmp;
                    }
                    else if (propertyName.StartsWith("IMG_") || propertyName.StartsWith("_IMG_"))
                    {
                        var img = _target.GetComponentsInChildren<UnityEngine.UI.Image>(true)
                            .FirstOrDefault(x => x.gameObject.name == propertyName || 
                                               x.gameObject.name == propertyName.TrimStart('_'));
                        if (img != null)
                            iterator.objectReferenceValue = img;
                    }
                    else if (propertyName.StartsWith("OBJ_") || propertyName.StartsWith("_OBJ_"))
                    {
                        var objName = propertyName.TrimStart('_');
                        var obj = _target.transform.Find(propertyName)?.gameObject ?? 
                                 _target.transform.Find(objName)?.gameObject;
                        if (obj != null)
                            iterator.objectReferenceValue = obj;
                    }
                    else if (propertyName.StartsWith("RECT_") || propertyName.StartsWith("_RECT_"))
                    {
                        var img = _target.GetComponentsInChildren<RectTransform>(true)
                            .FirstOrDefault(x => x.gameObject.name == propertyName ||
                                               x.gameObject.name == propertyName.TrimStart('_'));
                        if (img != null)
                            iterator.objectReferenceValue = img;
                    }
                    else if (propertyName.StartsWith("NBTN_") || propertyName.StartsWith("_NBTN_"))
                    {
                        var objName = propertyName.TrimStart('_');
                        var obj = _target.transform.Find(propertyName)?.gameObject ?? 
                                 _target.transform.Find(objName)?.gameObject;
                        if (obj != null)
                            iterator.objectReferenceValue = obj;
                    }
                    else if (propertyName.EndsWith("Group") || propertyName.EndsWith("Container"))
                    {
                        var objName = propertyName;
                        if (propertyName.StartsWith("_"))
                            objName = propertyName.Substring(1);
                        else
                            objName = "_" + propertyName;

                        Transform foundTransform = null;

                        var allTransforms = _target.GetComponentsInChildren<Transform>(true);
                        foundTransform = allTransforms.FirstOrDefault(x =>
                            x.name.Equals(objName, System.StringComparison.OrdinalIgnoreCase) ||
                            x.name.Equals(propertyName, System.StringComparison.OrdinalIgnoreCase));

                        if (foundTransform != null)
                            iterator.objectReferenceValue = foundTransform;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}