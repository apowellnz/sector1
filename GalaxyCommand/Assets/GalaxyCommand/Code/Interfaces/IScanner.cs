using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IScanner 
        : IUsePower
            ,IUseSlots
            ,IUtility
    {
        IEnumerable<HarvestType> ScannedTypes { get; set; }

    }
}