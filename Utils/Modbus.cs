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

            for (int i = 0; i < 6; i++)
            {
                int address = 50 + i * 2;
                int[] rawData = modbusClient.ReadInputRegisters(address, 2);
                joints[i] = ConvertRegistersToFloat(rawData);
            }

            return joints;
        }

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

        public void MoveToPose(float x, float y, float z)
        {
            int moveType = 1;

            int[] xRegisters = ConvertFloatToModbusRegisters(x);
            int[] yRegisters = ConvertFloatToModbusRegisters(y);
            int[] zRegisters = ConvertFloatToModbusRegisters(z);

            try
            {
                // Écrire chaque registre individuellement
                modbusClient.WriteSingleRegister(62, xRegisters[0]);
                modbusClient.WriteSingleRegister(63, xRegisters[1]);

                Thread.Sleep(100);

                modbusClient.WriteSingleRegister(64, yRegisters[0]);
                modbusClient.WriteSingleRegister(65, yRegisters[1]);

                Thread.Sleep(100);

                modbusClient.WriteSingleRegister(66, zRegisters[0]);
                modbusClient.WriteSingleRegister(67, zRegisters[1]);

                // Écrire le type de mouvement
                modbusClient.WriteSingleRegister(74, moveType);

                // Vérifier et réinitialiser la détection de collision
                if (modbusClient.ReadCoils(117, 1)[0])
                {
                    modbusClient.WriteSingleCoil(117, false);
                }

                // Activer le mouvement
                modbusClient.WriteSingleCoil(113, true);

                Console.WriteLine("Les coordonnées ont été envoyées. Le robot doit maintenant se déplacer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de communication Modbus : " + ex.Message);
            }
        }


        public bool isMoving()
        { 
            return modbusClient.ReadCoils(113, 1)[0];
        }

        private float ConvertRegistersToFloat(int[] registers)
        {
            byte[] bytes = new byte[4];

            bytes[0] = (byte)(registers[0] >> 8);
            bytes[1] = (byte)(registers[0] & 0xFF);
            bytes[2] = (byte)(registers[1] >> 8);
            bytes[3] = (byte)(registers[1] & 0xFF);

            return BitConverter.ToSingle(bytes, 0);
        }


        static int[] ConvertFloatToModbusRegisters(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            int[] registers = new int[2];

            // Inverser l'ordre des registres
            registers[0] = (bytes[0] << 8) | bytes[1];
            registers[1] = (bytes[2] << 8) | bytes[3];

            return registers;
        }

    }
}