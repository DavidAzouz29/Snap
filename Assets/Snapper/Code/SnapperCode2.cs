/// ------------------------------------------------
/// <summary>
/// Author: DAVID-PC - Created with Snapper!
/// Date: 17/12/2017
/// ------------------------------------------------
/// Brief: *Describe the script here*
/// 
/// viewed: 
/// 
/// </summary>
/// ------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class SnapperCode2 : MonoBehaviour
{

    #region Variables
    Rigidbody rb;
    MeshRenderer meshRenderer;
    public Color color;

    #endregion


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();

    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 300f, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (ColorUtility.TryParseHtmlString("#33cc00", out color))
        {
            meshRenderer.material.color = color;
        }
        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + name + "</color>");
    }
}
