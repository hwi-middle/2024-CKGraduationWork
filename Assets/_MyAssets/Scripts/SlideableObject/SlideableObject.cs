using UnityEngine;

public class SlideableObject : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    
    private void Awake()
    {
        _meshRenderer = transform.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        _meshRenderer.enabled = SceneManagerBase.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }
        
        PlayerMove.Instance.CountUpSlideableTriggerZone();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }
        
        PlayerMove.Instance.CountDownSlideableTriggerZone();
    }
}
