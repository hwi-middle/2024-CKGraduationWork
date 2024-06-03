using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EInteractionType
{
    Item,
    HideableObject,
    Overstep,
    Cube
}

public class InteractionObject
{
    public readonly EInteractionType type;
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
    
    private Dictionary<int, InteractionObject> _interactableObjects = new();
    
    private InteractionObject NearestObject => new (_nearestObjectType, _nearestObject);

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

    private void OnDisable()
    {
        _inputData.interactionEvent -= HandleInteraction;
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
        
        if (_interactableObjects.Count == 0 || PlayerStateManager.Instance.CheckPlayerState(EPlayerState.Hide))
        {
            return;
        }

        if (_interactableObjects.Count == 1)
        {
            var data = _interactableObjects.First();
            ProcessOnlyOneInteractableObjects(data);
            return;
        }

        RaycastHit hit = new();
        foreach (var data in _interactableObjects)
        {
            Vector3 curObjViewportPosition = _mainCamera.WorldToViewportPoint(data.Value.obj.transform.position);
            if (curObjViewportPosition.z < 0)
            {
                continue;
            }

            if (!IsNearest(curObjViewportPosition, out float distance))
            {
                continue;
            }

            if (IsAnyObstaclesExist(data, out hit))
            {
                return;
            }

            if (!IsValidInteraction(data, hit))
            {
                return;
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

    private void ProcessOnlyOneInteractableObjects(KeyValuePair<int, InteractionObject> data)
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(data.Value.obj.transform.position);
        if (viewportPosition.z < 0)
        {
            return;
        }

        if (IsAnyObstaclesExist(data, out RaycastHit hit))
        {
            return;
        }

        if (!IsValidInteraction(data, hit))
        {
            return;
        }

        _nearestObject = data.Value.obj;
        _nearestObjectType = data.Value.type;
    }

    private bool IsAnyObstaclesExist(KeyValuePair<int, InteractionObject> data, out RaycastHit hit)
    {
        Vector3 objPosition = data.Value.obj.transform.position;
        Vector3 playerPosition = transform.position;
        Vector3 direction = (objPosition - playerPosition).normalized;

        Ray ray = new Ray(playerPosition, direction);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return true;
        }

        return false;
    }
    
    private bool IsValidInteraction(KeyValuePair<int, InteractionObject> data, RaycastHit hit)
    {
        switch (data.Value.type)
        {
            case EInteractionType.Item when ItemThrowHandler.Instance.IsItemOnHand:
                return false;   
            case EInteractionType.HideableObject when !HideActionController.Instance.IsInFrontOfHideableObject(hit.transform):
                return false;
            case EInteractionType.Overstep when PlayerStateManager.Instance.CheckPlayerState(EPlayerState.Crouch):
                return false;
            case EInteractionType.Cube when data.Value.obj.GetComponent<InteractionObjectHandler>().IsCubeCorrect():
                return false;
            default:
                return true;
        }
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
        _interactionUI.SetActive(NearestObject.obj is not null && !CubeInteractionController.Instance.IsOnCube);

        if (!_interactionUI.activeSelf)
        {
            return;
        }
        
        Vector3 nearestObjectPosition = _nearestObject.transform.position;
        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(nearestObjectPosition);
        const float OFFSET = 50.0f;
        screenPosition.x += OFFSET;

        if (NearestObject.type is EInteractionType.Overstep)
        {
            screenPosition.y += OFFSET * 6.0f;
        }
        
        _interactionUIRectTransform.position = screenPosition;
    }

    public void AddInteractionObject(EInteractionType type, GameObject obj)
    {
        InteractionObject interactionObject = new(type, obj);
        _interactableObjects.Add(obj.GetInstanceID(), interactionObject);
    }

    public void RemoveInteractionObject(GameObject obj)
    {
        _interactableObjects.Remove(obj.GetInstanceID());
    }

    private void HandleInteraction()
    {
        if (!_interactionUI.activeSelf)
        {
            return;
        }

        if (NearestObject.type is EInteractionType.Overstep)
        {
            NearestObject.obj.GetComponentInParent<InteractionObjectHandler>().Interaction();
            return;
        }
        
        NearestObject.obj.GetComponentInChildren<InteractionObjectHandler>().Interaction();
    }
}