using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ShelfMeshGenerator
{

    public static ShelfMeshData GenerateShelf(Cylinder parentCylinder, int unitWidth, int unitHeight, int columns, int rows, float depth, float thickness, int cornerSegments)
    {
        ShelfMeshData shelfMeshData = GenerateShelf(parentCylinder.height, parentCylinder.radius + 0.005f, parentCylinder.sectors, parentCylinder.layers, unitWidth, unitHeight, columns, rows, depth, thickness, cornerSegments);

        return shelfMeshData;
    }

    public static ShelfMeshData GenerateShelf(float cylinderHeight, float cylinderRadius, int sectors, int layers, int unitWidth, int unitHeight, int columns, int rows, float depth, float thickness, int cornerSegments)
    {
        ShelfMeshData meshData = new ShelfMeshData(sectors, layers, rows, columns, unitWidth, cornerSegments);

        float heightStep = cylinderHeight / (float)layers;
        float angleStep = 2 * Mathf.PI / (float)sectors;

        float halfthick = 0.5f * thickness;

        int vertexIndex = 0;
        if (columns > 0 && rows > 0)
        {
            // Draw vertical shelves
            for (int x = 0; x <= columns; x++)
            {
                float angle = -angleStep * unitWidth * x;

                float yPos = 0; 
                float shelfHeight = rows * unitHeight * heightStep;

                // The outer columns need to be shorter and higher, in order to make room for the rounded corners
                if (x == 0 || x == columns) {
                    yPos = heightStep;
                    shelfHeight = (rows * unitHeight - 2) * heightStep;
                }
                DrawVerticalShelf(ref meshData, ref vertexIndex, cylinderRadius, angle, yPos, shelfHeight, depth, halfthick);

            }

            // Draw horizontal shelves
            for (int x = 0; x < columns * unitWidth; x++)
            {
                // angleA&B = the left and right angles that form the boundaries of this shelf
                float angleA = -angleStep * x;
                float angleB = -angleStep * (x + 1);

                for (int y = 0; y <= rows; y++)
                {
                    // Skip the corners
                    if ((y == 0 || y == rows) && (x == 0 || x == columns * unitWidth - 1))
                        continue;
                    DrawHorizontalShelf(ref meshData, ref vertexIndex, cylinderRadius, angleA, angleB, y * heightStep * unitHeight, depth, halfthick);

                }
            }

            // Top left corner
            DrawCorner(ref meshData, ref vertexIndex, cylinderRadius, 0, -angleStep, rows * heightStep * unitHeight - heightStep, depth, heightStep, halfthick, cornerSegments, false);
            // Top right corner
            DrawCorner(ref meshData, ref vertexIndex, cylinderRadius, -columns * angleStep * unitWidth, -columns * angleStep * unitWidth + angleStep, rows * heightStep * unitHeight - heightStep, depth, heightStep, halfthick, cornerSegments, true);
            // Bottom left corner
            DrawCorner(ref meshData, ref vertexIndex, cylinderRadius, 0, -angleStep, heightStep, depth, -heightStep, halfthick, cornerSegments, false);
            // Bottom right corner
            DrawCorner(ref meshData, ref vertexIndex, cylinderRadius, -columns * angleStep * unitWidth, -columns * angleStep * unitWidth + angleStep, heightStep, depth, -heightStep, halfthick, cornerSegments, true);
        }

        return meshData;
    }

    public static void DrawHorizontalShelf(ref ShelfMeshData meshData, ref int vertexIndex, float radius, float angleA, float angleB, float yPos, float depth, float halfthick)
    {
        // Create the vertices for a two-sided quad with zero thickness
        meshData.vertices[vertexIndex + 0] = meshData.vertices[vertexIndex + 4] = new Vector3(radius * Mathf.Cos(angleA), yPos, radius * Mathf.Sin(angleA));
        meshData.vertices[vertexIndex + 2] = meshData.vertices[vertexIndex + 6] = new Vector3(radius * Mathf.Cos(angleB), yPos, radius * Mathf.Sin(angleB));
        meshData.vertices[vertexIndex + 1] = meshData.vertices[vertexIndex + 5] = new Vector3((radius + depth) * Mathf.Cos(angleA), yPos, (radius + depth) * Mathf.Sin(angleA));
        meshData.vertices[vertexIndex + 3] = meshData.vertices[vertexIndex + 7] = new Vector3((radius + depth) * Mathf.Cos(angleB), yPos, (radius + depth) * Mathf.Sin(angleB));

        // Apply the thickness          
        meshData.vertices[vertexIndex + 0] += Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 1] += Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 2] += Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 3] += Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 4] -= Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 5] -= Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 6] -= Vector3.up * halfthick;
        meshData.vertices[vertexIndex + 7] -= Vector3.up * halfthick;

        // Top and bottom quads
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 3);
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 3, vertexIndex + 2);
        meshData.AddTriangle(vertexIndex + 4, vertexIndex + 7, vertexIndex + 5);
        meshData.AddTriangle(vertexIndex + 4, vertexIndex + 6, vertexIndex + 7);

        // Front and back quads
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 2, vertexIndex + 4);
        meshData.AddTriangle(vertexIndex + 2, vertexIndex + 6, vertexIndex + 4);
        meshData.AddTriangle(vertexIndex + 1, vertexIndex + 5, vertexIndex + 7);
        meshData.AddTriangle(vertexIndex + 1, vertexIndex + 7, vertexIndex + 3);

        vertexIndex += 8;
    }

    public static void DrawVerticalShelf(ref ShelfMeshData meshData, ref int vertexIndex, float radius, float angle, float yPos, float height, float depth, float halfthick)
    {
        // Create the vertices for a two-sided quad with zero thickness
        meshData.vertices[vertexIndex + 0] = meshData.vertices[vertexIndex + 1] = new Vector3(radius * Mathf.Cos(angle), yPos, radius * Mathf.Sin(angle));
        meshData.vertices[vertexIndex + 2] = meshData.vertices[vertexIndex + 3] = new Vector3(radius * Mathf.Cos(angle), yPos + height, radius * Mathf.Sin(angle));
        meshData.vertices[vertexIndex + 4] = meshData.vertices[vertexIndex + 5] = new Vector3((radius + depth) * Mathf.Cos(angle), yPos, (radius + depth) * Mathf.Sin(angle));
        meshData.vertices[vertexIndex + 6] = meshData.vertices[vertexIndex + 7] = new Vector3((radius + depth) * Mathf.Cos(angle), yPos + height, (radius + depth) * Mathf.Sin(angle));

        // Apply the thickness          
        meshData.vertices[vertexIndex + 0] -= LocalRight(meshData.vertices[vertexIndex + 0], halfthick);
        meshData.vertices[vertexIndex + 1] += LocalRight(meshData.vertices[vertexIndex + 1], halfthick);
        meshData.vertices[vertexIndex + 2] -= LocalRight(meshData.vertices[vertexIndex + 2], halfthick);
        meshData.vertices[vertexIndex + 3] += LocalRight(meshData.vertices[vertexIndex + 3], halfthick);
        meshData.vertices[vertexIndex + 4] -= LocalRight(meshData.vertices[vertexIndex + 4], halfthick);
        meshData.vertices[vertexIndex + 5] += LocalRight(meshData.vertices[vertexIndex + 5], halfthick);
        meshData.vertices[vertexIndex + 6] -= LocalRight(meshData.vertices[vertexIndex + 6], halfthick);
        meshData.vertices[vertexIndex + 7] += LocalRight(meshData.vertices[vertexIndex + 7], halfthick);

        // Right and left quads
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 6, vertexIndex + 4);
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 2, vertexIndex + 6);
        meshData.AddTriangle(vertexIndex + 3, vertexIndex + 1, vertexIndex + 7);
        meshData.AddTriangle(vertexIndex + 7, vertexIndex + 1, vertexIndex + 5);

        // Front and back quads
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 2);
        meshData.AddTriangle(vertexIndex + 3, vertexIndex + 2, vertexIndex + 1);
        meshData.AddTriangle(vertexIndex + 4, vertexIndex + 6, vertexIndex + 5);
        meshData.AddTriangle(vertexIndex + 6, vertexIndex + 7, vertexIndex + 5);

        vertexIndex += 8;
    }

    // Returns the a new position a distance away from v in the local right direction from v's perspective
    public static Vector3 LocalRight(Vector3 v, float distance)
    {
        Vector3 localRight = Vector3.Cross(v.normalized, Vector3.up).normalized;
        return localRight * distance;
    }

    public static void DrawCorner(ref ShelfMeshData meshData, ref int vertexIndex, float radius, float angleA, float angleB, float yPos, float depth, float height, float halfthick, int segments, bool inverted)
    {
        // Corners are drawn by creating four splines and filling them in with triangles.

        // "Start" = start of the spline
        // "End"   = target of the spline
        // "Mid"   = the half-way point between the start and the end points. This controls the curvature of the spline.
        // "Near"  = faces towards the center of the parent cylinder
        // "Far"   = faces away from the center of the parent cylinder
        // "Inner" = faces inwards towards the center of the shelf
        // "Outer" = faces outwards away from the center of the shelf

        Vector3 startNearInner, startFarInner, endNearInner, endFarInner, midNearInner, midFarInner;
        Vector3 startNearOuter, startFarOuter, endNearOuter, endFarOuter, midNearOuter, midFarOuter;

        // At first, the inner and outer vertices are in the same place
        startNearInner = startNearOuter = new Vector3(radius * Mathf.Cos(angleA), yPos, radius * Mathf.Sin(angleA));
        startFarInner = startFarOuter = new Vector3((radius + depth) * Mathf.Cos(angleA), yPos, (radius + depth) * Mathf.Sin(angleA));

        midNearInner = midNearOuter = new Vector3(radius * Mathf.Cos(angleA), yPos + height, radius * Mathf.Sin(angleA));
        midFarInner = midFarOuter = new Vector3((radius + depth) * Mathf.Cos(angleA), yPos + height, (radius + depth) * Mathf.Sin(angleA));

        endNearInner = endNearOuter = new Vector3(radius * Mathf.Cos(angleB), yPos + height, radius * Mathf.Sin(angleB));
        endFarInner = endFarOuter = new Vector3((radius + depth) * Mathf.Cos(angleB), yPos + height, (radius + depth) * Mathf.Sin(angleB));

        // Which orientation does the thickness point? (hack!)
        bool orientation = inverted ^ (height > 0);

        // Apply the thickness between "inner" and "outer" vertices
        startNearInner += (orientation ? -1 : 1) * LocalRight(startNearOuter, halfthick);
        startFarInner += (orientation ? -1 : 1) * LocalRight(startFarOuter, halfthick);
        startNearOuter += (orientation ? 1 : -1) * LocalRight(startNearOuter, halfthick);
        startFarOuter += (orientation ? 1 : -1) * LocalRight(startFarOuter, halfthick);

        // Applying thickness for the end points just moves the vertices straight up & down
        endNearInner -= Vector3.up * halfthick;
        endFarInner -= Vector3.up * halfthick;
        endNearOuter += Vector3.up * halfthick;
        endFarOuter += Vector3.up * halfthick;

        // The thickness for the mid points goes half right/left, half up/down
        midNearInner += (orientation ? -.7f : .7f) * LocalRight(startNearOuter, halfthick) - Vector3.up * halfthick * 0.7f;
        midFarInner += (orientation ? -.7f : .7f) * LocalRight(startFarOuter, halfthick) - Vector3.up * halfthick * 0.7f;
        midNearOuter += (orientation ? .7f : -.7f) * LocalRight(startNearOuter, halfthick) + Vector3.up * halfthick * 0.7f;
        midFarOuter += (orientation ? .7f : -.5f) * LocalRight(startFarOuter, halfthick) + Vector3.up * halfthick * 0.7f;

        // These vertices indicate the start and end points of each segment
        // beginning at the "start" points and terminating in the "end" points.
        Vector3 sni = startNearInner;
        Vector3 sfi = startFarInner;
        Vector3 sno = startNearOuter;
        Vector3 sfo = startFarOuter;

        Vector3 eni, efi, eno, efo;
        float t;

        // Draw the curvature of the corner in segments
        for (int i = 0; i < segments; i++)
        {
            t = (float)(i + 1) / segments;

            // Calculate bezier points for the end points of the current segment
            // https://catlikecoding.com/unity/tutorials/curves-and-splines/
            eni = Bezier(startNearInner, midNearInner, endNearInner, t);
            efi = Bezier(startFarInner, midFarInner, endFarInner, t);
            eno = Bezier(startNearOuter, midNearOuter, endNearOuter, t);
            efo = Bezier(startFarOuter, midFarOuter, endFarOuter, t);

            // Draw the current segment
            meshData.vertices[vertexIndex + 0] = sni;
            meshData.vertices[vertexIndex + 1] = sno;
            meshData.vertices[vertexIndex + 2] = eni;
            meshData.vertices[vertexIndex + 3] = eno;
            meshData.vertices[vertexIndex + 4] = sfi;
            meshData.vertices[vertexIndex + 5] = sfo;
            meshData.vertices[vertexIndex + 6] = efi;
            meshData.vertices[vertexIndex + 7] = efo;

            // Draw the inner and outer quads
            if (!inverted)
            {
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 6, vertexIndex + 4);    // Inner 
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 2, vertexIndex + 6);    // Inner 
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 5, vertexIndex + 7);    // Outer
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 7, vertexIndex + 3);    // Outer
            }
            else
            {
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 4, vertexIndex + 6);    // Inner 
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 6, vertexIndex + 2);    // Inner 
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 7, vertexIndex + 5);    // Outer
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 3, vertexIndex + 7);    // Outer
            }


            // Draw the near and far quads
            if (!inverted)
            {
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 3);    // Near
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 3, vertexIndex + 2);    // Near
                meshData.AddTriangle(vertexIndex + 4, vertexIndex + 7, vertexIndex + 5);    // Far
                meshData.AddTriangle(vertexIndex + 4, vertexIndex + 6, vertexIndex + 7);    // Far
            }
            else
            {
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 0, vertexIndex + 3);    // Near
                meshData.AddTriangle(vertexIndex + 3, vertexIndex + 0, vertexIndex + 2);    // Near
                meshData.AddTriangle(vertexIndex + 7, vertexIndex + 4, vertexIndex + 5);    // Far
                meshData.AddTriangle(vertexIndex + 6, vertexIndex + 4, vertexIndex + 7);    // Far
            }

            vertexIndex += 8;

            // The start points for the next segment are the end points of the current segment
            sni = eni;
            sno = eno;
            sfi = efi;
            sfo = efo;
        }

    }

    public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }
}



