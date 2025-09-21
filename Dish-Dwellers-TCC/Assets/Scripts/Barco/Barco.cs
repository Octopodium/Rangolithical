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
    public bool noPier = false;
    public Player angler, heater;

    public void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 1f;
        rb.angularDamping = 1f;
    }

    public void Interagir(Player jogador)
    {
        if(noPier){
            if(jogador.embarcado){
                SairDoBarco(jogador);
                jogador.embarcado = false;
                return;
            }
            parentConstraint = jogador.gameObject.GetComponent<ParentConstraint>();
            ConstraintSource posSource;
            if(jogador.personagem == QualPersonagem.Angler){
                posSource = new ConstraintSource {sourceTransform = pos1, weight = 1f};
                angler = jogador;
            }else{
                posSource = new ConstraintSource {sourceTransform = pos2, weight = 1f};
                heater = jogador;
            }

            parentConstraint.AddSource(posSource);
            parentConstraint.constraintActive = true;
            jogador.embarcado = true;
            jogador. velocidade = 0f;
            jogador.velocidadeRB = 0f;
        }
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

    public void SairDoBarco(Player jogador){
        if(jogador != null){
            ParentConstraint constraint = jogador.GetComponent<ParentConstraint>();
            if (constraint != null) {
                constraint.RemoveSource(0);
                constraint.constraintActive = false;
            }
            
            jogador.Teletransportar(outPos.position + new Vector3((Random.Range(1f,3f)), 0, 0));

            jogador.velocidade = 14f;
            jogador.velocidadeRB = 14f;
        }
    }

    public void NoPier(bool status){
        noPier = status;
    }

    public void SwitchOutPos(Transform outPosition){
        outPos = outPosition;
    }

    public void AplicarForcaVapor(Vector3 direcaoEmpurrada){
        if(heater.ferramenta.acionada){
            //  if (Vector3.Dot(transform.forward, direcaoEmpurrada) > 0.3f)
            //{
            Debug.Log("tentando forca escudo");
                rb.AddForce(direcaoEmpurrada * 50000 * Time.deltaTime, ForceMode.Force);
            //}
        }
    }

    public override void OnReset(){
        transform.position = inicialPos.position;
        sendoPuxado = false;
    }

}
