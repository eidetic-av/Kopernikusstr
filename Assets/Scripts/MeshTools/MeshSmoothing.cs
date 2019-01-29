/*
 * @author mattatz / http://mattatz.github.io
 * @author Eidetic

 * https://www.researchgate.net/publication/220507688_Improved_Laplacian_Smoothing_of_Noisy_Surface_Meshes
 * http://graphics.stanford.edu/courses/cs468-12-spring/LectureSlides/06_smoothing.pdf
 * http://wiki.unity3d.com/index.php?title=MeshSmoother
 * 
 * Original MeshSmoother by mattatz, additions by Eidetic
 */

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;

public class MeshSmoothing
{

    public static bool OffsetTriangles = false;
    public static List<int> PreviousTriangles;
    public static int[] LatestTriangles;

    public static Mesh LaplacianFilter(Mesh mesh, int times)
    {
        mesh.vertices = LaplacianFilter(mesh.vertices, mesh.triangles, times);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Dictionary<int, VertexConnection> Network;
    public static Vector3[] LatestVertices;

    public static Vector3[] LaplacianFilter(Vector3[] sourceVertices, int[] sourceTriangles, int times)
    {
        var filterJobHandlers = new List<JobHandle>();
        int batchSize = 250;

        Network = VertexConnection.BuildNetwork(sourceTriangles);
        LatestVertices = sourceVertices;

        for (int i = 0; i < times; i++)
        {

            var vertexArray = new NativeArray<Vector3>(LatestVertices, Allocator.TempJob);
            var trianglesArray = new NativeArray<int>(sourceTriangles, Allocator.TempJob);

            var job = new LaplactianFilterJob
            {
                vertices = vertexArray,
                triangles = trianglesArray
            };

            if (i == 0)
            {
                filterJobHandlers.Add(job.Schedule(vertexArray.Length, batchSize));
            }
            else
            {
                filterJobHandlers.Add(job.Schedule(vertexArray.Length, batchSize, filterJobHandlers[i - 1]));
            }

            filterJobHandlers.Last().Complete();

            // set vertices
            LatestVertices = new Vector3[vertexArray.Length];
            vertexArray.CopyTo(LatestVertices);

            vertexArray.Dispose();
            trianglesArray.Dispose();

        }

        return LatestVertices;
    }

    public struct LaplactianFilterJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        public NativeArray<int> triangles;

        public void Execute(int i)
        {
            if (!Network.ContainsKey(i)) return;

            var connection = Network[i].Connection;
            var v = Vector3.zero;
            foreach (int adj in connection)
            {
                v += LatestVertices[adj];
            }
            vertices[i] = v / connection.Count;
        }
    };

    static Vector3[] LaplacianFilter(Dictionary<int, VertexConnection> network, Vector3[] origin, List<int> limitedTriangles)
    {
        int limitedVertexCount = limitedTriangles.Max() + 1;
        Vector3[] vertices = new Vector3[limitedVertexCount];
        for (int i = 0, n = limitedVertexCount; i < n; i++)
        {
            if (!network.ContainsKey(i)) continue;
            var connection = network[i].Connection;
            var v = Vector3.zero;
            foreach (int adj in connection)
            {
                if (adj >= origin.Length) continue;
                v += origin[adj];
            }
            vertices[i] = v / connection.Count;
        }
        return vertices;
    }
}


