using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class AnimadorPlayer : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;

    #region Coisas de Áudio...
    private AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;  

    #endregion 

    #region Parâmetros do animator
    public static readonly int Anda = Animator.StringToHash(nameof(Anda));
    public static readonly int Caindo = Animator.StringToHash(nameof(Caindo));
    public static readonly int Carrega = Animator.StringToHash(nameof(Carrega));
    public static readonly int Arremesso = Animator.StringToHash(nameof(Arremesso));
    // public static readonly int Morre = Animator.StringToHash(nameof(Morre));
    public static readonly int Dano = Animator.StringToHash(nameof(Dano));
    public static readonly int Escudo = Animator.StringToHash(nameof(Escudo));
    public static readonly int JogarGancho = Animator.StringToHash(nameof(JogarGancho));
    private Quaternion deFrente = Quaternion.Euler(-15, 180, 0), deCostas = Quaternion.Euler(15, 0, 0);
    private float orientacao = 1;

    #region Tags de Morte

    public enum fonteDeDano {FOGO, AFOGADO, PORRADA};
    public static readonly int MorreFogo = Animator.StringToHash(nameof(MorreFogo));
    [SerializeField] private AnimationClip morreFogo;
    public static readonly int MorreAfoga = Animator.StringToHash(nameof(MorreAfoga));
    [SerializeField] private AnimationClip morreAfoga;
    public static readonly int MorrePorrada = Animator.StringToHash(nameof(MorrePorrada));
    [SerializeField] private AnimationClip morrePorrada;
    
    #endregion
     

    #endregion

    private void Start(){
        animator = GetComponent<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

    }

    #region Métodos de animação

    /// <summary>
    /// Atribui a animação de andar correta ao jogador, baseada no Vetor de velocidade.
    /// </summary>
    /// <param name="velocidade"></param>
    public void Mover(Vector3 velocidade){
        
        if(velocidade.z > 0){// Vira de costas
            transform.localRotation = deCostas;
        }
        else if(velocidade.z < 0){ // Vira para a frente
            transform.localRotation = deFrente;
        }

        // Quando o jogador vira de costas, como a rotação ta em 180, é preciso inverter pra qual direção ele vira.
        int rotacao = transform.localRotation == deFrente ? 1 : -1;

        // Virar o jogador pra direita ou pra esquerda
        Vector3 escala = transform.localScale;
        if(velocidade.x != 0) orientacao = velocidade.x;

        if(orientacao != 0){
            if(orientacao > 0){ // Vira para a esquerda
                escala.x = rotacao;
            }
            else if(orientacao < 0){// Vira para a direita
                escala.x = -rotacao;
            }

            transform.localScale = escala;
        }
        
        // Coloca a animação de andar no animator.
        animator.SetBool(Anda, velocidade.sqrMagnitude > 0);
    }

    /// <summary>
    /// Atribui a animação de andar correta ao jogador, baseada no Vetor de velocidade.
    /// </summary>
    /// <param name="velocidade"></param>
    public void Mover(Vector2 velocidade){
        Mover(new Vector3(velocidade.x, velocidade.y));
    }

    /// <summary>
    /// Toca a animação de carregar objeto do personagem. Permanece na pose de carregar enquanto AtirarObjeto não for chamado.
    /// </summary>
    public void Carregar(){
        animator.SetTrigger(Carrega);
    }

    /// <summary>
    /// Toca a animação de arremesso do personagem.
    /// </summary>
    public void AtirarObjeto(){
        animator.SetTrigger(Arremesso);
    }

    /// <summary>
    /// Toca a animação de morte do personagem.
    /// </summary>
    public float Morte(fonteDeDano fonte){
        switch(fonte) {
            case fonteDeDano.FOGO:
                animator.SetTrigger(MorreFogo);
                return morreFogo.length;
            case fonteDeDano.AFOGADO:
                animator.SetTrigger(MorreAfoga);
                return morreAfoga.length;
            case fonteDeDano.PORRADA:
                animator.SetTrigger(MorrePorrada);
                return morrePorrada.length;
        }
        Debug.Log($"<color=yellow>Fonte de morte não implementada!<color>");   
        return 1;
    }

    /// <summary>
    /// Toca a animação de receber dano do personagem.
    /// </summary>
    public void TomarDano(){
        animator.SetTrigger(Dano);
    }

    #endregion

    #region Heater

    public void AtivarEscudo(bool value) => animator.SetBool(Escudo, value);

    public float AtirarGancho() {
        animator.SetTrigger(JogarGancho);
        return animatorOverrideController["Angler_Hook"].length;
    }

    #endregion

    public void AfogaSfx() {
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }

    public void QueimaSfx() {
        audioSource.clip = audioClips[1];
        audioSource.Play();
    }

    public void ResetAnimador() {
        animator.Play("Parado", -1, 0);
        AtivarEscudo(false);
        animator.SetBool(Anda, false);
    }

}