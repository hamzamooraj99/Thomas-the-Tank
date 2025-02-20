using UnityEngine;

public class BatteryCollect : MonoBehaviour
{
    [Header("Battery Info")]
    [SerializeField] int restorationAmount = 5;

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log($"Triggered by: {collider.gameObject.name}"); // Add this line for debugging
        
        if (collider.TryGetComponent(out PlayerTankInfo pTankInfo))
        {
            if(pTankInfo.GetBattery() < 1000){
                pTankInfo.RestoreBattery(restorationAmount * 10);
                gameObject.SetActive(false);
            }
        }
        else if (collider.TryGetComponent(out EnemyTankInfo eTankInfo))
        {
            if(eTankInfo.GetBattery() < 100){
                eTankInfo.RestoreBattery(restorationAmount);
                gameObject.SetActive(false);
            }
        }
    }
}
