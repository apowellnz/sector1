using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Game.Controllers;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class GameUnitService
    {
        public static HashSet<GameUnitController> GetAllUnits()
        {
            return GameObject.FindObjectsOfType<GameUnitController>().ToHashSet();
        }

        public static HashSet<GameUnitController> GetSelectedUnits(string group = null)
        {
            if (string.IsNullOrEmpty(group) == false)
            {
                return GameObject.FindObjectsOfType<GameUnitController>().Where(u => u.Group == group).ToHashSet();
            }
            return GameObject.FindObjectsOfType<GameUnitController>().Where(u => u.IsSelected).ToHashSet();
        }
    }
}