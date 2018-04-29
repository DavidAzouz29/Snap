using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName= "MyQuestion", order =0)]
public class MyQuestion : ScriptableObject {
	[TextArea]
	[Header("What are we asking for?")]
	public string question = "";
	[Header("Does the code meet the criteria?")]
    public string criteria = ""; //RECODE: string[] for multiple criteria?
    public int mark = 1; // How many marks does this question have?

    [SerializeField] bool isCorrect = false;
    public bool IsCorrect { get { return isCorrect; } set { isCorrect = value; } }
}
