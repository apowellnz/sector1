using Assets.GalaxyCommand.Code.Game.Units;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestController : BaseUnitController
{
    public Material Normal;
    public Material Selected;


    public override void UpdateOverridable()
    {
        gameObject.GetComponentInChildren<Renderer>().material = IsSelected ? Selected : Normal;
        base.UpdateOverridable();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        Debug.Log("TestUnit Selected");
        base.OnSelect(eventData);
    }
}
