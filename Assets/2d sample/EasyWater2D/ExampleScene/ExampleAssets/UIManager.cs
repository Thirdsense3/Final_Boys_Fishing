using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyWater2D;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{

    public Water2D water;

    public Text waveHeight, segments, softness, speed, acceleration, randomness, presetName;

    public Slider sWave, sSegments, sSoftness, sSpeed, sAccel, sRand;

    public string pName;

    [SerializeField]
    List<string> names;

    [SerializeField]
    List<Preset> presets;

    int presetN;

    void Awake()
    {
        presetN = 0;
    }

    

    public void SavePreset()
    {
        if (presets == null)
        {
            names = new List<string>();
            presets = new List<Preset>();
        }

        names.Add(pName);

        Preset p = new Preset();

        p.segments = water.segments;
        p.softness = water.softness;
        p.speed = water.speedMultiplier;
        p.acceleration = water.accelerationMultiplier;
        p.power = water.waveHeight;
        p.randomness = water.randomness;

        presets.Add(p);

    }

    public void GoLeft()
    {

            presetN = (presetN - 1);

            if (presetN < 0)
            {
                presetN = presets.Count - 1;
            }

        UsePreset();
    }

    public void GoRight()
    {
            presetN = (presetN + 1) % presets.Count;

        UsePreset();
    }

    public void UsePreset()
    {
        Preset p = presets[presetN];

        presetName.text = names[presetN];

        float value = p.power;
        water.waveHeight = value;
        waveHeight.text = value.ToString("0.00");
        sWave.value = value;

        value = p.speed;
        water.speedMultiplier = value;
        speed.text = value.ToString("0.00");
        sSpeed.value = value;

        value = p.acceleration;
        water.accelerationMultiplier = value;
        acceleration.text = value.ToString("0.00");
        sAccel.value = value;

        value = p.segments;
        water.segments = (int)value;
        segments.text = value.ToString("0");
        sSegments.value = value;

        value = p.softness;
        water.softness = (int)value;
        softness.text = value.ToString("0");
        sSoftness.value = value;

        value = p.randomness;
        water.randomness = value;
        randomness.text = value.ToString("0.00");
        sRand.value = value;

        water.Setup();

    }


    public void SetWaveHeight(float value)
    {
        water.waveHeight = value;
        waveHeight.text = value.ToString("0.00");
        water.Setup();
    }

    public void SetSpeed(float value)
    {
        water.speedMultiplier = value;
        speed.text = value.ToString("0.00");
        water.Setup();
    }

    public void SetAcceleration(float value)
    {
        water.accelerationMultiplier = value;
        acceleration.text = value.ToString("0.00");
        water.Setup();
    }

    public void SetSegments(float value)
    {
        water.segments = (int)value;
        segments.text = value.ToString("0");
        water.Setup();
    }

    public void SetSoftness(float value)
    {
        water.softness = (int)value;
        softness.text = value.ToString("0");
        water.Setup();
    }

    public void SetRandomness(float value)
    {
        water.randomness = value;
        randomness.text = value.ToString("0.00");
        water.Setup();
    }


}

[System.Serializable]
public class Preset
{
    public float segments, softness, speed, acceleration, power, randomness;
}

#if UNITY_EDITOR

[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{

    UIManager m;

    void OnEnable()
    {
        m = (UIManager)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("SavePreset"))
        {
            m.SavePreset();
        }
    }

}

#endif