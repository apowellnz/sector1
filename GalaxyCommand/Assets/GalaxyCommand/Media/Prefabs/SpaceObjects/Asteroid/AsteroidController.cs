using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Enums;
using Assets.GalaxyCommand.Code.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

public class AsteroidController
  : NetworkBehaviour
    , IHarvestResource
{
  private int _speedX;
  private int _speedY;
  private int _speedZ;
  public IDictionary<HarvestType, int> Resources { get; private set; }

  public KeyValuePair<HarvestType, int> Harvest(HarvestingStyle tool)
  {
    return new KeyValuePair<HarvestType, int>(HarvestType.Gas, 1);
  }

  // Use this for initialization
  private void Start()
  {
    Resources = new Dictionary<HarvestType, int>();
    _speedX = Random.Range(-10, 10);
    _speedY= Random.Range(-10, 10);
    _speedZ= Random.Range(-10, 10);

    // todo - change to a beter system
    Resources.Add(HarvestType.Gas, 100);
    Resources.Add(HarvestType.Ice, 100);
  }

  // Update is called once per frame
  private void Update()
  {
    transform.Rotate(Vector3.up * Time.deltaTime * _speedX, Space.World);
    transform.Rotate(Vector3.right * Time.deltaTime * _speedY, Space.World);
    transform.Rotate(Vector3.forward * Time.deltaTime * _speedZ, Space.World);
  }
}
