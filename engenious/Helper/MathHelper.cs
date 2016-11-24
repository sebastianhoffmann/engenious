﻿using System;

namespace engenious
{
    public static class MathHelper
    {
        public const float Pi = (float) (Math.PI);
        public const float PiOver2 = (float) (Math.PI / 2);
        public const float PiOver4 = (float) (Math.PI / 4);
        public const float TwoPi = (float) (2 * Math.PI);

        public static float WrapAngle(float angle)
        {
            angle = (float) Math.IEEERemainder((double) angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }

        public static float ToRadians(float degrees)
        {
            return (float) (degrees * Math.PI / 180);
        }
    }
}