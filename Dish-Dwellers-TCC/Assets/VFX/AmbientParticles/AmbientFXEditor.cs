using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AmbientFX))]
public class AmbientFXEditor : Editor {

    AmbientFX ambientFX;

    SerializedProperty bounds;

    private void OnEnable() {
        ambientFX = target as AmbientFX;
        bounds = serializedObject.FindProperty(nameof(bounds));
    }

    public void OnSceneGUI() {
        Transform targetTransform = ((AmbientFX)target).transform;
        Handles.color = Color.cyan;
        float handleSize = HandleUtility.GetHandleSize(targetTransform.position) * 0.25f;
        Vector3[] handlePositions = {
            targetTransform.position + Vector3.forward * (targetTransform.localScale.z / 2),
            targetTransform.position + Vector3.back * (targetTransform.localScale.z / 2),
            targetTransform.position + Vector3.right * (targetTransform.localScale.x / 2),
            targetTransform.position + Vector3.left * (targetTransform.localScale.x / 2),
            targetTransform.position + Vector3.up * (targetTransform.localScale.y / 2),
            targetTransform.position + Vector3.down * (targetTransform.localScale.y / 2),
        };

        // Handles.PositionHandle(targetTransform.position + Vector3.right, targetTransform.rotation);
        
        Handles.DrawWireCube(targetTransform.position, targetTransform.localScale);
        foreach(Vector3 handlePosition in handlePositions) {
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(handlePosition, Quaternion.identity);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(targetTransform, "AmbientFx Bounds have been changed.");
                Vector3 distance = targetTransform.position - handlePosition;
                Vector3 newDistance = targetTransform.position - newPosition;
                Vector3 discrepancy = new Vector3(Mathf.Abs(distance.x - newDistance.x),Mathf.Abs(distance.y - newDistance.y), Mathf.Abs(distance.z - newDistance.z));
                targetTransform.localScale += distance.magnitude < newDistance.magnitude ? discrepancy : -discrepancy;
                targetTransform.position += (distance - newDistance) / 2;
                EditorUtility.SetDirty(targetTransform);
            }
            Handles.CubeHandleCap(
                0,
                handlePosition,
                targetTransform.rotation,
                handleSize,
                EventType.Repaint
            );
        }
    }

}
