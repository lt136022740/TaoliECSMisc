using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combine : MonoBehaviour {
    private static GameObject[] targetParts = new GameObject[9];
    public static Material material;
    public static GameObject target;

    // Use this for initialization
    void Start () {
        GameObject go = Instantiate(target);
        CombineIt(go.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static SkinnedMeshRenderer CombineIt(Transform root)
    {
        float startTime = Time.realtimeSinceStartup;

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Transform> boneList = new List<Transform>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>();
        List<Texture2D> textures = new List<Texture2D>();

        int width = 0;
        int height = 0;

        int uvCount = 0;

        List<Vector2[]> uvList = new List<Vector2[]>();

        // 遍历所有蒙皮网格渲染器，以计算出所有需要合并的网格、UV、骨骼的信息
        foreach (SkinnedMeshRenderer smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }

            uvList.Add(smr.sharedMesh.uv);
            uvCount += smr.sharedMesh.uv.Length;

            if (smr.sharedMaterial.mainTexture != null)
            {
                textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);
                width += smr.GetComponent<Renderer>().material.mainTexture.width;
                height += smr.GetComponent<Renderer>().material.mainTexture.height;
            }

            foreach (Transform bone in smr.bones)
            {
                foreach (Transform item in transforms)
                {
                    if (item.name != bone.name) continue;
                    boneList.Add(item);
                    break;
                }
            }
            smr.enabled = false;
        }

        //MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
        ////CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        //Material[] mats = new Material[meshFilters.Length];
        //Matrix4x4 matrix = root.worldToLocalMatrix;
        //for (int i = 0; i < meshFilters.Length; i++)
        //{
        //    MeshFilter mf = meshFilters[i];
        //    MeshRenderer mr = meshFilters[i].GetComponent<MeshRenderer>();
        //    if (mr == null)
        //    {
        //        continue;
        //    }
        //    CombineInstance ci = new CombineInstance();
        //    ci.mesh = mf.sharedMesh;
        //    ci.transform = matrix * mf.transform.localToWorldMatrix;
        //    //mr.enabled = false;
        //    combineInstances.Add(ci);
        //}

        // 获取并配置角色所有的SkinnedMeshRenderer
        SkinnedMeshRenderer tempRenderer = root.gameObject.GetComponent<SkinnedMeshRenderer>();
        if (!tempRenderer)
        {
            tempRenderer = root.gameObject.AddComponent<SkinnedMeshRenderer>();
        }

        tempRenderer.sharedMesh = new Mesh();

        // 合并网格，刷新骨骼，附加材质
        tempRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
        tempRenderer.bones = boneList.ToArray();
        tempRenderer.material = material;

        Texture2D skinnedMeshAtlas = new Texture2D(get2Pow(width), get2Pow(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] atlasUVs = new Vector2[uvCount];

        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        //for (int i = 0; i < uvList.Count; i++)
        //{
        //    foreach (Vector2 uv in uvList[i])
        //    {
        //        atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
        //        atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
        //        j++;
        //    }
        //}

        // 设置贴图和UV
        tempRenderer.material.mainTexture = skinnedMeshAtlas;
        //tempRenderer.sharedMesh.uv = atlasUVs;

        // 销毁所有部件
        foreach (GameObject goTemp in targetParts)
        {
            if (goTemp)
            {
                Destroy(goTemp);
            }
        }
        Debug.Log("合并耗时 : " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
        return tempRenderer;
    }

    private static int get2Pow(int into)
    {
        int outo = 1;
        for (int i = 0; i < 10; i++)
        {
            outo *= 2;
            if (outo > into)
            {
                break;
            }
        }

        return outo;
    }
}
