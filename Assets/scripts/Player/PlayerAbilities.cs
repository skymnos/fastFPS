using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    [SerializeField] private PlayerCam playerCam;
    private PlayerInput playerInput;
    [SerializeField] private GameObject targetImage;
    private PlayerMovement playerMovement;

    [Header("Ability1")]
    [SerializeField] private float ability1Cooldown;
    [SerializeField] private float jumpForce;
    [SerializeField] private float timeScale;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float ability1Time;
    [SerializeField] private Image ability1Image;
    [SerializeField] private Slider abilityTimeSlider;
    private Vector3 direction;
    private float ability1Timer;
    private bool canUseAbility1;
    private bool usingAbility1;
    private bool readyToAttack;
    private bool addForce;
    private bool maxPoint;
    private RaycastHit hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        ability1Image.color = Color.green;
        abilityTimeSlider.gameObject.SetActive(false);
        usingAbility1 = false;
        readyToAttack = false;
        addForce = false;
        maxPoint = false;
        canUseAbility1 = true;
        ability1Timer = 0;
    }

    private void Update()
    {
        if (usingAbility1 && rb.velocity.y < -0.5)
        {
            maxPoint = true;
            usingAbility1 = false;
        }
        else if (maxPoint && ability1Timer > 0)
        {
            ability1Timer -= Time.deltaTime;
            abilityTimeSlider.value = ability1Timer;
            Time.timeScale = timeScale;
            readyToAttack = true;
        }
        else if (maxPoint && ability1Timer <= 0)
        {
            CancelAbility1();
        }

        if (readyToAttack)
        {
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit);
            targetImage.SetActive(true);
            targetImage.transform.up = hit.normal;
            targetImage.transform.position = hit.point + targetImage.transform.up * 0.01f;
        }
        else
        {
            targetImage.SetActive(false);
        }

        if (addForce && Vector3.Distance(transform.position, hit.point) > 2 && !playerMovement.grounded)
        {
            rb.velocity = direction * accelerationSpeed;
        }
        else if (addForce)
        {
            ImpactAbility1();
        }
    }

    private void Ability1()
    {
        canUseAbility1 = false;
        ability1Image.color = Color.red;
        usingAbility1 = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        playerInput.SwitchCurrentActionMap("Ability1");

        float scaledAbility1Time = ability1Time * timeScale;
        ability1Timer = scaledAbility1Time;
        abilityTimeSlider.gameObject.SetActive(true);
        abilityTimeSlider.maxValue = ability1Timer;
        abilityTimeSlider.value = ability1Timer;

    }

    private void CancelAbility1()
    {
        maxPoint = false;
        Time.timeScale = 1;
        readyToAttack = false;
        playerInput.SwitchCurrentActionMap("Player");
        abilityTimeSlider.gameObject.SetActive(false);
        Invoke("ResetAbility1CoolDown", ability1Cooldown);
    }

    private void AttackAbility1()
    {
        abilityTimeSlider.gameObject.SetActive(false);
        readyToAttack = false;
        maxPoint = false;
        addForce = true;
        direction = (hit.point - transform.position).normalized;
        rb.mass = 0;
        Time.timeScale = 1;
    }

    private void ImpactAbility1()
    {
        rb.velocity = Vector3.zero;
        rb.mass = 1;
        playerInput.SwitchCurrentActionMap("Player");
        addForce = false;
        Invoke("ResetAbility1CoolDown", ability1Cooldown);
    }

    private void ResetAbility1CoolDown()
    {
        canUseAbility1 = true;
        ability1Image.color = Color.green;
    }

    public void OnAbility1(InputAction.CallbackContext context)
    {
        if (context.started && playerMovement.grounded && canUseAbility1) 
        {
            Ability1();
        }
    }

    public void OnAttackAbility1(InputAction.CallbackContext context)
    {
        if (context.started && readyToAttack)
        {
            AttackAbility1();
        }
    }
}
