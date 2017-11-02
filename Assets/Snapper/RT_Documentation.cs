// https://gist.github.com/JustinFincher/a8694e78c4c26e8cc22d95ef2a8a5c7a

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
using RavenTech;

namespace RavenTech.Editor
{
    /// <summary>
    /// 文档浏览器窗口 
    /// </summary>
    [InitializeOnLoad]
    public class RT_Documentation : ScriptableObject
    {
        static BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        [MenuItem("Window/RavenTech/Documentation")]
        static void Open()
        {

#if (UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)

		 var type = Types.GetType ("UnityEditor.Web.WebViewEditorWindow", "UnityEditor.dll");
        var methodInfo = type.GetMethod ("Create", Flags);
        methodInfo = methodInfo.MakeGenericMethod (typeof(RT_Documentation));

#elif UNITY_5_4

		var type = Types.GetType ("UnityEditor.Web.WebViewEditorWindowTabs", "UnityEditor.dll");
        var methodInfo = type.GetMethod ("Create", Flags);
        methodInfo = methodInfo.MakeGenericMethod (type);

#else

            System.Type type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.Web.WebViewEditorWindowTabs");
            var methodInfo = type.GetMethod("Create", Flags);
            methodInfo = methodInfo.MakeGenericMethod(type);

#endif

            string path = Directory.GetParent(Application.dataPath).FullName + "/Doc/html/index.html";
            methodInfo.Invoke(null, new object[]
            {
                "Documentation",
                path,
                200, 530, 800, 600
            });
        }
    }
}