using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] Slider healthSlider;

    public float health;
    public float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.value = health / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damage)
    {
        health -= damage;
        healthSlider.value = health / maxHealth;

        if (health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        SceneManager.LoadScene(0);
    }
}
