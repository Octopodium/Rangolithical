using UnityEngine;

public partial struct ValorGenerico {
    public string CodificadorCustomGameObject(GameObject obj) {
        if (obj == null) return "";

        Sincronizavel sincronizavel = obj.GetComponent<Sincronizavel>();
        if (sincronizavel == null || sincronizavel.GetID().Trim() == "") {

            SubSincronizavel sub = obj.GetComponent<SubSincronizavel>();
            if (sub == null || sub.dono == null || sub.dono.GetID().Trim() == "") {
                Debug.LogError("Para sincronizar um parâmetro <GameObject>, é necessário que este possua o componente <Sincronizavel> com um id único.");
                return "";
            }
            return sub.GetIdentificador();
        }

        return sincronizavel.GetID();
    }

    public ValorGenerico DecodificadorCustomGameObject(string id) {
        Sincronizavel sincronizavel = Sincronizador.instance.GetSincronizavel(id);

        if (sincronizavel == null) {
            SubSincronizavel sub = Sincronizador.instance.GetSubSincronizavel(id);
            if (sub == null) {
                Debug.LogError("Sincronizavel não encontrado com ID: " + id);
                return new ValorGenerico();
            }

            return new ValorGenerico(typeof(GameObject), sub.gameObject);
        }
        else {
            return new ValorGenerico(typeof(GameObject), sincronizavel.gameObject);
        }
    }
}