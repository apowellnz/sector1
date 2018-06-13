using UnityEngine;

namespace Assets.GalaxyCommand.Code.Interfaces
{
    public interface IProjectile
    {
        string Name { get; set; }
        float Damage { get; set; }
        float Speed { get; set; }

        GameObject Prefab { get; set; }
        GameObject HitPrefab { get; set; }
    }
}