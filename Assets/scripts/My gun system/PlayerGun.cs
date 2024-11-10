using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [SerializeField]
    private GunSO currentGun;
    private bool shooting;
    private bool reloading;
    private bool readyToShoot;
    [SerializeField]
    private TextMeshProUGUI reloadtext;
    [SerializeField]
    private Transform muzzlePos;
    private ObjectPool<TrailRenderer> TrailPool;
    [SerializeField]
    private bool isGunEquiped;
    private AudioSource audioSource;

    public void Awake()
    {
        //SelectGun(0);
        //currentGun.bulletsLeft = currentGun.magSize;
        if (!isGunEquiped)
        {
            reloadtext.SetText("No Weapon");
        }
        readyToShoot = true;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        currentGun = null;
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        MyInputs();
    }
    private void MyInputs()
    {
        if (isGunEquiped && shooting && !reloading && readyToShoot && currentGun.bulletsLeft > 0) 
        { 
            Shoot();
        }
        else if (isGunEquiped && shooting && !reloading && currentGun.bulletsLeft <= 0)
        {
            Reload();
        }
    }

    private void Reload() 
    {
        if (GameManager.instance.gamePaused) return;
        reloading = true;
        reloadtext.SetText("Reloading...");
        audioSource.PlayOneShot(currentGun.reloadingSound);
        Invoke("Reloaded", currentGun.reloadTime);
    }
    private void Reloaded() 
    {
        reloading = false;
        currentGun.bulletsLeft = currentGun.magSize;
        reloadtext.SetText(currentGun.bulletsLeft + "/" + currentGun.magSize);
    }
    private void CancelReload()
    {
        CancelInvoke();
        reloading = false;
        reloadtext.SetText(currentGun.bulletsLeft + "/" + currentGun.magSize);
    }

    private void Shoot()
    {
        if (GameManager.instance.gamePaused) return;

        if (currentGun.typeOfGun != TypeOfGun.auto)
        {
            shooting = false;
        }

        audioSource.PlayOneShot(currentGun.shootingSound);

        readyToShoot = false;

        for (int i = 0; i < currentGun.bulletsPerShot; i++)
        {
            Vector3 shootDirection = Camera.main.transform.forward
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

            if (Physics.Raycast(Camera.main.transform.position, shootDirection, out RaycastHit hit, currentGun.distance, currentGun.HitMask))
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

                    if (hit.collider.CompareTag("Enemy"))
                    {
                        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                        damageable?.Damage(currentGun.damage);
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
        
        if (currentGun.typeOfGun != TypeOfGun.melee)
        {
            currentGun.bulletsLeft--;
            reloadtext.SetText(currentGun.bulletsLeft + "/" + currentGun.magSize);
        }
        else
        {
            reloadtext.SetText("");
        }
        Invoke("ResetShoot", 1 / currentGun.fireRate); // in bullets/sec multiply firerate by 60 if we want the fire rate to be bullets/min
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }

    public void SelectGun()
    {
        if (currentGun != null)
        {
            if (transform.Find(currentGun.Name) != null)
            {
                transform.Find(currentGun.Name).gameObject.SetActive(false);
            }
        }

        currentGun = InventoryManager.instance.selectedGun;

        if (currentGun != null)
        {
            if (transform.Find(currentGun.Name) != null)
            {
                transform.Find(currentGun.Name).gameObject.SetActive(true);
            }
            isGunEquiped = true;
            CancelReload();
            ResetShoot();
        }
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

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (currentGun != null)
        {
            if (currentGun.typeOfGun == TypeOfGun.auto)
            {
                if (context.performed) shooting = true; else if (context.canceled) shooting = false;
            }
            else
            {
                if (context.started) shooting = true; else if (context.canceled) shooting = false;
            }
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (currentGun.bulletsLeft < currentGun.magSize && !reloading)
            {
                Reload();
            }
        }
    }

    public void OnChangeWeapon(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            SelectGun();
        }
    }
}
