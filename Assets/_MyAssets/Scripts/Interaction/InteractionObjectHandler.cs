using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectHandler : MonoBehaviour
{
    [SerializeField] private InteractionData _data;
    private readonly Vector3 COLLIDER_ADDITIVE_CENTER = new (0, 0.5f, 0);
    
    private void Awake()
    {
        SphereCollider coll = GetComponent<SphereCollider>();
        coll.radius = _data.detectRadius;
        if (_data.type is not EInteractionType.Overstep)
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
                InteractionController.Instance.RemoveInteractionObject(parentTransform.gameObject);
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

        if (_data.type is EInteractionType.Overstep)
        {
            InteractionController.Instance.AddInteractionObject(_data.type, transform.GetChild(0).gameObject);
            return;
        }

        InteractionController.Instance.AddInteractionObject(_data.type, transform.parent.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }

        if (_data.type is EInteractionType.Overstep)
        {
            InteractionController.Instance.RemoveInteractionObject(transform.GetChild(0).gameObject);
            return;
        }
        
        InteractionController.Instance.RemoveInteractionObject(transform.parent.gameObject);
    }
}