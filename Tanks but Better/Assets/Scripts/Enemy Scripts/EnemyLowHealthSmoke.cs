using UnityEngine;

public class EnemyLowHealthSmoke : MonoBehaviour
{
    [SerializeField] public Transform followTarget;

    private EnemyTankInfo playerTankInfo;
    public Renderer rend;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        playerTankInfo = GetComponentInParent<EnemyTankInfo>();
        // if(playerTankInfo == null)
            // Debug.Log("LowHealthSmoke: EnemyTankInfoTankInfo not found");
        // else
            // Debug.Log("LowHealthSmoke: EnemyTankInfoTankInfo found");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followTarget.position;
        if(playerTankInfo.GetBattery() < 200)
        {
            rend.enabled = true;
            // Debug.Log("LowHealthSmoke: Low health detected");
        }
        else if(playerTankInfo.GetBattery() <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            rend.enabled = false;
            // Debug.Log("LowHealthSmoke: Health above 20%");
        }
    }
}

