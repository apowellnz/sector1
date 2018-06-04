using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class EventsService
    {
        public static event EventHandler<SelectUnityArg> OnSelectUnity;

        public static void TiggerUnitSelectEvent(object sender,GameObject gameObject)
        {
            if (OnSelectUnity != null)
            {
                OnSelectUnity(sender, new SelectUnityArg(gameObject));
            }
        }
    }
}
