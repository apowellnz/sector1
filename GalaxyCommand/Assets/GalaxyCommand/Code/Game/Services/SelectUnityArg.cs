using System;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class SelectUnityArg : EventArgs
    {
        public SelectUnityArg(GameObject gameObject)
        {
            SelectedGameObject = gameObject;
        }

        public GameObject SelectedGameObject { get; set; }
    }
}