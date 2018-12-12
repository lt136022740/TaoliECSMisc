using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Burst;

public class GPUSKinAnimBakeSystem : ComponentSystem
{
    public bool init = false;
    public static int formationCount = 30;
    public static int width = 12;
    public static int height = 6;
    NativeList<FormationData> formationList = new NativeList<FormationData>(10000, Allocator.Persistent);
    //public AnimationClipDataBaked[] animationClipData = new AnimationClipDataBaked[100];
    NativeArray<AnimationClipDataBaked> animationClipData = new NativeArray<AnimationClipDataBaked>(100, Allocator.Persistent);
    LodedDrawerECS lodDrawer0;
    LodedDrawerECS lodDrawer1;
    LodedDrawerECS lodDrawer2;
    LodedDrawerECS lodDrawer3;
    float deltaTime;

    KeyframeTextureBaker.BakedData bakedData;
    protected override void OnUpdate()

    {
        initilize();
        deltaTime += Time.deltaTime;
        JobAnim job = new JobAnim
        {
            deltaTime = deltaTime,
            formationList = formationList,
            animationClips = animationClipData,
            motionDeltaTime = (long)Time.realtimeSinceStartup,
            DistanceMaxLod0 = bakedData.lods.Lod1Distance,
            DistanceMaxLod1 = bakedData.lods.Lod2Distance,
            DistanceMaxLod2 = bakedData.lods.Lod3Distance,
            CameraPosition = Camera.main.transform.position,

            listPositionData0 = lodDrawer0.listPositionData,
            listRotation0 = lodDrawer0.listRotationData,
            listTexturePosition0 = lodDrawer0.listTexturePositionData,

            listPositionData1 = lodDrawer1.listPositionData,
            listRotation1 = lodDrawer1.listRotationData,
            listTexturePosition1 = lodDrawer1.listTexturePositionData,

            listPositionData2 = lodDrawer2.listPositionData,
            listRotation2 = lodDrawer2.listRotationData,
            listTexturePosition2 = lodDrawer2.listTexturePositionData,

            listPositionData3 = lodDrawer3.listPositionData,
            listRotation3 = lodDrawer3.listRotationData,
            listTexturePosition3 = lodDrawer3.listTexturePositionData,
        };

        //for (int i = 0; i < formationList.Count * formationList[0].Count(); i++)
        //{

        //    lodDrawer0.listTexturePositionData.Add(texturePositionData);
        //    lodDrawer1.listTexturePositionData.Add(texturePositionData);
        //    lodDrawer2.listTexturePositionData.Add(texturePositionData);
        //    lodDrawer3.listTexturePositionData.Add(texturePositionData);
        //}

        job.Schedule().Complete();

        lodDrawer0.Draw();
        lodDrawer1.Draw();
        lodDrawer2.Draw();
        lodDrawer3.Draw();

        lodDrawer0.listPositionData.Clear();
        lodDrawer1.listPositionData.Clear();
        lodDrawer2.listPositionData.Clear();
        lodDrawer3.listPositionData.Clear();

        lodDrawer0.listRotationData.Clear();
        lodDrawer1.listRotationData.Clear();
        lodDrawer2.listRotationData.Clear();
        lodDrawer3.listRotationData.Clear();

        lodDrawer0.listTexturePositionData.Clear();
        lodDrawer1.listTexturePositionData.Clear();
        lodDrawer2.listTexturePositionData.Clear();
        lodDrawer3.listTexturePositionData.Clear();


    }
    [BurstCompile]
    public struct JobAnim : IJob
    {
        public float deltaTime;
        public float3 CameraPosition;

        [ReadOnly]
        public NativeList<FormationData> formationList;
        public NativeArray<AnimationClipDataBaked> animationClips;

        public NativeList<float4> listPositionData0;
        public NativeList<quaternion> listRotation0;
        public NativeList<float3> listTexturePosition0;

        public NativeList<float4> listPositionData1;
        public NativeList<quaternion> listRotation1;
        public NativeList<float3> listTexturePosition1;

        public NativeList<float4> listPositionData2;
        public NativeList<quaternion> listRotation2;
        public NativeList<float3> listTexturePosition2;

        public NativeList<float4> listPositionData3;
        public NativeList<quaternion> listRotation3;
        public NativeList<float3> listTexturePosition3;

        public float normalizedTime;

        [ReadOnly]
        public float DistanceMaxLod0;
        [ReadOnly]
        public float DistanceMaxLod1;
        [ReadOnly]
        public float DistanceMaxLod2;

        public long motionDeltaTime;

