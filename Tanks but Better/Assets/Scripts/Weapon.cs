// using System.Collections;
// using UnityEngine;

// public class Weapon : MonoBehaviour
// {
//     [SerializeField] WeaponData data;
//     [SerializeField] Transform projectileSpawnPoint;

//     private float timeSinceLastShot;

//     void Start()
//     {
//         PlayerShoot.shootInput += Shoot;
//         PlayerShoot.reloadInput += StartReload;
//     }

//     void Update()
//     {
//         timeSinceLastShot += Time.deltaTime;
//         Debug.DrawRay(projectileSpawnPoint.position, projectileSpawnPoint.forward);
//     }

//     private bool CanShoot() => !data.reloading && timeSinceLastShot > 1f / (data.fireRate / 60f);

//     public void Shoot()
//     {
//         if(data.currentAmmo > 0){
//             if(CanShoot()){
//                 if(Physics.Raycast(projectileSpawnPoint.position, transform.forward, out RaycastHit hitInfo, data.shootForce)){
//                     Debug.Log(hitInfo.transform.name);
//                 }

//                 data.currentAmmo--;
//                 timeSinceLastShot = 0;
//                 OnGunShot();
//             }
//         }
//     }

//     public void StartReload(){
//         if(!data.reloading)
//             StartCoroutine(Reload());
//     }

//     private IEnumerator Reload()
//     {
//         data.reloading = true;
//         yield return new WaitForSeconds(data.reloadTime);
//         data.currentAmmo = data.magSize;
//         data.reloading = false;
//     }

//     void OnGunShot()
//     {
//         //FX
//     }
// }
