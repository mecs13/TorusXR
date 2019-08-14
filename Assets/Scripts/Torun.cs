using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Torun : MonoBehaviour {
    private TorunMeshData torunMeshData;

    //[SerializeField]
    Cylinder parentCylinder;

    [SerializeField]
    Color normalColor, pressedColor;    // borderColor, 

    [SerializeField]
    Texture2D texture;

    [SerializeField]
    Vector2 textureTiling, textureOffset;

    [SerializeField]
    int unitWidth, unitHeight;

    [SerializeField]
    int cornerSegments = 1;

    [SerializeField]
    [Range(0f,1f)]
    float roundness;

    [SerializeField]
    TorunMeshGenerator.TorunStyle style;

    Renderer textureRenderer;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    Vector3 targetPosition;
    Quaternion targetRotation;
    float targetTime;

    private void Awake()
    {
        SetTargetPosition(transform.position.y, transform.localRotation.y, 0f, 0f);
        OnValidate();
        StartCoroutine(AnimatePosition(50f));
    }

    private void OnValidate()
    {
        if (!textureRenderer || !meshFilter || !meshRenderer)
            Setup();

        if (!HasParentCylinder())
        {
            Debug.LogError("No parent cylinder in " + this.name);
            return;
        }

        torunMeshData = TorunMeshGenerator.GenerateTorun(parentCylinder, unitWidth, unitHeight, roundness, cornerSegments, style);
        DrawMesh(torunMeshData);

        //transform.parent = parentCylinder.transform;
    }

    void DrawMesh(TorunMeshData meshData)
    {
        textureRenderer.materials[0].color = normalColor;
        textureRenderer.materials[0].mainTexture = texture;
        textureRenderer.materials[0].SetTextureScale("_MainTex", textureTiling);
        textureRenderer.materials[0].SetTextureOffset("_MainTex", textureOffset);
        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    void Setup()
    {
        textureRenderer = GetComponent<Renderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

    }

    // Returns true if parent cylinder is set
    bool HasParentCylinder()
    {
        if (transform.parent)
        {
            parentCylinder = GetComponentInParent<Cylinder>();
        }
        return parentCylinder != null;
    }

    public void SetParentCylinder(Cylinder newParent)
    {
        parentCylinder = newParent;
        OnValidate();
    }

    // Returns the height of the torun in pixels
    public float GetHeight()
    {
        return unitHeight * parentCylinder.height / parentCylinder.layers;
    }

    // Returns the width of the torun in degrees
    public float GetAngleWidth()
    {   //2f * Mathf.PI * 
        return 360f * unitWidth / parentCylinder.sectors;
    }

    public void SetTargetPosition(float targetPositionY, float targetAngleY, float targetDepth, float duration)
    {
        this.targetPosition = new Vector3(targetDepth, targetPositionY, 0);
        this.targetRotation = Quaternion.Euler(0, targetAngleY, 0);
        this.targetTime = Time.time + duration;
    }

    IEnumerator AnimatePosition(float fps)
    {
        float sleepTime = 1f / fps;
        float timeRemaining;

        // Only start an animation coroutine if I am the child of another torun that has an expand/collapse script attach
        if(transform.parent) {
            if (transform.parent.GetComponent<ExpandCollapse>())
            {
                while (true)
                {
                    timeRemaining = this.targetTime - Time.time;
                    if (timeRemaining > sleepTime)
                    {
                        transform.localPosition = Vector3.Lerp(transform.localPosition, this.targetPosition, (1f - timeRemaining) / timeRemaining);
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, this.targetRotation, (1f - timeRemaining) / timeRemaining);
                    }
                    else if (timeRemaining >= -5f)    // Ensure we always end up exactly on target
                    {
                        transform.localPosition = this.targetPosition;
                        transform.localRotation = this.targetRotation;
                    }
                    yield return new WaitForSeconds(sleepTime);
                }
            }
        }
    }

    // This is temporary - should have HoverStart, HoverEnd, PressStart and PressEnd methods
    IEnumerator _SetPressedColor(float duration)
    {
        SetPressedColor();
        yield return new WaitForSeconds(duration);
        SetNormalColor();
    }

    public void SetPressedColor(float duration)
    {
        StartCoroutine(_SetPressedColor(duration));
    }

    public void SetPressedColor()
    {
        textureRenderer.materials[0].color = pressedColor;
    }

    public void SetNormalColor()
    {
        textureRenderer.materials[0].color = normalColor;
    }
}
