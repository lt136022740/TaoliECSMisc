using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
    public NavMeshAgent agent;
    public Transform des;
	// Use this for initialization
	void Start () {
      
    }

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        des = GameObject.Find("Sphere").transform;
        agent.SetDestination(des.position);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
