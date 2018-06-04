using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameRootController : NetworkBehaviour
{

    [SerializeField]
    public GameObject Grid;

	// Use this for initialization
	void Start () {

        Grid.transform.position = Vector3.zero;
        
	    
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
