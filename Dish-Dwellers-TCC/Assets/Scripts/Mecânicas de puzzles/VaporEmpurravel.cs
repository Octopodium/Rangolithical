using UnityEngine;

public class VaporEmpurravel : MonoBehaviour
{
    [SerializeField] private float maxComprimento = 5.0f;
    [SerializeField] private LayerMask layers;
    private Ray ray;
    private RaycastHit hitInfo;


    private void FixedUpdate(){
        CastColisao();   
    }

    private void CastColisao(){
        ray = new Ray(transform.position, -transform.forward);

        if (Physics.Raycast(transform.position, -transform.forward, out hitInfo, maxComprimento, layers)){
            Debug.Log(hitInfo.collider.name, hitInfo.collider);
            if(hitInfo.transform.CompareTag("Barco")){
                //Debug.Log("barco hitado");
                hitInfo.transform.GetComponent<Barco>().AplicarForcaVapor(-transform.forward);
            }
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, -transform.forward.normalized * maxComprimento);
    }

}
