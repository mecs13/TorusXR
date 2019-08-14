using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSetup : MonoBehaviour {

    [SerializeField]
    GameObject[] environments;

    public void SetEnvironment(int index)
    {
        if (environments.Length > index)
        {
            Debug.Log("Setting environment " + index);
            for (int e = 0; e < environments.Length; e++)
            {
                if (environments[e])
                {
                    environments[e].SetActive(e == index);
                }
            }
        } else
        {
            Debug.LogError("Environment index out of range (" + index + ")");
        }
    }
}
