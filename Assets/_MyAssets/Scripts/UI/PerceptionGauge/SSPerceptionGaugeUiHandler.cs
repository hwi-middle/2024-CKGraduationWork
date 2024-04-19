using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SSPerceptionGaugeUiHandler : Singleton<SSPerceptionGaugeUiHandler>
{
    private readonly Dictionary<EnemyBase, ScreenSpacePerceptionGauge> _enemies = new Dictionary<EnemyBase, ScreenSpacePerceptionGauge>();

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
    }
}
