using System;
using Newtonsoft.Json;

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

    public enum RobotState
    {
        Wait,
        OnProcess,
        RobotOnMoving
    }
    public class RobotObject
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Color")]
        public string Color { get; set; }

        [JsonProperty("Shape")]
        public string Shape { get; set; }

        [JsonProperty("X")]
        public int X { get; set; }

        [JsonProperty("Y")]
        public int Y { get; set; }

        // Constructeur pour création d'un nouvel objet avec un ID unique
        public RobotObject(string color, string shape, int x, int y)
        {
            Id = Guid.NewGuid();
            Color = color;
            Shape = shape;
            X = x;
            Y = y;
        }

        // Constructeur pour parsing d'un objet reçu avec un ID
        [JsonConstructor]
        public RobotObject(Guid id, string color, string shape, int x, int y)
        {
            Id = id;
            Color = color;
            Shape = shape;
            X = x;
            Y = y;
        }

        // Sérialiser l'objet en JSON
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Désérialiser l'objet depuis une chaîne JSON
        public static RobotObject FromString(string data)
        {
            return JsonConvert.DeserializeObject<RobotObject>(data);
        }
    }
}