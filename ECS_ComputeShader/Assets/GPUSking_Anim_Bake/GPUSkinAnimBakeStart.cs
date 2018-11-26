using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.Mathematics;

public struct AnimationClipDataBaked
{
    public float TextureOffset;
    public float TextureRange;
    public float OnePixelOffset;
    public int TextureWidth;

    public float AnimationLength;
    public bool Looping;
}

public struct AnimationName
{
    public const int Attack1 = 0;
    public const int Attack2 = 1;
    public const int AttackRanged = 2;
    public const int Death = 3;
    public const int Falling = 4;
    public const int Idle = 5;
    public const int Walk = 6;
}
public class GPUSkinAnimBakeStart : MonoBehaviour {
    public GameObject go;
    public Material material;
    public Mesh mesh0;
    public Mesh mesh1;
    public Mesh mesh2;
   
    LodData lodData;
    LodedDrawer lodDrawer0;
    LodedDrawer lodDrawer1;
    LodedDrawer lodDrawer2;
    LodedDrawer lodDrawer3;
    List<Formation> formationList = new List<Formation>();
    int formationCount = 30;

    public AnimationClipDataBaked[] animationClipData = new AnimationClipDataBaked[100];
    public KeyframeTextureBaker.BakedData bakedData;
    
    void Start () {
        var bakingObject = GameObject.Instantiate(go);
        SkinnedMeshRenderer renderer = bakingObject.GetComponentInChildren<SkinnedMeshRenderer>();
        lodData = new LodData();
        lodData.Lod1Mesh = mesh0;
        lodData.Lod2Mesh = mesh1;
        lodData.Lod3Mesh = mesh2;
        lodData.Lod1Distance = 10;
        lodData.Lod2Distance = 20;
        lodData.Lod3Distance = 30;

        bakedData = KeyframeTextureBaker.BakeClips(renderer, GetAllAnimationClips(renderer.GetComponentInParent<Animation>()), lodData);

        TransferAnimationData();

        UnityEngine.Object fGo = Resources.Load("Formation");
        for (int i = 0; i < formationCount; i++)
        {
            int width = 12;
            int height = 6;

            Vector3 formationPosition = new Vector3(i % 5 * (width + 5), 1, i / 5 * (height + 4));

            GameObject instance =Instantiate(fGo) as GameObject;
            instance.name = "Formation_" + i;
            Formation form = instance.GetComponent<Formation>();
            form.Init(width, height);
            instance.transform.position = formationPosition;
            formationList.Add(form);
        }
            
        //SaveTextureToFile(bakedData.Texture0, "tp1.png");
        //SaveTextureToFile(bakedData.Texture1, "tp2.png");
        //SaveTextureToFile(bakedData.Texture2, "tp3.png");

        lodDrawer0 = new LodedDrawer(bakedData.NewMesh, material, bakedData, animationClipData, formationList);
        lodDrawer1 = new LodedDrawer(bakedData.lods.Lod1Mesh, material, bakedData, animationClipData, formationList);
        lodDrawer2 = new LodedDrawer(bakedData.lods.Lod2Mesh, material, bakedData, animationClipData, formationList);
        lodDrawer3 = new LodedDrawer(bakedData.lods.Lod3Mesh, material, bakedData, animationClipData, formationList);
    }

    //private static void SaveTextureToFile(Texture2D texture, string fileName)
    //{
    //    string path = Application.dataPath + "/" + fileName;
    //    var bytes = texture.EncodeToPNG();
    //    var file = File.Open(path, FileMode.Create);
    //    var binary = new BinaryWriter(file);
    //    binary.Write(bytes);
    //    file.Close();
    //}

