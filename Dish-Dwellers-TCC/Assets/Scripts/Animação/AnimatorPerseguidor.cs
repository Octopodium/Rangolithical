using UnityEngine;

public class AnimatorPerseguidor : MonoBehaviour {
    private Animator animator;

    #region Parâmetros do animator

    public static readonly int persegue = Animator.StringToHash("Persegue");
    public static readonly int patrulha = Animator.StringToHash("Patrulha");

    public static readonly int deCabecaPraBaixo = Animator.StringToHash("DeCabecaPraBaixo");

    //public static readonly int morre = Animator.StringToHash("Morre");

    #endregion

    float orientacao = 1f;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void Persegue(bool val) {
        animator.SetBool(persegue, val);
    }

    public void Patrulha(bool val) {
        animator.SetBool(patrulha, val);
    }

    public void DeCabecaPraBaixo(bool val) {
        animator.SetBool(deCabecaPraBaixo, val);
    }
    
    public void Olhar(Vector3 dirAlvo){
        Vector3 escala = transform.localScale;

        if(dirAlvo.x != 0){     
            orientacao = dirAlvo.x > 0 ? -1 : 1;
            escala.x = orientacao;
        }

        transform.localScale = escala;
    }
}
