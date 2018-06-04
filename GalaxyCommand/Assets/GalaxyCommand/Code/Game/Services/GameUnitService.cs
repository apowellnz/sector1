using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Common;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class GameUnitService
    {
        public HashSet<GameObject> GetAllUnits()
        {
            return GameObject.FindGameObjectsWithTag(TagCollection.GameUnitTag).ToHashSet();
        }
    }
}