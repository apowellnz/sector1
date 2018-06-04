using System.Collections;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Game.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestController : GameUnitController
{

    public Material Normal;
    public Material Selected;


    public void Update()
    {
        gameObject.GetComponentInChildren<Renderer>().material = IsSelected ? Selected : Normal;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        Debug.Log("TestUnit Selected");
        base.OnSelect(eventData);
    }
}
