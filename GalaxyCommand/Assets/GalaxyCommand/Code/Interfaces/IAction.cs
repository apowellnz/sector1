using UnityEngine;
using UnityEngine.UI;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IAction
    {
        [SerializeField]
        string Name { get; set; }

        [SerializeField]
        Image Icon { get; set; }
    }
}