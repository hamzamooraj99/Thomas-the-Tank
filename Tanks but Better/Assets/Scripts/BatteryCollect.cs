using System.Collections;
using UnityEngine;

public class BatteryCollect : MonoBehaviour
{
    [Header("Battery Info")]
    [SerializeField] int restorationAmount = 5;

    [SerializeField] AudioClip collectSound;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerTankInfo pTankInfo))
        {
            if(pTankInfo.GetBattery() < 1000){
                pTankInfo.RestoreBattery(restorationAmount * 10);
                SoundFXManager.instance.PlaySoundFXClip(collectSound, transform, 1f);
                gameObject.SetActive(false);
                CollectableManager.instance.Respawn(ammo:null, battery:this, delay:30f);
            }
        }
        else if (collider.TryGetComponent(out EnemyTankInfo eTankInfo))
        {
            if(eTankInfo.GetBattery() < 100){
                eTankInfo.RestoreBattery(restorationAmount);
                SoundFXManager.instance.PlaySoundFXClip(collectSound, transform, 1f);
                gameObject.SetActive(false);
                CollectableManager.instance.Respawn(ammo:null, battery:this, delay:30f);
            }
        }
    }
}
