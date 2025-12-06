using UnityEngine;

public class WaveMotion : MonoBehaviour{

    [SerializeField] private float amplitude = 1.0f;
    [SerializeField] private float offset = 0.0f;
    [SerializeField] private float speed = 1.0f;
    private float originalPosition;


    private void Start() => originalPosition = transform.position.y;
    

    private void LateUpdate(){
        float y = Mathf.Sin((Time.time * speed + offset) * Mathf.PI) * amplitude;
        transform.position = new Vector3(transform.position.x, originalPosition + y, transform.position.z);
    }

}
