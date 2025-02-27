using TMPro;
using UnityEngine;

public class KillCounterManager : MonoBehaviour
{
    public static KillCounterManager instance;
    public TMP_Text killCounter;

    public int killCount = 0;

    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);

        killCounter.text = killCount.ToString();
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillCounterUI();
    }

    private void UpdateKillCounterUI()
    {
        killCounter.text = killCount.ToString();
    }
}
