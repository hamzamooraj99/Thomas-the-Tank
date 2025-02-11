using UnityEngine;

public class TankShoot : MonoBehaviour
{
    [SerializeField] WeaponData weaponData;

    public float timeBetweenShooting, timeBetweenShots;

    public int bulletsShot;

    private bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform spawnPoint;

    public bool allowInvoke = true;

    private void Awake()
    {
        weaponData = Instantiate(weaponData);
        weaponData.currentAmmo = weaponData.magSize;
        weaponData.totalAmmo -= weaponData.magSize;
        readyToShoot = true;
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
        }

        //RELOAD
        if(Input.GetKeyDown(KeyCode.E) && weaponData.currentAmmo < weaponData.magSize && !reloading)
            Reloading();
        if(readyToShoot && shooting && !reloading && weaponData.currentAmmo == 0)
            Reloading();
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
        bulletsShot++;

        if(allowInvoke){
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
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
        Invoke("Reloaded", weaponData.reloadTime);
        Debug.Log("RELOADING");
    }

    void Reloaded()
    {
        if(weaponData.totalAmmo > 0){
            weaponData.currentAmmo = weaponData.magSize;
            weaponData.totalAmmo -= weaponData.magSize;
            reloading = false;
        } else {
            Debug.Log("NO AMMO");
            reloading = false;
        }
    }
}
