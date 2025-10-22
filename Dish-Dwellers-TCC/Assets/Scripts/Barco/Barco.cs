using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Animations;
using Unity.VisualScripting;
using UnityEngine.Events;

public class Barco : IResetavel, Interacao, IRecebeTemplate
{
    public Transform pos1, pos2, outPos, inicialPos;
    public Vector3 pontoPuxada;
    public bool sendoPuxado = false;
    public float distanciaMinimaParada = 3f;
    public float forcaPuxada = 10000f;
    private Rigidbody rb;
    public ParentConstraint parentConstraint;
    public bool noPier = false;
    public int playerNoBarco = 0;
    public Player angler, heater;

    public void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 1f;
        rb.angularDamping = 1f;
    }

    public void RecebeTemplate(GameObject template) {
        if (template == null) return;
        Barco barcoTemplate = template.GetComponent<Barco>();
        if (barcoTemplate == null) return;

        outPos = barcoTemplate.outPos;
        inicialPos = barcoTemplate.inicialPos;
    }

    public void Interagir(Player jogador)
    {
        if(noPier){
            if(jogador.embarcado){
                SairDoBarco(jogador);
                jogador.barcoEmbarcado = null;
                jogador.HandleEmbarcado(); 
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
            playerNoBarco++;

            jogador.barcoEmbarcado = this;
            jogador.HandleEmbarcado(); 
        }
    }


    public void IniciarPuxada(Vector3 pontoGancho){
        if(angler == null) return;
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
            rb.AddForce(direcaoPuxada * forcaPuxada, ForceMode.Force);
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

            if(jogador.personagem == QualPersonagem.Angler){
                angler = null;
            }else{
                heater = null;
            }

            playerNoBarco--;
        }
    }

    public void NoPier(bool status){
        noPier = status;
    }

    public void SwitchOutPos(Transform outPosition){
        outPos = outPosition;
    }

    public void AplicarForcaVapor(Vector3 direcaoEmpurrada){
        if (heater != null) {
            if (heater.ferramenta.acionada) {
                if (Vector3.Dot(heater.visualizarDirecao.transform.forward, direcaoEmpurrada) < -0.3f) {
                    //Debug.Log("tentando forca escudo");
                    rb.AddForce(direcaoEmpurrada * 100 * Time.deltaTime, ForceMode.Force);
                }
            }
        }
    }

    public override void OnReset(){
        transform.position = inicialPos.position;
        transform.rotation = Quaternion.identity;
        sendoPuxado = false;
        playerNoBarco = 0;
        angler = null;
        heater = null;
    }

}
