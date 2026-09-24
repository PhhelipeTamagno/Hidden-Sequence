using TMPro;
using UnityEngine;
using System.Collections;

public class IndicadorDigitando : MonoBehaviour
{
    public TMP_Text textoDigitando;

    void Start()
    {
        StartCoroutine(AnimacaoPontos());
    }

    IEnumerator AnimacaoPontos()
    {
        while (true)
        {
            textoDigitando.text = ".";
            yield return new WaitForSeconds(0.4f);

            textoDigitando.text = "..";
            yield return new WaitForSeconds(0.4f);

            textoDigitando.text = "...";
            yield return new WaitForSeconds(0.4f);
        }
    }
}