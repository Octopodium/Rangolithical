using UnityEngine;

public class AnimatorPerseguidor : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private Transform meshPerseguidor;

    #region Parâmetros do animator

    public static readonly int persegue = Animator.StringToHash("Persegue");
    public static readonly int deCabecaPraBaixo = Animator.StringToHash("DeCabecaPraBaixo");
    public static readonly int atacar = Animator.StringToHash("Ataca");

    //public static readonly int patrulha = Animator.StringToHash("Patrulha");

    //public static readonly int morre = Animator.StringToHash("Morre");

    #endregion

    float orientacao = 1f;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void Persegue(bool val) {
        animator.SetBool(persegue, val);
    }

    public void Ataca(bool val) {
        animator.SetBool(atacar, val);
        Persegue(false);
    }

    // public void Patrulha(bool val) {
    //     animator.SetBool(patrulha, val);
    // }

    public void Olhar(Vector3 dirAlvo) {
        Vector3 escalaX = transform.localScale;

        if (dirAlvo.x != 0) {
            orientacao = dirAlvo.x > 0 ? 1 : -1 ;
            escalaX.x = orientacao;
        }

        transform.localScale = escalaX;
    }

    bool taVirado = false;

    public void VirarDeCabecaPraBaixo() {
        if (taVirado) return;
        Debug.Log("Virando inimigo de cabeça pra baixo!");

        Vector3 escalaY = meshPerseguidor.localScale;
        escalaY.y = -1;
        meshPerseguidor.localScale = escalaY;

        Vector3 posY = meshPerseguidor.transform.position;
        posY.y += 2;
        meshPerseguidor.transform.position = posY;

        taVirado = true;
    }

    public void Desvirar() {
        if (!taVirado) return;

        Debug.Log("Desvirou Lindo DEMAISSS!!!");
        Vector3 escalaY = meshPerseguidor.localScale;
        escalaY.y = 1;
        meshPerseguidor.localScale = escalaY;

        Vector3 posY = meshPerseguidor.transform.position;
        posY.y -= 2;
        meshPerseguidor.transform.position = posY;

        taVirado = false;
    }
}
