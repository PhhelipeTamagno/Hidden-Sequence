using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Fade Final")]
    public CanvasGroup fade;
    public TMP_Text textoFinal;

    [Header("Cena para carregar")]
    public string nomeDaCena;

    [Header("Configuração do Fade")]
    public float velocidadeFade = 1f;

    private bool todasEncontradas = false;

    void Start()
    {
        AtualizarItem(garrafa);
        AtualizarItem(faca);
        AtualizarItem(digital);

        if (fade != null)
            fade.alpha = 0;

        if (textoFinal != null)
            textoFinal.gameObject.SetActive(false);
    }

    void Update()
    {
        GarantirImagemVisivel(garrafa);
        GarantirImagemVisivel(faca);
        GarantirImagemVisivel(digital);

        VerificarTodasEncontradas();
    }

    void GarantirImagemVisivel(Item item)
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
        garrafa.encontrado = true;
        AtualizarItem(garrafa);
    }
    public void EncontrarFaca()
    {
        faca.encontrado = true;
        AtualizarItem(faca);
    }
    public void EncontrarDigital()
    {
        digital.encontrado = true;
        AtualizarItem(digital);
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
    void VerificarTodasEncontradas()
    {
        if (todasEncontradas)
            return;

        if (garrafa.encontrado &&
            faca.encontrado &&
            digital.encontrado)
        {
            todasEncontradas = true;
            StartCoroutine(FadeFinal());
        }
    }
    IEnumerator FadeFinal()
    {
        if (fade == null)
            yield break;

        yield return new WaitForSeconds(1f);

        if (textoFinal != null)
        {
            textoFinal.gameObject.SetActive(true);

            textoFinal.text =
                "Depois que voce encontrou as pistas, voce voltou para a delegacia para terminar o caso.";
        }

   
        float tempo = 0;

        while (tempo < 1)
        {
            tempo += Time.deltaTime * velocidadeFade;
            fade.alpha = Mathf.Lerp(0, 1, tempo);

            yield return null;
        }

        fade.alpha = 1;

        
        yield return new WaitForSeconds(1f);

        
        if (!string.IsNullOrEmpty(nomeDaCena))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
        else
        {
            Debug.LogError("Nome da cena não foi definido no ItensEncontrados!");
        }
    }
    void AtualizarItem(Item item)
    {
        if (item == null || item.imagem == null)
            return;

        item.imagem.enabled = true;

        Color cor;

        if (item.encontrado)
            cor = item.corEncontrado;
        else
            cor = item.corNaoEncontrado;

        cor.a = 1f;
        item.imagem.color = cor;
    }

    public bool GarrafaFoiEncontrada()
    {
        return garrafa.encontrado;
    }

    public bool FacaFoiEncontrada()
    {
        return faca.encontrado;
    }

    public bool DigitalFoiEncontrada()
    {
        return digital.encontrado;
    }
}