using UnityEngine;
using System.Collections.Generic;

public class EnergyBullet : MonoBehaviour
{
    [SerializeField] private float maxHitDistance;
    [SerializeField] private int maxHit;
    private List<Collider> enemiesHit = new List<Collider>();
    private int hits;

    public float damage;
    public float bulletDropForce;
    private Rigidbody bulletRb;

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        hits = 0;
    }

    private void Update()
    {
        bulletRb.AddForce(Vector3.down * bulletDropForce, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Hit(other);
            Destroy(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private Collider NearestEnemy(RaycastHit[] rayHits)
    {
        Collider nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (RaycastHit rayHit in rayHits)
        {
            if (enemiesHit.Contains(rayHit.collider) || !rayHit.collider.CompareTag("Enemy"))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, rayHit.transform.position);
            if (distance < minDistance)
            {
                nearestEnemy = rayHit.collider;
                minDistance = distance;
            }
        }

        return nearestEnemy;
    }

    private void Hit(Collider other)
    {
        while (hits < maxHit)
        {
            hits++;
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable?.Damage(damage / hits);
            enemiesHit.Add(other);
            RaycastHit[] rayHits = Physics.SphereCastAll(other.transform.position, maxHitDistance, transform.forward);
            other = NearestEnemy(rayHits);
        }
        enemiesHit.Clear();
        hits = 0;
    }

}
