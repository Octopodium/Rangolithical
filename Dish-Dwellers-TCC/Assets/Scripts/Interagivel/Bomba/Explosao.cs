using UnityEngine;
using System.Collections.Generic;

public class Explosao : MonoBehaviour{

    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private GameObject explosaoVFX;
    private float timer;
    private List<GameObject> gameObjectsAtingidos = new List<GameObject>();


    private void Awake(){
        timer = lifeTime;
        Instantiate(explosaoVFX, transform.position, Quaternion.identity);
    }

    private void FixedUpdate(){
        timer -= Time.fixedDeltaTime;
        if(timer <= 0.0f){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(gameObjectsAtingidos.Contains(other.gameObject)) return;
        if(other.CompareTag("Quebravel")){
            other.GetComponent<Explodivel>().ReduzirIntegridade();
            gameObjectsAtingidos.Add(other.gameObject);
        }
        else if(other.CompareTag("Player")){
            Player player = other.GetComponent<Player>();
            Debug.Log($"<color=green>Player acertado</color>");
            player.MudarVida(-1);
            player.AplicarKnockback(transform);
            gameObjectsAtingidos.Add(other.gameObject);
        }
    }

}
