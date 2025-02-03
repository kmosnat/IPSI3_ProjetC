using EasyModbus;
using System;
using Utils;

public class RobotModbusHelper
{
    private readonly string ipAddress;
    private readonly int port;
    private ModbusClient modbusClient;
    private readonly object modbusLock = new object();

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
        return modbusClient != null && modbusClient.Connected;
    }

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

    public void MoveToPose(float x, float y, float z, float roll, float pitch, float yaw)
    {
        lock (modbusLock)
        {
            if (!IsConnected())
                return;

            const int moveType = 1; // 1 = MOVE_POSE

            try
            {
                int[] xRegisters = ConvertFloatToModbusRegisters(x);
                int[] yRegisters = ConvertFloatToModbusRegisters(y);
                int[] zRegisters = ConvertFloatToModbusRegisters(z);
                int[] rollRegisters = ConvertFloatToModbusRegisters(roll);
                int[] pitchRegisters = ConvertFloatToModbusRegisters(pitch);
                int[] yawRegisters = ConvertFloatToModbusRegisters(yaw);

                modbusClient.WriteMultipleRegisters(62, xRegisters);  // @62-63
                modbusClient.WriteMultipleRegisters(64, yRegisters);  // @64-65
                modbusClient.WriteMultipleRegisters(66, zRegisters);  // @66-67
                modbusClient.WriteMultipleRegisters(68, rollRegisters);   // @68-69
                modbusClient.WriteMultipleRegisters(70, pitchRegisters);  // @70-71
                modbusClient.WriteMultipleRegisters(72, yawRegisters);    // @72-73

                modbusClient.WriteSingleRegister(74, moveType);

                ClearCollisionIfAny();

                modbusClient.WriteSingleCoil(113, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de communication Modbus : " + ex.Message);
            }
        }
    }

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

    public bool isMoving()
    {
        return modbusClient.ReadCoils(113, 1)[0];
    }

    float ConvertRegistersToFloat(int[] registers)
    {
        byte[] bytes = new byte[4];
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
