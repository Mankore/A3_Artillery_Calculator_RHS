using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace A3_Arty_Calc
{

    abstract public class Artillery
    {
        public abstract string Name { get; }
        public abstract double simulationStep { get; }
        public abstract float minAngle { get; }
        public abstract float maxAngle { get; }
        public abstract FireMode[] modes { get; }
        public abstract ShellType[] shellTypes { get; }
        public abstract bool isAirFriction { get; }
        public abstract Vector3D getBaseProjectileSpawnPoint(double angle);
        public abstract AngleSolution[] angleSolutions { get; }
        public abstract double angleAdjustment { get; }
    }

    class Art_2S1_Direct : Artillery
    {
        public override string Name
        {
            get
            {
                return "2S1 Direct Fire";
            }
        }
        public override float minAngle
        {
            get
            {
                return 3;
            }
        }

        public override float maxAngle
        {
            get
            {
                return 70;
            }
        }

        public override FireMode[] modes
        {
            get
            {
                return new FireMode[] {
                    new FireMode("Full Charge", 1),
                    new FireMode("Reduced Charge", 0.81f),
                    new FireMode("Charge 1", 0.71f),
                    new FireMode("Charge 2", 0.6f),
                    new FireMode("Charge 3", 0.48f),
                    new FireMode("Charge 4", 0.4f)
                };
            }
        }

        public override double simulationStep
        {
            get
            {
                return 0.05;
            }
        }

        public override ShellType[] shellTypes
        {
            get
            {
                return new ShellType[] { new _3OF56(), new _3BK13() };
            }
        }

        public override bool isAirFriction
        {
            get
            {
                return true;
            }
        }

        public override Vector3D getBaseProjectileSpawnPoint(double angle)
        {
            //return new Vector3D(0, -11.5, Math.Sin(angle) * 8.6 + 18.6);
            //return new Vector3D(0, -Math.Cos(angle)*11.198, Math.Sin(angle)* 11.198 + 6.839);
            //return new Vector3D(0, -Math.Cos(angle) * 11.198, Math.Sin(angle) * 16.49 + 5.03);

            return new Vector3D(0, -Math.Cos(angle) * 0.9 - 3.7, Math.Sin(angle) * 1.5 + 9.1); // Best results Full Charge
            //return new Vector3D(0, -Math.Cos(angle) * 8.5, Math.Sin(angle) * 7.9 + 6.7); // Best results Charge 4
            //return new Vector3D(0, -Math.Cos(angle) * 0.2 - 4.8, Math.Sin(angle) * 7.9 + 6.7); // Best results Full + 4
        }

        public override AngleSolution[] angleSolutions
        {
            get
            {
                return new AngleSolution[]
                {
                    //new AngleSolution(5, 0, 4.3, 16.6, 11.8),
                    //new AngleSolution(10, 0, 0, 11.8, 15.3),
                    //new AngleSolution(15, 1.6, 3.6, 6.7, 19),
                    //new AngleSolution(20, 0, 2.8, 1.3, 21.6),
                    //new AngleSolution(25, 0.3, 8, 9.9, 18.1),
                    //new AngleSolution(30, 2.3, 10.1, 0.1, 23.9),
                    //new AngleSolution(35, 0, 12.7, 1.9, 23.2),
                    //new AngleSolution(40, 1.8, 15.7, 1, 23.5),
                    //new AngleSolution(45, 0, 18.2, 3.2, 21.4),
                    //new AngleSolution(50, 0.5, 21.3, 4.8, 19.3),
                    //new AngleSolution(55, 1.2, 24, 4.7, 17.5),
                    //new AngleSolution(60, 5.5, 24.9, 3.5, 17.6),
                    //new AngleSolution(65, 22, 24, 2.3, 17.6)
                };
            }
        }

        public override double angleAdjustment
        {
            get
            {
                return -0.135;
            }
        }
    }

    class Art_2S1 : Artillery
    {
        public override string Name
        {
            get
            {
                return "2S1";
            }
        }
        public override float minAngle
        {
            get
            {
                return 3;
            }
        }

        public override float maxAngle
        {
            get
            {
                return 70;
            }
        }

        public override FireMode[] modes
        {
            get
            {
                return new FireMode[] {
                    new FireMode("Full Charge", 0.564f),
                    new FireMode("Reduced Charge", 0.5f),
                    new FireMode("Charge 1", 0.44f),
                    new FireMode("Charge 2", 0.39f),
                    new FireMode("Charge 3", 0.32f),
                    new FireMode("Charge 4", 0.26f)
                };
            }
        }

        public override double simulationStep
        {
            get
            {
                return 0.05;
            }
        }

        public override ShellType[] shellTypes
        {
            get
            {
                return new ShellType[] { new _3OF56(), new _3BK13() };
            }
        }

        public override bool isAirFriction
        {
            get
            {
                return false;
            }
        }
        public override Vector3D getBaseProjectileSpawnPoint(double angle)
        {
            return new Vector3D(0, 0, 0);
        }
        public override AngleSolution[] angleSolutions
        {
            get
            {
                return null;
            }
        }

        public override double angleAdjustment
        {
            get
            {
                return 0;
            }
        }
    }

    class Art_M109A6 : Artillery
    {
        public override string Name
        {
            get
            {
                return "M109A6";
            }
        }
        public override float minAngle
        {
            get
            {
                return 3;
            }
        }

        public override float maxAngle
        {
            get
            {
                return 80;
            }
        }

        public override FireMode[] modes
        {
            get
            {
                return new FireMode[] {
                    new FireMode("Полуавтомат (близко)", 0.19f),
                    new FireMode("Полуавтомат (средне)", 0.3f),
                    new FireMode("Полуавтомат (далеко)", 0.48f),
                    new FireMode("Полуавт (дальше)", 0.8f),
                    new FireMode("Полуавт (предельная дальность)", 1f)
                };
            }
        }

        public override double simulationStep
        {
            get
            {
                return 0.05;
            }
        }

        public override ShellType[] shellTypes
        {
            get
            {
                return new ShellType[] { new _AMOS_HE(), new _AMOS_LG(), new _AMOS_CLUSTER() };
            }
        }

        public override bool isAirFriction
        {
            get
            {
                return false;
            }
        }
        public override Vector3D getBaseProjectileSpawnPoint(double angle)
        {
            return new Vector3D(0, 0, 5);
        }
        public override AngleSolution[] angleSolutions
        {
            get
            {
                return null;
            }
        }
        public override double angleAdjustment
        {
            get
            {
                return 0;
            }
        }
    }

    class Art_2S3M1 : Artillery
    {
        public override string Name
        {
            get
            {
                return "2S3M1";
            }
        }
        public override float minAngle
        {
            get
            {
                return 3;
            }
        }

        public override float maxAngle
        {
            get
            {
                return 66.5f;
            }
        }

        public override FireMode[] modes
        {
            get
            {
                return new FireMode[] {
                    new FireMode("Full Charge", 0.63f),
                    new FireMode("Charge 1", 0.54f),
                    new FireMode("Charge 2", 0.47f),
                    new FireMode("Charge 3", 0.4f),
                    new FireMode("Charge 4", 0.346f),
                    new FireMode("Charge 5", 0.298f),
                    new FireMode("Charge 6", 0.256f)
                };
            }
        }

        public override double simulationStep
        {
            get
            {
                return 0.05;
            }
        }

        public override ShellType[] shellTypes
        {
            get
            {
                return new ShellType[] { new _53_WOF_27() };
            }
        }

        public override bool isAirFriction
        {
            get
            {
                return false;
            }
        }
        public override Vector3D getBaseProjectileSpawnPoint(double angle)
        {
            return new Vector3D(0, 0, 10);
        }
        public override AngleSolution[] angleSolutions
        {
            get
            {
                return null;
            }
        }

        public override double angleAdjustment
        {
            get
            {
                return 0;
            }
        }
    }
}
