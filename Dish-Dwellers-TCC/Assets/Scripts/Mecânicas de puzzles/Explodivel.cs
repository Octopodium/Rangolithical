using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Explodivel : MonoBehaviour{

    [SerializeField] private int integridade;
    private Collider colliderExplodivel;
    private int integridadeMaxima;
    public UnityEvent OnExplode;
    

    private void Awake(){
        colliderExplodivel = GetComponent<Collider>();
        integridadeMaxima = integridade;
    }

    private void OnEnable(){
        //Quando for reativado
        integridade = integridadeMaxima;
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
            OnExplode?.Invoke();
            //colliderExplodivel.enabled = false;      
        }
    }

}
