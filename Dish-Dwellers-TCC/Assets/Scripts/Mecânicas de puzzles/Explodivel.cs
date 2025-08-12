using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Explodivel : IResetavel{

    [SerializeField] private int integridade;
    [SerializeField] private GameObject mesh;
    private Collider colliderParede;
    private int integridadeMaxima;
    

    private void Awake(){
        colliderParede = GetComponent<Collider>();
        integridadeMaxima = integridade;
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Explosao")){
            ReduzirIntegridade();
            Destroy(other.gameObject);
        }
    }

    public void ReduzirIntegridade(){
        --integridade;
        if(integridade <= 0){
            mesh.SetActive(false);  
            colliderParede.enabled = false;      
        }
    }

    public override void OnReset(){
        mesh.SetActive(true);
        colliderParede.enabled = true;
        integridade = integridadeMaxima;
    }

}