        public void Execute()
        {
            //deltaTime += Time.deltaTime;
            for (int i = 0; i < formationList.Length; ++i)
            {
                FormationData formation = formationList[i];
                float distance = math.length(CameraPosition - formation.formationPosition);
                for (int j = 0; j < formation.count(); j++)
                {
                    float3 center = formation.formationPosition;
                    float3 v3 = new float3(center.x + j % formation.width * 3f, 1, center.z + j / formation.width * 3);
                    float4 v4 = new float4(v3.x, v3.y, v3.z, 1);
                    int index = 1;//Randomizer.Range(0, 6, ref motionDeltaTime);
                    AnimationClipDataBaked clip = animationClips[(int)0 * 25 + index];
                    normalizedTime = /*normalizedTimeStart*/0 + deltaTime / clip.AnimationLength;
                    //Unity.Mathematics.Random r = new Unity.Mathematics.Random((uint)Time.realtimeSinceStartup);
                    float rad = Randomizer.Float(0.1f, 0.3f, ref motionDeltaTime);
                    normalizedTime = (normalizedTime + rad) % 1.0f;
                    float texturePosition = normalizedTime * clip.TextureRange + clip.TextureOffset;
                    int lowerPixelInt = (int)math.floor(texturePosition * clip.TextureWidth);

                    float lowerPixelCenter = (lowerPixelInt * 1.0f) / clip.TextureWidth;
                    float upperPixelCenter = lowerPixelCenter + clip.OnePixelOffset;
                    float lerpFactor = (texturePosition - lowerPixelCenter) / clip.OnePixelOffset;
                    float3 texturePositionData = new float3(lowerPixelCenter, upperPixelCenter, lerpFactor);
                    if (distance < DistanceMaxLod0)
                    {
                        listPositionData0.Add(v4);
                        listRotation0.Add(Quaternion.LookRotation(new Vector3(-90, 9, 0), new Vector3(0.0f, 1, 0.0f)));
                        listTexturePosition0.Add(texturePositionData);
                    }
                    else if (distance < DistanceMaxLod1)
                    {
                        listPositionData1.Add(v4);
                        listRotation1.Add(Quaternion.LookRotation(new Vector3(-90, 8, 0), new Vector3(0.0f, 1, 0.0f)));
                        listTexturePosition1.Add(texturePositionData);
                    }
                    else if (distance < DistanceMaxLod2)
                    {
                        listPositionData2.Add(v4);
                        listRotation2.Add(Quaternion.LookRotation(new Vector3(-90, 7, 0), new Vector3(0.0f, 1, 0.0f)));
                        listTexturePosition2.Add(texturePositionData);
                    }
                    else
                    {
                        listPositionData3.Add(v4);
                        listRotation3.Add(Quaternion.LookRotation(new Vector3(-90, 30, 0), new Vector3(0.0f, 1, 0.0f)));
                        listTexturePosition3.Add(texturePositionData);
                    }
                }
            }
        }

    }
    class LodedDrawerECS
    {
        public static int count = 0;
        private readonly uint[] indirectArgs = new uint[5] { 0, 0, 0, 0, 0 };

        public ComputeBuffer objectPositionsBuffer;
        public ComputeBuffer objectRotationsBuffer;
        public ComputeBuffer textureCoordinatesBuffer;
        public ComputeBuffer argsBuffer;

        public NativeList<float4> listPositionData;
        public NativeList<quaternion> listRotationData;
        public NativeList<float3> listTexturePositionData;

        public Mesh meshToDraw;
        public Material material;
        public KeyframeTextureBaker.BakedData bakedData;
        List<FormationData> formList;

