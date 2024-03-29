﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace A3_Arty_Calc
{
    class trajectoryParams
    {
        public double angle;
        public double apex;
        public double range;

        public trajectoryParams(double angle, double range, double apex)
        {
            this.angle = angle;
            this.apex = apex;
            this.range = range;
        }
    }

    class TestLogic
    {
        static double gravity = 9.8066;
        static double simulationStep = 0.01;

        /*
         * Old testing, values order (angle, apex, range)
         */
        //static public trajectoryParams[] trajectories = new trajectoryParams[] {
        //    new trajectoryParams(5, 144.519, 4716.66),
        //    new trajectoryParams(10, 439.277, 6697),
        //    new trajectoryParams(15, 824.9, 7871.84),
        //    new trajectoryParams(20, 1268.51, 8620.05),
        //    new trajectoryParams(25, 1751.06, 9065.48),
        //    new trajectoryParams(30, 2260.04, 9284.76),
        //    new trajectoryParams(35, 2780.34, 9313.15),
        //    new trajectoryParams(40, 3301.83, 9165.69),
        //    new trajectoryParams(45, 3814.6, 8863),
        //    new trajectoryParams(50, 4309.23, 8408.22),
        //    new trajectoryParams(55, 4775.74, 7809.66),
        //    new trajectoryParams(60, 5206.94, 7071.26),
        //    new trajectoryParams(65, 5592.81, 6192.66)
        //};

        /*
         * 3OF56 Shell new Testing
         * values order (angle, range, apex)
         */
        static public trajectoryParams[] trajectories = new trajectoryParams[] {
            new trajectoryParams(10, 6661.3, 430.952),
            new trajectoryParams(15, 7849.56, 813.626),
            new trajectoryParams(20, 8605.25, 1256.06),
            new trajectoryParams(25, 9059.92, 1738.87),
            new trajectoryParams(30, 9284.52, 2246.41),
            new trajectoryParams(35, 9314.81, 2766.3),
            new trajectoryParams(40, 9174.07, 3287.62),
            new trajectoryParams(45, 8872.77, 3801.27),
            new trajectoryParams(50, 8422.19, 4296.47),
            new trajectoryParams(55, 7827.29, 4764.69),
            new trajectoryParams(60, 7093.05, 5196.09),
        };

        /*
         * 3OF56 Shell Charge 4 Testing
         * values order (angle, range, apex)
         */
        //static public trajectoryParams[] trajectories = new trajectoryParams[] {
        //    new trajectoryParams(10, 2086.33, 110.792),
        //    new trajectoryParams(15, 2764.46, 224.979),
        //    new trajectoryParams(20, 3289.75, 368.797),
        //    new trajectoryParams(25, 3675.75, 537.382),
        //    new trajectoryParams(30, 3933.89, 723.743),
        //    new trajectoryParams(35, 4088.04, 921.191),
        //    new trajectoryParams(40, 4141.71, 1125.7),
        //    new trajectoryParams(45, 4094.58, 1334),
        //    new trajectoryParams(50, 3954.91, 1536.68),
        //    new trajectoryParams(55, 3726.66, 1730.44),
        //    new trajectoryParams(60, 3411.13, 1910.92),
        //};

        static (double, double, double, double) simulateForAngle(double angle, double Xmultiplier, double Xoffset, double Ymultiplier, double Yoffset)
        {
            Artillery artillery = new Art_2S1_Direct();
            ShellType shell = new _3OF56();
            double muzzleVelocity = shell.initSpeed * artillery.modes[0].artilleryCharge;
            double altDiff = 0;

            double deltaT = simulationStep;
            Vector3D speed = new Vector3D(0, Math.Cos(angle) * muzzleVelocity, Math.Sin(angle) * muzzleVelocity);
            Vector3D gravV = new Vector3D(0, 0, -gravity);

            Vector3D currentPos = new Vector3D(0, -(Math.Cos(angle) * Xmultiplier + Xoffset), Math.Sin(angle) * Ymultiplier + Yoffset); // Testing values here

            Vector3D changeInVelocity = new Vector3D(0, 0, 0);

            double tof = 0.0f;

            double apex = 0.0f;

            while (currentPos.Z >= altDiff || speed.Z > 0)
            {
                currentPos += speed * deltaT;
                changeInVelocity = speed.Length * speed * shell.airFriction + gravV;
                speed += changeInVelocity * deltaT;
                tof += deltaT;

                if (apex < currentPos.Z) apex = currentPos.Z;

                tof += deltaT;
                angle = Math.Atan2(speed.Z, speed.Y);
                //Console.WriteLine($"py: {currentPos.Z:F3}, px:{currentPos.Y:F3}, tof:{tof:F3}, V:{speed:F3}, Vx: {speed.Y:F3}, Vy: {speed.Z:F3}, Angle: {toDegrees(angle):F6}, changeInVelocity: {changeInVelocity}");
            }
            //Console.WriteLine($"px:{currentPos.Y:F3}, tof:{tof:F3}, V:{speed:F3}, exitAngle:{toDegrees(angle):F3}, py:{currentPos.Z:F3}");
            double vyRatio = (altDiff - currentPos.Z) / speed.Z;
            //Console.WriteLine($"altDiff:{altDiff:f3}, py:{currentPos.Z:f3}, vy:{speed.Z:f3}");
            double pxCorrection = Math.Abs(speed.Y * vyRatio);
            currentPos.Y -= pxCorrection;
            //Console.WriteLine($"vyRatio:{vyRatio:F3}, pxCorrection:{pxCorrection:F3}, pxAfter:{currentPos.Y:F3}, apex:{apex:f3}");

            if (apex < altDiff)
            {
                currentPos.Y = 0;
                tof = 0;
            };

            return (currentPos.Y, tof, Logic.toDegrees(angle), apex);
        }

        public static double bruteValues()
        {
            double startI = 0;
            double startJ = 0;
            double endI = 15;
            double endJ = 15;

            double step = 0.1;

            double apexI = 100;
            double apexJ = 100;
            double minApexDeviation = 9999;
            double rangeI = 100;
            double rangeJ = 100;
            double minRangeDeviation = 9999;


            for (double i = startI; i < endI; i += step)
            {
                for (double j = startJ; j < endJ; j += step)
                {
                    double apexTotal = 0;
                    double apexDeviationAverage;

                    double rangeTotal = 0;
                    double rangeDeviationAverage;
                    for (int k = 0; k < trajectories.Length; k++)
                    {
                        double px, tof, exitAngle, apex;
                        (px, tof, exitAngle, apex) = simulateForAngle(Logic.toRadians(trajectories[k].angle), Math.Round(i, 1), Math.Round(j, 1), 7.9, 6.7);
                        //(px, tof, exitAngle, apex) = simulateForAngle(Logic.toRadians(trajectories[k].angle), 0, 0, Math.Round(i, 1), Math.Round(j, 1));
                        apexTotal += Math.Abs(apex - trajectories[k].apex);
                        rangeTotal += Math.Abs(px - trajectories[k].range);
                    }
                    apexDeviationAverage = apexTotal / (trajectories.Length + 1);
                    rangeDeviationAverage = rangeTotal / (trajectories.Length + 1);

                    /* Test for Apex */
                    if (apexDeviationAverage < minApexDeviation)
                    {
                        minApexDeviation = apexDeviationAverage;
                        apexI = i;
                        apexJ = j;
                        Console.WriteLine($"apexDev:{minApexDeviation:f3}, i:{apexI}, j:{apexJ}");
                    }

                    /* Test for Range */
                    if (rangeDeviationAverage < minRangeDeviation)
                    {
                        minRangeDeviation = rangeDeviationAverage;
                        rangeI = i;
                        rangeJ = j;
                        Console.WriteLine($"rangeDev:{minRangeDeviation:f3}, i:{rangeI}, j:{rangeJ}");
                    }
                }
                Console.WriteLine($"Tested i:{i}");
            }
            Console.WriteLine($"\nEnd Output:");
            Console.WriteLine($"apexDev:{minApexDeviation:f3}, i:{apexI}, j:{apexJ}");
            Console.WriteLine($"rangeDev:{minRangeDeviation:f3}, i:{rangeI}, j:{rangeJ}");
            return 0;
        }

        public static double bruteForAngle(trajectoryParams trajectory)
        {
            Console.WriteLine($"Testing for angle:{trajectory.angle}");
            double startI = 0;
            double startJ = 0;
            double endI = 25;
            double endJ = 25;

            double step = 0.1;

            double apexI = 100;
            double apexJ = 100;
            double minApexDeviation = 9999;
            double rangeI = 100;
            double rangeJ = 100;
            double minRangeDeviation = 9999;


            for (double i = startI; i < endI; i += step)
            {
                for (double j = startJ; j < endJ; j += step)
                {
                    double px, tof, exitAngle, apex;
                    //(px, tof, exitAngle, apex) = simulateForAngle(toRadians(trajectory.angle), 0, 0, Math.Round(i, 1), Math.Round(j, 1));
                    (px, tof, exitAngle, apex) = simulateForAngle(Logic.toRadians(trajectory.angle), Math.Round(i, 1), Math.Round(j, 1), 2.3, 17.6);
                    double apexDiff = Math.Abs(apex - trajectory.apex);
                    double rangeDiff = Math.Abs(px - trajectory.range);

                    /* Test for Apex */
                    if (apexDiff < minApexDeviation)
                    {
                        minApexDeviation = apexDiff;
                        apexI = i;
                        apexJ = j;
                        Console.WriteLine($"AAA_apexDev:{minApexDeviation:f3}, i:{apexI}, j:{apexJ}");
                    }

                    /* Test for Range */
                    if (rangeDiff < minRangeDeviation)
                    {
                        minRangeDeviation = rangeDiff;
                        rangeI = i;
                        rangeJ = j;
                        Console.WriteLine($"RRR_rangeDev:{minRangeDeviation:f3}, i:{rangeI}, j:{rangeJ}");
                    }
                }
                Console.WriteLine($"Tested i:{i}");
            }

            Console.WriteLine($"\nEnd Output:");
            Console.WriteLine($"apexDev:{minApexDeviation:f3}, i:{apexI}, j:{apexJ}");
            Console.WriteLine($"rangeDev:{minRangeDeviation:f3}, i:{rangeI}, j:{rangeJ}");
            return 0;
        }
    }
}
