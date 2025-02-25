using System.Collections;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;

    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Respawn(AmmoCollect ammo, BatteryCollect battery, float delay)
    {
        if(ammo != null && battery == null) StartCoroutine(RespawnCoroutine(ammo, delay));
        else if(battery != null && ammo == null) StartCoroutine(RespawnCoroutine(battery, delay));
        else Debug.LogError("CollectableManager.Respawn() cannot have two collectable types");
    }

    private IEnumerator RespawnCoroutine(Component collectable, float delay)
    {
        yield return new WaitForSeconds(delay);
        collectable.gameObject.SetActive(true);
    }
}
