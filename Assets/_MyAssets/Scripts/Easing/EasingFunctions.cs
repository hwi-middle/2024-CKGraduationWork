using System;
using UnityEngine;

public static class EasingFunctions
{
    public static float EaseOutBounce(float x)
    {
        const float N1 = 7.5625f;
        const float D1 = 2.75f;

        float result = 0.0f;

        if (x < 1.0f / D1)
        {
            result = N1 * x * x;
        }
        else if (x < 2.0f / D1)
        {
            result = N1 * (x -= 1.5f / D1) * x + 0.75f;
        }
        else if (x < 2.5f / D1)
        {
            result = N1 * (x -= 2.25f / D1) * x + 0.9375f;
        }
        else
        {
            result = N1 * (x -= 2.625f / D1) * x + 0.984375f;
        }

        return result;
    }

    public static float EaseInOutBounce(float x)
    {
        return x < 0.5f
            ? (1 - EaseOutBounce(1 - 2 * x)) / 2
            : (1 + EaseOutBounce(2 * x - 1)) / 2;
    }
}
