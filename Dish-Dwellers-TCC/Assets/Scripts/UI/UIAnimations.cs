using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler{
    Button buttonText;
    Vector3 scaleFactor = new Vector3(1.1f, 1.1f, 1.1f);
    Vector3 defaultFactor = new Vector3(1f, 1f, 1f);

    void Awake(){
        buttonText = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData){
        HandleAnimation(0.1f, scaleFactor);
    }

    public void OnDeselect(BaseEventData eventData){
        HandleAnimation(0.1f, defaultFactor);
    }

    public void OnPointerEnter(PointerEventData eventData){
        HandleAnimation(0.1f, scaleFactor);
    }

    public void OnPointerExit(PointerEventData eventData){
        HandleAnimation(0.1f, defaultFactor);
    }
     
    IEnumerator ScaleText(float duration, Vector3 endScale){
        float tempo = 0f;
        while (tempo < duration){
            float t = tempo / duration;
            buttonText.gameObject.transform.localScale = Vector3.Lerp(defaultFactor, endScale, t);
            tempo += Time.deltaTime;
            yield return null;
        }
        buttonText.gameObject.transform.localScale = endScale;
    }

    public void HandleAnimation(float duration, Vector3 endScale){ //momentaneo, vi um video pra nao ter quer fazer isso
        if (buttonText != null){
            StartCoroutine(ScaleText(duration, endScale));
        }
    }
}
