using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SSPerceptionGaugeUiHandler : Singleton<SSPerceptionGaugeUiHandler>
{
    private readonly Dictionary<EnemyBase, ScreenSpacePerceptionGauge> _enemies = new Dictionary<EnemyBase, ScreenSpacePerceptionGauge>();
    public float yOffset = 0.0f;
    public float radius = 0.0f;
    
    private void Start()
    {
    }

    public void RegisterEnemy(EnemyBase enemy)
    {
        if (_enemies.ContainsKey(enemy))
        {
            return;
        }
        
        var gauge = Instantiate(Resources.Load("PerceptionNote/SSPerceptionGauge"), transform).GetComponent<ScreenSpacePerceptionGauge>();
        _enemies.Add(enemy, gauge);
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        if (!_enemies.ContainsKey(enemy))
        {
            return;
        }
        
        Destroy(_enemies[enemy].gameObject);
        _enemies.Remove(enemy);
    }

    public void UpdateEnemyPerceptionGauge(EnemyBase enemy)
    {
        if (!_enemies.TryGetValue(enemy, out ScreenSpacePerceptionGauge gauge))
        {
            return;
        }

        // 게이지 업데이트
        float currentPerceptionGauge = enemy.PerceptionGauge;
        float alertThreshold = enemy.AiData.alertThreshold;
        if (currentPerceptionGauge >= alertThreshold)
        {
            gauge.SetPerceptionGauge(1.0f, (currentPerceptionGauge - alertThreshold) / (100.0f - alertThreshold));
        }
        else
        {
            gauge.SetPerceptionGauge(currentPerceptionGauge / alertThreshold, 0.0f);
        }
        
        // 위치 업데이트

        Vector3 dir = enemy.transform.position - Camera.main.transform.position;
        float z = Mathf.Acos(Vector3.Dot(Camera.main.transform.forward, dir.normalized)) * Mathf.Rad2Deg;
        if (Vector3.Cross(Camera.main.transform.forward, dir.normalized).y > 0)
        {
            z *= -1;
        }
        gauge.transform.rotation = Quaternion.Euler(0, 0, z);
    }
}
