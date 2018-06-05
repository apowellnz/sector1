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
        private static GameUnitService _gameUnitService;
        private Canvas _localCanvas;
        public GameObject SelectionBox;
        private GameObject _selectionImage;
        private RectTransform _rectTransform;

        protected static HashSet<GameObject> AllUnits
        {
            get { return _gameUnitService.GetAllUnits(); }
        }

        public bool IsSelected { get; set; }


        public virtual void OnDeselect(BaseEventData eventData)
        {
            IsSelected = false;
            
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnSelect(eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {

           

            if (!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl))
            {
                foreach (var unit in AllUnits)
                    unit.GetComponent<GameUnitController>().OnDeselect(eventData);
            }
            IsSelected = true;
            //            EventsService.TiggerUnitSelectEvent(this,this.gameObject); // todo - turn this back on when completed testing
        }

        void Update()
        {
            OverridableUpdate();
        }

        public virtual void OverridableUpdate()
        {
            _selectionImage.SetActive(IsSelected);
            if (IsSelected)
            {
                var rect = GameUnitService.GetBoundsOfUnity(this);

                _rectTransform.position = new Vector2(rect.xMin + rect.width /2, rect.yMin +rect.height/2);
                _rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            }  
        }

        public void Start()
        {
            _gameUnitService = new GameUnitService();
            _localCanvas = gameObject.GetComponentInChildren<Canvas>();
            if(_localCanvas == null)
                throw  new NullReferenceException("No Ship canvas was detected.");
            _localCanvas.transform.SetParent(transform,true);

            _selectionImage = Instantiate(SelectionBox);
            _selectionImage.transform.SetParent(_localCanvas.transform,true);
            _rectTransform = _selectionImage.GetComponent<RectTransform>();
            _selectionImage.SetActive(false);
            tag = TagCollection.GameUnitTag;
        }
    }
}