using System.Linq;
using Assets.GalaxyCommand.Code.Game.Services;
using com.t7t.formation;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridController 
    : MonoBehaviour
    , IPointerClickHandler
    
{
    private bool _rightButtonPressed;

    public GameObject TargetPositionObject;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var target = Instantiate(TargetPositionObject);
            target.transform.position = eventData.pointerCurrentRaycast.worldPosition;

            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                foreach (var unit in GameUnitService.GetSelectedUnits())
                    unit.AddWayPoint(target.transform);
            }
            else
            {
                var selectedUnits = GameUnitService.GetSelectedUnits().Select(g => g.gameObject).ToList();
                FormationGrid formationGrid = FormationManager.GetFormationGridInstance();
                formationGrid.SetAnchorTransform(target.transform);
                formationGrid.AssignObjectsToGrid(selectedUnits); 
                formationGrid.ChangeState(FormationStates.Move);
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        if (TargetPositionObject == null) Debug.LogError("TargetPositionObject can not be null on Grid Controller");
    }

    // Update is called once per frame
    private void Update()
    {
    }
}