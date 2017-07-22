///<summary>
///
/// https://docs.unrealengine.com/latest/INT/GettingStarted/FromUnity/
/// </summary>

using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.VR;


public class Category : MonoBehaviour {

    /*Component[] categories = new Component[]
        { Mesh, ParticleSystem, Physics, Physics2D, Behaviour,
            NavMeshAgent, AudioSource, VideoClip,
        Renderer, LayoutElement, VRDevice, Network }; */
    // Use this for initialization
    public void GenerateHeadings ()
    {
        var categories = new System.Type[]// UnityEngine.Object[]
        {   typeof(Mesh), typeof(ParticleSystem), typeof(Physics), typeof(Physics2D),
            typeof(Behaviour), typeof(NavMeshAgent), typeof(AudioSource), typeof(VideoClip),
            typeof(Renderer), typeof(LayoutElement), typeof(VRDevice), typeof(Network) };

        for (int i = 0; i <= categories.Length; i++)
        {
            Debug.Log(categories[i].Name.ToString());
            Debug.Log(categories[i].ReflectedType.ToString());

            if(categories[i].ReflectedType == typeof(Component))
            {
                Debug.Log("Hello");
            }

        }

    }

    void OnInspectorGUI()
    {
        if(GUILayout.Button("Generate Headings"))
        {
            GenerateHeadings();
        }
    }
}