        public LodedDrawerECS(Mesh mesh, Material mat, KeyframeTextureBaker.BakedData data)
        {
            count = GPUSKinAnimBakeSystem.formationCount * GPUSKinAnimBakeSystem.width * GPUSKinAnimBakeSystem.height; //formList.Count * formList[0].count();
            bakedData = data;
            material = new Material(mat);
            meshToDraw = mesh;
            indirectArgs[0] = meshToDraw.GetIndexCount(0);
            argsBuffer = new ComputeBuffer(1, indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            objectRotationsBuffer = new ComputeBuffer(count, 16);
            objectPositionsBuffer = new ComputeBuffer(count, 16);
            textureCoordinatesBuffer = new ComputeBuffer(count, 12);

            listPositionData = new NativeList<float4>(count, Allocator.Persistent);
            listRotationData = new NativeList<quaternion>(count, Allocator.Persistent);
            listTexturePositionData = new NativeList<float3>(count, Allocator.Persistent);

            material.SetTexture("_AnimationTexture0", bakedData.Texture0);
            material.SetTexture("_AnimationTexture1", bakedData.Texture1);
            material.SetTexture("_AnimationTexture2", bakedData.Texture2);

        }

        public void Draw()
        {
            int countToDraw = listPositionData.Length;

            objectPositionsBuffer.SetData((NativeArray<float4>)listPositionData, 0, 0, countToDraw);
            objectRotationsBuffer.SetData((NativeArray<quaternion>)listRotationData, 0, 0, countToDraw);
            textureCoordinatesBuffer.SetData((NativeArray<float3>)listTexturePositionData, 0, 0, countToDraw);

            material.SetBuffer("textureCoordinatesBuffer", textureCoordinatesBuffer);
            material.SetBuffer("objectPositionsBuffer", objectPositionsBuffer);
            material.SetBuffer("objectRotationsBuffer", objectRotationsBuffer);

            indirectArgs[1] = (uint)countToDraw;
            argsBuffer.SetData(indirectArgs);

            Graphics.DrawMeshInstancedIndirect(
                meshToDraw,
                0,
                material,
                new Bounds(Vector3.zero, 10000 * Vector3.one),
                argsBuffer,
                0,
                new MaterialPropertyBlock(),
                UnityEngine.Rendering.ShadowCastingMode.On,
                true
                );
        }
    }
    private void initilize()
    {
        if (init)
        {
            return;
        }
        init = true;
        PreSetting setting = Resources.Load<PreSetting>("PreSetting");
        var bakingObject = GameObject.Instantiate(setting.go) as GameObject;
        SkinnedMeshRenderer renderer = bakingObject.GetComponentInChildren<SkinnedMeshRenderer>();
        LodData lodData = new LodData();
        lodData.Lod1Mesh = setting.mesh0;
        lodData.Lod2Mesh = setting.mesh1;
        lodData.Lod3Mesh = setting.mesh2;
        lodData.Lod1Distance = 10;
        lodData.Lod2Distance = 20;
        lodData.Lod3Distance = 30;
        Material material = setting.material;

        //SkinnedMeshRenderer r = Combine.CombineIt(bakingObject.transform);
        bakedData = KeyframeTextureBaker.BakeClips(renderer, GetAllAnimationClips(renderer.GetComponentInParent<Animation>()), lodData);
        //bakedData = KeyframeTextureBaker.BakeClips(r, GetAllAnimationClips(r.GetComponentInParent<Animation>()), lodData);

        TransferAnimationData();

        //UnityEngine.Object fGo = Resources.Load("Formation");
        for (int i = 0; i < formationCount; i++)
        {
            Vector3 formationPosition = new Vector3(i % 5 * (width + 30), 1, i / 5 * (height + 20));

            //GameObject instance = Object.Instantiate(fGo) as GameObject; 
            //instance.name = "Formation_" + i;
            //Formation form = instance.GetComponent<Formation>();
            //form.Init(width, height);
            //instance.transform.position = formationPosition;
            FormationData data = new FormationData();
            data.width = width;
            data.height = height;
            data.formationPosition = formationPosition;
            formationList.Add(data);
        }

        lodDrawer0 = new LodedDrawerECS(bakedData.NewMesh, material, bakedData);
        lodDrawer1 = new LodedDrawerECS(bakedData.lods.Lod1Mesh, material, bakedData);
        lodDrawer2 = new LodedDrawerECS(bakedData.lods.Lod2Mesh, material, bakedData);
        lodDrawer3 = new LodedDrawerECS(bakedData.lods.Lod3Mesh, material, bakedData);
    }
    private void TransferAnimationData()
    {
        for (int i = 0; i < bakedData.Animations.Count; i++)
        {
            AnimationClipDataBaked data = new AnimationClipDataBaked();
            data.AnimationLength = bakedData.Animations[i].Clip.length;
            GetTextureRangeAndOffset(bakedData, bakedData.Animations[i], out data.TextureRange, out data.TextureOffset, out data.OnePixelOffset, out data.TextureWidth);
            //data.Looping = bakedData.Animations[i].Clip.wrapMode == WrapMode.Loop;
            animationClipData[(int)0 * 25 + i] = data;
        }
    }
    private void GetTextureRangeAndOffset(KeyframeTextureBaker.BakedData bakedData, KeyframeTextureBaker.AnimationClipData clipData, out float range, out float offset, out float onePixelOffset, out int textureWidth)
    {
        float onePixel = 1f / bakedData.Texture0.width;
        float start = (float)clipData.PixelStart / bakedData.Texture0.width + onePixel * 0.5f;
        float end = (float)clipData.PixelEnd / bakedData.Texture0.width + onePixel * 0.5f;
        onePixelOffset = onePixel;
        textureWidth = bakedData.Texture0.width;
        range = end - start;
        offset = start;
    }
    private AnimationClip[] GetAllAnimationClips(Animation animation)
    {
        List<AnimationClip> animationClips = new List<AnimationClip>();
        foreach (AnimationState state in animation)
        {
            animationClips.Add(state.clip);
        }

        //animationClips.Sort((x, y) => String.Compare(x.name, y.name, StringComparison.Ordinal));

        return animationClips.ToArray();
    }
}