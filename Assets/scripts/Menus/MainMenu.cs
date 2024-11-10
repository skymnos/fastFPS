using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject mainMenuCanvasGO;
    [SerializeField] private GameObject settingsMenuCanvasGO;
    [SerializeField] private GameObject keyboardCanvas;
    [SerializeField] private GameObject gamepadCanvas;
    [SerializeField] private GameObject audioSettingsCanvas;
    [SerializeField] private GameObject videoSettingsCanvasGO;


    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject settingsMenuFirst;
    [SerializeField] private GameObject keyboardMenuFirst;
    [SerializeField] private GameObject gamepadMenuFirst;
    [SerializeField] private GameObject audioSettingsFirst;
    [SerializeField] private GameObject videoSettingsFirst;

    [Header("Sens settings")]
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private Slider gamepadSensSlider;
    private float mouseSens;
    private float gamepadSens;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Start()
    {
        mainMenuCanvasGO.SetActive(true);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        if (PlayerPrefs.HasKey("mouseSens") && PlayerPrefs.HasKey("gamepadSens"))
        {
            mouseSens = PlayerPrefs.GetFloat("mouseSens");
            gamepadSens = PlayerPrefs.GetFloat("gamepadSens");

            mouseSensSlider.value = mouseSens;
            gamepadSensSlider.value = gamepadSens;
        }
    }

    public void UpdateMouseSensSlider()
    {
        mouseSens = mouseSensSlider.value;
        PlayerPrefs.SetFloat("mouseSens", mouseSens);
    }

    public void UpdateGamepadSensSlider()
    {
        gamepadSens = gamepadSensSlider.value;
        PlayerPrefs.SetFloat("gamepadSens", gamepadSens);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OpenMainMenu()
    {
        mainMenuCanvasGO.SetActive(true);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    private void OpenSettingsMenuHandle()
    {
        mainMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(true);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }

    private void OpenKeyboardControlsMenu()
    {
        mainMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(true);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(keyboardMenuFirst);
    }

    private void OpenGamepadControlsMenu()
    {
        mainMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(true);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(gamepadMenuFirst);
    }

    private void OpenAudioMenu()
    {
        mainMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(true);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(audioSettingsFirst);
    }

    private void OpenVideoMenu()
    {
        mainMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(true);

        EventSystem.current.SetSelectedGameObject(videoSettingsFirst);
    }



    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    public void OnKeyboardControlsPress()
    {
        OpenKeyboardControlsMenu();
    }

    public void OnGamepadControlsPress()
    {
        OpenGamepadControlsMenu();
    }

    public void OnAudioPress()
    {
        OpenAudioMenu();
    }

    public void OnVideoPress()
    {
        OpenVideoMenu();
    }

    public void OnSubSettingsBackPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnQuitPress()
    {
        QuitGame();
    }

    public void OnStartPress()
    {
        StartGame();
    }
}
