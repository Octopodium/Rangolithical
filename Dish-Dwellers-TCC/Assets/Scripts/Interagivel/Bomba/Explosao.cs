using UnityEngine;

public class Explosao : MonoBehaviour{

    [SerializeField] private float lifeTime = 1.0f;
    private float timer;


    private void Awake(){
        timer = lifeTime;
    }

    private void FixedUpdate(){
        timer -= Time.fixedDeltaTime;
        if(timer <= 0.0f){
            Destroy(gameObject);
        }
    }

}
