using System.Linq;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.GalaxyCommand.Code.Game.Grid
{
  public class GridController 
    : MonoBehaviour
      , IPointerClickHandler
    
  {
    private bool _rightButtonPressed;

    public GameObject TargetPositionObject;
    private FormationsSerivce _formationService;

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == PointerEventData.InputButton.Right && GameUnitService.GetSelectedUnits().Any())
      {
        var target = Instantiate(TargetPositionObject);
        target.transform.position = eventData.pointerCurrentRaycast.worldPosition;
        if (InputService.IsPressingAlt())
        {
          var selectedUnits = GameUnitService.GetSelectedUnits();
          Vector3 lastWaypoint = selectedUnits.First().WayPoints.Any()
            ? selectedUnits.First().WayPoints.Peek()
            : selectedUnits.First().transform.position;
          var formation = _formationService.GetFomation(target.transform, selectedUnits, lastWaypoint, FormationType.Lines);
          foreach (var waypoints in formation)
          {
            waypoints.Value.AddWayPoint(waypoints.Key,InputService.IsPressingShift() );
          }
        }
        else
        {
          if (GameUnitService.GetSelectedUnits().Any())
          {
            var formation = _formationService.GetFomation(target.transform, GameUnitService.GetSelectedUnits(), GameUnitService.GetSelectedUnits().First().transform.position, FormationType.Lines);
            foreach (var waypoints in formation)
            {
              waypoints.Value.CmdMoveAndReset(waypoints.Key); 
            }
          }
        }
        Destroy(target);
      }
    }

    // Use this for initialization
    private void Start()
    {
      if (TargetPositionObject == null) Debug.LogError("TargetPositionObject can not be null on Grid Controller");
      _formationService = new FormationsSerivce(TargetPositionObject,6);
    }

  }
}
