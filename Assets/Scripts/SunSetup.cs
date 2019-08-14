using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSetup : MonoBehaviour {

    [SerializeField]
    Light[] lights;

    private void Start()
    {
        lights = FindObjectsOfType<Light>();
    }
    public void SetIntensity(float intensity)
    {
        Debug.Log("Setting sun intensity " + intensity);
        for (int e = 0; e < lights.Length; e++)
        {
            if (lights[e])
            {
            lights[e].intensity = intensity;
            }
        }
    }
}
