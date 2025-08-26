using UnityEngine;
using System.Collections;
public class Bomba : MonoBehaviour{
    [SerializeField] private GameObject explosaoPrefab;
    [SerializeField] private Renderer bombaRenderer;
    [SerializeField] private Color corPiscante = Color.red;
    private Color corNormal = Color.white;
    private MaterialPropertyBlock mpb;
    [SerializeField] private float tempoParaExplodir = 5.0f;
    private int numeroDePiscadas = 2;
    private float timer;


    private void Awake(){
        corNormal = bombaRenderer.material.color;
        mpb = new MaterialPropertyBlock();
    }

    private void OnEnable(){
        timer = tempoParaExplodir;
        StartCoroutine(PiscarBomba());
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

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosaoPrefab.transform.localScale.x / 2);
    }

    IEnumerator PiscarBomba(){
        for(int i = 0; i < tempoParaExplodir; i++){
            numeroDePiscadas *= i + 1;
            float step = 1.0f / numeroDePiscadas;
            for(int j = 0; j < numeroDePiscadas; j++){
                yield return new WaitForSeconds(step);
                if(j % 2 == 0) mpb.SetColor("_BaseColor", corPiscante);
                else mpb.SetColor("_BaseColor", corNormal);
                bombaRenderer.SetPropertyBlock(mpb);
            }
        }
    }

}
