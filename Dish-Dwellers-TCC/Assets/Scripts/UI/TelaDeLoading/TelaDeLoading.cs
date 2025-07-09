using UnityEngine;

public class TelaDeLoading : MonoBehaviour{

    [SerializeField] private Animator backgroundAnimator;
    public float tempoDeTransicao = 1.0f;
    private readonly int carregandoParameter = Animator.StringToHash("Carregando");


    public float GetTempoDeTransicao(){
        return backgroundAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void AtivarTelaDeCarregamento(bool carregando){
        backgroundAnimator.SetBool(carregandoParameter, carregando);
    }

}
