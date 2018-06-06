using System.Collections;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Game.Controllers;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class GridController : MonoBehaviour, IPointerClickHandler
{

    public GameObject TargetPositionObject;
    private bool _rightButtonPressed;

    // Use this for initialization
	void Start () {
		if(TargetPositionObject == null) Debug.LogError("TargetPositionObject can not be null on Grid Controller");
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var target = Instantiate(TargetPositionObject);
            target.transform.position = eventData.pointerCurrentRaycast.worldPosition;



            foreach (var unit in GameUnitService.GetSelectedUnits())
            {
                unit.MovePosition(target.transform);
            }
            Destroy(target, 5);
        }
           

    }

}
