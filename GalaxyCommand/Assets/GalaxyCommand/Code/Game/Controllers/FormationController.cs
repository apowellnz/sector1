using System.Collections;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;

public class FormationController : MonoBehaviour
{

    public int UnitCount;
    public float Margin =1.5f;
    public GameObject NavigationObject;

	// Update is called once per frame
	void Update ()
	{
	    var selectedUnits = GameUnitService.GetSelectedUnits();


        
	    for (int i = 0; i < selectedUnits.Count; i++)
	    {
	        var navigationPoint = Instantiate(NavigationObject);
	        var bounds = navigationPoint.GetComponentInChildren<Renderer>().bounds;
	        float startPosition = ((bounds.max.x + Margin) * selectedUnits.Count) / 2 * -1;

            navigationPoint.transform.parent = transform;
            navigationPoint.transform.localPosition = new Vector3(navigationPoint.transform.position.x+((bounds.max.x + Margin) * i)+ startPosition, navigationPoint.transform.position.y, navigationPoint.transform.position.z);
	        Destroy(navigationPoint, 3);
        }

        
	}
}
