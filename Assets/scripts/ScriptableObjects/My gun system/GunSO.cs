using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "MyGuns/Gun", order = 0)]
public class GunSO : ScriptableObject
{
    //Gun
    [Header("Gun")]
    public string Name;
    public TypeOfGun typeOfGun;
    public LayerMask HitMask;
    public float damage;
    public float fireRate;
    public int bulletsPerShot;
    public float reloadTime;
    public float distance;
    public int magSize;
    public int bulletsLeft;
    public int totalBulletsLeft;
    public Vector3 Spread = new Vector3();
    public TrailConfigScriptableObject TrailConfig;
    public AudioClip shootingSound;
    public AudioClip reloadingSound;

    [Space]
    [Header("Ammo")]
    public TypeOfAmmo typeOfAmmo;
    public float bulletSpead;
    public float bulletDropForce;
    public GameObject bulletPrefab;

}
