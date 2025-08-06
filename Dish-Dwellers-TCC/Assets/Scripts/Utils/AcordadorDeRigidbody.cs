using UnityEngine;

public class AcordadorDeRigidbody : MonoBehaviour{

    private void Start(){
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if(rigidbody != null){
            rigidbody.sleepThreshold = 0.0f;
        }
    }

}
