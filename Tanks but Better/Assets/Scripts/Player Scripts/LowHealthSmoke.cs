using UnityEngine;

public class LowHealthSmoke : MonoBehaviour
{
    [SerializeField] public Transform followTarget;

    private PlayerTankInfo playerTankInfo;
    public Renderer rend;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        playerTankInfo = GetComponentInParent<PlayerTankInfo>();
        // if(playerTankInfo == null)
        //     Debug.Log("LowHealthSmoke: PlayerTankInfo not found");
        // else
        //     Debug.Log("LowHealthSmoke: PlayerTankInfo found");
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
