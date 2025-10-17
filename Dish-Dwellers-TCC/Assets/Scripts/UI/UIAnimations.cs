using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler{
    Button buttonText;
    public Image underline;
    public Vector3 scaleFactor = new Vector3(1.1f, 1.1f, 1.1f); //o quanto queremos escalar
    public Vector3 defaultFactor = new Vector3(1f, 1f, 1f); //escala default
    Vector3 initialFactor = new Vector3(1f, 1f, 1f); //guarda o ultimo valor da escala

    void Awake(){
        buttonText = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData){
        HandleAnimation(0.1f, scaleFactor);
        HandleFillUnderline(true);
    }

    public void OnDeselect(BaseEventData eventData){
        HandleAnimation(0.1f, defaultFactor);
        HandleFillUnderline(false);
    }

    public void OnPointerEnter(PointerEventData eventData){
        HandleAnimation(0.1f, scaleFactor);
        HandleFillUnderline(true);
    }

    public void OnPointerExit(PointerEventData eventData){
        HandleAnimation(0.1f, defaultFactor);
        HandleFillUnderline(false);
    }
     
    IEnumerator ScaleText(float duration, Vector3 endScale){
        float tempo = 0f;
        while (tempo < duration){
            float animDurantion = tempo / duration;
            buttonText.gameObject.transform.localScale = Vector3.Lerp(initialFactor, endScale, animDurantion);
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }
        buttonText.gameObject.transform.localScale = endScale;
        initialFactor = endScale;
    }

    IEnumerator FillUnderline(float amount, bool fill){
        if(fill){
            while(underline.fillAmount != 1.0f){
                underline.fillAmount += amount;
                yield return null;
            }
        }else{
            while(underline.fillAmount != 0.0f){
                underline.fillAmount -= amount;
                yield return null;
            }
        }
    }

    public void HandleAnimation(float duration, Vector3 endScale){
        if (buttonText != null){
            StartCoroutine(ScaleText(duration, endScale));
        }
    }

    public void HandleFillUnderline(bool fill){
        if (underline != null){
            StartCoroutine(FillUnderline(0.02f, fill));
        }
    }
}
