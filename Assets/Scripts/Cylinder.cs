using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Cylinder : MonoBehaviour {
    private CylinderMeshData cylinderMeshData;

    [SerializeField]
    Color color = Color.blue;

    public float height = 4.0f, radius = 5.0f;          // Should have get-methods

    public int sectors=72, layers=14;                   // Should have get-methods

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        cylinderMeshData = CylinderMeshGenerator.GenerateCylinder(height, radius, sectors, layers);
        DrawMesh(cylinderMeshData);
    }

    void DrawMesh(CylinderMeshData meshData)
    {
        textureRenderer.sharedMaterial.color = color;
        meshFilter.sharedMesh = meshData.CreateMesh();
    }

}
