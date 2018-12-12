using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class masker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        setClip();
    }
	
	// Update is called once per frame
	void Update () {
		setClip();
	}

    public Image img;
    public RectTransform mask;
    public Material mt;
    private void Awake()
    {
        mask = GetComponent<RectTransform>();
        mt =  img.material;
        GetComponentInParent<ScrollRect>().onValueChanged.AddListener((e) => { setClip(); });
        setClip();

    }

    void setClip()
    {
        Vector3[] wc = new Vector3[4];
        mask.GetComponent<RectTransform>().GetWorldCorners(wc);        // 计算world space中的点坐标
        var clipRect = new Vector4(wc[0].x, wc[0].y, wc[2].x, wc[2].y);// 选取左下角和右上角
        mt.SetVector("_ClipRect1", clipRect);                           // 设置裁剪区域
    }
}
