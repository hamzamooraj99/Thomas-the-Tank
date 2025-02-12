using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TankShoot : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] WeaponData weaponData;
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
    [SerializeField] Slider batterySlider;

    [HideInInspector] public bool allowInvoke = true;

    private void Awake()
    {
        weaponData = Instantiate(weaponData);
        weaponData.currentAmmo = weaponData.magSize;
        readyToShoot = true;
        reloadBarObject.SetActive(false);
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

        //Instantiate Bullet
        GameObject currBullet = Instantiate(weaponData.bullet, spawnPoint.position, Quaternion.identity);
        //Move bullet
        currBullet.transform.forward = directionWithSpread.normalized;
        currBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * weaponData.shootForce, ForceMode.Impulse);
        currBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * weaponData.upwardForce, ForceMode.Impulse);

        weaponData.currentAmmo--;
        weaponData.totalAmmo--;
        bulletsShot++;

        AmmoUI();

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
        reloading = true;
        reloadBarObject.SetActive(true);
        StartCoroutine(ReloadTimer());
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

    private void AmmoUI()
    {
        int index = weaponData.totalAmmo;
        ammoCount.text = index.ToString();
        ammoList[index].isOn = false;
    }
}
