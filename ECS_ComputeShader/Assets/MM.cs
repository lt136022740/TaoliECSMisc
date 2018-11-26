using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public struct V4
{
    float x;
    float y;
    float z;
    float w;

    public V4(float a1, float a2, float a3, float a4)
    {
        x = a1;
        y = a2;
        z = a3;
        w = a4;
    }
}

public class MM : MonoBehaviour {
    public ComputeBuffer buffer;
    public ComputeBuffer argsBuffer;
    public Material mat;
    public Mesh mesh;
    private readonly uint[] indirectArgs = new uint[5] { 0, 0, 0, 0, 0 };
    int count = 800000;
    public GameObject go;
    List<V4> goList;
    private void Start()
    {
        goList = new List<V4>();
        for (int i=0; i< count; ++i)
        {
           Vector3 v3 = new Vector3(i % 2000, 1, i/1000);
            V4 v4 = new V4(v3.x, v3.y, v3.z, 1);
            goList.Add(v4);
        }
        indirectArgs[0] = mesh.GetIndexCount(0);
        argsBuffer = new ComputeBuffer(1, indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        buffer = new ComputeBuffer(count, 16);
    }

    float epal = 0;
    private void Update()
    {
       
        {
            epal = 0;
            indirectArgs[1] = (uint)count;
           
            argsBuffer.SetData(indirectArgs);

            Profiler.BeginSample("11111");
            buffer.SetData(goList, 0, 0, count);
            Profiler.EndSample();
            mat.SetBuffer("objectPositionsBuffer", buffer);
          
            Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, new Bounds(Vector3.zero, 10000 * Vector3.one), argsBuffer, 0, new MaterialPropertyBlock(), UnityEngine.Rendering.ShadowCastingMode.On, true);
           
        }
       
    }

    private void OnDisable()
    {
        argsBuffer.Dispose();
        buffer.Dispose();
    }
}
