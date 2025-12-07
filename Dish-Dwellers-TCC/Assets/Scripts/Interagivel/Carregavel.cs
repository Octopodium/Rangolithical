using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations;

public enum Peso { Leve=0, Pesado=1 }

[RequireComponent(typeof(Rigidbody))]
public class Carregavel : MonoBehaviour, InteracaoCondicional {
    public Peso peso = Peso.Leve;

    public UnityEvent onCarregado, onSolto;
    public System.Action<Carregador> OnCarregado, OnSolto; // Chamado quando o carregador carrega ou solta um objeto

    Rigidbody rb;
    Collider col;
    bool _sendoCarregado = false;
    public bool sendoCarregado => _sendoCarregado;
    public Carregador carregador { get; private set; } // O carregador que está carregando o objeto, se houver
    [HideInInspector] public Grudavel grudavel;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        grudavel = gameObject.GetComponent<Grudavel>();
        if (grudavel == null) grudavel = gameObject.AddComponent<Grudavel>();
    }

    void OnDisable() {
        carregador?.Soltar();
    }

    /// <summary>
    /// Condições para que o jogador possa interagir com o objeto. 
    /// Se o jogador estiver carregando outro objeto, não poderá interagir com este.
    /// Se o objeto já estiver sendo carregado, não poderá interagir com ele.
    /// </summary>
    /// <param name="jogador">Jogador que interagiu</param>
    /// <returns>Positivo se pode ser interagido</returns>
    public bool PodeInteragir(Player jogador) {
        return PodeInteragir(jogador.carregador);
    }

    /// <summary>
    /// Motivos pelo qual o jogador não pode interagir, chamado após PodeInteragir retornar true, checa condições que serão informadas para o jogador pelo Indicador.
    /// </summary>
    /// <param name="jogador">Jogador que interagiu</param>
    /// <returns>Se o jogador pode interagir, retorna MotivoNaoInteracao.Nenhum, caso contrário, retorna o motivo pelo qual o jogador não pode interagir com o Interagivel.</returns>
    public MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        return NaoPodeInteragirPois(jogador.carregador);
    }

    /// <summary>
    /// Condições para que o jogador possa interagir com o objeto. 
    /// Se o jogador estiver carregando outro objeto, não poderá interagir com este.
    /// Se o objeto já estiver sendo carregado, não poderá interagir com ele.
    /// </summary>
    /// <param name="carregador">Carregador que interagiu</param>
    /// <returns>Positivo se pode ser interagido</returns>
    public bool PodeInteragir(Carregador carregador) {
        return !carregador.estaCarregando && !_sendoCarregado;
    }

    /// <summary>
    /// Motivos pelo qual o carregador não pode interagir, chamado após PodeInteragir retornar true, checa condições que serão informadas para o carregador pelo Indicador.
    /// </summary>
    /// <param name="carregador">Carregador que interagiu</param>
    /// <returns>Se o carregador pode interagir, retorna MotivoNaoInteracao.Nenhum, caso contrário, retorna o motivo pelo qual o carregador não pode interagir com o Interagivel.</returns>
    public MotivoNaoInteracao NaoPodeInteragirPois(Carregador carregador) {
        if (carregador.aguentaCarregar < peso) return MotivoNaoInteracao.Fraco;
        return MotivoNaoInteracao.Nenhum;
    }

    /// <summary>
    /// Chamado quando algum jogador tenta interagir com o objeto.
    /// </summary>
    /// <param name="jogador">Jogador que interagiu</param>
    public void Interagir(Player jogador) {
        Carregar(jogador.carregador);
    }

    /// <summary>
    /// Carrega o objeto com o carregador passado como parâmetro. Se o objeto já estiver sendo carregado, não faz nada.
    /// </summary>
    /// <param name="carregador">Carregador que irá carregar o objeto</param>
    public void Carregar(Carregador carregador) {
        if (!PodeInteragir(carregador) || NaoPodeInteragirPois(carregador) != MotivoNaoInteracao.Nenhum) return;
        if (carregador.carregado != this && !carregador.Carregar(this)) return; // Se não conseguiu carregar, não faz nada

        this.carregador = carregador;

        onCarregado.Invoke();
        OnCarregado?.Invoke(carregador);
    }

    bool tinhaGravidade = false;

    /// <summary>
    /// Chamado automaticamente pelo Carregador quando o objeto é carregado
    /// </summary>
    public void HandleSendoCarregado() {
        _sendoCarregado = true;

        if (!rb.isKinematic) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        rb.isKinematic = true;
        tinhaGravidade = rb.useGravity;
        rb.useGravity = false; // Desabilita a gravidade enquanto o objeto estiver sendo carregado
    }

    /// <summary>
    /// Chamado automaticamente pelo Carregador quando o objeto é solto
    /// </summary>
    public void HandleSolto() {
        onSolto.Invoke();
        OnSolto?.Invoke(carregador);

        _sendoCarregado = false;
        rb.isKinematic = false;
        rb.useGravity = tinhaGravidade; // Restaura a gravidade
        this.carregador = null;
    }

    public bool EstaAcimaDe(Vector3 outro) {
        return (col.bounds.center.y - col.bounds.size.y / 2f) > outro.y;
    }
}
