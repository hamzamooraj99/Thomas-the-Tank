using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]

public class WeaponData : ScriptableObject
{
    [Header("Gun Info")]
    public new string name;

    [Header("Gun Stats")]
    public float damage;
    public float shootForce;
    public float upwardForce;
    public int currentAmmo;
    public int magSize;
    public int totalAmmo;
    public int fireRate;
    public float spread;
    public float reloadTime;
    public bool allowButtonHold;

    [Header("Bullet Info")]
    public GameObject bullet;    
}
