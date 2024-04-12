using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int TakeDamage(int damageAmount, GameObject damageCauser);
}
