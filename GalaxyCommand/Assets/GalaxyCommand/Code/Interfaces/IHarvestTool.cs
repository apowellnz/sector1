using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IHarvestTool
        : IUsePower
            , IUseSlots
    {
        IDictionary<HarvestType,float> HarvestChance { get; set; }
        KeyValuePair<HarvestType, int> Harvest(IHarvestable harvestableItem);
    }
}