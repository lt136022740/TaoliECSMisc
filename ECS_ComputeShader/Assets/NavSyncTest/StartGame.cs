using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject go = Resources.Load("Player") as GameObject;
        for (int i = 0; i < 120; i++)
        {
            float x = -23 + i % 40 * 2;
            float y = 1;
            float z = -53 + i / 40 * 2;
            Vector3 v = new Vector3(x, y, z);
            GameObject g = Instantiate(go, v, Quaternion.identity) as GameObject;
            Player player = g.GetComponent<Player>();
            player.Init();
        }
    }
	
	// Update is called once per frame
	void Update () {
                                                 
	}
}
