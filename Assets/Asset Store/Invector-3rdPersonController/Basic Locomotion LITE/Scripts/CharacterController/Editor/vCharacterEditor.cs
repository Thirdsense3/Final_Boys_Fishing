using UnityEngine;
using UnityEditor;
using System.Collections;
using Invector.CharacterController;
using System.Reflection;

[CustomEditor(typeof(vThirdPersonMotor), true)]
public class vCharacterEditor : Editor
{
    GUISkin skin;
    SerializedObject character;
    bool showWindow;

    void OnEnable()
    {
        vThirdPersonMotor motor = (vThirdPersonMotor)target;
        var playerLayer = LayerMask.NameToLayer("Player");
        if (motor.gameObject.layer == LayerMask.NameToLayer("Default") || (playerLayer > 7 && motor.gameObject.layer != playerLayer))
        {
            PopUpInfoEditor window = ScriptableObject.CreateInstance<PopUpInfoEditor>();
            window.position = new Rect(Screen.width, Screen.height / 2, 360, 100);
            window.ShowPopup();
        }
        else if (playerLayer < 7)
        {
            vLayerManager.Create();
        }
    }

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        vThirdPersonMotor motor = (vThirdPersonMotor)target;

        if (!motor) return;

        GUILayout.BeginVertical("Basic Controller LITE by Invector", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Purchase FULL Version"))
        {
            UnityEditorInternal.AssetStore.Open("/content/59332");
        }

        EditorGUILayout.Space();

        if (motor.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please assign the Layer of the Character to 'Player'", MessageType.Warning);
        }

        if (motor.groundLayer == 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please assign the Ground Layer to 'Default' ", MessageType.Warning);
        }

        EditorGUILayout.BeginVertical();        

        base.OnInspectorGUI();
      
        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();        

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }   
}

public class PopUpInfoEditor : EditorWindow
{
    GUISkin skin;
    Vector2 rect = new Vector2(360, 100);

    void OnGUI()
    {
        this.titleContent = new GUIContent("Warning!");
        this.minSize = rect;

        EditorGUILayout.HelpBox("Please assign your ThirdPersonController to the Layer 'Player'.", MessageType.Warning);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("OK", GUILayout.Width(80), GUILayout.Height(20)))
            this.Close();
    }
}