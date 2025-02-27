using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Seagull.Interior_01;

public class EnemyTankShoot : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField, HideInInspector] WeaponData weaponData;
    public float timeBetweenShots = 5f; // 5 seconds between shots
    private bool readyToShoot = true; // Ensure it's set to true initially

    [Header("Enemy References")]
    [SerializeField] Rigidbody enemyRB;
    [SerializeField] Transform spawnPoint;

    [SerializeField] AudioClip cannonSound;

    public GameObject muzzleFlash;
    public Transform muzzlePosition;

    void Start()
    {
        EnemyTankInfo enemyTankInfo = GetComponentInParent<EnemyTankInfo>();
        if (enemyTankInfo != null)
            weaponData = enemyTankInfo.weapon;
        else
            Debug.Log("TankShoot: Tank Info not found");

        weaponData.currentAmmo = weaponData.magSize;
        readyToShoot = true; // Ensure it's ready to shoot
    }

    public void Shoot()
    {
        if (!readyToShoot) return; // Check if ready to shoot

        readyToShoot = false; // Set readyToShoot to false to prevent multiple shots
        Vector3 shootDirection = spawnPoint.forward; // Fire in turret's forward direction

        StartCoroutine(ShowMuzzleFlash());

        // Instantiate Bullet
        GameObject currBullet = Instantiate(weaponData.bullet, spawnPoint.position, Quaternion.identity);
        currBullet.GetComponent<Rigidbody>().AddForce(shootDirection * weaponData.shootForce, ForceMode.Impulse);
        currBullet.layer = spawnPoint.gameObject.layer;
        
        BulletDamage bulletDamage = currBullet.GetComponent<BulletDamage>();
        if(bulletDamage != null)
            bulletDamage.damage = weaponData.damage;

        weaponData.currentAmmo--;
        
        PlayCannonSound();

        // Ensure ResetShot() runs by using StartCoroutine
        StartCoroutine(ResetShot());
    }

    public IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(timeBetweenShots); // Wait before allowing another shot
        readyToShoot = true; // Reset readyToShoot to allow next shot
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    private void PlayCannonSound()
    {
        GameObject tempAudioSourceObj = new GameObject("TempCannonSound");
        tempAudioSourceObj.transform.position = transform.position;
        AudioSource tempAudioSource = tempAudioSourceObj.AddComponent<AudioSource>();

        tempAudioSource.clip = cannonSound;
        tempAudioSource.spatialBlend = 1.0f;
        tempAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        tempAudioSource.minDistance = 5f; 
        tempAudioSource.maxDistance = 50f; 
        tempAudioSource.volume = 0.5f;
        tempAudioSource.Play();

        Destroy(tempAudioSourceObj, cannonSound.length);
    }
}