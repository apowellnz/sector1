using System.Collections;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Game.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestController : GameUnitController
{

    public Material Normal;
    public Material Selected;

    public override void OverridableUpdate()
    {
        gameObject.GetComponentInChildren<Renderer>().material = IsSelected ? Selected : Normal;
        base.OverridableUpdate();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        Debug.Log("TestUnit Selected");
        base.OnSelect(eventData);
    }
}
