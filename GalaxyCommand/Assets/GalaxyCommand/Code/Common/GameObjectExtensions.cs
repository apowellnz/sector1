using System.Collections.Generic;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Common
{
  public static class GameObjectExtensions
  {
    public static GameObject GetClosest(this IEnumerable<GameObject> val, GameObject owner)
    {
      float? distance = null;
      GameObject result = null;
      foreach (var gameObject in val)
      {
        var distanceBetween = Vector3.Distance(owner.transform.position, gameObject.transform.position);
        if (distance == null || distance > distanceBetween)
        {
          distance = distanceBetween;
          result = gameObject;
        }
      }
      return result;
    }
  }
}
