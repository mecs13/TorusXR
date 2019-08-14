using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CylinderMeshGenerator
{

    public static CylinderMeshData GenerateCylinder(float height, float radius, int sectors, int layers)
    {

        CylinderMeshData meshData = new CylinderMeshData(sectors, layers);

        float heightStep = height / (float)layers;
        float angleStep = 2 * Mathf.PI / (float)sectors;
        int baseIndex;

        for (int y = 0; y <= layers; y++) 
        {
            for (int x = 0; x < sectors; x++)
            {
                float angle = angleStep * x;

                meshData.vertices[y*sectors + x] = new Vector3(radius * Mathf.Cos(angle), y * heightStep, radius * Mathf.Sin(angle));

                if (y < layers) 
                {
                    baseIndex = x + (y * sectors);
                    if (x < sectors - 1)
                    {
                        meshData.AddTriangle(baseIndex + 0, baseIndex + 1, baseIndex + 1 + sectors);
                        meshData.AddTriangle(baseIndex + 0, baseIndex + 1 + sectors, baseIndex + sectors);
                    }
                    else // wrap around the last sector in the layer
                    {
                        meshData.AddTriangle(baseIndex + 0, baseIndex - x, baseIndex - x + sectors);
                        meshData.AddTriangle(baseIndex + 0, baseIndex - x + sectors, baseIndex + sectors);
                    }
                }
                          
            }

        }

        return meshData;
    }



}


public class CylinderMeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    int triangleIndex;

    public CylinderMeshData(int sectors, int layers)
    {
        triangleIndex = 0;
        vertices = new Vector3[4 * sectors * layers];               // 1 quad per sector & layer
        triangles = new int[2 * 3 * sectors * layers];              // 2 tris per quad
    }

    public void AddTriangle(int a, int b, int c)
    {
        //Debug.Log(a +"," + b +"," + c + " (" + triangleIndex + "/" + triangles.Length +")");
        if (triangleIndex >= triangles.Length)
        {
            Debug.LogError("triangleIndex out of range (" + triangleIndex + ")");
        }
        else
        {
            triangles[triangleIndex + 0] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
            //Debug.Log("triangleIndex: " + triangleIndex + " / " + triangles.Length);
        }
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

}

