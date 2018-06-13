using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Enums;
using Assets.GalaxyCommand.Code.Game.Services;
using Assets.GalaxyCommand.Code.Interfaces;
using UnityEngine;

public class AsteroidController : MonoBehaviour, IHarvestable {

	// Use this for initialization
	void Start ()
	{
	  OreAmount = OreService.GenerateOreAmount(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public IDictionary<Ore, int> OreAmount { get; set; }
  public Ore Harvest(IHarvestTool harvestTool)
  {
    return new Ore
    {
      HarvestType = HarvestType.Gold
    };
  }
}
