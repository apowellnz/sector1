using System;
using Assets.GalaxyCommand.Code.Game.Controllers;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class UISelectionController : MonoBehaviour
{

    public GameObject SelectionBoxImage;
	// Use this for initialization
	void Start () {
	    if (SelectionBoxImage == null)
	    {
	        throw new NullReferenceException("SelectionBoxImage is not set in the UISelectionController.");
	    }	

	}

    // Update is called once per frame
	void Update () {
		
	}
}
