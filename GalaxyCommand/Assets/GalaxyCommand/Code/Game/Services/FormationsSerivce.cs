using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Game.Controllers;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class FormationsSerivce
    {
        private readonly int _maxRows;
        private static GameObject _waypointObject;

        public FormationsSerivce(GameObject waypointObject, int maxRows = 10)
        {
            _maxRows = maxRows;
            _waypointObject = waypointObject;
        }

        public Dictionary<Vector3, GameUnitController> GetFomation(
            Transform targetLocation,
            HashSet<GameUnitController> units,
            Vector3 StartLocation,
            FormationType formationType)
        {
            var formationAnchor = new GameObject("Formation Anchor");
            var customerGrid = new Dictionary<Vector3, GameUnitController>();
            var unitQueue = new Queue<GameUnitController>(units);
            formationAnchor.transform.position = targetLocation.position;
            formationAnchor.transform.LookAt(StartLocation);

            switch (formationType)
            {
                case FormationType.Lines:
                    var rows = units.Count / _maxRows;
                    var startY = 0f;
                    var startX = Mathf.Round(_maxRows / 2) * -1;

                    for (var r = 0; r < rows; r++)
                    {
                        for (var j = 0; j < _maxRows; j++)
                            startX = AddWaypoint(unitQueue, startX, formationAnchor, startY, customerGrid);
                        startY += -1f;
                    }
                    startX = Mathf.Round(unitQueue.Count / 2) * -1;
                    do
                    {
                        startX = AddWaypoint(unitQueue, startX, formationAnchor, startY, customerGrid);
                    } while (unitQueue.Any());

                    break;
            }
            Object.Destroy(formationAnchor,3);
            return customerGrid;
        }

        private static float AddWaypoint(Queue<GameUnitController> _unitQueue, float startX, GameObject formationAnchor,
            float startY,
            Dictionary<Vector3, GameUnitController> customerGrid)
        {
            var newUnit = _unitQueue.Dequeue();
            var agent = newUnit.GetComponentInChildren<NavMeshAgent>();
            startX += agent.radius * 2;
            var wayPoint = GameObject.Instantiate(_waypointObject);
            wayPoint.transform.parent = formationAnchor.transform;
            wayPoint.transform.localPosition = new Vector3(startX, 0f, startY);
            customerGrid.Add(wayPoint.transform.position, newUnit);
            return startX;
        }
    }

    public enum FormationType
    {
        Lines
    }
}