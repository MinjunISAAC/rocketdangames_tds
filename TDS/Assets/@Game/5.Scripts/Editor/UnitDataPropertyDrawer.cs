#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Game;

[CustomPropertyDrawer(typeof(UnitSpawnData))]
public class UnitDataPropertyDrawer : PropertyDrawer
{
    private const float PREVIEW_SIZE = 128f;
    private const float SPACING = 2f;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var singleLineHeight = EditorGUIUtility.singleLineHeight;
        
        // 레이블이 있는 경우에만 표시
        bool hasLabel = !string.IsNullOrEmpty(label.text);
        if (hasLabel)
        {
            position.height = singleLineHeight;
            EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
            position.y += singleLineHeight + SPACING;
        }
        
        EditorGUI.indentLevel++;
        
        // Unit Prefab 필드
        SerializedProperty unitPrefabProp = property.FindPropertyRelative("_unitPrefab");
        position.height = singleLineHeight;
        EditorGUI.PropertyField(position, unitPrefabProp, new GUIContent("Unit Prefab"));
        position.y += singleLineHeight + SPACING;
        
        // 전체 너비 활용을 위한 레이아웃 계산
        float totalWidth = position.width;
        float previewWidth = PREVIEW_SIZE;
        float variableColumnWidth = totalWidth - previewWidth - 30f; // 30f는 간격용
        
        // 스프라이트 미리보기 (왼쪽)
        Rect previewRect = new Rect(position.x, position.y, previewWidth, PREVIEW_SIZE);
        EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f, 1f)); // 어두운 배경
        EditorGUI.DrawRect(new Rect(previewRect.x + 1, previewRect.y + 1, previewRect.width - 2, previewRect.height - 2), 
                          new Color(0.8f, 0.8f, 0.8f, 1f)); // 밝은 테두리
        
        // 스프라이트 미리보기 (모든 SpriteRenderer 합성)
        if (unitPrefabProp.objectReferenceValue != null)
        {
            // UnitBase 타입이므로 UnitBase로 캐스팅
            UnitBase unitBase = unitPrefabProp.objectReferenceValue as UnitBase;
            if (unitBase != null)
            {
                GameObject prefabObj = unitBase.gameObject;
                SpriteRenderer[] spriteRenderers = prefabObj.GetComponentsInChildren<SpriteRenderer>();
                
                if (spriteRenderers.Length > 0)
                {
                    // 모든 스프라이트를 하나씩 그리기
                    for (int i = 0; i < spriteRenderers.Length; i++)
                    {
                        SpriteRenderer sr = spriteRenderers[i];
                        if (sr != null && sr.sprite != null)
                        {
                            // 간단하게 중앙에 겹쳐서 그리기
                            Rect spriteRect = new Rect(
                                previewRect.x + 16f,
                                previewRect.y + 16f,
                                previewRect.width - 32f,
                                previewRect.height - 32f
                            );
                            
                            // 스프라이트 그리기
                            GUI.DrawTexture(spriteRect, sr.sprite.texture, ScaleMode.ScaleToFit);
                        }
                    }
                }
            }
        }
        
        // 오른쪽에 변수들 나열 (더 넓게)
        float rightColumnX = previewRect.x + previewWidth + 20f;
        Rect rightRect = new Rect(rightColumnX, position.y, variableColumnWidth, singleLineHeight);
        
        // 나머지 프로퍼티들을 오른쪽에 세로로 배치
        rightRect.height = singleLineHeight;
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_maxHp"), new GUIContent("Max HP"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_moveSpeed"), new GUIContent("Move Speed"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_jumpPower"), new GUIContent("Jump Power"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_jumpForwardSpeed"), new GUIContent("Jump Forward Speed"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_jumpDuration"), new GUIContent("Jump Duration"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_attackDelay"), new GUIContent("Attack Delay"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_attackPower"), new GUIContent("Attack Power"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_startDelay"), new GUIContent("Start Delay"));
        rightRect.y += singleLineHeight + SPACING;
        
        EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("_spawnDelay"), new GUIContent("Spawn Delay"));
        
        // 실제 사용된 높이 계산 (변수들이 더 높으면 변수들 기준으로)
        float variablesHeight = (singleLineHeight + SPACING) * 9;
        float usedHeight = Mathf.Max(PREVIEW_SIZE, variablesHeight);
        position.y += usedHeight + SPACING;
        
        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var singleLineHeight = EditorGUIUtility.singleLineHeight;
        
        // 레이블이 있는 경우에만 높이 추가
        bool hasLabel = !string.IsNullOrEmpty(label.text);
        float height = hasLabel ? singleLineHeight + SPACING : 0; // 메인 레이블
        height += singleLineHeight + SPACING; // Unit Prefab
        
        // OnGUI와 동일한 높이 계산
        float variablesHeight = (singleLineHeight + SPACING) * 9; // 변수 9개
        float usedHeight = Mathf.Max(PREVIEW_SIZE, variablesHeight);
        height += usedHeight + SPACING;
        
        return height;
    }
}
#endif 