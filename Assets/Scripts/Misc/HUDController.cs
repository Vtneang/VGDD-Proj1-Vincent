using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The part of the Health decreases")]
    private RectTransform m_HealthBar;

    [SerializeField]
    [Tooltip("Current mana of player")]
    private RectTransform m_mana;
    #endregion

    #region Private Variables
    private float p_HealthBarOrigWidth;
    private float p_ManaBarwidth;
    #endregion

    #region Intialization
    private void Awake()
    {
        p_HealthBarOrigWidth = m_HealthBar.sizeDelta.x;
        p_ManaBarwidth = m_mana.sizeDelta.x;
    }
    #endregion

    #region Update Health Bar
    public void UpdateHealth(float percent)
    {
        m_HealthBar.sizeDelta = new Vector2(p_HealthBarOrigWidth * percent, m_HealthBar.sizeDelta.y);
    }
    #endregion

    #region Update Mana Bar
    public void UpdateMana(float percent)
    {
        m_mana.sizeDelta = new Vector2(p_ManaBarwidth * percent, m_mana.sizeDelta.y);
    }
    #endregion
}
