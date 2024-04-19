using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheckPointData
{
    private static string _currentSceneName = "";
    public static Vector3 CheckPoint { get; private set; }
    
    public static bool IsChecked { get; private set; }

    public static bool IsSameScene => SceneManager.GetActiveScene().name == _currentSceneName;

    public static void SetCheckPoint(Vector3 point)
    {
        CheckPoint = point;
        IsChecked = true;
    }

    public static void ChangeSceneName(string sceneName)
    {
        if (IsSameScene)
        {
            return;
        }

        CheckPoint = Vector3.zero;
        _currentSceneName = sceneName;
        IsChecked = false;
    }
}
