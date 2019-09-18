using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityInfo
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much power this attack has")]
    private int m_Power;
    public int Power
    {
        get
        {
            return m_Power;
        }
    }
    [SerializeField]
    [Tooltip("Describes how far the attack goes")]
    private float m_Range;
    public float Range
    {
        get
        {
            return m_Range;
        }
    }
    #endregion

}

