using System;
using System.Collections.Generic;
using Assets.GalaxyCommand.Code.Common;
using Assets.GalaxyCommand.Code.Game.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Assets.GalaxyCommand.Code.Game.Controllers
{
    public abstract class GameUnitController :
        NetworkBehaviour
        , ISelectHandler
        , IPointerClickHandler
        , IDeselectHandler
    {
        private Canvas _localCanvas;
        private RectTransform _rectTransform;
        private GameObject _selectionImage;
        public GameObject SelectionBox;

        protected static HashSet<GameObject> AllUnits
        {
            get { return GameUnitService.GetAllUnits(); }
        }

        public bool IsSelected { get; set; }


        public virtual void OnDeselect(BaseEventData eventData)
        {
            IsSelected = false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            DeselectAll(eventData);
            OnSelect(eventData);
        }

        public  static void DeselectAll(BaseEventData eventData)
        {
            if (!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl))
                foreach (var unit in AllUnits)
                    unit.GetComponent<GameUnitController>().OnDeselect(eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
           
            IsSelected = true;
            EventsService.TiggerUnitSelectEvent(this, gameObject);
        }

        private void Update()
        {
            OverridableUpdate();
        }

        public virtual void OverridableUpdate()
        {
            _selectionImage.SetActive(IsSelected);
            if (IsSelected)
            {
                var rect = GameUnitService.GetBoundsOfUnity(this);

                _rectTransform.position = new Vector2(rect.xMin + rect.width / 2, rect.yMin + rect.height / 2);
                _rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            }
        }

        public void Start()
        {
            _localCanvas = gameObject.GetComponentInChildren<Canvas>();
            if (_localCanvas == null)
                throw new NullReferenceException("No Ship canvas was detected.");
            _localCanvas.transform.SetParent(transform, true);

            _selectionImage = Instantiate(SelectionBox);
            _selectionImage.transform.SetParent(_localCanvas.transform, true);
            _rectTransform = _selectionImage.GetComponent<RectTransform>();
            _selectionImage.SetActive(false);
            tag = TagCollection.GameUnitTag;
        }
    }
}