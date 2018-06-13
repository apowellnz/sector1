using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;
using Assets.GalaxyCommand.Code.Interfaces;

namespace Assets.GalaxyCommand.Code.Common
{
    public class Ore
    {
        public HarvestType HarvestType { get; set; }
        public IEnumerable<IHarvestTool> AcceptedHarvestingTools { get; set; }
    }
}