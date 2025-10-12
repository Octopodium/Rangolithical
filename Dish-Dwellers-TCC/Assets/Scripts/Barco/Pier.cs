using UnityEngine;

[RequireComponent(typeof(OnTrigger))]
public class Pier : MonoBehaviour {
    public OnTrigger onTrigger;
    public Transform outPos;

    void Start() {
        onTrigger.onTriggerEnterAction += BarcoEnter;
        onTrigger.onTriggerExitAction += BarcoExit;
    }

    void BarcoEnter(Collider barcoCol) {
        Barco barco = barcoCol.gameObject.GetComponent<Barco>();
        if (barco == null) return;

        barco.NoPier(true);
        barco.SwitchOutPos(outPos);
    }

    void BarcoExit(Collider barcoCol) {
        Barco barco = barcoCol.gameObject.GetComponent<Barco>();
        if (barco == null) return;
        barco.NoPier(false);
    }
}
