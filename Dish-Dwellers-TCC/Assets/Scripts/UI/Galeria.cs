using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Galeria : MonoBehaviour {
    public TMP_Text batataQuant, cenouraQuant;
    public GameObject[] desativar;
    public GameObject arte; 
    public bool comprado = false;

    //TUDO nesse código é temporario, funciona so pra essa build, estarei arrumando tudo nessa semana

    public void Comprar(IngredienteData ingrediente){ 
        if(ColecionavelController.instance.TemDisponivel(ingrediente) && !comprado){
            foreach(GameObject obj in desativar){
                obj.SetActive(false);
            }
            arte.transform.localScale += new Vector3(1.2f,1.2f,0);
            comprado = true;
        }else{
            Debug.Log("insuficiente");
        }
    }
   
}
