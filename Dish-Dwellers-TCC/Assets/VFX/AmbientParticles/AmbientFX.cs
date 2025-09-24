using UnityEditor;
using UnityEngine;

public class AmbientFX : MonoBehaviour {

    [SerializeField] private Bounds bounds = new Bounds(
        center : Vector3.zero,
        size: Vector3.one
    );

}
