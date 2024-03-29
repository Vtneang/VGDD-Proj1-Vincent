﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    #region Editior Variable
    [SerializeField]
    [Tooltip("The Player to follow")]
    private Transform m_PlayerTransform;

    [SerializeField]
    [Tooltip("The offset from the player's orgin to the camera")]
    private Vector3 m_Offset;

    [SerializeField]
    [Tooltip("How fast the player rotates the camera left and right")]
    private float m_RotationSpeed = 10;
    #endregion

    #region Main Updates
    private void LateUpdate()
    {
        Vector3 newPos = m_PlayerTransform.position + m_Offset;

        transform.position = Vector3.Slerp(transform.position, newPos, 1);

        float RotationAmount = m_RotationSpeed * Input.GetAxis("Mouse X");
        transform.RotateAround(m_PlayerTransform.position, Vector3.up, RotationAmount);

        m_Offset = transform.position - m_PlayerTransform.position;
    }
    #endregion
}
