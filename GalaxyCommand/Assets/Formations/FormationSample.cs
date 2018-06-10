using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using com.t7t.formation;

public class FormationSample : MonoBehaviour
{

    public FormationGrid formationGrid;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            Debug.Log("Disband!");
            formationGrid.ChangeState(FormationStates.Disband);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Get moving!");
            // get the grid moving
            formationGrid.ChangeState(FormationStates.Move);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            formationGrid.AssignObjectsToGrid(GameUnitService.GetSelectedUnits().Select(g => g.gameObject).ToList());
            formationGrid.ChangeState(FormationStates.Form);
            
            
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            Debug.Log("Changing to Wedge!");

            formationGrid.CalculateGrid(GridTypes.Wedge9);
            formationGrid.ChangeState(FormationStates.Form);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            Debug.Log("Changing to Column!");

            formationGrid.CalculateGrid(GridTypes.Column10);
            formationGrid.ChangeState(FormationStates.Form);
        }



    }
}
