using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class InputService
    {
        public static bool  IsPressingCtrl()
        {
            return Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.A);
        }
    }
}
