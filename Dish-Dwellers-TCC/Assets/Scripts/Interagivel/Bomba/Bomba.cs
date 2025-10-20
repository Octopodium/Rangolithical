using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class Bomba : MonoBehaviour{
    [SerializeField] private GameObject explosaoPrefab;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Renderer bombaRenderer;
    [SerializeField] private Color corPiscante = Color.red;
    private Color corNormal = Color.white;
    private MaterialPropertyBlock mpb;
    private static int baseColorID = Shader.PropertyToID("_BaseColor");
    [SerializeField] private float tempoParaExplodir = 5.0f;
    private int numeroDePiscadas = 2;
    private float timer;
    public UnityEvent OnExplode;


    private void Awake(){
        corNormal = bombaRenderer.material.color;
        mpb = new MaterialPropertyBlock();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable(){
        timer = tempoParaExplodir;
        StartCoroutine(PiscarBomba());
    }

    private void OnDisable(){
        StopAllCoroutines();
        mpb.SetColor(baseColorID, corNormal);
        bombaRenderer.SetPropertyBlock(mpb);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate(){
        timer -= Time.fixedDeltaTime;
        if(timer <= 0.0f){
            // bombaRenderer.gameObject.SetActive(false);
            // GetComponent<Collider>().enabled = false;
            Instantiate(explosaoPrefab, transform.position, explosaoPrefab.transform.rotation);
            // Destroy(gameObject);
            OnExplode?.Invoke();
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosaoPrefab.transform.localScale.x / 2);
    }

    IEnumerator PiscarBomba(){
        float piscadas = numeroDePiscadas;
        for(int i = 0; i < tempoParaExplodir; i++){
            piscadas *= i + 1;
            float step = 1.0f / piscadas;
            for(int j = 0; j < piscadas; j++){
                yield return new WaitForSeconds(step);
                if(j % 2 == 0) mpb.SetColor(baseColorID, corPiscante);
                else mpb.SetColor(baseColorID, corNormal);
                bombaRenderer.SetPropertyBlock(mpb);
            }
        }
    }

}
