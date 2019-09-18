using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Editior Variables
    [SerializeField]
    [Tooltip("How much health the enemy has")]
    private int m_MaxHealth;

    [SerializeField]
    [Tooltip("How fast the enemey can move")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("Approx. amount of damage dealt per frame")]
    private float m_Damage;

    [SerializeField]
    [Tooltip("Explositon that occurs after enemey dies")]
    private ParticleSystem m_DeathExplosion;

    [SerializeField]
    [Tooltip("The probability that this enemy drops a health pill")]
    private float m_HealthPillDropRate;

    [SerializeField]
    [Tooltip("The type of health pill this enemy drops")]
    private GameObject m_HealthPill;

    [SerializeField]
    [Tooltip("How many points killing this enemey provides")]
    private int m_Score;

    #endregion

    #region Private Variables
    private float p_CurrHealth;

    #endregion

    #region Cached Components
    private Rigidbody cc_Rb;
    #endregion

    #region Cached References
    private Transform cr_Player;
    #endregion

    #region Initialization
    private void Awake()
    {
        p_CurrHealth = m_MaxHealth;

        cc_Rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cr_Player = FindObjectOfType<PlayerController>().transform;
    }
    #endregion

    #region Main Updates
    private void FixedUpdate()
    {
        Vector3 dir = cr_Player.position - transform.position;
        dir.Normalize();
        cc_Rb.MovePosition(cc_Rb.position + dir * m_Speed * Time.fixedDeltaTime);
    }
    #endregion

    #region Collision Method
    private void OnCollisionStay(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().DecreaseHealth(m_Damage);
        }
    }
    #endregion

    #region Health Methods
    public void DecreaseHealth(float amount)
    {
        p_CurrHealth -= amount;
        if (p_CurrHealth <= 0)
        {
            ScoreManager.singleton.IncreaseScore(m_Score);
            if (Random.value < m_HealthPillDropRate)
            {
                Instantiate(m_HealthPill, transform.position, Quaternion.identity);
            }
            Instantiate(m_DeathExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    #endregion
}
