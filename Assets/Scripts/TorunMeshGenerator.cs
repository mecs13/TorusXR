using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TorunMeshGenerator
{
    public enum TorunStyle
    {
        Rounded, Square, SquareBottom, SquareBottomLeft, SquareBottomRight
    }

    public static TorunMeshData GenerateTorun(Cylinder parentCylinder, int unitWidth, int unitHeight, float roundness, int cornerSegments, TorunStyle style)
    {
        TorunMeshData torunMeshData = GenerateTorun(parentCylinder.height, parentCylinder.radius - 0.005f, parentCylinder.sectors, parentCylinder.layers, unitWidth, unitHeight, roundness, cornerSegments, style);
        return torunMeshData;
    }

    public static TorunMeshData GenerateTorun(float cylinderHeight, float cylinderRadius, int sectors, int layers, int torunWidth, int torunHeight, float roundness, int cornerSegments, TorunStyle style)
    {
        TorunMeshData torunMeshData = new TorunMeshData(torunWidth, cornerSegments);

        float heightStep = cylinderHeight / (float)layers;
        float angleStep = 2 * Mathf.PI / (float)sectors;

        int vertexIndex = 0;
        if (torunWidth > 0 && torunHeight > 0)
        {
            for (int x = 0; x < torunWidth; x++)
            {
                float angleA = -angleStep * x;
                float angleB = -angleStep * (x + 1);

                bool isEdge = x == 0 || x == torunWidth - 1;

                float segmentHeight = isEdge ? torunHeight - 2 : torunHeight;
                float yPos = isEdge ? heightStep : 0f;

                float uvOffset = isEdge ? 1f / torunHeight : 0f;

                torunMeshData.uvs[vertexIndex + 0] = new Vector2((float)x / (float)torunWidth, uvOffset);
                torunMeshData.uvs[vertexIndex + 1] = new Vector2((float)x / (float)torunWidth, 1f - uvOffset);
                torunMeshData.uvs[vertexIndex + 2] = new Vector2((float)(x + 1) / (float)torunWidth, uvOffset);
                torunMeshData.uvs[vertexIndex + 3] = new Vector2((float)(x + 1) / (float)torunWidth, 1f - uvOffset);

                DrawTorunSquare(ref torunMeshData, ref vertexIndex, cylinderRadius, angleA, angleB, yPos, segmentHeight * heightStep);
            }

            // Top left corner
            DrawTorunCorner(ref torunMeshData, ref vertexIndex, cylinderRadius, 0, -angleStep, (torunHeight-1) * heightStep, heightStep, (style == TorunStyle.Square) ? 0f : roundness, cornerSegments, torunHeight, torunWidth, false);

            // Top right corner
            DrawTorunCorner(ref torunMeshData, ref vertexIndex, cylinderRadius, -angleStep * torunWidth, -angleStep * torunWidth + angleStep, (torunHeight - 1) * heightStep, heightStep, (style == TorunStyle.Square) ? 0f : roundness, cornerSegments, torunHeight, torunWidth, true);

            // Bottom left corner
            DrawTorunCorner(ref torunMeshData, ref vertexIndex, cylinderRadius, 0, -angleStep, heightStep, -heightStep, (style == TorunStyle.SquareBottom || style == TorunStyle.SquareBottomLeft || style == TorunStyle.Square) ? 0f : roundness, cornerSegments, torunHeight, torunWidth, true);

            // Bottom right corner
            DrawTorunCorner(ref torunMeshData, ref vertexIndex, cylinderRadius, -angleStep * torunWidth, -angleStep * torunWidth + angleStep, heightStep, -heightStep, (style == TorunStyle.SquareBottom || style == TorunStyle.SquareBottomRight || style==TorunStyle.Square) ? 0f : roundness, cornerSegments, torunHeight, torunWidth, false);

        }


        return torunMeshData;
    }

    public static void DrawTorunSquare(ref TorunMeshData meshData, ref int vertexIndex, float radius, float angleA, float angleB, float yPos, float height)
    {
        // Create the vertices for a one-sided quad 
        meshData.vertices[vertexIndex + 0] = new Vector3(radius * Mathf.Cos(angleA), yPos, radius * Mathf.Sin(angleA));
        meshData.vertices[vertexIndex + 2] = new Vector3(radius * Mathf.Cos(angleB), yPos, radius * Mathf.Sin(angleB));
        meshData.vertices[vertexIndex + 1] = new Vector3(radius * Mathf.Cos(angleA), yPos + height, radius * Mathf.Sin(angleA));
        meshData.vertices[vertexIndex + 3] = new Vector3(radius * Mathf.Cos(angleB), yPos + height, radius * Mathf.Sin(angleB));

        // Front and back quads
        meshData.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 2);
        meshData.AddTriangle(vertexIndex + 3, vertexIndex + 2, vertexIndex + 1);

        vertexIndex += 4;
    }

    public static void DrawTorunCorner(ref TorunMeshData meshData, ref int vertexIndex, float radius, float angleA, float angleB, float yPos, float height, float roundness, float cornerSegments, int torunHeight, int torunWidth, bool inverted)
    {
        Vector3 start, end, mid;

        start = new Vector3(radius * Mathf.Cos(angleA), yPos, radius * Mathf.Sin(angleA));

        mid = new Vector3(radius * Mathf.Cos(angleA), yPos + height, radius * Mathf.Sin(angleA));

        end = new Vector3(radius * Mathf.Cos(angleB), yPos + height, radius * Mathf.Sin(angleB));

        // These vertices indicate the start and end points of each segment
        // beginning at the "start" points and terminating in the "end" points.
        Vector3 s = Vector3.Lerp(mid, start, roundness);
        Vector3 e;
        float t;

        // Draw the curvature of the corner in segments
        for (int i = 0; i < cornerSegments; i++)
        {
            t = (float)(i + 1) / cornerSegments;

            // Calculate bezier points for the end point of the current segment
            // https://catlikecoding.com/unity/tutorials/curves-and-splines/
            e = Bezier(start, mid, end, t, roundness);

            // "Base" points for the segment (these are not rounded)
            Vector3 sb = new Vector3(s.x, yPos, s.z);
            Vector3 eb = new Vector3(e.x, yPos, e.z);

            // Draw the current segment
            meshData.vertices[vertexIndex + 0] = s;
            meshData.vertices[vertexIndex + 1] = e;
            meshData.vertices[vertexIndex + 2] = eb;
            meshData.vertices[vertexIndex + 3] = sb;

            // Draw the inner and outer quads
            if (!inverted)
            {
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 1, vertexIndex + 3);
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 2, vertexIndex + 3);
            }
            else
            {
                meshData.AddTriangle(vertexIndex + 0, vertexIndex + 3, vertexIndex + 1);
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 3, vertexIndex + 2);
            }

            // Work out the UVs for this corner.
            // I'm sure this can be cleaned up - but it works!
            if (angleB > angleA)
            {
                if (height < 0)
                {
                    meshData.uvs[vertexIndex + 0] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, s.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, s.y) / torunHeight);
                    meshData.uvs[vertexIndex + 1] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, e.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, e.y) / torunHeight);
                    meshData.uvs[vertexIndex + 2] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, eb.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, eb.y) / torunHeight);
                    meshData.uvs[vertexIndex + 3] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, sb.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, sb.y) / torunHeight);
                }
                else
                {
                    meshData.uvs[vertexIndex + 0] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, s.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, s.y) / torunHeight);
                    meshData.uvs[vertexIndex + 1] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, e.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, e.y) / torunHeight);
                    meshData.uvs[vertexIndex + 2] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, eb.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, eb.y) / torunHeight);
                    meshData.uvs[vertexIndex + 3] = new Vector2(1-Mathf.InverseLerp(start.x, end.x, sb.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, sb.y) / torunHeight);
                }
            }
            else
            {
                if (height < 0)
                {
                    meshData.uvs[vertexIndex + 0] = new Vector2(Mathf.InverseLerp(start.x, end.x, s.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, s.y) / torunHeight);
                    meshData.uvs[vertexIndex + 1] = new Vector2(Mathf.InverseLerp(start.x, end.x, e.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, e.y) / torunHeight);
                    meshData.uvs[vertexIndex + 2] = new Vector2(Mathf.InverseLerp(start.x, end.x, eb.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, eb.y) / torunHeight);
                    meshData.uvs[vertexIndex + 3] = new Vector2(Mathf.InverseLerp(start.x, end.x, sb.x) / torunWidth, Mathf.InverseLerp(end.y, start.y, sb.y) / torunHeight);
                }
                else
                {
                    meshData.uvs[vertexIndex + 0] = new Vector2(Mathf.InverseLerp(start.x, end.x, s.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, s.y) / torunHeight);
                    meshData.uvs[vertexIndex + 1] = new Vector2(Mathf.InverseLerp(start.x, end.x, e.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, e.y) / torunHeight);
                    meshData.uvs[vertexIndex + 2] = new Vector2(Mathf.InverseLerp(start.x, end.x, eb.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, eb.y) / torunHeight);
                    meshData.uvs[vertexIndex + 3] = new Vector2(Mathf.InverseLerp(start.x, end.x, sb.x) / torunWidth, 1-Mathf.InverseLerp(end.y, start.y, sb.y) / torunHeight);
                }
            }


            vertexIndex += 4;

            // The start point for the next segment is the end point of the current segment
            s = e;
        }
    }

    public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t, float roundness)
    {
        Vector3 b = Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);

        // Lerp between rounded and square corners
        //e = new Vector3((i==0 ? start.x : e.x), Mathf.Lerp(mid.y, e.y, roundness), (i == 0 ? start.z : e.z));

        b = new Vector3(b.x, Mathf.Lerp(p1.y, b.y, roundness), b.z);

        return b;
    }


}

public class TorunMeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public TorunMeshData(int unitWidth, int cornerSegments)
    {
        if (unitWidth >= 0)
        {
            triangleIndex = 0;
            vertices = new Vector3[4 * unitWidth + 4 * 4 * (cornerSegments + 1)];           // 1 quad per vertical slice + 4 quads per corner segment
            triangles = new int[3 * 2 * unitWidth + 3 * 8 * (cornerSegments)];              // 2 tris per quad
            uvs = new Vector2[vertices.Length];                                             // same number of uvs as vertices
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (triangleIndex > triangles.Length - 3)
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
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}


