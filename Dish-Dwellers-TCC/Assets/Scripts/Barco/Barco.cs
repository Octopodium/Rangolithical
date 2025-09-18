using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Animations;

public class Barco : IResetavel, Interacao
{
    public Transform pos1, pos2, outPos, inicialPos;
    public Vector3 pontoPuxada;
    public bool sendoPuxado = false;
    public float distanciaMinimaParada = 2f;
    public float forcaPuxada = 10000f;
    private Rigidbody rb;
    public ParentConstraint parentConstraint;
    private Player jogadorEmbarcado1, jogadorEmbarcado2;
    private bool jogadorEstaEmbarcado = false;

    public void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 1f;
        rb.angularDamping = 1f;
    }

    /*void Update(){
        if (jogadorEstaEmbarcado && jogadorEmbarcado != null && jogadorEmbarcado.ehJogadorAtual){
            if (jogadorEmbarcado.playerInput != null && 
                jogadorEmbarcado.playerInput.currentActionMap["Interact"].WasPressedThisFrame()){
                SairDoBarco();
            }
        }
    }*/

    public void Interagir(Player jogador)
    {
        if(jogador.embarcado){
            SairDoBarco();
            jogador.embarcado = false;
        }
        parentConstraint = jogador.gameObject.GetComponent<ParentConstraint>();
        ConstraintSource posSource;
        if(jogador.personagem == QualPersonagem.Angler){
            jogadorEmbarcado1 = jogador;
            posSource = new ConstraintSource {sourceTransform = pos1, weight = 1f};
        }else{
            jogadorEmbarcado2 = jogador;
            posSource = new ConstraintSource {sourceTransform = pos2, weight = 1f};
        }

        parentConstraint.AddSource(posSource);
        parentConstraint.constraintActive = true;
        jogador.embarcado = true;
        jogador. velocidade = 0f;
        jogador.velocidadeRB = 0f;
    }

    public void IniciarPuxada(Vector3 pontoGancho){
        sendoPuxado = true;
        pontoPuxada = pontoGancho;

        StartCoroutine(PuxarBarco());
    }

    public void PararPuxada(){
        sendoPuxado = false;
    }

    public IEnumerator PuxarBarco(){
        Vector3 direcaoPuxada = (pontoPuxada - transform.position).normalized;

        float distancia = Vector3.Distance(transform.position, pontoPuxada);

        if(distancia > distanciaMinimaParada){
            rb.AddForce(direcaoPuxada * forcaPuxada * Time.deltaTime, ForceMode.Force);
        }else{
            rb.linearVelocity = Vector3.zero;
            PararPuxada();
        }
        yield return null;
    }

    public void SairDoBarco(){
        if(jogadorEmbarcado1 != null){
            ParentConstraint constraint = jogadorEmbarcado1.GetComponent<ParentConstraint>();
            if (constraint != null) {
                constraint.RemoveSource(0);
                constraint.constraintActive = false;
            }
            
            jogadorEmbarcado1.Teletransportar(outPos);

            jogadorEmbarcado1.velocidade = 14f;
            jogadorEmbarcado1.velocidadeRB = 14f;
        }

        if(jogadorEmbarcado2 != null){
            ParentConstraint constraint = jogadorEmbarcado2.GetComponent<ParentConstraint>();
            if (constraint != null) {
                constraint.RemoveSource(0);
                constraint.constraintActive = false;
            }
            
            jogadorEmbarcado2.Teletransportar(outPos.position - new Vector3(2, 0, 0));

            jogadorEmbarcado2.velocidade = 14f;
            jogadorEmbarcado2.velocidadeRB = 14f;
        }
    }

    public override void OnReset(){
        transform.position = inicialPos.position;
        jogadorEmbarcado1 = null;
        jogadorEmbarcado2 = null;
        sendoPuxado = false;
    }

}
