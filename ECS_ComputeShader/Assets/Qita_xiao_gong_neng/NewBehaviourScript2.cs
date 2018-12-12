using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript2 : MonoBehaviour {
    public Slider slider;
	// Use this for initialization
	void Start () {
        slider.onValueChanged.AddListener(this.Onv);
        slider.value = 1;
    }

    public void Onv(float v)
    {
        Debug.Log("v: " + v);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
