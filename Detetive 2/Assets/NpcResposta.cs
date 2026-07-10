using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCResposta : MonoBehaviour
{
     [Header("UI")]
    public TMP_InputField campoTexto;
    public TMP_Text textoNPC;

    [Header("Mensagem Final")]
    public GameObject painelMensagem;
    public TMP_Text textoMensagem;

    private int perguntasFeitas = 0;

   public TMP_Text textoResposta;
   public TMP_Text textoDigitando;


   IEnumerator FinalInterrogatorio()
{
    textoResposta.gameObject.SetActive(false);
    textoDigitando.gameObject.SetActive(false);
    campoTexto.gameObject.SetActive(false);

    painelMensagem.SetActive(true);

    textoMensagem.text =
        "Após interrogar o suspeito, você decide seguir para a cena do crime em busca de pistas.";

    yield return new WaitForSeconds(5f);

    SceneManager.LoadScene("CenaDoCrime");
}
    
    
    public void EnviarPergunta()
{
    string pergunta = campoTexto.text;

    if (string.IsNullOrWhiteSpace(pergunta))
        return;

    perguntasFeitas++;

    campoTexto.text = "";

    StartCoroutine(ResponderNPC());

    if (perguntasFeitas >= 3)
    {
        StartCoroutine(FinalInterrogatorio());
    }
}
IEnumerator ResponderNPC()
{
    textoResposta.text = "";
    textoDigitando.text = ".";

    yield return new WaitForSeconds(0.3f);

    textoDigitando.text = "..";

    yield return new WaitForSeconds(0.3f);

    textoDigitando.text = "...";

    yield return new WaitForSeconds(1f);

    textoDigitando.text = "";

    textoResposta.text = "Não sei de nada. Eu não estava envolvido.";
}
}