using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class PointCloudReceiver : MonoBehaviour
{
    public static PointCloudReceiver Instance;

    TcpClient socket;
    public int port = 48002;

    bool bReadyForNextFrame = true;
    bool bConnected = false;

    public Vector3 PointCloudRotation = Vector3.zero;
    public Vector3 PointCloudTranslation = Vector3.zero;
    public Vector3 PointCloudScale = Vector3.one;

    public Vector3 MinimumBounds = new Vector3(-5, -5, -5);
    public Vector3 MaximumBounds = new Vector3(5, 5, 5);

    public Mesh Mesh;
    public MeshRenderer MeshRenderer;
    [SerializeField] public List<ParticleSystemWrapper> ParticleSystems = new List<ParticleSystemWrapper>();

    public int EmissionRounds = 12;
    public float EmissionInterval = 0.1f;

    int CurrentEmissionRound = 0;
    float LastEmissionTime;

    void Start()
    {
        Instance = this;
        Connect("127.0.0.1");
        if (MeshRenderer == null) MeshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (!bConnected)
            return;

        float[] vertices;
        byte[] colors;

        if (bReadyForNextFrame)
        {
            // Debug.Log("Requesting frame");

            RequestFrame();
            bReadyForNextFrame = false;
        }
        if (ReceiveFrame(out vertices, out colors))
        {
            // Debug.Log("Frame received");
            int nPoints = vertices != null ? (vertices.Length / 3) : 0;

            Vector3[] points = new Vector3[nPoints];
            int[] indices = new int[nPoints];
            Color[] pointColors = new Color[nPoints];

            var bounds = new Bounds();
            bounds.SetMinMax(MinimumBounds, MaximumBounds);

            int skippedVertices = 0;

            for (int i = 0; i < nPoints; i++)
            {
                int pointIndex = 3 * i;

                var vertexPosition = Quaternion.Euler(PointCloudRotation) *
                    new Vector3(vertices[pointIndex + 0], vertices[pointIndex + 1], -vertices[pointIndex + 2]) + PointCloudTranslation;

                vertexPosition.x = vertexPosition.x * PointCloudScale.x;
                vertexPosition.y = vertexPosition.y * PointCloudScale.y;
                vertexPosition.z = vertexPosition.z * PointCloudScale.z;

                if (bounds.Contains(vertexPosition))
                {
                    points[i - skippedVertices] = vertexPosition;
                    indices[i - skippedVertices] = i - skippedVertices;
                    pointColors[i - skippedVertices] = new Color((float)colors[pointIndex + 0] / 256.0f, (float)colors[pointIndex + 1] / 256.0f, (float)colors[pointIndex + 2] / 256.0f, 1.0f);
                }
                else
                    skippedVertices++;
            }

            if (Mesh != null)
                Destroy(Mesh);
            Mesh = new Mesh();
            Mesh.vertices = points;
            Mesh.colors = pointColors;

            Mesh.SetIndices(indices, MeshTopology.Points, 0);
            GetComponent<MeshFilter>().mesh = Mesh;

            for (int s = 0; s < ParticleSystems.Count; s++)
            {
                var system = ParticleSystems[s];

                if (system.Emit)
                {
                    system.ParticleSystem.Clear();
                    for (int p = 0; p < points.Length; p+=EmissionRounds)
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
                else if (system.ConstantEmission && Time.time > LastEmissionTime + EmissionInterval)
                {
                    var emitCount = points.Length / EmissionRounds;

                    for (int p = 0; p < emitCount; p++)
                    {
                        var pointNumber = (p * EmissionRounds) + EmissionRounds - (EmissionRounds - CurrentEmissionRound);
                        if (pointNumber >= points.Length) break;

                        var emitParams = new ParticleSystem.EmitParams();
                        emitParams.startColor = pointColors[pointNumber];
                        emitParams.position = points[pointNumber];
                        system.ParticleSystem.Emit(emitParams, 1);
                    }
                    var mainModule = system.ParticleSystem.main;
                    mainModule.ringBufferMode = ParticleSystemRingBufferMode.LoopUntilReplaced;
                    system.ParticleSystem.Play();

                    CurrentEmissionRound++;
                    if (CurrentEmissionRound == EmissionRounds) CurrentEmissionRound = 0;
                    LastEmissionTime = Time.time;
                }
                else
                {
                    var mainModule = system.ParticleSystem.main;
                    mainModule.ringBufferMode = ParticleSystemRingBufferMode.Disabled;
                    //if system.ParticleSystem.Stop();
                }
            }

            bReadyForNextFrame = true;
        }
    }

    public void Connect(string IP)
    {
        socket = new TcpClient(IP, port);
        bConnected = true;
        Debug.Log("Connected");
    }

    //Frame receiving for the editor
    void RequestFrame()
    {
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
        public bool ConstantEmission;
    }
}
