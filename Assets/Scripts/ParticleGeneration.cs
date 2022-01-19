using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ParticleGeneration : MonoBehaviour
{
    private Vector2 mousePosition;

    // struct
    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }

    private const int SIZE_PARTICLE = 28;
    private int particleCount = 1000000;
    public Material material;
    public ComputeShader computeShader;
    private int computeShaderId;
    ComputeBuffer particleBuffer;
    private const int WARP_SIZE = 256;
    private int mWarpCount;

    void Start()
    {
        Init();
    }

    void Init()
    {
        mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);
        Particle[] particleArray = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 xyz = new Vector3(Random.value * 2 - 1.0f, Random.value * 2 - 1.0f, Random.value * 2 - 1.0f)
                .normalized;
            particleArray[i].position = xyz;
            particleArray[i].velocity = Vector3.zero;
            particleArray[i].life = Random.value * 5.0f;
        }
        
        // A L'AIDE ANTOINE
        particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE);
        particleBuffer.SetData(particleArray);
        computeShaderId = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(computeShaderId, "particleBuffer", particleBuffer);
        material.SetBuffer("particleBuffer", particleBuffer);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 4, particleCount);
    }

    void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
    }

    void Update()
    {
        float[] mousePosition2D = { mousePosition.x, mousePosition.y };
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloats("mousePosition", mousePosition2D);
        computeShader.Dispatch(computeShaderId, mWarpCount, 1, 1);
    }

    void OnGUI()
    {
        Camera c = Camera.main;
        mousePosition = c.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            c.nearClipPlane - c.transform.position.z));
    }
}