using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public enum CalculateType {SpriteRenderer, SortingGroup }

public class SortingLayerCalculator : MonoBehaviour
{
    private static int MULTIPLESCALE => 100;

    [SerializeField] private CalculateType calculateType;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private SortingGroup[] sortingGroups;

    void Start()
    {
        SetOrderInLayer();
    }

    public float GetAllZPos()
    {
        float retval = 0.0f;

        if (calculateType == CalculateType.SpriteRenderer)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                retval += spriteRenderer.transform.position.z;
            }
        }
        else if (calculateType == CalculateType.SortingGroup)
        {
            foreach (var sortingGroup in sortingGroups)
            {
                retval += sortingGroup.transform.position.z;
            }
        }

        return retval;
    }

    private void SetOrderInLayer()
    {
        if (calculateType == CalculateType.SpriteRenderer)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = GetLayer(spriteRenderer);
            }
        }
        else if (calculateType == CalculateType.SortingGroup)
        {
            foreach (var sortingGroup in sortingGroups)
            {
                sortingGroup.sortingOrder = GetLayer(sortingGroup);
            }
        }
    }

    private int GetLayer(Component obj)
    {
        return Mathf.RoundToInt(transform.position.y * -MULTIPLESCALE);
    }
}

#if UNITY_EDITOR || UNITY_EDITOR_64
[CustomEditor(typeof(SortingLayerCalculator))]
public class CustomInspector : Editor
{
    SerializedProperty calculateTypeProperty;
    SerializedProperty spriteRenderersProperty;
    SerializedProperty sortingGroupsProperty;

    void OnEnable()
    {
        calculateTypeProperty = serializedObject.FindProperty("calculateType");
        spriteRenderersProperty = serializedObject.FindProperty("spriteRenderers");
        sortingGroupsProperty = serializedObject.FindProperty("sortingGroups");
    }

    /* Inspector를 그리는 함수 */
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("레이어 계산할 객체 타입");
        EditorGUILayout.PropertyField(calculateTypeProperty);

        switch ((CalculateType)calculateTypeProperty.enumValueIndex)
        {
            case CalculateType.SpriteRenderer:
                EditorGUILayout.PropertyField(spriteRenderersProperty, true);
                break;
            case CalculateType.SortingGroup:
                EditorGUILayout.PropertyField(sortingGroupsProperty, true);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
