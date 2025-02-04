using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    // Enumération pour les états du serveur
    public enum ServerState
    {
        Calibration,
        Wait,
        Ready
    }

    // Enumération pour les étapes de calibration
    public enum CalibrationStep
    {
        None,
        Point1,
        Point2
    }
}
