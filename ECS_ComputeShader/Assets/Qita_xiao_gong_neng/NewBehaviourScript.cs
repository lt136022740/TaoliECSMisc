using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {
    public Text text;
	// Use this for initialization
	void Start () {
        //string path = Application.dataPath + "/abc.prefab";
        //Debug.Log(path);
        //Debug.Log(AssetDatabase.LoadAssetAtPath<GameObject>("assets/abc.prefab"));
        //AssetDatabase.ge
        Object b = AssetDatabase.LoadAssetAtPath<Object>("assets/StreamingAssets/android/ASsetbundles_#font");
        Debug.Log(b);

        Debug.Log(text.text);
        Debug.Log(text.preferredHeight);
        Debug.Log(text.preferredWidth);
      }
	
	// Update is called once per frame
	void Update () {
		
	}
}
