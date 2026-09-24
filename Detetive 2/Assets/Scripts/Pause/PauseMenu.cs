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

        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        AudioListener.volume = volumeSlider.value;

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    void Update()
    {

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        configPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenConfig()
    {
        configPanel.SetActive(true);
    }

    public void CloseConfig()
    {
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