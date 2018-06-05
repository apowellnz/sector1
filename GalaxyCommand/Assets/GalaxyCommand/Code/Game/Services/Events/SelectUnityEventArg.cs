using System;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services.Events
{
    public class SelectUnityEventArg : EventArgs
    {
        public SelectUnityEventArg(GameObject gameObject)
        {
            SelectedGameObject = gameObject;
        }

        public GameObject SelectedGameObject { get; set; }
    }
}