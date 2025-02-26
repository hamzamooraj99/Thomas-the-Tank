using System.Collections;
using UnityEngine;

public class AmmoCollect : MonoBehaviour
{
    [Header("Battery Info")]
    [SerializeField] int restorationAmount = 1;

    [SerializeField] AudioClip collectSound;

    private void OnTriggerEnter(Collider collider)
    {        
        if (collider.TryGetComponent(out PlayerTankInfo pTankInfo))
        {
            if(pTankInfo.weapon.totalAmmo < pTankInfo.maxAmmo){
                int totalAmmo = pTankInfo.weapon.totalAmmo;
                pTankInfo.weapon.totalAmmo = Mathf.Min(totalAmmo + restorationAmount, pTankInfo.maxAmmo);
                pTankInfo.weapon.currentAmmo = Mathf.Min(pTankInfo.weapon.currentAmmo + restorationAmount, pTankInfo.weapon.magSize);
                SoundFXManager.instance.PlaySoundFXClip(collectSound, transform, 1.5f);
                gameObject.SetActive(false);
                CollectableManager.instance.Respawn(ammo:this, battery:null, delay:30f);
                TankShoot tankShoot = pTankInfo.GetComponentInChildren<TankShoot>();
                if(tankShoot != null)
                    tankShoot.AmmoUI("collect");
                    tankShoot.FlashingEffect();
            }
        }
        else if (collider.TryGetComponent(out EnemyTankInfo eTankInfo))
        {
            if(eTankInfo.weapon.totalAmmo < eTankInfo.maxAmmo){
                int totalAmmo = eTankInfo.weapon.totalAmmo;
                eTankInfo.weapon.totalAmmo = Mathf.Min(totalAmmo + restorationAmount, eTankInfo.maxAmmo);
                eTankInfo.weapon.currentAmmo = Mathf.Min(eTankInfo.weapon.currentAmmo + restorationAmount, eTankInfo.weapon.magSize);
                SoundFXManager.instance.PlaySoundFXClip(collectSound, transform, 1.5f);
                gameObject.SetActive(false);
                CollectableManager.instance.Respawn(ammo:this, battery:null, delay:30f);
            }
        }
    }
}
