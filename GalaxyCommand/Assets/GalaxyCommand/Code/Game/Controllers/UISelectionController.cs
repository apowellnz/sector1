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

        EventsService.OnSelectUnity += EventsServiceOnOnSelectUnity;
	}

    private void EventsServiceOnOnSelectUnity(object sender, SelectUnityArg selectUnityArg)
    {
        var selectionBox = Instantiate(SelectionBoxImage);
        selectionBox.transform.parent = gameObject.GetComponent<Canvas>().transform;
        var rect = GameUnitService.GetBoundsOfUnity(selectUnityArg.SelectedGameObject.GetComponent<GameUnitController>());

        var rectTransform = selectionBox.GetComponent<RectTransform>();

        rectTransform.position = new Vector2(rect.xMin, rect.yMin);

        rectTransform.sizeDelta = new Vector2(rect.width, rect.height);

    }

    // Update is called once per frame
	void Update () {
		
	}
}
