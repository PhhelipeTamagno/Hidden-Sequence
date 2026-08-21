using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MenuButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Animação")]
    [SerializeField] private float escalaHover = 1.05f;
    [SerializeField] private float escalaClique = 0.97f;
    [SerializeField] private float velocidade = 10f;

    [Header("Som - Opcional")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somHover;
    [SerializeField] private AudioClip somClique;

    private Vector3 escalaOriginal;
    private Vector3 escalaAlvo;

    private void Start()
    {
        escalaOriginal = transform.localScale;
        escalaAlvo = escalaOriginal;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaAlvo,
            Time.unscaledDeltaTime * velocidade
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal * escalaHover;

        if (audioSource != null && somHover != null)
            audioSource.PlayOneShot(somHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal * escalaClique;

        if (audioSource != null && somClique != null)
            audioSource.PlayOneShot(somClique);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        escalaAlvo = escalaOriginal * escalaHover;
    }
}