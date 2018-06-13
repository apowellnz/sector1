using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Common;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IHaveCost
    {
        IDictionary<Ore,int> Cost { get; set; }

    }
}