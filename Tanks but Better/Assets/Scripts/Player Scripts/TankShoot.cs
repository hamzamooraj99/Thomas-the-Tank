using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TankShoot : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField, HideInInspector] WeaponData weaponData;
    public float timeBetweenShooting, timeBetweenShots;
    public int bulletsShot;
    private bool shooting, readyToShoot, reloading;

    [Header("Player References")]
    [SerializeField] Rigidbody playerRB;
    [SerializeField] Camera fpsCam;
    [SerializeField] Transform spawnPoint;

    [Header("UI References")]
    [SerializeField] Toggle[] ammoList;
    [SerializeField] TMP_Text ammoCount;
    [SerializeField] Slider reloadBar;
    [SerializeField] GameObject reloadBarObject;
    [SerializeField] TMP_Text noAmmoWarning;
    private Coroutine noAmmoWarningFlash;
    private bool isFlashing = false;

    [Header("Audio Clips")]
    [SerializeField] AudioClip cannonSound;

    [SerializeField] public GameObject muzzleFlash;

    [HideInInspector] public bool allowInvoke = true;

    private void Start()
    {
        PlayerTankInfo playerTankInfo = GetComponentInParent<PlayerTankInfo>();
        if(playerTankInfo != null)
            weaponData = playerTankInfo.weapon;
        else
            Debug.Log("TankShoot: Tank Info not found");
        weaponData.currentAmmo = weaponData.magSize;
        readyToShoot = true;
        reloadBarObject.SetActive(false);
        if(noAmmoWarning != null)
            noAmmoWarning.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _Input();
    }

    void _Input()
    {
        //SHOOT
        if(weaponData.allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if(readyToShoot && shooting && !reloading && weaponData.currentAmmo > 0){
            bulletsShot = 0;
            Shoot();
            Reloading();
        }

        if(weaponData.currentAmmo == 0 && !reloading)
            ShowNoAmmoWarning();

        // //RELOAD
        // if(Input.GetKeyDown(KeyCode.E) && weaponData.currentAmmo < weaponData.magSize && !reloading)
        //     Reloading();
        // if(readyToShoot && shooting && !reloading && weaponData.currentAmmo == 0)
        //     Reloading();
    }

    void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        //Shooting Direction
        if(Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);
        
        Vector3 directionWithoutSpread = targetPoint - spawnPoint.position;

        //Shooting Direction with Spread
        float x = Random.Range(-weaponData.spread, weaponData.spread);
        float y = Random.Range(-weaponData.spread, weaponData.spread);
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        StartCoroutine(ShowMuzzleFlash());

        //Instantiate Bullet
        GameObject currBullet = Instantiate(weaponData.bullet, spawnPoint.position, Quaternion.identity);
        currBullet.layer = spawnPoint.gameObject.layer;
        //Set Bullet damage from weapon data
        BulletDamage bulletDamage = currBullet.GetComponent<BulletDamage>();
        if(bulletDamage != null)
            bulletDamage.damage = weaponData.damage;
        //Move bullet
        currBullet.transform.forward = directionWithSpread.normalized;
        currBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * weaponData.shootForce, ForceMode.Impulse);
        currBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * weaponData.upwardForce, ForceMode.Impulse);
        SoundFXManager.instance.PlaySoundFXClip(cannonSound, transform, 0.5f);

        weaponData.currentAmmo--;
        weaponData.totalAmmo--;
        bulletsShot++;

        AmmoUI("shoot");

        if(allowInvoke){
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
            playerRB.AddForce(-directionWithSpread.normalized * weaponData.recoilForce, ForceMode.Impulse);

        }

        if (bulletsShot < weaponData.fireRate && weaponData.currentAmmo > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    void Reloading()
    {
        if(weaponData.totalAmmo > 0){
            reloading = true;
            reloadBarObject.SetActive(true);
            // SoundFXManager.instance.PlaySoundFXClip(reloadSound, transform, 0.7f);
            StartCoroutine(ReloadTimer());
        }
        
        // Invoke("Reloaded", weaponData.reloadTime);
        // Debug.Log("RELOADING");
    }

    IEnumerator ReloadTimer()
    {
        reloadBar.value = 0;
        float elapsedTime = 0;

        while(elapsedTime < weaponData.reloadTime){
            elapsedTime += Time.deltaTime;
            reloadBar.value = elapsedTime / weaponData.reloadTime;
            yield return null;
        }

        Reloaded();
    }

    void Reloaded()
    {
        if(weaponData.totalAmmo > 0){
            weaponData.currentAmmo = weaponData.magSize;
            reloadBarObject.SetActive(false);
            reloading = false;
        } else {
            Debug.Log("NO AMMO");
            reloading = false;
        }
    }

    public void AmmoUI(string state)
    {
        int index = weaponData.totalAmmo;
        ammoCount.text = index.ToString();
        if(state == "shoot")
            ammoList[index].isOn = false;
        else
            ammoList[index-1].isOn = true;
    }

    private void ShowNoAmmoWarning()
    {
        if(noAmmoWarning != null)
            FlashingEffect();
    }

    public void FlashingEffect()
    {
        if(weaponData.totalAmmo == 0){
            if(!isFlashing){
                noAmmoWarning.gameObject.SetActive(true);
                isFlashing = true;
                noAmmoWarningFlash = StartCoroutine(FlashNoAmmo());
            }
        }else{
            isFlashing = false;
            if(noAmmoWarningFlash != null){
                StopCoroutine(FlashNoAmmo());
                noAmmoWarningFlash = null;
            }
            noAmmoWarning.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashNoAmmo()
    {
        yield return new WaitForSeconds(0.2f);
        while (isFlashing){
            float duration = 0.75f;
            float elapsedTime = 0f;

            while(elapsedTime < duration){
                float t = Mathf.PingPong(elapsedTime * 2, 1);
                Color newRed = new Color(0.7450981f, 0.1882353f, 0.1882353f);
                noAmmoWarning.color = Color.Lerp(newRed, Color.white, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }
}
