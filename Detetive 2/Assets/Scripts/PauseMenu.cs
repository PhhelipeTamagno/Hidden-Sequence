using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject configPanel;

    [Header("Volume")]
    public Slider volumeSlider;

    [Header("Cena do Menu Principal")]
    public string menuSceneName = "Menu";

    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        configPanel.SetActive(false);

        // Carrega o volume salvo
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        AudioListener.volume = volumeSlider.value;

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    void Update()
    {
        // ESC só abre o pause.
        // Depois que abrir, ESC não fecha.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        // Libera o mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        configPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        // Trava o mouse novamente
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenConfig()
    {
        // NÃO fecha o Pause Menu
        configPanel.SetActive(true);
    }

    public void CloseConfig()
    {
        // Apenas fecha as configurações
        configPanel.SetActive(false);
    }

    public void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;

        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(menuSceneName);
    }
}