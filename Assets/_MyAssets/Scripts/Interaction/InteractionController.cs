using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EInteractionType
{
    Item,
    HideableObject
}

public class InteractionObject
{
    public EInteractionType type;
    public readonly GameObject obj;

    public InteractionObject(EInteractionType type, GameObject obj)
    {
        this.type = type;
        this.obj = obj;
    }
}

public class InteractionController : Singleton<InteractionController>
{
    [SerializeField] private PlayerInputData _inputData;
    
    private static readonly Vector3 CAMERA_CENTER_POINT = new (0.5f, 0.5f, 0);
    
    private Dictionary<int, InteractionObject> _interactionObjects = new();
    
    public InteractionObject NearestObject => new (_nearestObjectType, _nearestObject);

    private GameObject _nearestObject;
    private EInteractionType _nearestObjectType;
    
    private float _nearestObjectDistance;

    private Camera _mainCamera;
    
    private EInteractionType _currentInteractionType;

    private GameObject _interactionUI;
    private RectTransform _interactionUIRectTransform;

    private void Start()
    {
        _mainCamera = Camera.main;
        _interactionUI = PlayerMove.Instance.PlayerCanvas.transform.Find("InteractionUI").gameObject;
        _interactionUIRectTransform = _interactionUI.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _inputData.interactionEvent += HandleInteraction;
    }

    private void Update()
    {
        SelectCloserInteractionObject();
        ShowInteractionUI();
    }
    
    private void SelectCloserInteractionObject()
    {
        _nearestObjectDistance = Mathf.Infinity;
        _nearestObject = null;
        
        if (_interactionObjects.Count == 0)
        {
            return;
        }

        bool isOnlyOne = _interactionObjects.Count == 1;

        foreach (var data in _interactionObjects)
        {
            Vector3 objPosition = data.Value.obj.transform.position;
            Vector3 playerPosition = transform.position;
            Vector3 direction = (objPosition - playerPosition).normalized;
            
            Debug.DrawRay(transform.position, direction * 5.0f, Color.blue);
            
            Ray ray = new Ray(transform.position, direction);
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)
                || hit.transform.gameObject.GetInstanceID() != data.Key)
            {
                continue;
            }
            
            bool isItem = data.Value.type == EInteractionType.Item;
            Vector3 curObjViewportPosition = _mainCamera.WorldToViewportPoint(data.Value.obj.transform.position);
            if (curObjViewportPosition.z < 0)
            {
                continue;
            }

            if (!isItem && PlayerMove.Instance.CheckPlayerState(EPlayerState.Hide))
            {
                return;
            }
            
            if (isOnlyOne)
            {
                if (isItem)
                {
                    if (ItemController.Instance.IsItemInHand)
                    {
                        continue;
                    }
                }
                else
                {
                    // infront?
                    if (!HideActionController.Instance.IsInFrontOfHideableObject(hit.transform))
                    {
                        continue;
                    }
                }
                
                _nearestObject = data.Value.obj;
                _nearestObjectType = data.Value.type;
                _nearestObjectDistance = CalculateDistanceFromCenterToObject(curObjViewportPosition);
                return;
            }

            if (!IsNearest(curObjViewportPosition, out float distance))
            {
                continue;
            }

            if (isItem && ItemController.Instance.IsItemInHand)
            {
                continue;
            }
            
            _nearestObject = data.Value.obj;
            _nearestObjectType = data.Value.type;
            _nearestObjectDistance = distance;
        }
        
    }

    private bool IsInScreen(Vector3 viewportPosition)
    {
        bool isXInScreen = viewportPosition.x < 1.0f && viewportPosition.x > 0.0f;
        bool isYInScreen = viewportPosition.y < 1.0f && viewportPosition.y > 0.0f;
        return isXInScreen && isYInScreen;
    }

    private bool IsNearest(Vector3 viewportPosition, out float distanceFromCenter)
    {
        distanceFromCenter = Mathf.Infinity;
        if (!IsInScreen(viewportPosition))
        {
            return false;
        }
        
        distanceFromCenter = CalculateDistanceFromCenterToObject(viewportPosition);
        bool isNearestObject = distanceFromCenter < _nearestObjectDistance;

        return isNearestObject;
    }

    private float CalculateDistanceFromCenterToObject(Vector3 viewportPosition)
    {
        viewportPosition.z = 0;
        return (viewportPosition - CAMERA_CENTER_POINT).magnitude;
    }

    private void ShowInteractionUI()
    {
        _interactionUI.SetActive(_nearestObject != null);

        if (!_interactionUI.activeSelf)
        {
            return;
        }
        
        Vector3 nearestObjectPosition = _nearestObject.transform.position;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(nearestObjectPosition);
        const float OFFSET = 50.0f;
        screenPosition.x += OFFSET;
        _interactionUIRectTransform.position = screenPosition;
    }

    public void AddInteractionObject(EInteractionType type, GameObject obj)
    {
        InteractionObject interactionObject = new(type, obj);
        _interactionObjects.Add(obj.GetInstanceID(), interactionObject);
    }

    public void RemoveInteractionObject(GameObject obj)
    {
        _interactionObjects.Remove(obj.GetInstanceID());
    }

    private void HandleInteraction()
    {
        if (!_interactionUI.activeSelf)
        {
            return;
        }

        NearestObject.obj.GetComponentInChildren<InteractionObjectHandler>().Interaction();
    }
}