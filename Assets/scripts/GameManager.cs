using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool maintainCrouch;
    public bool gamePaused = false;
    public bool gameStopped = false;
    public static GameManager instance;
    public bool menuOpened;
    [SerializeField] private GunSO[] guns;
    [SerializeField] public PlayerInput playerInput;


    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("il y a plus d'une instance de InventoryManager dans la scène");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        menuOpened = false;
        InitialiseWeapons();
    }

    private void InitialiseWeapons()
    {
        foreach (GunSO gun in guns)
        {
            gun.bulletsLeft = gun.magSize;
        }
    }

    public void PauseGame()
    {
        gameStopped = true;
        Time.timeScale = 0;

        playerInput.currentActionMap.Disable();
        playerInput.SwitchCurrentActionMap("Menu");
        playerInput.currentActionMap.Enable();
    }

    public void UnpauseGame()
    {
        gameStopped = false;
        Time.timeScale = 1;

        playerInput.currentActionMap.Disable();
        playerInput.SwitchCurrentActionMap("Player");
        playerInput.currentActionMap.Enable();
    }
}
