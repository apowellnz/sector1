using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Game.Units;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class GameUnitService
    {
        public static HashSet<BaseUnitController> GetAllUnits()
        {
            return GameObject.FindObjectsOfType<BaseUnitController>().ToHashSet();
        }

        public static HashSet<BaseUnitController> GetSelectedUnits(string group = null)
        {
            if (string.IsNullOrEmpty(group) == false)
            {
                return GameObject.FindObjectsOfType<BaseUnitController>().Where(u => u.Group == group).ToHashSet();
            }
            return GameObject.FindObjectsOfType<BaseUnitController>().Where(u => u.IsSelected).ToHashSet();
        }
    }
}
