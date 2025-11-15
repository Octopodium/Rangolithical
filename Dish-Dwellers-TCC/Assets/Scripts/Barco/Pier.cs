using UnityEngine;

[RequireComponent(typeof(OnTrigger))]
public class Pier : MonoBehaviour {
    public OnTrigger onTrigger;
    public Transform outPos;
    public Transform indicadorSaidaPos;

    void Start() {
        onTrigger.onTriggerEnterAction += BarcoEnter;
        onTrigger.onTriggerExitAction += BarcoExit;
    }

    void BarcoEnter(Collider barcoCol) {
        Barco barco = barcoCol.gameObject.GetComponent<Barco>();
        if (barco == null) return;

        barco.SetPier(this);
    }

    void BarcoExit(Collider barcoCol) {
        Barco barco = barcoCol.gameObject.GetComponent<Barco>();
        if (barco == null) return;
        if (barco.pier == this)
            barco.SetPier(null);
    }
}
