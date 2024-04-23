using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectHandler : MonoBehaviour
{
    [SerializeField] private InteractionData _data;
    
    private void Awake()
    {
        GetComponent<SphereCollider>().radius = _data.detectRadius;
    }

    public void Interaction()
    {
        if (_data.type == EInteractionType.Item)
        {
            InteractionController.Instance.RemoveInteractionObject(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
            ItemThrowHandler.Instance.GetItem();
            return;
        }
        
        HideActionController.Instance.HideAction(transform.parent);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
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

        InteractionController.Instance.RemoveInteractionObject(transform.parent.gameObject);
    }
}