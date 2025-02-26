using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [Header("Bullet Damage")]
    [SerializeField] public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.CompareTag("Enemy")){
            collision.gameObject.TryGetComponent<EnemyTankInfo>(out EnemyTankInfo eTankInfo);
            eTankInfo.TakeDamage((int)damage);
            Destroy(gameObject);
        }
        else if(gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.gameObject.CompareTag("Player")){
            collision.gameObject.TryGetComponent<PlayerTankInfo>(out PlayerTankInfo pTankInfo);
            pTankInfo.TakeDamage((int)damage);
            Destroy(gameObject);
        }
    }
}
