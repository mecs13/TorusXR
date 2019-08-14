using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSetup : MonoBehaviour {

    [SerializeField]
    Material[] materials;

    public void SetSkyboxMaterial(int index)
    {
        if (materials.Length > index)
        {
            RenderSettings.skybox = materials[index];
        } else
        {
            Debug.LogError("Skybox index out of range (" + index + ")");
        }
    }
}
