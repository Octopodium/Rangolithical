using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ParedeDeVinhas : IResetavel {
    [SerializeField] private int integridade = 3;
    [SerializeField] private Color[] cores;
    private Renderer[] renderers;
    private Collider col;
    private MaterialPropertyBlock mpb;
    [Header("Configurações da animação de queimar:")]
    [SerializeField] private float velocidadeDeQueima = 1.0f;
    [SerializeField] private float raioMaximo = 10.0f;
    private readonly int pointOfContactID = Shader.PropertyToID("_PointOfContact");
    private readonly int radiusID = Shader.PropertyToID("_Radius");

    private void Awake() {
        mpb = new MaterialPropertyBlock();
        renderers = GetComponentsInChildren<Renderer>();
        col = GetComponent<Collider>();
    }

    public override void OnReset() {
        StopAllCoroutines();
        mpb.SetFloat(radiusID, 0.0f);
        foreach(Renderer render in renderers) {
            render.SetPropertyBlock(mpb);
        }
        col.enabled = true;
    }

    public void ReduzirIntegridade(Vector3 pontoDeContato) {
        if (--integridade <= 0) {
            //AtivarVinhas(false);
            mpb.SetVector(pointOfContactID, pontoDeContato);
            foreach(Renderer render in renderers) {
                render.SetPropertyBlock(mpb);
            }
            StartCoroutine(QueimarVinhas());
            AudioManager.PlaySounds(TiposDeSons.VINESBURNING);
            return;
        }
    }

    private void SetarCor(int integridade) {
        if (integridade <= 0) return;
        
        mpb.SetColor("_BaseColor", cores[integridade - 1]);

        foreach (Renderer render in renderers) {
            render.SetPropertyBlock(mpb);
        }
    }

    IEnumerator QueimarVinhas() {
        col.enabled = false;
        float raio = 0.0f;
        while(raio < raioMaximo) {
            raio += Time.fixedDeltaTime * velocidadeDeQueima;
            mpb.SetFloat(radiusID, raio);
            foreach(Renderer render in renderers) {
                render.SetPropertyBlock(mpb);
            }
            yield return new WaitForFixedUpdate();
        }
    }

}
