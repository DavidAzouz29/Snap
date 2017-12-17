/// ------------------------------------------------
/// <summary>
/// Author: DAVID-PC - Created with Snapper!
/// Date: 16/12/2017
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

[RequireComponent(typeof(Rigidbody))]
public class SnapperCode1 : MonoBehaviour
{

    #region Variables
    Rigidbody rb;
    MeshRenderer meshrenderer;
    public Color color;

    #endregion


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshrenderer = GetComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 5f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-Vector3.forward * Time.deltaTime * 5f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-Vector3.right * Time.deltaTime * 5f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * 5f);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(Vector3.up * Time.deltaTime * 5f);
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.Translate(-Vector3.up * Time.deltaTime * 5f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 300f, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(-Vector3.up * Time.deltaTime * 90f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 90f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ColorUtility.TryParseHtmlString("#ff9900", out color))
        {
            meshrenderer.material.color = color;
        }
        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + name + "</color>");

    }
}
