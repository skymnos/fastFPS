using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    public GameObject Inventory;
    public static InventoryManager instance;
    public bool isOpen;

    [Header("Slots")]
    [SerializeField] private WeaponWheelButton[] slots;
    [SerializeField] private PlayerInput playerInput;

    private float angle;
    private Vector2 screenCenter;
    private float angleOfSelection;
    private WeaponWheelButton selectedSlot;
    public GunSO selectedGun;



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
        isOpen = false;
        angleOfSelection = 360/slots.Length;
        screenCenter = new Vector2 (Screen.width/2, Screen.height/2);
    }
    public void OnPointerMove(InputAction.CallbackContext context)
    {
        if (isOpen)
        {
            selectedSlot?.Unselected();

            if (playerInput.currentControlScheme == "Gamepad")
            {
                Vector2 gamepadVector = context.ReadValue<Vector2>();
                if (gamepadVector != new Vector2(0, 0))
                {
                    angle = Vector2.Angle(Vector2.up, gamepadVector);
                    if (gamepadVector.x > 0)
                    {
                        angle = 360 - angle;
                    }
                }
            }
            else if (playerInput.currentControlScheme == "Keyboard&Mouse")
            {
                Vector2 mousePosition = context.ReadValue<Vector2>();
                Vector2 mouseVector = mousePosition - screenCenter;

                angle = Vector2.Angle(Vector2.up, mouseVector);
                if (mousePosition.x > screenCenter.x)
                {
                    angle = 360 - angle;
                }
            }
            int selectedIndex = (int)Mathf.Ceil(angle / angleOfSelection) - 1;
            if (angle == 0) selectedIndex = 9;

            selectedSlot = slots[selectedIndex];
            selectedSlot.Selected();
            selectedGun = selectedSlot.gun;
        }
        
    }
}
