using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The main info about the ability")]
    protected AbilityInfo m_info;
    #endregion

    #region Cached Components
    protected ParticleSystem cc_Ps;
    #endregion

    #region Intialization
    private void Awake()
    {
        cc_Ps = GetComponent<ParticleSystem>();
    }
    #endregion

    #region Use Methods
    public abstract void Use(Vector3 spawnPos);
    #endregion
}