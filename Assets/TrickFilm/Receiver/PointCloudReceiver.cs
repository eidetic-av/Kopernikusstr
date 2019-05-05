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

    TcpClient socket;
    public int port = 48002;

    bool ReadyForNextFrame = true;
    bool Connected = false;

    public Vector3 PointCloudRotation = Vector3.zero;
    public Vector3 PointCloudTranslation = Vector3.zero;
    public Vector3 PointCloudScale = Vector3.one;

    public Vector3 MinimumBounds = new Vector3(-5, -5, -5);
    public Vector3 MaximumBounds = new Vector3(5, 5, 5);

    public Mesh Mesh;
    public MeshRenderer MeshRenderer;
    [SerializeField] public List<ParticleSystemWrapper> ParticleSystems = new List<ParticleSystemWrapper>();

    void Start() => Connect("127.0.0.1");

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

                Vector3 vertexPosition = new Vector3(vertices[point + 0], vertices[point + 1], -vertices[point + 2]);
                    //.RotateBy(PointCloudRotation)
                    //.TranslateBy(PointCloudTranslation)
                    //.ScaleBy(PointCloudScale);

                points[i] = vertexPosition;
                indices[i] = i;
                pointColors[i] = new Color(colors[point + 0] / 256.0f, colors[point + 1] / 256.0f, colors[point + 2] / 256.0f, 1.0f);

            }

            if (Mesh != null) Destroy(Mesh);

            Mesh = new Mesh()
            {
                vertices = points,
                colors = pointColors
            };

            Mesh.SetIndices(indices, MeshTopology.Points, 0);

            GetComponent<MeshFilter>().mesh = Mesh;

            foreach (var system in ParticleSystems)
            {
                if (system.ClearOnEmit) system.ParticleSystem.Clear();

                if (system.Emit)
                {
                    for (int p = 0; p < points.Length; p += 3)
                    {
                        var emitParams = new ParticleSystem.EmitParams();
                        emitParams.startColor = pointColors[p];
                        emitParams.position = points[p];
                        system.ParticleSystem.Emit(emitParams, 1);
                    }
                    var mainModule = system.ParticleSystem.main;
                    mainModule.ringBufferMode = ParticleSystemRingBufferMode.Disabled;
                    system.ParticleSystem.Play();
                    system.Emit = false;
                }
                else if (system.ConstantEmission && Time.time > system.LastEmissionTime + system.EmissionInterval)
                {
                    var maxEmit = points.Length > system.ParticleSystem.main.maxParticles * system.EmissionRounds ? system.ParticleSystem.main.maxParticles * system.EmissionRounds : points.Length;
                    var emitCount = maxEmit / system.EmissionRounds;

                    for (int p = 0; p < emitCount; p++)
                    {
                        var pointNumber = (p * system.EmissionRounds) + system.EmissionRounds - (system.EmissionRounds - system.CurrentEmissionRound);
                        if (pointNumber >= points.Length) break;

                        var emitParams = new ParticleSystem.EmitParams();
                        emitParams.startColor = pointColors[pointNumber];
                        emitParams.position = points[pointNumber];
                        system.ParticleSystem.Emit(emitParams, 1);
                    }
                    var mainModule = system.ParticleSystem.main;
                    mainModule.ringBufferMode = ParticleSystemRingBufferMode.LoopUntilReplaced;
                    system.ParticleSystem.Play();

                    system.CurrentEmissionRound++;
                    if (system.CurrentEmissionRound == system.EmissionRounds) system.CurrentEmissionRound = 0;
                    system.LastEmissionTime = Time.time;
                }
                else
                {
                    var mainModule = system.ParticleSystem.main;
                    mainModule.ringBufferMode = ParticleSystemRingBufferMode.Disabled;
                    //if system.ParticleSystem.Stop();
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
        Debug.Log("Connected");
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

        public bool ConstantEmission;
        public int EmissionRounds = 15;
        public float EmissionInterval = 0.015f;

        [NonSerialized] public int CurrentEmissionRound = 0;
        [NonSerialized] public float LastEmissionTime = 0;
    }
}
