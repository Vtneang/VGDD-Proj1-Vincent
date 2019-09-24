using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttackInfo
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The name of the attack")]
    private string m_Name;
    public string Name
    {
        get
        {
            return m_Name;
        }
    }
    [SerializeField]
    [Tooltip("The button to call the attack. The button must be in input settings.")]
    private string m_Button;

    public string Button
    {
        get
        {
            return m_Button;
        }

    }
    [SerializeField]
    [Tooltip("The trigger to activate this attack in the animator")]
    private string m_TriggerName;

    public string TriggerName
    {
        get
        {
            return m_TriggerName;
        }
    }
    [SerializeField]
    [Tooltip("The prefab of the game object representing the ability")]
    private GameObject m_AbilityGo;

    public GameObject AbilityGo
    {
        get
        {
            return m_AbilityGo;
        }
    }
    [SerializeField]
    [Tooltip("Where to spawn the abiltiy with respect to the player")]
    private Vector3 m_offset;

    public Vector3 Offset
    {
        get
        {
            return m_offset;
        }
    }
    [SerializeField]
    [Tooltip("How long to wait before this attack should be activated after the button is pressed")]
    private float m_WindUpTime;

    public float WindUpTime
    {
        get
        {
            return m_WindUpTime;
        }
    }
    [SerializeField]
    [Tooltip("How long to wait before the player can do anything")]
    private float m_FrozenTime;

    public float FrozenTime
    {
        get
        {
            return m_FrozenTime;
        }
    }
    [SerializeField]
    [Tooltip("How long the player has to wait before the ability is ready")]
    private float m_Cooldown;

    [SerializeField]
    [Tooltip("How much health this ability costs")]
    private int m_HealthCost;

    public int HealthCost
    {
        get
        {
            return m_HealthCost;
        }
    }
    [SerializeField]
    [Tooltip("How much mana this ability costs")]
    private int m_ManaCost;
    public int ManaCost
    {
        get
        {
            return m_ManaCost;
        }
    }
    [SerializeField]
    [Tooltip("The color to change to when using the ability")]
    private Color m_Color;

    public Color AbilityColor
    {
        get
        {
            return m_Color;
        }
    }
    #endregion

    #region Public Variables
    public float Cooldown
    {
        get;
        set;
    }
    #endregion

    #region Cooldown Methods
    public void ResetCooldown()
    {
        Cooldown = m_Cooldown;
    }
    public bool IsReady()
    {
        return Cooldown <= 0;
    }
    #endregion
}