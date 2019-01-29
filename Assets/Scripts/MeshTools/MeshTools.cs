using System;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public static class Temp
{
    public static bool TryGetComponent<T>(this GameObject gameObject, out T component)
    {
        component = gameObject.GetComponent<T>();
        if (component.Equals(null))
            return false;
        else
            return true;
    }
}

public class MeshTools : MonoBehaviour
{

    public enum SmoothingType
    {
        Laplacian, HC
    };

    public bool Bypass = false;
    public SmoothingType Type = SmoothingType.Laplacian;
    public int SmoothingTimes = 0;
    public float NoiseIntensity = 0;
    public float HCAlpha = 0.5f;
    public float HCBeta = 0.5f;
    public bool OffsetTriangles = false;

    MeshFilter MeshFilter;
    bool IsSkinnedMesh;
    SkinnedMeshRenderer SkinnedMeshRenderer;

    void Start()
    {
        if (gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            MeshFilter = meshFilter;
        }
        else if (gameObject.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
        {
            IsSkinnedMesh = true;
            SkinnedMeshRenderer = skinnedMeshRenderer;

            MeshFilter = gameObject.AddComponent<MeshFilter>();
            MeshFilter.mesh.vertices = SkinnedMeshRenderer.sharedMesh.vertices.Clone() as Vector3[];
            MeshFilter.mesh.triangles = SkinnedMeshRenderer.sharedMesh.triangles.Clone() as int[];
            
            gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            Debug.LogErrorFormat("No mesh attached to object '{0}'.", gameObject.name);
            Bypass = true;
        }
    }
    void LateUpdate()
    {
        if (Bypass) return;
        if (IsSkinnedMesh) {
            MeshFilter.mesh.vertices = SkinnedMeshRenderer.sharedMesh.vertices.Clone() as Vector3[];
            Debug.Log(MeshFilter.mesh.vertices.Length);
            MeshFilter.mesh.triangles = SkinnedMeshRenderer.sharedMesh.triangles.Clone() as int[];
        } 
        ApplySmoothing(MeshFilter.mesh, Type);
    }

    void ApplySmoothing(Mesh mesh, SmoothingType type)
    {
        JobHandle normalNoiseJobHandler;
        if (mesh.vertexCount != 0)
        {
            mesh = ApplyNormalNoise(mesh, out normalNoiseJobHandler, OffsetTriangles);

            switch (type)
            {
                case SmoothingType.Laplacian:
                    mesh = MeshSmoothing.LaplacianFilter(mesh, SmoothingTimes, normalNoiseJobHandler);
                    break;
                case SmoothingType.HC:
                    mesh = MeshSmoothing.HCFilter(mesh, SmoothingTimes, HCAlpha, HCBeta);
                    break;
            }
        }
    }

    Mesh ApplyNormalNoise(Mesh sourceMesh, out JobHandle jobHandler, bool offsetTriangles = false)
    {
        var mesh = sourceMesh;

        // get the current mesh data
        var vertices = mesh.vertices;
        var normals = mesh.normals;

        // by instantiating the native arrays we are manually allocating memory for the parrallel processing job
        var vertexArray = new NativeArray<Vector3>(vertices, Allocator.TempJob);
        var normalArray = new NativeArray<Vector3>(normals, Allocator.TempJob);

        // Instantiate the job
        var normalNoiseJob = new NormalNoiseJob
        {
            Vertices = vertexArray,
            Normals = normalArray,
            Intensity = this.NoiseIntensity
        };

        // schedule the job for asynchronous processing
        // split the job into batches; loops of 250
        int batchSize = 250;
        jobHandler = normalNoiseJob.Schedule(mesh.vertexCount, batchSize);
        // make sure the multi-threaded job completes before moving on in this method
        jobHandler.Complete();

        // now copy the resulting data from the job back into the mesh
        vertexArray.CopyTo(vertices);
        normalArray.CopyTo(normals);

        // and dispose of the memory we manually allocated before
        vertexArray.Dispose();
        normalArray.Dispose();

        Mesh outputMesh = sourceMesh;
        outputMesh.vertices = vertices;
        outputMesh.normals = normals;

        if (offsetTriangles)
        {
            // if we are offsetting the triangles
            // remove any triangles that reference vertices that don't exist in this frame
            int lastVertex = outputMesh.vertices.Length;
            if (MeshSmoothing.PreviousTriangles == null)
                MeshSmoothing.PreviousTriangles = mesh.triangles.ToList();
            MeshSmoothing.PreviousTriangles.RemoveAll(v => (v >= lastVertex));
            // and make sure the count is divisable by three to make a full triangle
            while ((MeshSmoothing.PreviousTriangles.Count % 3) != 0)
                MeshSmoothing.PreviousTriangles.Add(lastVertex);
            outputMesh.triangles = MeshSmoothing.PreviousTriangles.ToArray();
            MeshSmoothing.LatestTriangles = outputMesh.triangles;
        }

        return outputMesh;
    }

    struct NormalNoiseJob : IJobParallelFor
    {
        public NativeArray<Vector3> Vertices;
        public NativeArray<Vector3> Normals;
        public float Intensity;

        public void Execute(int i)
        {
            Vertices[i] = Vertices[i] + Normals[i] * RandomGenerator.NextFloat() * Intensity;
        }
    }

}

