using System;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Enums;
using Random = UnityEngine.Random;

namespace Assets.GalaxyCommand.Code.Game.Services
{
  internal class OreService
  {
    public static IDictionary<Ore, int> GenerateOreAmount(AsteroidController this0)
    {
      var result = new Dictionary<Ore, int>();
      foreach (var oreType in Enum.GetValues(typeof(HarvestType)))
        result.Add(new Ore
        {
          HarvestType = (HarvestType) oreType
        }, Random.Range(0, 100));

      return result;
    }
  }
}
