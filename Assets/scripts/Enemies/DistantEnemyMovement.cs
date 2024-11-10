using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class DistantEnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float visionDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private LayerMask attackable;
    private float distanceToTarget;
    NavMeshAgent agent;

    [SerializeField] private float decreseDamageFactor;
    [SerializeField] private GunSO currentGun;
    private ObjectPool<TrailRenderer> TrailPool;
    private bool readyToShoot;
    private Transform muzzlePos;
    private bool reloading;

    private Vector3 lastTargetPosition;
    [SerializeField] private float refreshPositionCooldown; // la precision avec laquelle l'ennemi traque le joueur
    private float timer;


    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
        readyToShoot = true;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        muzzlePos = transform; // temporary while no gun in ennemis hand so don't know the exact position
        timer = 0;
    }
    private void Update()
    {
        if (timer > refreshPositionCooldown)
        {
            lastTargetPosition = target.position;
            timer = 0;
        }

        timer += Time.deltaTime;

        distanceToTarget = Vector3.Distance(transform.position, target.position);
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        if (distanceToTarget < attackDistance)
        {
            Attack();
        }
        else if (distanceToTarget > attackDistance && distanceToTarget < visionDistance) 
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        agent.isStopped = true;
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    private void Attack()
    {
        agent.isStopped = true;
        Vector3 direction = (lastTargetPosition - transform.position).normalized;

        if (!reloading && readyToShoot && currentGun.bulletsLeft > 0)
        {
            Shoot(direction);
        }
        else if (!reloading && currentGun.bulletsLeft <= 0)
        {
            Reload();
        }

    }

    private void Reload()
    {
        if (GameManager.instance.gamePaused) return;
        reloading = true;
        Invoke("Reloaded", currentGun.reloadTime);
    }
    private void Reloaded()
    {
        reloading = false;
        currentGun.bulletsLeft = currentGun.magSize;
    }

    private void Shoot( Vector3 direction)
    {
        readyToShoot = false;

        for (int i = 0; i < currentGun.bulletsPerShot; i++)
        {
            Vector3 shootDirection = direction
            + new Vector3(
                Random.Range(
                    -currentGun.Spread.x,
                    currentGun.Spread.x
                ),
                Random.Range(
                    -currentGun.Spread.y,
                    currentGun.Spread.y
                ),
                Random.Range(
                    -currentGun.Spread.z,
                    currentGun.Spread.z
                )
            );

            if (Physics.Raycast(transform.position, shootDirection, out RaycastHit hit, currentGun.distance, currentGun.HitMask))
            {
                if (currentGun.typeOfAmmo == TypeOfAmmo.classic)
                {
                    StartCoroutine(
                        PlayTrail(
                        muzzlePos.position,
                        hit.point,
                        hit
                        )
                    );

                    if (hit.collider.CompareTag("Player"))
                    {
                        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                        damageable?.Damage(currentGun.damage / decreseDamageFactor);
                    }
                }
                else
                {
                    Rigidbody bulletRb = Instantiate(currentGun.bulletPrefab, new Vector3(muzzlePos.position.x, muzzlePos.position.y, muzzlePos.position.z), Quaternion.Euler(hit.point.normalized.x, hit.point.normalized.y, hit.point.normalized.z)).GetComponent<Rigidbody>(); // Penser à utiliser ObjectPool si trop de puisssance demandée
                    bulletRb.velocity = Vector3.zero;
                    bulletRb.AddForce(
                        (
                        (hit.point - bulletRb.transform.position).normalized + shootDirection) * currentGun.bulletSpead,
                        ForceMode.Impulse
                    );
                }

            }
            else
            {
                if (currentGun.typeOfAmmo == TypeOfAmmo.classic)
                {
                    StartCoroutine(
                    PlayTrail(
                        muzzlePos.position,
                        muzzlePos.position + (shootDirection * currentGun.TrailConfig.MissDistance), // if acting weird might be because shootDirection depend on camera and not the weapon itself
                        new RaycastHit()
                        )
                    );
                }
                else
                {
                    Rigidbody bulletRb = Instantiate(currentGun.bulletPrefab, new Vector3(muzzlePos.position.x, muzzlePos.position.y, muzzlePos.position.z), Quaternion.Euler(hit.point.normalized.x, hit.point.normalized.y, hit.point.normalized.z)).GetComponent<Rigidbody>(); // Penser à utiliser ObjectPool si trop de puisssance demandée
                    bulletRb.velocity = Vector3.zero;
                    bulletRb.AddForce(
                        (muzzlePos.position + (shootDirection * currentGun.TrailConfig.MissDistance) - bulletRb.transform.position).normalized * currentGun.bulletSpead,
                        ForceMode.Impulse
                    );
                }
            }

        }


        currentGun.bulletsLeft--;
        Invoke("ResetShoot", 1 / currentGun.fireRate); // in bullets/sec multiply firerate by 60 if we want the fire rate to be bullets/min
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null; // avoid position carry-overfrom last frame if used

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= currentGun.TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        if (hit.collider != null)
        {
            // surfaceManager is part of this video : https://www.youtube.com/watch?v=kT2ZxjMuT_4
            /* SurfaceManager.Instance.HandleImpact(
                   Hit.transform.gameObject,
                   EndPoint,
                   Hit.normal,
                   ImpactType,
                   0
               );
            */
        }

        yield return new WaitForSeconds(currentGun.TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);

    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = currentGun.TrailConfig.Color;
        trail.material = currentGun.TrailConfig.Material;
        trail.widthCurve = currentGun.TrailConfig.WidthCurve;
        trail.time = currentGun.TrailConfig.Duration;
        trail.minVertexDistance = currentGun.TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}