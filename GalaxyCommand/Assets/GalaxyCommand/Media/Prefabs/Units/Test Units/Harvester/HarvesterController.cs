using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.AI;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Game.Controllers;
using Assets.GalaxyCommand.Code.Interfaces;
using UnityEngine;

public class HarvesterController
  : GameUnitController 
{

  public StateMachine<HarvesterController> StateMachine;
	// Use this for initialization
	void Start () {
	  if (StateMachine == null)
	  {
	    StateMachine = new StateMachine<HarvesterController>(this);
	    StateMachine.Add(new HarvestingState(5));
      StateMachine.Add(new ReturnToDepo(5));
	    StateMachine.Add(new FleeFromAttack(6));
	    StateMachine.Add(new UserImportMove(10));
	  }
	 
	}
	
	// Update is called once per frame
	void Update () {
		StateMachine.Update();
	}
}

internal class HarvestingState : State<HarvesterController>
{
  public override bool DecideThisState(HarvesterController owner)
  {
    
    var harvestableObject = GameObject.FindObjectsOfType<Harvestable>().Select(h =>h.gameObject).GetClosest(owner.gameObject);
    if (harvestableObject != null && HasRoomForMore(owner))
    {
      return true;
    }
    
    return false;
  }

  private bool HasRoomForMore(HarvesterController owner)
  {
      var storage = owner.
  }

  public override void EnterState(HarvesterController owner)
  {
    throw new System.NotImplementedException();
  }

  public override void ExitState(HarvesterController owner)
  {
    throw new System.NotImplementedException();
  }

  public override void UpdateState(HarvesterController owner)
  {
    throw new System.NotImplementedException();
  }

  public HarvestingState(int priority) : base(priority)
  {
  }
}
