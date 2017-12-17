/// ------------------------------------------------
/// <summary>
/// Author: DAVID-PC - Created with Snapper!
/// Date: 5/12/2017
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

public class PlayerMovement : MonoBehaviour {

	#region Variables

#endregion


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W))
{
  transform.Translate(Vector3.forward * Time.deltaTime * 5f);
}
if (Input.GetKey(KeyCode.S))
{
  transform.Translate(Vector3.right * Time.deltaTime * 5f);
}
if (Input.GetKey(KeyCode.E))
{
  transform.Rotate(Vector3.forward * Time.deltaTime * 90f);
}
if (Input.GetKey(KeyCode.Q))
{
  transform.Rotate(-Vector3.up * Time.deltaTime * 90f);
}

	}
}
