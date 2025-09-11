using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Animations;

public class Barco : MonoBehaviour, Interacao
{
    public Transform pos1, pos2;
    public Vector3 pontoPuxada;
    public bool sendoPuxado = false;
    public float distanciaMinimaParada = 1f;
    public float forcaPuxada = 5000f;
    private Rigidbody rb;
    public ParentConstraint parentConstraint;

    public void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 1f;
        rb.angularDamping = 1f;
    }

    public void Interagir(Player jogador){
        parentConstraint = jogador.gameObject.GetComponent<ParentConstraint>();
        ConstraintSource posSource;
        if(jogador.personagem == QualPersonagem.Angler){
            posSource = new ConstraintSource {sourceTransform = pos1, weight = 1f};
        }else{
            posSource = new ConstraintSource {sourceTransform = pos2, weight = 1f};
        }

        parentConstraint.AddSource(posSource);
        parentConstraint.constraintActive = true;

        jogador.velocidade = 0f;
        jogador.velocidadeRB = 0f;
        //desabilita o indicador
        //desabilita animação
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

}
