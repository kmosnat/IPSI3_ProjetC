using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public enum ServerState
    {
        Calibration,
        Wait,
        Ready
    }

    public enum CalibrationStep
    {
        None,
        Point1,
        Point2
    }
}
