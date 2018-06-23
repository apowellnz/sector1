using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;

namespace Assets.GalaxyCommand.Code.Interfaces
{
  public interface IHarvestResource
  {
    IDictionary<HarvestType, int> Resources { get; }
    KeyValuePair<HarvestType, int> Harvest(HarvestingStyle tool);
  }
}
