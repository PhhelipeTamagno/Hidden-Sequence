
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DistribuidorPistas : MonoBehaviour
{
    [Header("Outras provas (NÃO incluir a faca nem as digitais)")]
    public GameObject[] provas;

    [Header("Faca")]
    public GameObject faca;

    [Header("Todos os pontos da casa")]
    public Transform[] pontosSpawn;

    [Header("Pontos permitidos para a faca")]
    public Transform[] pontosFaca;

    [Header("Todas as digitais da casa")]
    public Image[] digitais;

    [Header("Quantidade de digitais por rodada")]
    public int quantidadeDigitais = 5;

    private static Dictionary<string, string> ultimosPontos =
        new Dictionary<string, string>();

    private static List<string> ultimasDigitais =
        new List<string>();

    private void Start()
    {
        DistribuirProvas();
        SortearDigitais();
    }

    public void DistribuirProvas()
    {
        if (faca == null || pontosSpawn == null ||
            pontosFaca == null || provas == null)
        {
            Debug.LogError("Distribuidor: configure todas as referências!");
            return;
        }

        List<Transform> pontosDisponiveis = new List<Transform>();

        foreach (Transform ponto in pontosSpawn)
        {
            if (ponto != null && !pontosDisponiveis.Contains(ponto))
                pontosDisponiveis.Add(ponto);
        }

        List<Transform> pontosValidosFaca = new List<Transform>();

        foreach (Transform ponto in pontosFaca)
        {
            if (ponto != null &&
                !pontosValidosFaca.Contains(ponto) &&
                pontosDisponiveis.Contains(ponto))
            {
                pontosValidosFaca.Add(ponto);
            }
        }

        List<GameObject> provasValidas = new List<GameObject>();

        foreach (GameObject prova in provas)
        {
            if (prova != null &&
                prova != faca &&
                !provasValidas.Contains(prova))
            {
                provasValidas.Add(prova);
            }
        }

        if (pontosValidosFaca.Count == 0)
        {
            Debug.LogError("Não existem pontos válidos para a faca!");
            return;
        }

        if (pontosDisponiveis.Count < provasValidas.Count + 1)
        {
            Debug.LogError("Não existem pontos suficientes para as provas!");
            return;
        }

        List<Transform> opcoesFaca = new List<Transform>();

        string ultimoPontoFaca = ObterUltimoPonto(faca);

        foreach (Transform ponto in pontosValidosFaca)
        {
            if (ponto.name != ultimoPontoFaca)
                opcoesFaca.Add(ponto);
        }

        if (opcoesFaca.Count == 0)
            opcoesFaca = new List<Transform>(pontosValidosFaca);

        Transform pontoEscolhidoFaca =
            opcoesFaca[Random.Range(0, opcoesFaca.Count)];

        PosicionarProva(faca, pontoEscolhidoFaca);
        pontosDisponiveis.Remove(pontoEscolhidoFaca);

        foreach (GameObject prova in provasValidas)
        {
            List<Transform> opcoes = new List<Transform>();

            string ultimoPonto = ObterUltimoPonto(prova);

            foreach (Transform ponto in pontosDisponiveis)
            {
                if (ponto.name != ultimoPonto)
                    opcoes.Add(ponto);
            }

            if (opcoes.Count == 0)
                opcoes = new List<Transform>(pontosDisponiveis);

            if (opcoes.Count == 0)
            {
                Debug.LogError("Sem ponto disponível para: " + prova.name);
                return;
            }

            Transform pontoEscolhido =
                opcoes[Random.Range(0, opcoes.Count)];

            PosicionarProva(prova, pontoEscolhido);
            pontosDisponiveis.Remove(pontoEscolhido);
        }

        Debug.Log("Provas distribuídas!");
    }

    private void SortearDigitais()
    {
        if (digitais == null || digitais.Length == 0)
        {
            Debug.LogWarning("Nenhuma digital configurada!");
            return;
        }

        List<Image> digitaisValidas = new List<Image>();

        foreach (Image digital in digitais)
        {
            if (digital != null && !digitaisValidas.Contains(digital))
            {
                digitaisValidas.Add(digital);
                digital.gameObject.SetActive(false);
            }
        }

        if (quantidadeDigitais <= 0 ||
            digitaisValidas.Count < quantidadeDigitais)
        {
            Debug.LogError(
                "Cadastre pelo menos " + quantidadeDigitais +
                " digitais no Inspector!"
            );
            return;
        }

        List<Image> opcoes = new List<Image>();

        foreach (Image digital in digitaisValidas)
        {
            if (!ultimasDigitais.Contains(digital.name))
                opcoes.Add(digital);
        }

        if (opcoes.Count < quantidadeDigitais)
            opcoes = new List<Image>(digitaisValidas);

        for (int i = opcoes.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            Image temp = opcoes[i];
            opcoes[i] = opcoes[j];
            opcoes[j] = temp;
        }

        ultimasDigitais.Clear();

        for (int i = 0; i < quantidadeDigitais; i++)
        {
            Image digital = opcoes[i];

            digital.gameObject.SetActive(true);
            ultimasDigitais.Add(digital.name);
        }

        Debug.Log(quantidadeDigitais + " digitais selecionadas!");
    }

    private string ObterUltimoPonto(GameObject prova)
    {
        if (ultimosPontos.TryGetValue(prova.name, out string ponto))
            return ponto;

        return "";
    }

    private void PosicionarProva(GameObject prova, Transform ponto)
    {
        prova.transform.SetPositionAndRotation(
            ponto.position,
            ponto.rotation
        );

        prova.SetActive(true);
        ultimosPontos[prova.name] = ponto.name;
    }
}