    private void TransferAnimationData()
    {
        for (int i = 0; i < bakedData.Animations.Count; i++)
        {
            AnimationClipDataBaked data = new AnimationClipDataBaked();
            data.AnimationLength = bakedData.Animations[i].Clip.length;
            GetTextureRangeAndOffset(bakedData, bakedData.Animations[i], out data.TextureRange, out data.TextureOffset, out data.OnePixelOffset, out data.TextureWidth);
            data.Looping = bakedData.Animations[i].Clip.wrapMode == WrapMode.Loop;
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

        animationClips.Sort((x, y) => String.Compare(x.name, y.name, StringComparison.Ordinal));

        return animationClips.ToArray();
    }

    private float deltaTime = 0;
    private float normalizedTime = 0.0f;
    private float normalizedTimeStart = 0f;

    void Update () {
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

        Vector3 CameraPosition = Camera.main.transform.position; ;
        deltaTime += Time.deltaTime;
        for (int i = 0; i < formationList.Count; ++i)
        {
            Formation formation = formationList[i];
            float distance = math.length(CameraPosition - formation.transform.position);
            Debug.Log("Dis: " + distance);
            for (int j = 0; j < formation.Count(); j++)
            {
                Vector3 center = formation.transform.position;
                Vector3 v3 = new Vector3(center.x + j % formation.width, 1, center.z + j / formation.width);
                V4 v4 = new V4(v3.x, v3.y, v3.z, 1);

                AnimationClipDataBaked clip = animationClipData[(int)0 * 25 + AnimationName.Attack1];
                normalizedTime = normalizedTimeStart + deltaTime / clip.AnimationLength;
                normalizedTime = normalizedTime % 1.0f;

                float texturePosition = normalizedTime * clip.TextureRange + clip.TextureOffset;
                int lowerPixelInt = (int)math.floor(texturePosition * clip.TextureWidth);

                float lowerPixelCenter = (lowerPixelInt * 1.0f) / clip.TextureWidth;
                float upperPixelCenter = lowerPixelCenter + clip.OnePixelOffset;
                float lerpFactor = (texturePosition - lowerPixelCenter) / clip.OnePixelOffset;
                Vector3 texturePositionData = new Vector3(lowerPixelCenter, upperPixelCenter, lerpFactor);

                if (distance < bakedData.lods.Lod1Distance)
                {
                    lodDrawer0.listPositionData.Add(v4);
                    lodDrawer0.listRotationData.Add(Quaternion.LookRotation(new Vector3(0, UnityEngine.Random.Range(1, 50), 0), new Vector3(0.0f, 1, 0.0f)));
                    lodDrawer0.listTexturePositionData.Add(texturePositionData);
                }
                else if (distance < bakedData.lods.Lod2Distance)
                {
                    lodDrawer1.listPositionData.Add(v4);
                    lodDrawer1.listRotationData.Add(Quaternion.LookRotation(new Vector3(0, UnityEngine.Random.Range(1, 50), 0), new Vector3(0.0f, 1, 0.0f)));
                    lodDrawer1.listTexturePositionData.Add(texturePositionData);
                }
                else if (distance < bakedData.lods.Lod3Distance)
                {
                    lodDrawer2.listPositionData.Add(v4);
                    lodDrawer2.listRotationData.Add(Quaternion.LookRotation(new Vector3(0, UnityEngine.Random.Range(1, 50), 0), new Vector3(0.0f, 1, 0.0f)));
                    lodDrawer2.listTexturePositionData.Add(texturePositionData);
                }
                else
                {
                    lodDrawer3.listPositionData.Add(v4);
                    lodDrawer3.listRotationData.Add(Quaternion.LookRotation(new Vector3(0, UnityEngine.Random.Range(1, 50), 0), new Vector3(0.0f, 1, 0.0f)));
                    lodDrawer3.listTexturePositionData.Add(texturePositionData);
                }
            }
        }

       
        //for (int i = 0; i < formationList.Count * formationList[0].Count(); i++)
        //{
           
        //    lodDrawer0.listTexturePositionData.Add(texturePositionData);
        //    lodDrawer1.listTexturePositionData.Add(texturePositionData);
        //    lodDrawer2.listTexturePositionData.Add(texturePositionData);
        //    lodDrawer3.listTexturePositionData.Add(texturePositionData);
        //}

        lodDrawer0.Draw();
        lodDrawer1.Draw();
        lodDrawer2.Draw();
        lodDrawer3.Draw();
    }
}

class LodedDrawer
{
    public static int count = 0;
    private readonly uint[] indirectArgs = new uint[5] { 0, 0, 0, 0, 0 };
   
    public ComputeBuffer objectPositionsBuffer;
    public ComputeBuffer objectRotationsBuffer;
    public ComputeBuffer textureCoordinatesBuffer;
    public ComputeBuffer argsBuffer;

    public List<V4> listPositionData = new List<V4>(count);
    public List<Quaternion> listRotationData = new List<Quaternion>(count);
    public List<Vector3> listTexturePositionData = new List<Vector3>(count);
    public AnimationClipDataBaked[] animationClipData = new AnimationClipDataBaked[100];

    public Mesh meshToDraw;
    public Material material;
    public KeyframeTextureBaker.BakedData bakedData;
    List<Formation> formList;

    public LodedDrawer(Mesh mesh, Material mat, KeyframeTextureBaker.BakedData data, AnimationClipDataBaked[] clipData, List<Formation> formList)
    {
        this.formList = formList;
        count = formList.Count * formList[0].Count();
        animationClipData = clipData;
        bakedData = data;
        material = new Material(mat);
        meshToDraw = mesh;
        indirectArgs[0] = meshToDraw.GetIndexCount(0);
        argsBuffer = new ComputeBuffer(1, indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        objectRotationsBuffer = new ComputeBuffer(count, 16);
        objectPositionsBuffer = new ComputeBuffer(count, 16);
        textureCoordinatesBuffer = new ComputeBuffer(count, 12);

        material.SetTexture("_AnimationTexture0", bakedData.Texture0);
        material.SetTexture("_AnimationTexture1", bakedData.Texture1);
        material.SetTexture("_AnimationTexture2", bakedData.Texture2);
       
    }
    
    public void Draw()
    {
        int countToDraw = listPositionData.Count;

        objectPositionsBuffer.SetData(listPositionData, 0, 0, countToDraw);
        objectRotationsBuffer.SetData(listRotationData, 0, 0, countToDraw);
        textureCoordinatesBuffer.SetData(listTexturePositionData, 0, 0, countToDraw);

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
