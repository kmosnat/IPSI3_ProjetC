using EasyModbus;
using System;
using Utils;

// Classe pour communiquer avec le robot via Modbus
public class RobotModbus
{
    private readonly string ipAddress;
    private readonly int port;
    private ModbusClient modbusClient;
    private readonly object modbusLock = new object();

    // Constructeur
    public RobotModbus(string ipAddress, int port = 5020)
    {
        this.ipAddress = ipAddress;
        this.port = port;
    }

    // Méthode pour se connecter au robot
    public void Connect()
    {
        modbusClient = new ModbusClient(ipAddress, port);
        modbusClient.ConnectionTimeout = -1;
        modbusClient.Connect();
    }

    // Méthode pour se déconnecter du robot
    public void Disconnect()
    {
        if (modbusClient != null && modbusClient.Connected)
        {
            modbusClient.Disconnect();
        }
    }

    // Méthode pour vérifier si le robot est connecté
    public bool IsConnected()
    {
        return modbusClient != null && modbusClient.Connected;
    }

    // Méthode pour vérifier si une calibration est nécessaire
    public bool calibrationNeeded()
    {
        lock (modbusLock)
        {
            if (IsConnected())
            {
                return modbusClient.ReadCoils(115, 1)[0];
            }
            return false;
        }
    }

    // Méthode pour calibrer le robot
    public void calibrate()
    {
        lock (modbusLock)
        {
            if (IsConnected())
            {
                modbusClient.WriteSingleCoil(116, true);
            }
        }
    }

    // Méthode pour ouvrir la pince du robot
    public void openGripper()
    {
        lock (modbusLock)
        {
            if (IsConnected())
            {
                modbusClient.WriteSingleCoil(51, false);
            }
        }
    }

    // Méthode pour fermer la pince du robot
    public void closeGripper()
    {
        lock (modbusLock)
        {
            if (IsConnected())
            {
                modbusClient.WriteSingleCoil(51, true);
            }
        }
    }

    // Méthode pour obtenir les états actuels des joints du robot
    public float[] GetCurrentJointStates()
    {
        lock (modbusLock)
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
    }

    // Méthode pour obtenir la pose actuelle du robot
    public RobotPose GetCurrentPose()
    {
        lock (modbusLock)
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
    }

    // Méthode pour déplacer le robot à une pose donnée
    public void MoveToPose(float x, float y, float z, float roll, float pitch, float yaw)
    {
        lock (modbusLock)
        {
            if (!IsConnected())
                return;

            const int moveType = 1; // 1 = MOVE_POSE

            try
            {
                // Convertir les valeurs float en registres Modbus
                int[] xRegisters = ConvertFloatToModbusRegisters(x);
                int[] yRegisters = ConvertFloatToModbusRegisters(y);
                int[] zRegisters = ConvertFloatToModbusRegisters(z);
                int[] rollRegisters = ConvertFloatToModbusRegisters(roll);
                int[] pitchRegisters = ConvertFloatToModbusRegisters(pitch);
                int[] yawRegisters = ConvertFloatToModbusRegisters(yaw);

                // Écrire les valeurs dans les registres Modbus
                modbusClient.WriteMultipleRegisters(62, xRegisters);  
                modbusClient.WriteMultipleRegisters(64, yRegisters);  
                modbusClient.WriteMultipleRegisters(66, zRegisters);  
                modbusClient.WriteMultipleRegisters(68, rollRegisters);   
                modbusClient.WriteMultipleRegisters(70, pitchRegisters);  
                modbusClient.WriteMultipleRegisters(72, yawRegisters);    

                // Écrire le type de mouvement
                modbusClient.WriteSingleRegister(74, moveType);
                
                ClearCollisionIfAny();

                // Déclencher le mouvement
                modbusClient.WriteSingleCoil(113, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de communication Modbus : " + ex.Message);
            }
        }
    }

    // Méthode pour detecter une collision 
    public void ClearCollisionIfAny()
    {
        lock (modbusLock)
        {
            if (!IsConnected()) return;
            bool[] collision = modbusClient.ReadCoils(117, 1);
            if (collision[0])
            {
                modbusClient.WriteSingleCoil(117, false);
            }
        }
    }

    // Méthode pour vérifier si le robot est en mouvement
    public bool isMoving()
    {
        return modbusClient.ReadCoils(113, 1)[0];
    }

    // Méthode pour convertir des registres Modbus en float
    float ConvertRegistersToFloat(int[] registers)
    {
        byte[] bytes = new byte[4];
        bytes[0] = (byte)(registers[1] >> 8);
        bytes[1] = (byte)(registers[1] & 0xFF);
        bytes[2] = (byte)(registers[0] >> 8);
        bytes[3] = (byte)(registers[0] & 0xFF);
        return BitConverter.ToSingle(bytes, 0);
    }

    // Méthode pour convertir un float en registres Modbus
    static int[] ConvertFloatToModbusRegisters(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        int[] registers = new int[2];
        registers[1] = (bytes[0] << 8 | bytes[1]);
        registers[0] = (bytes[2] << 8 | bytes[3]);
        return registers;
    }
}
