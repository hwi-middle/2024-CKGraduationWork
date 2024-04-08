using UnityEngine;

public class SlideableZone : MonoBehaviour
{
    public static int slideableZoneCount;
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

        slideableZoneCount++ ;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Player"))
        {
            return;
        }

        slideableZoneCount--;
    }
}
