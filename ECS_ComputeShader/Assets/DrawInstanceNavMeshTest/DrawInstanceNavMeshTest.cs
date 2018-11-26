using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class DrawInstanceNavMeshTest : MonoBehaviour {
    public ComputeBuffer buffer;
    public ComputeBuffer argsBuffer;
    public Material mat;
    public Mesh mesh;
    private readonly uint[] indirectArgs = new uint[5] { 0, 0, 0, 0, 0 };
    int count = 120;
    List<V4> dataList = new List<V4>();
    List<Transform> transList = new List<Transform>();

    private void Start()
    {
        GameObject go = Resources.Load("Player") as GameObject;
        for (int i = 0; i < count; i++)
        {
            float x = -414 + i % 40 * 2;
            float y = 1;
            float z = -428 + i / 40 * 2;
            Vector3 v = new Vector3(x, y, z);
            GameObject g = Instantiate(go, v, Quaternion.identity) as GameObject;
            Player player = g.GetComponent<Player>();
            player.Init();
            transList.Add(g.transform);
        }

        indirectArgs[0] = mesh.GetIndexCount(0);
        argsBuffer = new ComputeBuffer(1, indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        buffer = new ComputeBuffer(count, 16);
    }

    private void Update()
    {
        dataList.Clear();
        for (int i = 0; i < count; i++)
        {
            Vector3 v3 = transList[i].position;
            V4 v = new V4(v3.x, v3.y, v3.z, 1);
            dataList.Add(v);
        }
        indirectArgs[1] = (uint)count;
        argsBuffer.SetData(indirectArgs);
        buffer.SetData(dataList, 0, 0, count);
        mat.SetBuffer("objectPositionsBuffer", buffer);
          
        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, new Bounds(Vector3.zero, 10000 * Vector3.one), argsBuffer, 0, new MaterialPropertyBlock(), UnityEngine.Rendering.ShadowCastingMode.On, true);
    }

    private void OnDisable()
    {
        argsBuffer.Dispose();
        buffer.Dispose();
    }
}
