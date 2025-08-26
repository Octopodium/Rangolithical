using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Explodivel : IResetavel{

    [SerializeField] private int integridade;
    [SerializeField] private GameObject mesh;
    private Collider colliderParede;
    private int integridadeMaxima;
    

    private void Awake(){
        colliderParede = GetComponent<Collider>();
        integridadeMaxima = integridade;
    }

    private void OnValidate(){
        if(gameObject.tag != "Quebravel"){
            gameObject.tag = "Quebravel";
            Debug.Log($"<color=green>{gameObject.name} tag foi alterada para 'Quebravel' para que funcione corretamente.<color>");
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
