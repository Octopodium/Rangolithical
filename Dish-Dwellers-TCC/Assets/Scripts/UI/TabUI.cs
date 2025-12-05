using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TabUI : MonoBehaviour, ISelectHandler, IDeselectHandler 
{
    public GameObject[] tabs;
    public Button botao;
    
    Vector3 selectedOffset = new Vector3(0f, 20f, 0f);
    Vector3[] normalPositions;

    void Start() 
    {
        botao = GetComponent<Button>();
        
        if (tabs != null && tabs.Length > 0){
            normalPositions = new Vector3[tabs.Length];
            for (int i = 0; i < tabs.Length; i++){
                if (tabs[i] != null){
                    normalPositions[i] = tabs[i].transform.position;
                }
            }
        }
    }

    public void OnSelect(BaseEventData eventData) {
        if (botao.interactable){
            ResetTodas();
            ElevarTab();
        }
    }

    public void OnDeselect(BaseEventData eventData) {
    }

    void ResetTodas(){
        if (tabs == null || normalPositions == null) return;
        
        for (int i = 0; i < tabs.Length; i++){
            if (tabs[i] != null && i < normalPositions.Length){
                tabs[i].transform.position = normalPositions[i];
            }
        }
    }
    
    void ElevarTab(){
        int index = -1;
        for (int i = 0; i < tabs.Length; i++){
            if (tabs[i] == this.gameObject){
                index = i;
                break;
            }
        }
        
        if (index >= 0 && index < normalPositions.Length){
            this.transform.position = normalPositions[index] + selectedOffset;
        }else{
            this.transform.position += selectedOffset;
        }
    }
    
    public void ForcarSelecaoTab(){
        if (botao.interactable){
            ResetTodas();
            ElevarTab();
            botao.Select();
        }
    }
}