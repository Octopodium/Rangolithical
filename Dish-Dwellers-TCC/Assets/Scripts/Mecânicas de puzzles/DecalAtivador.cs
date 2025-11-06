using UnityEngine;
using System;

public class DecalAtivador : MonoBehaviour
{

    [SerializeField] private Renderer[] decalques;
    [SerializeField][ColorUsage(true, true)] private Color corAtivado, corDesativado;
    private MaterialPropertyBlock materialPropertyBlock;


    private void Awake() {
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void AtivarDecalques() => MudarCorDosDecalques(corAtivado);
    public void DesativarDecalques() => MudarCorDosDecalques(corDesativado);

    private void MudarCorDosDecalques(Color col) {
        if(decalques == null) throw new Exception("Decalques não estão atribuidos corretamente ao ativador.");
        materialPropertyBlock.SetColor("_EmissionColor", col);
        foreach(Renderer renderer in decalques) {
            renderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

}
