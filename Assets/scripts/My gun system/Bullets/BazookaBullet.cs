using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BazookaBullet : MonoBehaviour
{
    [SerializeField] private float explosionArea;
    [SerializeField] private float explosionSpeed;
    public float damage;
    public float bulletDropForce;
    private Rigidbody bulletRb;
    private SphereCollider col;
    private bool explode;
    private Vector3 explosionPosition;

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        explode = false;
    }

    private void Update()
    {
        if (explode && col.radius <= explosionArea)
        {
            col.radius += Time.deltaTime * explosionSpeed;
            transform.position = explosionPosition;

        }
        else if (explode)
        {
            Destroy(gameObject);
        }
        else
        {
            bulletRb.AddForce(Vector3.down * bulletDropForce, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player")  && !explode)
        {
            return;
        }

        if (explode)
        {
            if (collision.collider.CompareTag("Player"))
            {
                IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();
                damageable?.Damage(damage);
            }
            else
            {
                IDamageable damageable = collision.collider.GetComponent<IDamageable>();
                damageable?.Damage(damage);
            }
        }
        else
        {
            if (collision.collider.CompareTag("Player"))
            {
                IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();
                damageable?.Damage(damage);
            }
            else
            {
                IDamageable damageable = collision.collider.GetComponent<IDamageable>();
                damageable?.Damage(damage);
            }
            explode = true;
            explosionPosition = transform.position;
        }
    }
}
