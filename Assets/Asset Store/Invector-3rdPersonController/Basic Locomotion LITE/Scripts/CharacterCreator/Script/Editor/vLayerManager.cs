using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[InitializeOnLoad]
public class vLayerManager : Editor
{
    static List<string> InvectorLayers = new List<string>
    {"Player"};    

    static vLayerManager()
    {
        Create();
    }

    public static void Create()
    {
        CreateLayer();        
    }

    static void CreateLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }

        List<string> list = new List<string>();
        for (int a = 0; a < layers.arraySize; a++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(a);
            list.Add(layerSP.stringValue);
        }

        for (int i = 0; i < InvectorLayers.Count; i++)
        {
            if (!list.Contains(InvectorLayers[i]))
            {
                bool canApplay = false;
                string layerName = "";
                for (int a = 0; a < layers.arraySize; a++)
                {
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(a);
                    layerName = InvectorLayers[i];
                    if (string.IsNullOrEmpty(layerSP.stringValue) && a > 7)
                    {
                        layerSP.stringValue = layerName;
                        list[a] = layerName;
                        Debug.Log("Invector Layer Manager info:\nSetting  up layers.  Layer " + a + " is now called " + layerName);
                        tagManager.ApplyModifiedProperties();
                        canApplay = true;
                        break;
                    }
                }
                if (!canApplay)
                {
                    Debug.LogWarning("Invector Layer Manager info:\nCan't Apply Layer " + layerName);
                }
            }
        }
    }   
}
