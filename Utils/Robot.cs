using System;
using Newtonsoft.Json;

namespace Utils
{
    // Classe pour représenter la position et l'orientation du robot
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

    // Machine à états pour le robot
    public enum RobotState
    {
        Wait,
        OnProcess,
        RobotOnMoving
    }

    // Classe pour représenter un objet détecté par le robot
    public class RobotObject
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Color")]
        public string Color { get; set; }

        [JsonProperty("Shape")]
        public string Shape { get; set; }

        [JsonProperty("X")]
        public float X { get; set; }

        [JsonProperty("Y")]
        public float Y { get; set; }

        // Propriété pour générer une clé unique basée sur les propriétés de l'objet
        [JsonIgnore]
        public string Key => GenerateKey();

        // Méthode pour générer la clé unique améliorée
        private string GenerateKey()
        {
            const float tolerance = 10.0f;
            // Quantifier les positions X et Y selon la tolérance
            float quantizedX = (float)Math.Round(X / tolerance) * tolerance;
            float quantizedY = (float)Math.Round(Y / tolerance) * tolerance;

            // On utilise ToLowerInvariant() et Trim() pour standardiser les chaînes
            return $"{Color.Trim().ToLowerInvariant()}_{Shape.Trim().ToLowerInvariant()}_{quantizedX:F1}_{quantizedY:F1}";
        }

        // Constructeur pour création d'un nouvel objet avec un ID unique
        public RobotObject(string color, string shape, float x, float y)
        {
            Id = Guid.NewGuid();
            Color = color;
            Shape = shape;
            X = x;
            Y = y;
        }

        // Constructeur pour parsing d'un objet reçu avec un ID
        [JsonConstructor]
        public RobotObject(Guid id, string color, string shape, float x, float y)
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
