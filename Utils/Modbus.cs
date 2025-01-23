using System;
using EasyModbus;

namespace Utils
{
    public class RobotModbusHelper
    {
        private readonly string ipAddress;
        private readonly int port;
        private ModbusClient modbusClient;

        public RobotModbusHelper(string ipAddress, int port = 5020)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        /// <summary>
        /// Ouvre la connexion Modbus TCP vers le robot.
        /// </summary>
        public void Connect()
        {
            modbusClient = new ModbusClient(ipAddress, port);
            modbusClient.Connect();
        }

        /// <summary>
        /// Ferme la connexion Modbus.
        /// </summary>
        public void Disconnect()
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                modbusClient.Disconnect();
            }
        }

        /// <summary>
        /// Lit les 6 positions d’articulations (Current Joint State) 
        /// via Input Registers (adresses 50-61).
        /// </summary>
        /// <returns>Tableau de 6 floats contenant les valeurs de chaque joint en radians.</returns>
        public float[] GetCurrentJointStates()
        {
            float[] joints = new float[6];

            // On va lire chaque paire de registres pour chaque joint
            for (int i = 0; i < 6; i++)
            {
                int address = 50 + i * 2; // Joint1(50-51), Joint2(52-53), etc.
                int[] rawData = modbusClient.ReadInputRegisters(address, 2);
                joints[i] = ConvertRegistersToFloat(rawData);
            }

            return joints;
        }

        /// <summary>
        /// Lit la pose cartésienne courante (X, Y, Z, Roll, Pitch, Yaw) 
        /// via Input Registers (adresses 62-73).
        /// </summary>
        public RobotPose GetCurrentPose()
        {
            RobotPose pose = new RobotPose();

            // X : 62-63
            int[] rawX = modbusClient.ReadInputRegisters(62, 2);
            pose.X = ConvertRegistersToFloat(rawX);

            // Y : 64-65
            int[] rawY = modbusClient.ReadInputRegisters(64, 2);
            pose.Y = ConvertRegistersToFloat(rawY);

            // Z : 66-67
            int[] rawZ = modbusClient.ReadInputRegisters(66, 2);
            pose.Z = ConvertRegistersToFloat(rawZ);

            // Roll : 68-69
            int[] rawRoll = modbusClient.ReadInputRegisters(68, 2);
            pose.Roll = ConvertRegistersToFloat(rawRoll);

            // Pitch : 70-71
            int[] rawPitch = modbusClient.ReadInputRegisters(70, 2);
            pose.Pitch = ConvertRegistersToFloat(rawPitch);

            // Yaw : 72-73
            int[] rawYaw = modbusClient.ReadInputRegisters(72, 2);
            pose.Yaw = ConvertRegistersToFloat(rawYaw);

            return pose;
        }

        /// <summary>
        /// Exemple de déplacement en coordonnées cartésiennes (X, Y, Z) 
        /// avec orientation par défaut (si vous n’écrivez pas Roll, Pitch, Yaw).
        /// moveType = 1 => MOVE_POSE
        /// 
        /// Addresses cibles:
        ///   - Pose Target X : 62-63
        ///   - Pose Target Y : 64-65
        ///   - Pose Target Z : 66-67
        ///   - moveType      : 74 (écriture d’un mot unique)
        ///   - Start Move    : 113 (coil)
        ///   - Collision Coil: 117 (coil)
        /// </summary>
        public void MoveToPose(float x, float y, float z)
        {
            // 1 = MOVE_POSE, 0 = MOVE_JOINT
            int moveType = 1;

            // Convertir en registres Modbus
            int[] xRegisters = ConvertFloatToModbusRegisters(x);
            int[] yRegisters = ConvertFloatToModbusRegisters(y);
            int[] zRegisters = ConvertFloatToModbusRegisters(z);

            // Écrire les coordonnées dans les registres
            modbusClient.WriteMultipleRegisters(62, xRegisters); // X
            modbusClient.WriteMultipleRegisters(64, yRegisters); // Y
            modbusClient.WriteMultipleRegisters(66, zRegisters); // Z

            // Écrire le type de mouvement (MOVE_POSE)
            modbusClient.WriteSingleRegister(74, moveType);

            // Vérifier la coil 117 (collisions) et l’annuler si nécessaire
            bool[] collisions = modbusClient.ReadCoils(117, 1);
            if (collisions.Length > 0 && collisions[0])
            {
                modbusClient.WriteSingleCoil(117, false);
            }

            // Déclencher le mouvement
            modbusClient.WriteSingleCoil(113, true);
        }

        /// <summary>
        /// Convertit un tableau de 2 registres (int) en un float (32 bits).
        /// Le robot Niryo Ned2 travaille en "Big-Endian par mot".
        /// </summary>
        private float ConvertRegistersToFloat(int[] registers)
        {
            // registers[0] = mot haut (16 bits), registers[1] = mot bas (16 bits)
            byte[] bytes = new byte[4];

            // Reconstitution Big-Endian par mot
            bytes[0] = (byte)(registers[1] >> 8);
            bytes[1] = (byte)(registers[1] & 0xFF);
            bytes[2] = (byte)(registers[0] >> 8);
            bytes[3] = (byte)(registers[0] & 0xFF);

            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Convertit un float .NET (32 bits) en 2 registres (int) 
        /// pour l’écriture Modbus (Big-Endian par mot).
        /// </summary>
        private int[] ConvertFloatToModbusRegisters(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                // On manipule pour se conformer au Big-Endian par mot
                // Sur Niryo : mot 0 = 16 bits hauts, mot 1 = 16 bits bas
                Array.Reverse(bytes);
            }

            // Découpage en 2 registres 16 bits
            int highWord = (bytes[0] << 8) | bytes[1];
            int lowWord = (bytes[2] << 8) | bytes[3];

            // Sur certains robots, il faut éventuellement inverser 
            // (dans ce cas, tester si c’est lowWord puis highWord).
            // Ici on assume le mot haut en premier : [0]-[1], puis [2]-[3].
            return new int[] { highWord, lowWord };
        }
    }
}
