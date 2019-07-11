using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using Eidetic.Unity.Utility;

public class PointCloudReceiver : MonoBehaviour
{
    public static PointCloudReceiver Instance;
    [SerializeField] int port = 48002;
    [SerializeField] Vector3 PointCloudRotation = Vector3.zero;
    [SerializeField] Vector3 PointCloudTranslation = Vector3.zero;
    [SerializeField] Vector3 PointCloudScale = Vector3.one;
    // [SerializeField] Vector3 MinimumBounds = new Vector3(-5, -5, -5);
    // [SerializeField] Vector3 MaximumBounds = new Vector3(5, 5, 5);
    [SerializeField] public List<ParticleSystemWrapper> ParticleSystems = new List<ParticleSystemWrapper>();

    TcpClient socket;
    bool ReadyForNextFrame = true;
    bool Connected = false;

    void Start()
    {
        Connect("127.0.0.1");
    }

    void Update()
    {
        if (!Connected) return;

        float[] vertices;
        byte[] colors;

        if (ReadyForNextFrame) RequestFrame();

        if (ReceiveFrame(out vertices, out colors))
        {
            int nPoints = vertices == null ? 0 : (vertices.Length / 3);

            Vector3[] points = new Vector3[nPoints];
            int[] indices = new int[nPoints];
            Color[] pointColors = new Color[nPoints];

            for (int i = 0; i < nPoints; i++)
            {
                int point = 3 * i;

                Vector3 vertexPosition = new Vector3(vertices[point + 0], vertices[point + 1], -vertices[point + 2])
                    .RotateBy(PointCloudRotation)
                    .ScaleBy(PointCloudScale)
                    .TranslateBy(PointCloudTranslation);

                points[i] = vertexPosition;
                indices[i] = i;
                pointColors[i] = new Color(colors[point + 0] / 256.0f, colors[point + 1] / 256.0f, colors[point + 2] / 256.0f, 1.0f);

            }

            foreach (var system in ParticleSystems)
            {
                if (system == null || !system.ParticleSystem.gameObject.activeInHierarchy) continue;

                // validations
                if (system.EmissionRounds == 0) system.EmissionRounds = 1;

                if (system.Emit)
                {
                    system.ParticleSystem.Clear();

                    var emitCount = points.Length / system.ManualEmissionSkip;

                    system.ParticleSystem.Emit(emitCount);

                    var particles = new ParticleSystem.Particle[emitCount];

                    system.ParticleSystem.GetParticles(particles);

                    for (int p = 0; p < particles.Length; p++)
                    {
                        var pointIndex = (p * (system.ManualEmissionSkip)) % points.Length;
                        particles[p].startColor = pointColors[pointIndex];
                        particles[p].position = points[pointIndex];
                    }

                    system.ParticleSystem.SetParticles(particles);

                    system.ParticleSystem.Play();

                    system.Emit = false;
                }
                else if (system.ConstantEmission && Time.time > system.LastEmissionTime + system.EmissionInterval)
                {
                    var emitCount = system.ParticleSystem.main.maxParticles / system.EmissionRounds;

                    var particleIndexOffset = emitCount * system.CurrentEmissionRound;

                    system.ParticleSystem.Emit(emitCount);

                    var newParticleCount = system.ParticleSystem.particleCount;

                    var allParticles = new ParticleSystem.Particle[newParticleCount];

                    system.ParticleSystem.GetParticles(allParticles);

                    for (int p = particleIndexOffset; p < newParticleCount; p++)
                    {
                        var pointNumber = (p * system.EmissionRounds) + system.EmissionRounds - (system.EmissionRounds - system.CurrentEmissionRound);
                        if (pointNumber >= points.Length) pointNumber = pointNumber - points.Length;

                        allParticles[p].startColor = pointColors[pointNumber];
                        allParticles[p].position = points[pointNumber];
                    }

                    system.ParticleSystem.SetParticles(allParticles);

                    var mainModule = system.ParticleSystem.main;

                    system.ParticleSystem.Play();

                    system.CurrentEmissionRound++;
                    if (system.CurrentEmissionRound == system.EmissionRounds) system.CurrentEmissionRound = 0;
                    system.LastEmissionTime = Time.time;
                }
                else if (!system.ConstantEmission)
                {
                    //var mainModule = system.ParticleSystem.main;
                    //mainModule.ringBufferMode = ParticleSystemRingBufferMode.Disabled;
                }
            }
            ReadyForNextFrame = true;
        }
    }

    public void Connect(string IP)
    {
        socket = new TcpClient(IP, port);
        Connected = true;
        Instance = this;
        UnityEngine.Debug.Log("Connected");
    }

    void RequestFrame()
    {
        ReadyForNextFrame = false;

        byte[] byteToSend = new byte[1];
        byteToSend[0] = 0;

        socket.GetStream().Write(byteToSend, 0, 1);
    }

    int ReadInt()
    {
        byte[] buffer = new byte[4];
        int nRead = 0;
        while (nRead < 4)
            nRead += socket.GetStream().Read(buffer, nRead, 4 - nRead);

        return BitConverter.ToInt32(buffer, 0);
    }

    bool ReceiveFrame(out float[] lVertices, out byte[] lColors)
    {
        int nPointsToRead = ReadInt();

        lVertices = new float[3 * nPointsToRead];
        short[] lShortVertices = new short[3 * nPointsToRead];
        lColors = new byte[3 * nPointsToRead];


        int nBytesToRead = sizeof(short) * 3 * nPointsToRead;
        int nBytesRead = 0;
        byte[] buffer = new byte[nBytesToRead];

        while (nBytesRead < nBytesToRead)
            nBytesRead += socket.GetStream().Read(buffer, nBytesRead, Math.Min(nBytesToRead - nBytesRead, 64000));

        System.Buffer.BlockCopy(buffer, 0, lShortVertices, 0, nBytesToRead);

        for (int i = 0; i < lShortVertices.Length; i++)
            lVertices[i] = lShortVertices[i] / 1000.0f;

        nBytesToRead = sizeof(byte) * 3 * nPointsToRead;
        nBytesRead = 0;
        buffer = new byte[nBytesToRead];

        while (nBytesRead < nBytesToRead)
            nBytesRead += socket.GetStream().Read(buffer, nBytesRead, Math.Min(nBytesToRead - nBytesRead, 64000));

        System.Buffer.BlockCopy(buffer, 0, lColors, 0, nBytesToRead);

        return true;
    }

    [System.Serializable]
    public class ParticleSystemWrapper
    {
        public ParticleSystem ParticleSystem;
        public bool Emit;
        public bool ClearOnEmit = true;
        public int ManualEmissionSkip = 3;

        public bool ConstantEmission;
        public int EmissionRounds = 5;
        public float EmissionInterval = 0.015f;

        [NonSerialized] public int CurrentEmissionRound = 0;
        [NonSerialized] public float LastEmissionTime = 0;
    }

    public float TriggerEmit
    {
        set
        {
            var rounded = Mathf.RoundToInt(value);
            if (rounded > -1 && rounded < ParticleSystems.Count())
                ParticleSystems[rounded].Emit = true;
        }
    }

    public float ActivateConstant
    {
        set
        {
            var rounded = Mathf.RoundToInt(value);
            if (rounded > -1 && rounded < ParticleSystems.Count())
                ParticleSystems[rounded].ConstantEmission = true;
        }
    }

    public float DeactivateConstant
    {
        set
        {
            var rounded = Mathf.RoundToInt(value);
            if (rounded > -1 && rounded < ParticleSystems.Count())
                ParticleSystems[rounded].ConstantEmission = false;
        }
    }
}
