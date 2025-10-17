using UnityEngine;

public partial struct ValorGenerico {
    public string CodificadorCustomIndicadorSeletor(IndicadorSeletor indicador) {
        if (indicador == null) return "";
        return CodificadorCustomGameObject(indicador.gameObject);
    }

    public ValorGenerico DecodificadorCustomIndicadorSeletor(string id) {
        ValorGenerico val = DecodificadorCustomGameObject(id);
        if (!val.IsValid) return val;

        GameObject go = val.valor as GameObject;
        return new ValorGenerico(typeof(IndicadorSeletor), go.GetComponent<IndicadorSeletor>());
    }
}