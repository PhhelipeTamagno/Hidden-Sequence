using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider sliderMaster;
    public Slider sliderEfeitos;

    private const string MASTER_KEY = "VolumeMaster";
    private const string EFEITOS_KEY = "VolumeEfeitos";

    private void Awake()
    {
        // Impede que existam vários AudioManagers
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Carrega os volumes salvos
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float efeitos = PlayerPrefs.GetFloat(EFEITOS_KEY, 1f);

        if (sliderMaster != null)
            sliderMaster.value = master;

        if (sliderEfeitos != null)
            sliderEfeitos.value = efeitos;

        AlterarMaster(master);
        AlterarEfeitos(efeitos);

        // Liga os eventos dos sliders
        if (sliderMaster != null)
            sliderMaster.onValueChanged.AddListener(AlterarMaster);

        if (sliderEfeitos != null)
            sliderEfeitos.onValueChanged.AddListener(AlterarEfeitos);
    }

    public void AlterarMaster(float valor)
    {
        // Converte 0-1 para decibéis
        float volume = Mathf.Log10(Mathf.Clamp(valor, 0.0001f, 1f)) * 20f;

        audioMixer.SetFloat("MasterVolume", volume);

        PlayerPrefs.SetFloat(MASTER_KEY, valor);
        PlayerPrefs.Save();
    }

    public void AlterarEfeitos(float valor)
    {
        // Converte 0-1 para decibéis
        float volume = Mathf.Log10(Mathf.Clamp(valor, 0.0001f, 1f)) * 20f;

        audioMixer.SetFloat("EfeitosVolume", volume);

        PlayerPrefs.SetFloat(EFEITOS_KEY, valor);
        PlayerPrefs.Save();
    }
}