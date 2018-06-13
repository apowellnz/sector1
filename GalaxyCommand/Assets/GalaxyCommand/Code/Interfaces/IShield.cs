using System.Collections.Generic;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IShield 
        :IUsePower
            ,IUseSlots
            ,IUtility
    {
        IDictionary<IProjectile, float> ProtectionAmount { get; set; }
    }
}