using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ControladorDeObjeto)), CanEditMultipleObjects]
public class ControladorDeObjetoEditor : Editor {

    private SerializedProperty respawnPos;

    private ControladorDeObjeto controladorDeObjeto;
    [SerializeField] private Vector3 boxSize = Vector3.one;

    private void OnEnable() {
        respawnPos = serializedObject.FindProperty(nameof(respawnPos));
        controladorDeObjeto = target as ControladorDeObjeto;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        Handles.color = Color.green;
        Vector3 respawnPosition = controladorDeObjeto.respawnPos;
        Handles.DrawWireCube(controladorDeObjeto.transform.TransformPoint(respawnPosition), boxSize);
        EditorGUI.BeginChangeCheck();
        Vector3 modifiedRespawnPosition = Handles.DoPositionHandle(controladorDeObjeto.transform.TransformPoint(respawnPosition), controladorDeObjeto.transform.rotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Respawn position was altered.");
            controladorDeObjeto.respawnPos = controladorDeObjeto.transform.InverseTransformPoint(modifiedRespawnPosition);
            EditorUtility.SetDirty(target);
        }
        Ray ray = new Ray(controladorDeObjeto.transform.TransformPoint(respawnPosition), Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 30.0f)) {
            Handles.color = Color.yellow;
            Vector3 predictedCubePosition = hitInfo.point + Vector3.up * boxSize.y / 2;
            Handles.DrawDottedLine(ray.origin, predictedCubePosition, 6f);
            Handles.DrawWireCube(predictedCubePosition, boxSize);
        }
    }

}
