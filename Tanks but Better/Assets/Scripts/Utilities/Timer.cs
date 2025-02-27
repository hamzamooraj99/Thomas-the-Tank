using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float timeDuration = 5f * 60f;
    private float timer;
    [SerializeField] private TextMeshProUGUI tenthMinute;
    [SerializeField] private TextMeshProUGUI onethMinute;
    [SerializeField] private TextMeshProUGUI tenthSecond;
    [SerializeField] private TextMeshProUGUI onethSecond;

    private bool gameOverTriggered = false;
    private PlayerTankInfo tank;

    void Start()
    {
        ResetTimer();
        tank = FindFirstObjectByType<PlayerTankInfo>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)){
            timer = 5f;
            UpdateTimerDisplay(timer);
        }
        
        if(timer > 0){
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer);
        }else if(!gameOverTriggered){
            gameOverTriggered = true;
            tank.PlayerDied();
        }else
            Flash();
    }

    private void ResetTimer()
    {
        timer = timeDuration;
    }

    private void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        string currTime = string.Format("{00:00}{1:00}", minutes, seconds);
        // Debug.Log(currTime);
        tenthMinute.text = currTime[0].ToString();
        onethMinute.text = currTime[1].ToString();
        tenthSecond.text = currTime[2].ToString();
        onethSecond.text = currTime[3].ToString();
    }

    private void Flash()
    {

    }
}
