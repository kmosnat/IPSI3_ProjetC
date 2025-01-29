using System;
using System.Threading;
using System.Xml.Serialization;
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


        public void Connect()
        {
            modbusClient = new ModbusClient(ipAddress, port);
            modbusClient.ConnectionTimeout = -1;
            modbusClient.Connect();
        }


        public void Disconnect()
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                modbusClient.Disconnect();
            }
        }

        public bool IsConnected()
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                return modbusClient.Connected;
            }
            return false;
        }


        public bool calibrationNeeded()
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                return modbusClient.ReadCoils(115, 1)[0];
            }
            return false;
        }

        public void calibrate()
        {
            if (modbusClient != null && modbusClient.Connected)
            {
                modbusClient.WriteSingleCoil(116, true);
            }
        }

        public float[] GetCurrentJointStates()
        {
            float[] joints = new float[6];

            // On va lire chaque pair de registres en boucle
            // On pourrait aussi faire des lectures en bloc plus grandes si la plage est continue.
            for (int i = 0; i < 6; i++)
            {
                int address = 50 + i * 2; // Joint1(50,51), Joint2(52,53), etc.
                int[] rawData = modbusClient.ReadInputRegisters(address, 2);
                joints[i] = ConvertRegistersToFloat(rawData);
            }

            return joints;
        }

        public RobotPose GetCurrentPose()
        {
            RobotPose pose = new RobotPose();

            // X
            int[] rawX = modbusClient.ReadInputRegisters(62, 2);
            pose.X = ConvertRegistersToFloat(rawX);

            // Y
            int[] rawY = modbusClient.ReadInputRegisters(64, 2);
            pose.Y = ConvertRegistersToFloat(rawY);

            // Z
            int[] rawZ = modbusClient.ReadInputRegisters(66, 2);
            pose.Z = ConvertRegistersToFloat(rawZ);

            // Roll
            int[] rawRoll = modbusClient.ReadInputRegisters(68, 2);
            pose.Roll = ConvertRegistersToFloat(rawRoll);

            // Pitch
            int[] rawPitch = modbusClient.ReadInputRegisters(70, 2);
            pose.Pitch = ConvertRegistersToFloat(rawPitch);

            // Yaw
            int[] rawYaw = modbusClient.ReadInputRegisters(72, 2);
            pose.Yaw = ConvertRegistersToFloat(rawYaw);

            return pose;
        }

        public void MoveToPose(float x, float y, float z, float roll, float pitch, float yaw)
        {
            if (!IsConnected())
                return;

            const int moveType = 1; // 1 = MOVE_POSE

            try
            {
                // Conversion float → tableau de 2 registres
                int[] xRegisters = ConvertFloatToModbusRegisters(x);
                int[] yRegisters = ConvertFloatToModbusRegisters(y);
                int[] zRegisters = ConvertFloatToModbusRegisters(z);
                int[] rollRegisters = ConvertFloatToModbusRegisters(roll);
                int[] pitchRegisters = ConvertFloatToModbusRegisters(pitch);
                int[] yawRegisters = ConvertFloatToModbusRegisters(yaw);

                // Écriture des registres pour X, Y, Z
                modbusClient.WriteMultipleRegisters(62, xRegisters);  // @62-63
                modbusClient.WriteMultipleRegisters(64, yRegisters);  // @64-65
                modbusClient.WriteMultipleRegisters(66, zRegisters);  // @66-67

                // Écriture des registres pour Roll, Pitch, Yaw
                modbusClient.WriteMultipleRegisters(68, rollRegisters);   // @68-69
                modbusClient.WriteMultipleRegisters(70, pitchRegisters);  // @70-71
                modbusClient.WriteMultipleRegisters(72, yawRegisters);    // @72-73

                // Indiquer le type de mouvement => @74 (1 = MOVE_POSE)
                modbusClient.WriteSingleRegister(74, moveType);

                // Vérifier et réinitialiser la collision si besoin (coil @117)
                ClearCollisionIfAny();

                // Envoyer la commande pour démarrer le mouvement (coil @113)
                modbusClient.WriteSingleCoil(113, true);

                Console.WriteLine("Commande de mouvement envoyée (X, Y, Z, Roll, Pitch, Yaw).");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de communication Modbus : " + ex.Message);
            }
        }



        public void MoveToJoints(float j1, float j2, float j3, float j4, float j5, float j6)
        {
            if (!IsConnected())
                return;

            const int moveType = 0; // 0 = MOVE_JOINT

            int[] j1Registers = ConvertFloatToModbusRegisters(j1);
            int[] j2Registers = ConvertFloatToModbusRegisters(j2);
            int[] j3Registers = ConvertFloatToModbusRegisters(j3);
            int[] j4Registers = ConvertFloatToModbusRegisters(j4);
            int[] j5Registers = ConvertFloatToModbusRegisters(j5);
            int[] j6Registers = ConvertFloatToModbusRegisters(j6);


            modbusClient.WriteMultipleRegisters(50, j1Registers);  // Écrire la coordonnée J1 
            modbusClient.WriteMultipleRegisters(52, j2Registers);  // Écrire la coordonnée J2
            modbusClient.WriteMultipleRegisters(54, j3Registers);  // Écrire la coordonnée J3 
            modbusClient.WriteMultipleRegisters(56, j4Registers);  // Écrire la coordonnée J4 
            modbusClient.WriteMultipleRegisters(58, j5Registers);  // Écrire la coordonnée J5 
            modbusClient.WriteMultipleRegisters(60, j6Registers);  // Écrire la coordonnée J6

            modbusClient.WriteSingleRegister(74, moveType);

            ClearCollisionIfAny();

            // Envoyer la commande pour démarrer le mouvement
            modbusClient.WriteSingleCoil(113, true);  // 113 : Start Move Command (coil)

        }

        public void ClearCollisionIfAny()
        {
            if (!IsConnected()) return;
            bool[] collision = modbusClient.ReadCoils(117, 1);
            if (collision[0])
            {
                // On met à false pour l’effacer (si autorisé).
                modbusClient.WriteSingleCoil(117, false);
            }
        }

        public bool isMoving()
        { 
            return modbusClient.ReadCoils(113, 1)[0];
        }

        float ConvertRegistersToFloat(int[] registers)
        {
            // registers[0] = mot haut (16 bits), registers[1] = mot bas (16 bits)
            byte[] bytes = new byte[4];

            // Reconstitution (ex. Big-Endian par mot) 
            bytes[0] = (byte)(registers[1] >> 8);
            bytes[1] = (byte)(registers[1] & 0xFF);
            bytes[2] = (byte)(registers[0] >> 8);
            bytes[3] = (byte)(registers[0] & 0xFF);

            return BitConverter.ToSingle(bytes, 0);
        }


        static int[] ConvertFloatToModbusRegisters(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            int[] registers = new int[2];

            registers[1] = (bytes[0] << 8 | bytes[1]);
            registers[0] = (bytes[2] << 8 | bytes[3]);

            return registers;
        }

    }
}