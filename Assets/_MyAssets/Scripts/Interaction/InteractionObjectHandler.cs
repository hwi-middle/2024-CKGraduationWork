using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SphereCollider))]
public class InteractionObjectHandler : MonoBehaviour
{
    [SerializeField] private InteractionData _data;
    [SerializeField] private GameObject _connectedCubeRoot;
    private readonly Vector3 COLLIDER_ADDITIVE_CENTER = new (0, 0.5f, 0);

    private void Awake()
    {
        if (_data.type == EInteractionType.Cube)
        {
            Debug.Assert(_connectedCubeRoot != null, "Connected cube object is null");
        }
        
        SphereCollider coll = GetComponent<SphereCollider>();
        coll.isTrigger = true;
        coll.radius = _data.detectRadius;
        if (_data.type is EInteractionType.Overstep)
        {
            coll.center = COLLIDER_ADDITIVE_CENTER;
        }
    }

    public void Interaction()
    {
        Transform parentTransform = transform.parent;

        switch (_data.type)
        {
            case EInteractionType.Item:
                InteractionController.Instance.RemoveInteractionObject(gameObject);
                Destroy(parentTransform.gameObject);
                ItemThrowHandler.Instance.GetItem();
                return;
            
            case EInteractionType.HideableObject:
                HideActionController.Instance.HideAction(parentTransform);
                return;
            
            case EInteractionType.Overstep:
                Transform childTransform = transform.GetChild(0);
                OverstepActionController.Instance.OverstepAction(childTransform, _data.detectRadius);
                return;
            
            case EInteractionType.Cube:
                CubeInteractionController.Instance.SetCurrentCube(transform, _connectedCubeRoot);
                return;
            
            default:
                Debug.Assert(false);
                return;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }
        
        InteractionController.Instance.AddInteractionObject(_data.type, transform.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }

        InteractionController.Instance.RemoveInteractionObject(transform.gameObject);
    }
}