using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GalaxyCommand.Code.AI;
using Assets.GalaxyCommand.Code.Game.Units;
using UnityEngine;

namespace Assets.GalaxyCommand.Media.Prefabs.Units.TestUnit
{
    public class GuardingState<TUnit> : State<TUnit> where TUnit: BaseUnitController
    {
        public override bool CheckState(TUnit unit)
        {
            return true;
        }

        public override void EnterState(TUnit owner)
        {
            Debug.Log("EnterState");
        }

        public override void ExitState(TUnit owner)
        {
            Debug.Log("ExitState");
        }

        public override void UpdateState(TUnit owner)
        {
            Debug.Log("UpdateState");
        }
    }
}
