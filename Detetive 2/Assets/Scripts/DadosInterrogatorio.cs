
using System.Collections.Generic;

public static class DadosInterrogatorio
{
    public static List<string> anotacoes = new List<string>();

    public static void Limpar()
    {
        anotacoes.Clear();
    }

    public static void Adicionar(string frase)
    {
        if (!anotacoes.Contains(frase))
        {
            anotacoes.Add(frase);
        }
    }
}
