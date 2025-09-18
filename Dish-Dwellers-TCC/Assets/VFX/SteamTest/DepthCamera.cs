using UnityEngine;
using System.Collections;

public class DepthCamera : MonoBehaviour {

    private void Start() => StartCoroutine(DeactivateCameraAfterFrame());

    IEnumerator DeactivateCameraAfterFrame() {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }

}
