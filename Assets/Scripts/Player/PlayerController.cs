using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("How fast a player should be moving")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("The transfrom of the camera following the Player")]
    private Transform m_CameraTransform;

    [SerializeField]
    [Tooltip("A list of all attacks and info about them")]
    private PlayerAttackInfo[] m_attacks;

    [SerializeField]
    [Tooltip("Amount of health that the player starts with")]
    private int m_Health;

    [SerializeField]
    [Tooltip("The HUD script")]
    private HUDController m_HUD;

    [SerializeField]
    [Tooltip("Amount of mana the player starts with")]
    private int m_Mana;

    [SerializeField]
    [Tooltip("Amount of Mana Regenerated every TimeToRegen seconds")]
    private float m_ManaGain;

    [SerializeField]
    [Tooltip("Time till mana is regenerated")]
    private int m_TimeToRegen;
    #endregion

    #region Cached References
    private Animator cr_anim;
    private Renderer cr_Renderer;
    #endregion

    #region Cached Components
    private Rigidbody cc_Rb;
    #endregion

    #region Private Variables
    // The current move direction of the player. Doesn't inlcude magnitude
    private Vector2 p_Velocity;

    // In order to do anything, we must not be frozen (timer must be 0)
    private float p_FrozenTimer;

    // The default color. Cached so we can switch between colors.
    private Color p_DefaultColor;

    // The current amount of health the player has
    private float p_CurrHealth;

    // The current amount of mana the player has
    private float p_CurrMana;

    // The amount of mana regenerated per x seconds
    private float p_ManaRegen;

    //The time to regen a set amount of mana
    private float p_ManaTimer;

    //Time elapsed to help track ManaTimer;
    private float p_elaspedtime;
    #endregion

    #region Initialization 
    private void Awake()
    {
        p_Velocity = Vector2.zero;
        cc_Rb = GetComponent<Rigidbody>();
        cr_anim = GetComponent<Animator>();
        cr_Renderer = GetComponentInChildren<Renderer>();
        p_DefaultColor = cr_Renderer.material.color;
        p_elaspedtime = 0;
        p_CurrHealth = m_Health;
        p_CurrMana = m_Mana;
        p_ManaRegen = m_ManaGain;
        p_ManaTimer = m_TimeToRegen;
        p_FrozenTimer = 0;

        for (int i = 0; i < m_attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_attacks[i];
            attack.Cooldown = 0;

            if (attack.WindUpTime > attack.FrozenTime)
            {
                Debug.LogError(attack.Name + " has a wind up time larger than the time the player is frozen for");
            }
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

    #region Main Update
    private void Update()
    {


        if (p_FrozenTimer > 0)
        {
            p_Velocity = Vector2.zero;
            p_FrozenTimer -= Time.deltaTime;
            return;
        }
        else
        {
            p_FrozenTimer = 0;
        }

        if (p_ManaTimer > 0)
        {
            p_elaspedtime += Time.fixedDeltaTime;
            p_ManaTimer -= p_elaspedtime;
        }
        else
        {
            IncreaseMana(p_ManaRegen);
            p_ManaTimer = m_TimeToRegen;
            p_elaspedtime = 0;
        }

        //Abiltiy Use
        for (int i = 0; i < m_attacks.Length; i++)
        {
            PlayerAttackInfo attack = m_attacks[i];

            if (attack.IsReady())
            {
                if (Input.GetButtonDown(attack.Button))
                {
                    p_FrozenTimer = attack.FrozenTime;
                    if (p_CurrMana >= attack.ManaCost)
                    {
                        DecreaseMana(attack.ManaCost);
                    } else if (p_CurrMana > 0 && p_CurrMana < attack.ManaCost)
                    {
                        float rest = attack.ManaCost - p_CurrMana;
                        DecreaseMana(p_CurrMana);
                        DecreaseHealth(rest);
                    }
                    else
                    {
                        DecreaseHealth(attack.HealthCost);
                    }
                    StartCoroutine(UseAttack(attack));
                    break;
                }
            }
            else if (attack.Cooldown > 0)
            {
                attack.Cooldown -= Time.deltaTime;
            }
        }

        // Set how hard Player is pushing movement buttons
        float forward = Input.GetAxis("Vertical");
        float right = Input.GetAxis("Horizontal");

        //Updating the Animation
        cr_anim.SetFloat("Speed", Mathf.Clamp01(Mathf.Abs(forward) + Mathf.Abs(right)));

        // Updating velocity
        float moveThreshold = 0.3f;

        if (forward > 0 && forward < moveThreshold)
        {
            forward = 0;
        }
        else if (forward < 0 && forward > -moveThreshold)
        {
            forward = 0;
        }
        if (right > 0 && right < moveThreshold)
        {
            right = 0;
        }
        else if (right < 0 && right > -moveThreshold)
        {
            right = 0;
        }
        p_Velocity.Set(right, forward);
    }

    private void FixedUpdate()
    {
        // Update the position of the Player
        cc_Rb.MovePosition(cc_Rb.position + m_Speed * Time.fixedDeltaTime * transform.forward * p_Velocity.magnitude);

        // Update the rotation of the Player
        cc_Rb.angularVelocity = Vector3.zero;

        if (p_Velocity.sqrMagnitude > 0)
        {
            float angleToRotCam = Mathf.Deg2Rad * Vector2.SignedAngle(Vector2.up, p_Velocity);
            Vector3 camForward = m_CameraTransform.forward;
            Vector3 newRot = new Vector3(Mathf.Cos(angleToRotCam) * camForward.x - Mathf.Sin(angleToRotCam) * camForward.z, 0,
                Mathf.Cos(angleToRotCam) * camForward.z + Mathf.Sin(angleToRotCam) * camForward.x);
            float theta = Vector3.SignedAngle(transform.forward, newRot, Vector3.up);
            cc_Rb.rotation = Quaternion.Slerp(cc_Rb.rotation, cc_Rb.rotation * Quaternion.Euler(0, theta, 0), 0.2f);
        }
    }

    #endregion

    #region Health/Dying/Mana Method

    private IEnumerator UseAttack(PlayerAttackInfo attack)
    {
        cc_Rb.rotation = Quaternion.Euler(0, m_CameraTransform.eulerAngles.y, 0);
        cr_anim.SetTrigger(attack.TriggerName);
        IEnumerator toColor = ChangeColor(attack.AbilityColor, 10);
        StartCoroutine(toColor);
        yield return new WaitForSeconds(attack.WindUpTime);

        Vector3 offset = transform.forward * attack.Offset.z + transform.right * attack.Offset.x + transform.up * attack.Offset.y;
        GameObject go = Instantiate(attack.AbilityGo, transform.position + offset, cc_Rb.rotation);
        go.GetComponent<Ability>().Use(transform.position + offset);

        StopCoroutine(toColor);
        StartCoroutine(ChangeColor(p_DefaultColor, 50));
        yield return new WaitForSeconds(attack.Cooldown);

        attack.ResetCooldown();

    }

    public void DecreaseHealth(float amount)
    {
        p_CurrHealth -= amount;
        m_HUD.UpdateHealth(1.0f * p_CurrHealth / m_Health);
        if (p_CurrHealth <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void DecreaseMana(float amount)
    {
        p_CurrMana -= amount;
        m_HUD.UpdateMana(1.0f * p_CurrMana / m_Mana);

    }

    public void IncreaseMana(float amount)
    {
        p_CurrMana += amount;
        if (p_CurrMana >= m_Mana)
        {
            p_CurrMana = m_Mana;
        }
        m_HUD.UpdateMana(1.0f * p_CurrMana / m_Mana);
    }

    public void IncreaseHealth(int amount)
    {
        p_CurrHealth += amount;
        if (p_CurrHealth >= m_Health)
        {
            p_CurrHealth = m_Health;
        }
        m_HUD.UpdateHealth(1.0f * p_CurrHealth / m_Health);
    }
    #endregion

    #region Misc Methods
    private IEnumerator ChangeColor(Color newcolor, float speed)
    {
        Color curColor = cr_Renderer.material.color;
        while (curColor != newcolor)
        {
            curColor = Color.Lerp(curColor, newcolor, speed / 100);
            cr_Renderer.material.color = curColor;
            yield return null;
        }
    }
    #endregion

    #region Collision Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPill"))
        {
            IncreaseHealth(other.GetComponent<HealthPill>().HealthGain);
            Destroy(other.gameObject);
        }
    }
    #endregion
}