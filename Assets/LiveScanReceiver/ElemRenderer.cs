using UnityEngine;
using System.Collections;

public class ElemRenderer : MonoBehaviour
{
    Mesh mesh;

    public void UpdateMesh(float[] arrVertices, byte[] arrColors, int nPointsToRender, int nPointsRendered)
    {
        int nPoints;

        if (arrVertices == null || arrColors == null)
            nPoints = 0;
        else
            nPoints = System.Math.Min(nPointsToRender, (arrVertices.Length / 3) - nPointsRendered);
        nPoints = System.Math.Min(nPoints, 65535);

        Vector3[] points = new Vector3[nPoints];
        int[] indices = new int[nPoints];
        Color[] colors = new Color[nPoints];

        var bounds = new Bounds();
        bounds.SetMinMax(PointCloudRenderer.Instance.MinimumBounds, PointCloudRenderer.Instance.MaximumBounds);

        for (int i = 0; i < nPoints; i++)
        {
            int ptIdx = 3 * (nPointsRendered + i);
            points[i] = new Vector3(arrVertices[ptIdx + 0], arrVertices[ptIdx + 1], -arrVertices[ptIdx + 2]);
            indices[i] = i;
            if (bounds.Contains(points[i]))
                colors[i] = new Color((float)arrColors[ptIdx + 0] / 256.0f, (float)arrColors[ptIdx + 1] / 256.0f, (float)arrColors[ptIdx + 2] / 256.0f, 1.0f);       
            else colors[i] = Color.clear; 
        }

        if (mesh != null)
            Destroy(mesh);
        mesh = new Mesh();
        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
