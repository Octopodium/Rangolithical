using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Barco : MonoBehaviour, Interacao
{
    public Transform pos1, pos2;
    public Vector3 pontoPuxada;
    public bool sendoPuxado = false;
    public float distanciaMinimaParada = 1f;
    public float forcaPuxada = 500f;
    private Rigidbody rb;

    public void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 1f;
        rb.angularDamping = 1f;
    }

    public void Interagir(Player jogador){
        if(jogador.personagem == QualPersonagem.Angler){
            jogador.transform.position = pos1.position;
        }else{
            jogador.transform.position = pos2.position;
        }

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
