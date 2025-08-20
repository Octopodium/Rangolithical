using UnityEngine;
using System.Collections;

public class Bomba : IResetavel{

    [SerializeField] private GameObject explosaoPrefab;
    [SerializeField] private Renderer bombaRenderer;
    [SerializeField] private float tempoParaExplodir = 4.0f;
    [SerializeField] private Color corPiscante = Color.red;
    private int numeroDePiscadas = 2;
    private float timer;


    private void Awake(){
        timer = tempoParaExplodir;
    }

    private void Start(){
        sala sala = GameManager.instance.salaAtual;
        if(!sala.resetaveis.Contains(this)){
            sala.resetaveis.Add(this);
        }
    }

    private void FixedUpdate(){
        timer -= Time.fixedDeltaTime;
        if(timer <= 0.0f){
            bombaRenderer.gameObject.SetActive(false);
            GetComponent<Collider>().enabled = false;
            Instantiate(explosaoPrefab, transform.position, explosaoPrefab.transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnDestroy(){
        sala sala = GameManager.instance.salaAtual;
        if (sala.resetaveis.Contains(this)) {
            sala.resetaveis.Remove(this);
        }
    }

    public override void OnReset(){
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosaoPrefab.transform.localScale.x);
    }

    //IEnumerator PiscarBomba(){
        
    //}

}
