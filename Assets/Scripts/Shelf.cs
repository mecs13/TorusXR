using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Shelf : MonoBehaviour {

    private ShelfMeshData shelfMeshData;

    //[SerializeField]
    Cylinder parentCylinder;

    [SerializeField]
    Color color = Color.blue;

    [SerializeField]
    int unitWidth, unitHeight, columns, rows, cornerSegments;
    [SerializeField]
    float extrusion, thickness;

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {

        if (!HasParentCylinder())
        {
            Debug.LogError("No parent cylinder in " + this.name);
            return;
        }

        shelfMeshData = ShelfMeshGenerator.GenerateShelf(parentCylinder, unitWidth, unitHeight, columns, rows, extrusion, thickness, cornerSegments); 
        DrawMesh(shelfMeshData);
        transform.parent = parentCylinder.transform;
    }

    void DrawMesh(ShelfMeshData meshData)
    {
        textureRenderer.materials[0].color = color;

        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    // Returns true if parent cylinder is set
    bool HasParentCylinder()
    {
        if(transform.parent)
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
}
