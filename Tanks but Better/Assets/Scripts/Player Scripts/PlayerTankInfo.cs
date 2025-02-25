using System;
using System.Collections;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor.Recorder.Encoder;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankInfo : MonoBehaviour
{
    [SerializeField] TankData tankData;

    [Header("UI References")]
    [SerializeField] Slider batteryBar;
    [SerializeField] TMP_Text batteryAmount;
    [SerializeField] Image batteryFill;

    [Header("Battery Colours")]
    public Color fullCharge = new Color(0f, 0.735849f, 0f);
    public Color halfCharge = new Color(0.9339623f, 0.6079912f, 0f);
    public Color lowCharge = new Color(0.754717f, 0f, 0f);

    public bool debug = false;

    private string tankName;
    private int currBattery;
    private bool isPlayable;
    private bool isFriendly;
    private bool isFlashing = false;
    private Coroutine flashRoutine;
    private VignetteManager vignetteManager;

    [HideInInspector] public WeaponData weapon;
    [HideInInspector] public int maxAmmo;

    #region Getters
    public string GetName() => tankName;
    public int GetBattery() => currBattery;
    public bool GetPlayableFlag() => isPlayable;
    public bool GetFriendlyFlag() => isFriendly;
    #endregion

    #region Setters
    public void SetBattery(int battery) => currBattery = battery;
    #endregion

    void Awake()
    {
        InitialiseTank();
    }

    //DEBUGGING PURPOSES
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K) && debug){
            TakeDamage(10);
            Debug.Log($"Battery reduced to {currBattery}");
        }
    }

    void InitialiseTank()
    {
        tankData = Instantiate(tankData);
        tankName = tankData.name;
        currBattery = tankData.battery;
        weapon = Instantiate(tankData.weapon);
        isPlayable = tankData.playable;
        isFriendly = tankData.friendly;
        maxAmmo = weapon.totalAmmo;

        if(batteryBar) batteryBar.maxValue = tankData.battery;
        UpdateHealthUI();

        vignetteManager = FindFirstObjectByType<VignetteManager>();

        // string play = isPlayabale ? "playable" : "not playable";
        // Debug.Log($"Tank {tankData.name} initialised with {currArmour} armour, {currBattery} battery and a {weapon.name}. The character is {play}");
    }

    public void TakeDamage(int damage)
    {
        currBattery -= damage;
        if(vignetteManager != null)
            vignetteManager.FlashVignette();
            
        if(currBattery <= 0){
            Destroy(gameObject);
            Debug.Log($"{tankData.name} destroyed");
        }

        UpdateHealthUI();
    }

    public void RestoreBattery(int restoration)
    {
        currBattery = Mathf.Min(currBattery + restoration, 1000);
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if(batteryBar){
            batteryBar.value = currBattery;
            if(batteryAmount) batteryAmount.text = Mathf.RoundToInt(currBattery/10).ToString();
            if(batteryFill) batteryFill.color = GetBatteryColor(currBattery / (float)tankData.battery);
        }
        FlashingEffect();
    }

    #region BATTERY EFFECTS
    private Color GetBatteryColor(float level)
    {
        if(level > 0.6f)
            return fullCharge;
        else if(level > 0.3f)
            return halfCharge;
        else
            return lowCharge;
    }

    private void FlashingEffect()
    {
        if(currBattery / (float)tankData.battery < 0.3f){
            if(!isFlashing){
                isFlashing = true;
                flashRoutine = StartCoroutine(FlashBatteryBar());
            }
        }else{
            isFlashing = false;
            if(flashRoutine != null){
                StopCoroutine(FlashBatteryBar());
                flashRoutine = null;
            }
            batteryFill.color = GetBatteryColor(currBattery / (float)tankData.battery);
        }
    }

    private IEnumerator FlashBatteryBar()
    {
        while (isFlashing){
            float duration = 0.75f;
            float elapsedTime = 0f;

            while(elapsedTime < duration){
                float t = Mathf.PingPong(elapsedTime * 2, 1);
                batteryFill.color = Color.Lerp(lowCharge, Color.white, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
    #endregion
}
