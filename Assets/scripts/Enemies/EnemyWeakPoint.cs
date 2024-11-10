using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour, IDamageable
{
    [SerializeField] private float damageFactor;
    [SerializeField] private EnemyHealth enemyHealth;
    public void Damage(float damage)
    {
        enemyHealth.Damage(damage * damageFactor);
    }
}
