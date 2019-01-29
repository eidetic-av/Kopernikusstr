using System;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
public class MeshTools : MonoBehaviour
{
    public Mesh InputMesh;
    public bool Bypass;
    public bool Freeze;
    public float NoiseIntensity;
    public int SmoothingTimes;
    public MeshFilter OutputMeshFilter;

    void Update()
    {
        if (Bypass)
        {
            OutputMeshFilter.mesh = InputMesh;
            return;
        }
        else if (Freeze)
            return;

        var intermediateMesh = Instantiate(InputMesh);

        var noiseJobVertices = new NativeArray<Vector3>(intermediateMesh.vertices, Allocator.TempJob);
        var noiseJobNormals = new NativeArray<Vector3>(intermediateMesh.normals, Allocator.TempJob);

        new NoiseJob(noiseJobVertices, noiseJobNormals, NoiseIntensity)
            .Schedule(InputMesh.vertexCount, 50)
            .Complete();

        OutputMeshFilter.mesh.SetVertices(noiseJobVertices.ToList());

        noiseJobVertices.Dispose();
        noiseJobNormals.Dispose();

        OutputMeshFilter.mesh = MeshSmoothing.LaplacianFilter(OutputMeshFilter.mesh, SmoothingTimes);
    }

    struct NoiseJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        public NativeArray<Vector3> normals;
        public float intensity;

        public NoiseJob(NativeArray<Vector3> vertices, NativeArray<Vector3> normals, float intensity)
        {
            this.vertices = vertices;
            this.normals = normals;
            this.intensity = intensity;
        }

        public void Execute(int i)
        {
            vertices[i] = vertices[i] + (normals[i] * RandomGenerator.NextFloat() * intensity);
        }
    }
}

