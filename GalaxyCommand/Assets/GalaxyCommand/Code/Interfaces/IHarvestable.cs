using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Common;
using UnityEngine.Networking;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public abstract class Harvestable : NetworkBehaviour
    {
        public abstract IDictionary<Ore, int> OreAmount { get; set; }
      public abstract Ore Harvest(IHarvestTool harvestTool);
    }
}
