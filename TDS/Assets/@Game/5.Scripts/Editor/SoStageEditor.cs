#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Game;

[CustomEditor(typeof(SoStage))]
public class SoStageEditor : Editor
{
    private SerializedProperty stageIndexProp;
    private SerializedProperty unitDataListProp;

    private void OnEnable()
    {
        stageIndexProp = serializedObject.FindProperty("_stageIndex");
        unitDataListProp = serializedObject.FindProperty("_unitDataList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("1. 스테이지 정보", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(stageIndexProp, new GUIContent("Stage Index"));

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("2. 유닛 데이터 리스트", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(unitDataListProp.FindPropertyRelative("Array.size"), new GUIContent("유닛 개수"));

        EditorGUILayout.Space(5);
        if (unitDataListProp.arraySize > 0)
        {
            for (int i = 0; i < unitDataListProp.arraySize; i++)
            {
                SerializedProperty unitDataProp = unitDataListProp.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(unitDataProp, new GUIContent($"Unit Index : {i}"), true);
                EditorGUILayout.EndVertical();
                
                if (i < unitDataListProp.arraySize - 1)
                    GUILayout.Space(2);
            }
        }
        else
            EditorGUILayout.HelpBox("유닛 데이터가 없습니다. 리스트 크기를 늘려주세요.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif 