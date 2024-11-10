using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] FloatingHealthBar healthBar;

    [Header("Health")]
    public float health;
    public float maxHealth;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    private void Start()
    {
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    public void Damage(float damage)
    {
        health -= damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        Destroy(gameObject);
    }
}
