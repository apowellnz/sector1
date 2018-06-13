using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IUseSlots
    {
        IDictionary<UnitSlot,int> SlotUsed { get; set; }
    }
}