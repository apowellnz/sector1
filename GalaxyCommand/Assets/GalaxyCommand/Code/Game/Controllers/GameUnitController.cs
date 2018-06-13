using System;
using System.Collections.Generic;
using System.Linq;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Enums;
using Assets.GalaxyCommand.Code.Game.Services;
using Assets.GalaxyCommand.Code.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.GalaxyCommand.Code.Game.Controllers
{
  [RequireComponent(typeof(NavMeshAgent))]
  public abstract class GameUnitController :
    NetworkBehaviour
    , ISelectHandler
    , IPointerClickHandler
    , IDeselectHandler
  {
    private Canvas _localCanvas;
    private NavMeshAgent _navMesh;
    private RectTransform _rectTransform;
    private GameObject _selectionImage;
    private Queue<Vector3> _wayPoints;
    public GameObject SelectionBox;

    public UnitSizeEnum UnitSize;

    protected static HashSet<GameUnitController> AllUnits
    {
      get { return GameUnitService.GetAllUnits(); }
    }

    public bool IsSelected { get; set; }

    public string Group { get; set; }

    public Queue<Vector3> WayPoints
    {
      get
      {
        if (_wayPoints == null) _wayPoints = new Queue<Vector3>();
        return _wayPoints;
      }
      set { _wayPoints = value; }
    }


    public virtual void OnDeselect(BaseEventData eventData)
    {
      IsSelected = false;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      DeselectAll(eventData);
      OnSelect(eventData);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
      IsSelected = true;
      EventsService.TiggerUnitSelectEvent(this, gameObject);
    }



    public static void DeselectAll(BaseEventData eventData)
    {
      if (!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl))
        foreach (var unit in AllUnits)
          unit.OnDeselect(eventData);
    }

    private void Update()
    {
      UpdateOverridable();
      UpdateGroupingCheck();
      UpdateSelectionBox();
      UpdateCheckWaypoints();
      UpdateAvoidencePriority();
    }

    private void UpdateAvoidencePriority()
    {
      _navMesh.avoidancePriority = (int) _navMesh.remainingDistance + (int) UnitSize;
    }


    private void UpdateSelectionBox()
    {
      _selectionImage.SetActive(IsSelected);
      if (IsSelected)
      {
        var rect = BoundsService.GetBoundsOfObject(this);

        _rectTransform.position = new Vector2(rect.xMin + rect.width / 2, rect.yMin + rect.height / 2);
        _rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        _selectionImage.GetComponentInChildren<Text>().text = Group;
      }
    }


    private void UpdateCheckWaypoints()
    {
      if (WayPoints.Any() && _navMesh.remainingDistance <= 0.5f)
        MovePosition(WayPoints.Dequeue(), false);
    }


    private void UpdateGroupingCheck()
    {
      if (InputService.IsPressingCtrl())
        foreach (var group in KeyBindingsService.GroupList)
          if (Input.GetKey(group))
            CreateGroup(group.ToString().Replace("Keypad", string.Empty));

      foreach (var group in KeyBindingsService.GroupList)
        if (Input.GetKey(group))
        {
          foreach (var unit in AllUnits)
            unit.OnDeselect(new BaseEventData(EventSystem.current));

          foreach (var unit in GameUnitService.GetSelectedUnits(group.ToString()
            .Replace("Keypad", string.Empty)))
            unit.OnSelect(new BaseEventData(EventSystem.current));
        }
    }

    private void CreateGroup(string group)
    {
      foreach (var unit in GameUnitService.GetSelectedUnits(group))
        unit.OnDeselect(new BaseEventData(EventSystem.current));

      foreach (var unit in GameUnitService.GetSelectedUnits())
        unit.Group = group;
    }

    public virtual void UpdateOverridable()
    {
    }

    public void Start()
    {
      _localCanvas = gameObject.GetComponentInChildren<Canvas>();
      _navMesh = GetComponent<NavMeshAgent>();
      if (_localCanvas == null)
        throw new NullReferenceException("No Ship canvas was detected.");
      _localCanvas.transform.SetParent(transform, true);

      _selectionImage = Instantiate(SelectionBox);
      _selectionImage.transform.SetParent(_localCanvas.transform, true);
      _rectTransform = _selectionImage.GetComponent<RectTransform>();
      _selectionImage.SetActive(false);
      tag = TagCollection.GameUnitTag;

    }


    public void MovePosition(Vector3 position, bool clearWaypoints = true)
    {
      _navMesh.SetDestination(position);
      if (clearWaypoints)
        WayPoints.Clear();
    }

    public void AddWayPoint(Vector3 targetTransform)
    {
      WayPoints.Enqueue(targetTransform);
    }
  }
}
