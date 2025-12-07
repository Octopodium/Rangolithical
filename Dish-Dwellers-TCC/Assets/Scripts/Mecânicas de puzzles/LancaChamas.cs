using UnityEngine;
using UnityEngine.VFX;

public class LancaChamas : MonoBehaviour
{
    [SerializeField] private float maxComprimento = 6.0f;
    [SerializeField] private LayerMask layers;
    [SerializeField] private VisualEffect visualEffect;
    private Ray ray;
    private Ray[] rays = new Ray[3];
    private RaycastHit hitInfo;

    #region propriedades do VFX

    private static readonly int boxCenterID = Shader.PropertyToID("Box_Center");
    private static readonly int boxAnglesID = Shader.PropertyToID("Box_Angles");
    private static readonly int boxSizeID = Shader.PropertyToID("Box_Size");

    [SerializeField] private Vector3 boxSize;

    #endregion


    private void FixedUpdate(){
        CastColisao(transform.position, transform.forward, maxComprimento, 0);   
    }

    private void CastColisao(Vector3 origem, Vector3 direcao, float distancia, int index){
        if(index>0) Debug.Log("Refletiu");
        if(index > 1) return;
        rays[index] = new Ray(origem , direcao);

        if(Physics.SphereCast(origem, 0.35f, direcao, out hitInfo, distancia,  layers)){
            if(hitInfo.transform.CompareTag("Player")){
                hitInfo.transform.GetComponent<Player>().MudarVida(-999, AnimadorPlayer.fonteDeDano.FOGO);
            }
            if(index == 0) SetColisaoComParticula(hitInfo);
            CastColisao(hitInfo.point, Vector3.Reflect(direcao, hitInfo.collider.transform.forward), distancia - hitInfo.distance, index + 1);
        }
        else{
            if(index == 0)SetSemColisaoComParticula();
        }
    }

    private void SetSemColisaoComParticula(){
        visualEffect.SetVector3(boxCenterID, Vector3.negativeInfinity);
        visualEffect.SetVector3(boxSizeID, Vector3.zero);
    }

    private void SetColisaoComParticula(RaycastHit hitInfo){
        Vector3 boxCenter = hitInfo.collider.bounds.center;
        Vector3 boxAngles = hitInfo.collider.transform.eulerAngles;
        Vector3 boxSize;
        BoxCollider boxCollider = hitInfo.collider.GetComponent<BoxCollider>();
        if(boxCollider != null){
            Vector3 scale = boxCollider.transform.localScale;
            Vector3 colSize = boxCollider.size;
            if(colSize == Vector3.one) boxSize = new Vector3(colSize.x * scale.x, colSize.y * scale.y, colSize.z * scale.z );
            else boxSize = colSize;
        }
        else boxSize = hitInfo.collider.bounds.extents * 2; 
        visualEffect.SetVector3(boxCenterID, boxCenter);
        visualEffect.SetVector3(boxAnglesID, boxAngles);
        visualEffect.SetVector3(boxSizeID, boxSize);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach(Ray ray in rays) {
            Gizmos.DrawRay(ray);
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        float distance = hitInfo.distance;
        Gizmos.DrawWireCube(Vector3.zero + Vector3.forward * (distance/2 + 0.5f), new Vector3(1, 1, distance + 1));
    
    }

}
