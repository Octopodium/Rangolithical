using UnityEngine;
using TMPro;

public class IndicadorQuantidade : MonoBehaviour
{
    public TMP_Text quantidadeCenoura;
    public TMP_Text quantidadeRoma;
    public IngredienteData cenoura, roma;

    public void Start(){
        UpdateQuantidade();
    }

    public void UpdateQuantidade(){
        quantidadeCenoura.text = ColecionavelController.instance.GetQuantidadePossuidaDisponivel(cenoura).ToString();
        quantidadeRoma.text = ColecionavelController.instance.GetQuantidadePossuidaDisponivel(roma).ToString();
    }

}
