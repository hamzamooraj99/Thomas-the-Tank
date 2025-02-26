using System;
using System.Collections;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor.Recorder.Encoder;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTankInfo : MonoBehaviour
{
    [SerializeField] TankData tankData;

    private string tankName;
    private int currBattery;
    private int maxBattery;
    private bool isPlayable;
    private EnemyAI tankAI;

    [Header("Enemy UI References")]
    [SerializeField] Slider healthBar;
    [SerializeField] Camera cameraPosition;
    [SerializeField] Transform enemy;
    [SerializeField] Vector3 offset;

    [HideInInspector] public WeaponData weapon;
    [HideInInspector] public int maxAmmo;

    [SerializeField] public bool debug = false;

    public delegate void DamageTakenHandler();
    public event DamageTakenHandler onDamageTaken;

    #region Getters
    public string GetName() => tankName;
    public int GetBattery() => currBattery;
    public bool GetPlayableFlag() => isPlayable;
    #endregion

    #region Setters
    public void SetBattery(int battery) => currBattery = battery;
    #endregion

    void Awake()
    {
        InitialiseTank();

        Canvas canvas = GetComponentInChildren<Canvas>();
        healthBar = canvas.GetComponentInChildren<Slider>();
    }

    void Start()
    {
        UpdateHealthBar(currBattery);    
    }

    void Update()
    {        
        healthBar.transform.rotation = cameraPosition.transform.rotation;
        healthBar.transform.position = enemy.transform.position + offset;

        if(debug){
            if(Input.GetKeyDown(KeyCode.B) && debug){
                TakeDamage(100);
                // Debug.Log($"Battery reduced to {currBattery}");
            }
        }
    }

    void InitialiseTank()
    {
        tankData = Instantiate(tankData);
        tankName = tankData.name;
        currBattery = tankData.battery;
        maxBattery = tankData.battery;
        weapon = Instantiate(tankData.weapon);
        maxAmmo = weapon.totalAmmo;
        isPlayable = tankData.playable;
        // Debug.Log($"{currBattery} battery left");
        tankAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        currBattery -= damage;
        UpdateHealthBar(currBattery);
        // Debug.Log($"Tank taken {damage} damage. Current Battery = {currBattery}");
        if(currBattery <= 0){
            KillCounterManager.instance.AddKill();
            tankAI.Explode();
            Debug.Log($"{tankData.name} destroyed");
        }
        onDamageTaken?.Invoke();
    }

    public void RestoreBattery(int restoration)
    {
        currBattery = Mathf.Min(currBattery + restoration, 100);
    }

    private void UpdateHealthBar(int currValue)
    {
        healthBar.value = (float) currValue / maxBattery;
    }
}
