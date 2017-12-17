/// ------------------------------------------------
/// <summary>
/// Author: David Azouz
/// Date: 17/12/2017
/// ------------------------------------------------
/// Brief: Allows teachers to assess students work via a wizard.
/// 
/// viewed: 
/// 
/// TODO:
/// fix studentname hack
/// </summary>
/// ------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Question
{
    public string question = "";
    public string criteria = ""; //RECODE: string[] for multiple criteria?
    public int mark = 1; // How many marks does this question have?

    [SerializeField] bool isCorrect = false;
    public bool IsCorrect { get { return isCorrect; } set { isCorrect = value; } }

}

public class AssessmentWizard : ScriptableWizard
{
    [Header("Marks")]
    public string studentName = "Tony"; //TODO: retrieve this from the Author section of the comments.
    [SerializeField] float totalQuestionsCorrect = 0.0f;
    [SerializeField] float studentTotalMarks = 0.0f; // students score.
    [SerializeField] int totalMarks = 0; // Max marks
    [SerializeField] float studentMark = 0.0f;
    [Header("Questions")]
    public Question[] questions = new Question[totalQuestions];

    [Header("Scripts")]
    public List<MonoScript> scriptsToMark = new List<MonoScript>();
    
    //----------------------------------------------
    // Private Variables
    //----------------------------------------------
    const int passMark = 50; // Percent e.g. 50% to pass.
    const int totalQuestions = 4;
    // We know there are "totalQuestions" questions.
    string[] questionsArray = new string[]
    {
        "Can the character move?",
        "Can the character move with WASD?",
        "Incorporate Physics?",
        "Change a Material Color?"
    };
    string[] criteriaArray = new string[] //totalQuestions
    {
        "Translate",
        "Input.GetKey(KeyCode.W)",
        "AddForce",
        "material.color"
    };
    int[] marksArray = new int[]
    {
        1,
        2,
        4,
        1
    };

    [MenuItem(BlocklyPlayground.snapperPath + "Assessment Wizard &a", priority = 3)]
    public static void CreateWizard()
    {
        DisplayWizard<AssessmentWizard>("Run Assessment", "Close", "Assess");
    }

    void OnWizardCreate()
    {
        //TODO: auto populate scriptsToMark list.
        
    }

    void ResetMarks()
    {
        totalQuestionsCorrect = 0.0f;
        studentTotalMarks = 0.0f;
        totalMarks = 0;
        studentMark = 0.0f;
        studentName = SystemInfo.deviceName; //TODO: REMOVE HACK ASAP 17/12/17
        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i].IsCorrect)
            {
                questions[i].IsCorrect = false;
            }
        }

        //TODO: get Author's name - using Regex?
        
    }

    void OnWizardOtherButton()
    {
        ResetMarks();
        // Scripts to Mark boilerplate code.
        if (scriptsToMark.Count == 0)
        {
            Debug.LogWarningFormat("Please drag a script from {0} onto \"Scripts to Mark\". One student at a time.", BlocklyPlayground.snapperCodePath);
            return;
        }
        for (int i = 0; i < scriptsToMark.Count; i++)
        {
            if (scriptsToMark[i] == null)
            {
                Debug.LogError("You need a script within the \"Scripts to Mark\" list.");
                return;
            }
        }

        if (questions != null)
        {
            // Setup Questions[]
            for (int i = 0; i < questions.Length; i++)
            {
                // Keys for Editor Prefs
                string keyQ = "Q" + (i + 1).ToString();
                string keyC = "C" + (i + 1).ToString();
                string keyM = "M" + (i + 1).ToString();

                // Questions
                if (EditorPrefs.HasKey(keyQ))
                {
                    questionsArray[i] = EditorPrefs.GetString(keyQ);
                }
                else
                {
                    EditorPrefs.SetString(keyQ, questionsArray[i]);
                }
                // Criteria
                if (EditorPrefs.HasKey(keyC))
                {
                    criteriaArray[i] = EditorPrefs.GetString(keyC);
                }
                else
                {
                    EditorPrefs.SetString(keyC, criteriaArray[i]);
                }
                // Marks
                if (EditorPrefs.HasKey(keyM))
                {
                    marksArray[i] = EditorPrefs.GetInt(keyM);
                }
                else
                {
                    EditorPrefs.SetInt(keyM, marksArray[i]);
                }

                // Set up questions and criteria
                questions[i].question = questionsArray[i];
                questions[i].criteria = criteriaArray[i];
                questions[i].mark = marksArray[i];
                totalMarks += questions[i].mark;
            }

            // Loop through all our scripts to mark
            for (int i = 0; i < scriptsToMark.Count; i++)
            {
                // Search for criteria within each script
                for (int j = 0; j < questions.Length; j++)
                {
                    // If a script contains criteria suitable for a mark...
                    if (scriptsToMark[i].text.Contains(questions[j].criteria))
                    {
                        //... award a mark
                        questions[j].IsCorrect = true;
                        Debug.LogFormat("{0} has {1} correct.", studentName, questions[j].criteria);
                    }
                }
            }

            // Teacher has override to correct question if system has failed.
            for (int i = 0; i < questions.Length; i++)
            {
                if (questions[i].IsCorrect)
                {
                    totalQuestionsCorrect++;
                    studentTotalMarks += questions[i].mark;
                }
            }
            // ----------------
            studentMark = studentTotalMarks / totalMarks * 100f; // Fraction to percentage.
            string colour = studentMark < passMark ? "red" : "green";
            Debug.LogFormat("Assessment Mark: <color={0}>{1}</color>%.", colour, studentMark);

        }
    }

    void OnWizardUpdate()
    {
        helpString = "Enter scripts to assess.";
    }

}