public class ShelfMeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public ShelfMeshData(int sectors, int layers, int rows, int columns, int unitWidth, int cornerSegments)
    {
        triangleIndex = 0;

        /* 
         * How many vertices and tris will this shelf have?

         * Per vertical shelf
              vertices : (columns + 1) * 8 
              tris     : (columns + 1) * 8
         
         * Per horizontal shelf
              vertices : ((rows + 1) * columns * unitWidth - 4) * 8
              tris     : ((rows + 1) * columns * unitWidth - 4) * 8

         * Per corner (x4)
              vertices : cornerSegments * 8
              tris     : cornerSegments * 8
        */

        int vertcount = (columns + 1) * 8 + ((rows + 1) * columns * unitWidth - 4) * 8 + cornerSegments * 4 * 8;
        int tricount = vertcount * 3;

        vertices = new Vector3[vertcount];  
        triangles = new int[tricount];      
        uvs = new Vector2[vertcount];        // same number of uvs as vertices
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (triangleIndex > triangles.Length-3)
        {
            Debug.Log("triangleIndex (" + triangleIndex + ") is out of range (" + triangles.Length + ")");
        }
        else
        {
            triangles[triangleIndex + 0] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
        }
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        FlatShading();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        //mesh.tangents = new Vector4[triangles.Length];
        //Debug.Log("TraingleIndex: " + triangleIndex + ". Triangles.length: " + triangles.Length);
        //CalculateMeshTangents();
        return mesh;
    }

    public void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

}

