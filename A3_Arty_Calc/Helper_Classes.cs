using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3_Arty_Calc
{
    public class FireMode
    {
        public string name;
        public float artilleryCharge;
        public FireMode(string name, float artilleryCharge)
        {
            this.name = name;
            this.artilleryCharge = artilleryCharge;
        }
    }

    public class AngleSolution
    {
        public double angle;
        public double Xmultiplier;
        public double Xoffset;
        public double Ymultiplier;
        public double Yoffset;
        public AngleSolution(double angle, double Xmultiplier, double Xoffset, double Ymultiplier, double Yoffset)
        {
            this.angle = angle;
            this.Xmultiplier = Xmultiplier;
            this.Xoffset = Xoffset;
            this.Ymultiplier = Ymultiplier;
            this.Yoffset = Yoffset;
        }
    }

    class TargetLogItem
    {
        public string CoordLog { get; set; }
    }

    class SimulatedAngle
    {
        public double Angle { get; set; }
        public double Range { get; set; }
        public double Tof { get; set; }
        public double ExitAngle { get; set; }
        public double Apex { get; set; }
    }
}
