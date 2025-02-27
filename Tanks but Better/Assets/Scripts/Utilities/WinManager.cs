using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public static WinManager instance;
    private PlayerTankInfo playerTank;
    private bool hasWon = false;
    private int enemyKillCount = 0;

    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        playerTank = FindFirstObjectByType<PlayerTankInfo>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) // Press 'K' to increase kill count
        {
            KillCounterManager.instance.AddKill();
            enemyKillCount++;
            Debug.Log($"Kill Count: {enemyKillCount}/5");
        }
        
        if(enemyKillCount >= 5 && playerTank.GetBattery() >= 800 && !hasWon){
            hasWon = true;
            WinGame();
        }
    }

    public void EnemyDestroyed()
    {
        enemyKillCount++;
    }

    void WinGame()
    {
        LevelManager.instance.GameWin();
    }
}
