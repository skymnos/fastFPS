using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject weaponWheelCanvas;
    [SerializeField] private GameObject inGameMenuCanvasGO;
    [SerializeField] private GameObject settingsMenuCanvasGO;
    [SerializeField] private GameObject keyboardCanvas;
    [SerializeField] private GameObject gamepadCanvas;
    [SerializeField] private GameObject audioSettingsCanvas;
    [SerializeField] private GameObject videoSettingsCanvasGO;


    [Header("First Selected Options")]
    [SerializeField] private GameObject inGameMenuFirst;
    [SerializeField] private GameObject settingsMenuFirst;
    [SerializeField] private GameObject keyboardMenuFirst;
    [SerializeField] private GameObject gamepadMenuFirst;
    [SerializeField] private GameObject audioSettingsFirst;
    [SerializeField] private GameObject videoSettingsFirst;

    private void Start()
    {
        inGameCanvas.SetActive(true);
        weaponWheelCanvas.SetActive(false);
        inGameMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);
    }

    public void Menu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.instance.menuOpened = !GameManager.instance.menuOpened;
            if (GameManager.instance.menuOpened)
            {
                OpenInGameMenu();
            }
            else
            {
                CloseAllMenus();
            }
        }
    }

    private void OpenInGameMenu()
    {
        GameManager.instance.PauseGame();

        inGameMenuCanvasGO.SetActive(true);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(inGameMenuFirst);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OpenSettingsMenuHandle()
    {
        inGameMenuCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(true);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }

    private void OpenKeyboardControlsMenu()
    {
        inGameMenuCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(true);
        inGameCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(keyboardMenuFirst);
    }

    private void OpenGamepadControlsMenu()
    {
        inGameMenuCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        gamepadCanvas.SetActive(true);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(gamepadMenuFirst);
    }

    private void OpenAudioMenu()
    {
        inGameMenuCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(true);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(audioSettingsFirst);
    }

    private void OpenVideoMenu()
    {
        inGameMenuCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(false);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(true);

        EventSystem.current.SetSelectedGameObject(videoSettingsFirst);
    }

    private void OpenWeaponWheel()
    {
        inGameMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(true);

        InventoryManager.instance.isOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.instance.gamePaused = true;
    }

    private void CloseWeaponWheel()
    {
        inGameMenuCanvasGO.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);

        InventoryManager.instance.isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.instance.gamePaused = false;
    }

    private void CloseAllMenus()
    {
        GameManager.instance.UnpauseGame();
        GameManager.instance.menuOpened = false;

        inGameMenuCanvasGO.SetActive(false);
        weaponWheelCanvas.SetActive(false);
        settingsMenuCanvasGO.SetActive(false);
        keyboardCanvas.SetActive(false);
        inGameCanvas.SetActive(true);
        gamepadCanvas.SetActive(false);
        audioSettingsCanvas.SetActive(false);
        videoSettingsCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }



    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        CloseAllMenus();
    }
    public void OnSettingsBackPress()
    {
        OpenInGameMenu();
    }

    public void OnMainMenuPress()
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

    public void OnOpenWeaponWheel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OpenWeaponWheel();
        }
        else if (context.canceled)
        {
            CloseWeaponWheel();
        }
    }
}
