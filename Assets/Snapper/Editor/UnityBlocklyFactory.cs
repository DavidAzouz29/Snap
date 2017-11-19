///<summary>
/// Author: David Azouz
/// Date: 05/11/17
/// -------------------------------------------------
/// Brief: Using the built-in Unity's in-editor WebView
/// 
/// viewed: 
/// https://stackoverflow.com/questions/35021125/how-to-write-to-a-file-using-streamwriter
/// https://msdn.microsoft.com/en-us/library/system.io.streamwriter(v=vs.110).aspx
/// https://msdn.microsoft.com/en-us/library/system.io.streamreader(v=vs.110).aspx
/// 
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class UnityBlocklyFactory : ScriptableWizard// EditorWindow  
{
    public string filePath = Path.Combine(Application.streamingAssetsPath, "MyFile");
    public string result = "";
    IEnumerator Example()
    {
        if (filePath.Contains("://"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.Send(); // SendWebRequest();
            result = www.downloadHandler.text;
        }
        else
            result = File.ReadAllText(filePath);
    }

    public int Id = 0;
    public int dOB = 0;
    public new string name = "";
    public int age = 0;
    //
    public string cod = "";
    public string credits = "";

    public string[] category;// = { "", "" };

    // Use this for initialization
    void Init ()
    {
        // Use Reflection to find Categories via Namespaces and other hacky means.


        // Categories
        // Using directive will take care of Close() -ing the Stream.
        AssetDatabase.CreateFolder("Assets", "StreamingAssets");
        for (int i = 0; i < category.Length; i++)
        {
            //filePath = Path.Combine(Application.streamingAssetsPath, category[i]);
            string guid = AssetDatabase.CreateFolder(Application.streamingAssetsPath, category[i]);// filePath.Split('/').Last());
            // update path
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            // 
            using (var fileStream = new FileStream(System.String.Format("{0}.js", category[i]), FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("ID: " + Id);
                streamWriter.WriteLine("DOB: " + dOB);
                streamWriter.WriteLine("Name: " + name);
                streamWriter.WriteLine("Age: " + age);
            }
        }
        AssetDatabase.Refresh();

        

        // Using directive will take care of Close() -ing the Stream.
        using (var memoryStream = new MemoryStream())
        using (var writer = new StreamWriter(memoryStream))
        {
            // Various for loops etc as necessary that will ultimately do this:
            writer.Write("...");
        }
    }

    // Approach
    void SaveData()
    {
        using (var fileStream = new FileStream(System.String.Format("Person{0}.txt", Id), FileMode.OpenOrCreate))
        using (var streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.WriteLine("ID: " + Id);
            streamWriter.WriteLine("DOB: " + dOB);
            streamWriter.WriteLine("Name: " + name);
            streamWriter.WriteLine("Age: " + age);
        }

        // Subclass
        //base.SaveData();
        using (var fileStream = new FileStream(System.String.Format("Person{0}.txt", Id), FileMode.Append))
        using (var streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.WriteLine("Cod: " + cod);
            streamWriter.WriteLine("Credits: " + credits);
        }
    }
}
