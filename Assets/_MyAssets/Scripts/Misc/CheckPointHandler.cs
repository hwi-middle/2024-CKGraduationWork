using UnityEngine;

public class CheckPointHandler : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = CheckPointSceneManager.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player") || PlayerMove.Instance.CheckPlayerState(EPlayerState.Dead))
        {
            return;
        }

        RespawnHelper.Instance.SaveCheckPoint(transform.position);
        GetComponent<Collider>().enabled = false;
    }
}
