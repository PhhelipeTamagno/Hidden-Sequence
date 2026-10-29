using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using EasyTransition;

public class ItensEncontrados : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string nome;

        [Header("Imagem da Hotbar")]
        public Image imagem;

        [Header("Cor quando encontrado")]
        public Color corEncontrado = Color.white;

        [Header("Cor antes de encontrar")]
        public Color corNaoEncontrado = Color.black;

        [HideInInspector]
        public bool encontrado = false;
    }

    [Header("Itens que precisam ser encontrados")]
    public Item garrafa;
    public Item faca;
    public Item digital;

    [Header("Texto Final")]
    public TMP_Text textoFinal;

    [Header("Fala do Policial")]
    public GameObject painelFala;
    public TMP_Text textoFala;

    [TextArea(2, 4)]
    public string falaPolicial = "EU ACHEI TUDO QUE EU PRECISO, AGORA TENHO PROVAS PARA USAR NO INTERROGATÓRIO E VER SE É ELE MESMO.";

    [SerializeField] private float velocidadeDigitacao = 0.04f;
    [SerializeField] private float tempoAposFala = 1.5f;

    [Header("Cena para carregar")]
    public string nomeDaCena;

    [Header("Easy Transition")]
    [SerializeField] private Transition transition;
    [SerializeField] private float delay = 1f;

    private bool todasEncontradas = false;
    private bool transicaoIniciada = false;

    private void Start()
    {
        AtualizarItem(garrafa);
        AtualizarItem(faca);
        AtualizarItem(digital);

        if (textoFinal != null)
        {
            textoFinal.gameObject.SetActive(false);
        }

        if (painelFala != null)
        {
            painelFala.SetActive(false);
        }
    }

    private void Update()
    {
        GarantirImagemVisivel(garrafa);
        GarantirImagemVisivel(faca);
        GarantirImagemVisivel(digital);

        VerificarTodasEncontradas();
    }

    private void GarantirImagemVisivel(Item item)
    {
        if (item == null || item.imagem == null)
            return;

        item.imagem.enabled = true;

        Color cor = item.imagem.color;
        cor.a = 1f;

        item.imagem.color = cor;
    }

    public void EncontrarGarrafa()
    {
        if (garrafa == null || garrafa.encontrado)
            return;

        garrafa.encontrado = true;
        AtualizarItem(garrafa);
    }

    public void EncontrarFaca()
    {
        if (faca == null || faca.encontrado)
            return;

        faca.encontrado = true;
        AtualizarItem(faca);
    }

    public void EncontrarDigital()
    {
        if (digital == null || digital.encontrado)
            return;

        digital.encontrado = true;
        AtualizarItem(digital);

        Debug.Log("DIGITAL LIBERADA NA HOTBAR!");
    }

    public void DigitalMirada()
    {
        if (digital == null || digital.imagem == null)
            return;

        if (!digital.encontrado)
        {
            EncontrarDigital();
        }
    }

    public void DigitalNaoMirada()
    {
        if (digital == null || digital.imagem == null)
            return;

        if (!digital.encontrado)
        {
            digital.imagem.enabled = true;

            Color cor = digital.corNaoEncontrado;
            cor.a = 1f;

            digital.imagem.color = cor;
        }
    }

    private void VerificarTodasEncontradas()
    {
        if (todasEncontradas)
            return;

        if (garrafa != null &&
            faca != null &&
            digital != null &&
            garrafa.encontrado &&
            faca.encontrado &&
            digital.encontrado)
        {
            todasEncontradas = true;

            StartCoroutine(TransicaoECarregarCena());
        }
    }

    private IEnumerator TransicaoECarregarCena()
    {
        if (transicaoIniciada)
            yield break;

        transicaoIniciada = true;

      
        if (textoFinal != null)
        {
            textoFinal.gameObject.SetActive(true);

            textoFinal.text =
                "Depois que voce encontrou as pistas, voce voltou para a delegacia para terminar o caso.";
        }

        yield return new WaitForSeconds(1f);

        
        if (painelFala != null && textoFala != null)
        {
            if (textoFinal != null)
                textoFinal.gameObject.SetActive(false);

            painelFala.SetActive(true);

            yield return StartCoroutine(DigitarFala(falaPolicial));

       
            yield return new WaitForSeconds(tempoAposFala);
        }

        if (string.IsNullOrEmpty(nomeDaCena))
        {
            Debug.LogError(
                "ItensEncontrados: Nome da cena nao foi definido!"
            );

            yield break;
        }

        if (transition != null)
        {
           
            transition.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "ItensEncontrados: Transition nao foi colocado no Inspector!"
            );
        }

     
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(nomeDaCena);
    }

    private IEnumerator DigitarFala(string fala)
    {
        textoFala.text = "";

        foreach (char letra in fala)
        {
            textoFala.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
        }
    }

    private void AtualizarItem(Item item)
    {
        if (item == null || item.imagem == null)
            return;

        item.imagem.enabled = true;

        Color cor = item.encontrado
            ? item.corEncontrado
            : item.corNaoEncontrado;

        cor.a = 1f;
        item.imagem.color = cor;
    }

    public bool GarrafaFoiEncontrada()
    {
        return garrafa != null && garrafa.encontrado;
    }

    public bool FacaFoiEncontrada()
    {
        return faca != null && faca.encontrado;
    }

    public bool DigitalFoiEncontrada()
    {
        return digital != null && digital.encontrado;
    }
}