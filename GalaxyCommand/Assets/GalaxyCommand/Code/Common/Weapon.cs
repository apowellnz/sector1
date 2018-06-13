using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;
using Assets.GalaxyCommand.Code.Interfaces;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Common
{
    public abstract class Weapon<TProjectile>
        : IUseSlots
            , IUsePower
            , IUtility
        where TProjectile: IProjectile
    {
        public int Ammo;

        public abstract bool Fire(GameObject target);

        public abstract void StopFiring();

        public IDictionary<UnitSlot, int> SlotUsed { get; set; }
        public float ReloadTime { get; private set; }
        public float PowerConsumption { get; set; }
    }
}