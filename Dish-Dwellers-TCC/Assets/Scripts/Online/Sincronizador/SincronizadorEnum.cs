using UnityEngine;
using System;
using System.Collections.Generic;

public partial struct ValorGenerico {
    public string CodificadorCustomQualPersonagem(QualPersonagem personagem) {
        return personagem.ToString();
    }

    public ValorGenerico DecodificadorCustomQualPersonagem(string personagem) {
        if (Enum.TryParse(personagem, out QualPersonagem resultado)) {
            return new ValorGenerico(typeof(QualPersonagem), resultado);
        } else {
            Debug.LogError("QualPersonagem inválido: " + personagem);
            return new ValorGenerico();
        }
    }

    public string CodificadorCustomFonteDano(AnimadorPlayer.fonteDeDano fonte) {
        return fonte.ToString();
    }

    public ValorGenerico DecodificadorCustomFonteDano(string fonte) {
        if (Enum.TryParse(fonte, out AnimadorPlayer.fonteDeDano resultado)) {
            return new ValorGenerico(typeof(AnimadorPlayer.fonteDeDano), resultado);
        } else {
            Debug.LogError("FonteDeDano inválido: " + fonte);
            return new ValorGenerico();
        }
    }
}