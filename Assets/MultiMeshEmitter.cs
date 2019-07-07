using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Eidetic.Unity.Utility;

public class MultiMeshEmitter : MonoBehaviour
{
    [SerializeField] List<MeshFilter> MeshFilters;
    [SerializeField] float EmissionRate;
    [SerializeField] bool FollowInsteadOfEmit;
    float LastEmitTime = 0;
    ParticleSystem ParticleSystem;
    int currentMesh = 0;
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= LastEmitTime + EmissionRate)
        {
            var positions = new List<Vector3>();
            foreach (var filter in MeshFilters)
            {
                var vertexPositions = filter.mesh.vertices;
                var transform = filter.gameObject.transform;
                foreach (var vertexPosition in vertexPositions)
                {
                    var position = vertexPosition
                                    .RotateBy(transform.rotation.eulerAngles)
                                    .ScaleBy(transform.localScale)
                                    .TranslateBy(transform.position);
                    positions.Add(position);
                }
            }
            var particles = new ParticleSystem.Particle[ParticleSystem.particleCount];
            ParticleSystem.GetParticles(particles);
            for (int i = 0; i < ParticleSystem.particleCount; i++)
            {
                var positionIndex = i % positions.Count;
                particles[i].position = positions[positionIndex];
            }
            ParticleSystem.SetParticles(particles);
            LastEmitTime = Time.time;
        }
    }
}
