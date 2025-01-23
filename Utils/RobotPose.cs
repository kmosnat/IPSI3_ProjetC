using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class RobotPose
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Roll { get; set; }
        public float Pitch { get; set; }
        public float Yaw { get; set; }

        public override string ToString()
        {
            return $"Pose => X:{X:F3}, Y:{Y:F3}, Z:{Z:F3}, " +
                   $"Roll:{Roll:F3}, Pitch:{Pitch:F3}, Yaw:{Yaw:F3}";
        }
    }
}
