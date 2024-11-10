using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerCam : MonoBehaviour
{

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private Slider gamepadSensSlider;

    private float mouseSens;
    private float gamepadSens;

    private float sensX;
    private float sensY;

    private float mouseX;
    private float mouseY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (PlayerPrefs.HasKey("mouseSens"))
        {
            mouseSens = PlayerPrefs.GetFloat("mouseSens");

            mouseSensSlider.value = mouseSens;
        }
        else
        {
            UpdateMouseSensSlider();
        }

        if (PlayerPrefs.HasKey("gamepadSens"))
        {
            gamepadSens = PlayerPrefs.GetFloat("gamepadSens");

            gamepadSensSlider.value = gamepadSens;
        }
        else
        {
            UpdateGamepadSensSlider();
        }

    }


    void Update()
    {
        if (!GameManager.instance.gamePaused)
        {
            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -89, 89);

            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }  
    }


    public IEnumerator SmoothTiltChange(float zTilt)
    {
        float time = 0;
        float difference = Mathf.Abs(zTilt - transform.localRotation.z);
        float startValue = transform.localRotation.z;

        while (time < difference) 
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, Mathf.Lerp(startValue, zTilt, time * 4));
            time += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, zTilt);
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            sensX = gamepadSens;
            sensY = gamepadSens;
        }
        else if(playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            sensX = mouseSens; 
            sensY = mouseSens;
        }

        Vector2 mouse = context.ReadValue<Vector2>();
        mouseX = mouse.x * sensX;
        mouseY = mouse.y * sensY;
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

}
