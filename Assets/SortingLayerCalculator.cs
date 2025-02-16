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

[CustomEditor(typeof(SortingLayerCalculator))]
public class CustomInspector : Editor
{
    SerializedProperty calculateTypeProperty;

    void OnEnable()
    {
        calculateTypeProperty = serializedObject.FindProperty("calculateType");
    }

    /* Inspector를 그리는 함수 */
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        GUILayout.Label("레이어 계산할 객체 타입");
        EditorGUILayout.PropertyField(calculateTypeProperty);

        if ((CalculateType)calculateTypeProperty.enumValueIndex == CalculateType.SpriteRenderer)
        {
            var list = serializedObject.FindProperty("spriteRenderers");
            EditorGUILayout.PropertyField(list);
        }
        else if ((CalculateType)calculateTypeProperty.enumValueIndex == CalculateType.SortingGroup)
        {
            var list = serializedObject.FindProperty("sortingGroups");
            EditorGUILayout.PropertyField(list);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
