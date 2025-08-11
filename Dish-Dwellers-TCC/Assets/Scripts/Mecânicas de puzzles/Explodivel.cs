using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Explodivel : IResetavel{

    [SerializeField] private int integridade;
    private int integridadeMaxima;
    [SerializeField] private GameObject mesh;
    private Collider colliderParede;
    

    private void Awake(){
        colliderParede = GetComponent<Collider>();
        integridadeMaxima = integridade;
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
