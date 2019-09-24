using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager singleton;


    #region Private Variable
    private int m_CurrScore;
    #endregion

    #region Intialization
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
        m_CurrScore = 0;
    }
    #endregion

    #region Score Methods
    public void IncreaseScore(int amount)
    {
        m_CurrScore += amount;

    }

    private void UpdateHighScore()
    {
        if (!PlayerPrefs.HasKey("HS"))
        {
            PlayerPrefs.SetInt("HS", m_CurrScore);
            return;
        }

        int hs = PlayerPrefs.GetInt("HS");
        if (hs < m_CurrScore)
        {
            PlayerPrefs.SetInt("HS", m_CurrScore);
        }
    }
    #endregion

    #region Destruction
    private void OnDisable()
    {
        UpdateHighScore();
    }
    #endregion

}
