using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Media3D;

namespace A3_Arty_Calc
{
    class Logic
    {
        static public double gravity = 9.8066;

        static public double simulationStep = 0.01;

        static public double toDegrees(double rad)
        {
            return (180 / Math.PI) * rad;
        }

        static public double toRadians(double grad)
        {
            return Math.PI / 180 * grad;
        }

        static public double getRange(double x1, double y1, double x2, double y2, double z1 = 0, double z2 = 0)
        {
            double valX = Math.Pow(x1 - x2, 2);
            double valY = Math.Pow(y1 - y2, 2);
            double valZ = Math.Pow(z1 - z2, 2);
            double range = 10 * Math.Sqrt(valX + valY + valZ);
            return range;
        }

        static public double getBearing(double x1, double y1, double x2, double y2)
        {
            double valX = x1 - x2;
            double valY = y1 - y2;
            double atan = Math.Atan(valY / valX);
            double degrees = toDegrees(atan);
            double approxBearing;
            if (x1 > x2)
            {
                approxBearing = 270;
            }
            else
            {
                approxBearing = 90;
            }
            double result = approxBearing - degrees;
            return result;
        }

        static public double getAltitudeDiff(double x, double y)
        {
            return y - x;
        }

        static public (double, double, double, double, double) getAngleSolutionForRange2(double zeroRange, double muzzleVelocity, double altDiff, Artillery artillery, ShellType shell, bool isTopDown)
        {
            double angleTolerance = toRadians(0.05);
            double minAngle = artillery.minAngle;
            double maxAngle = artillery.maxAngle;
            int attemptCount = 0;
            double px = 0, tof = 0, exitAngle = 0, apex = 0, currentAngle = 0;

            while (attemptCount < 50)
            {
                currentAngle = (minAngle + maxAngle) / 2;
                (px, tof, exitAngle, apex) = simulateForAngle2(muzzleVelocity, toRadians(currentAngle), artillery, shell, altDiff);
                if (zeroRange <= px)
                {
                    if (!isTopDown)
                    {
                        maxAngle = currentAngle;
                    }
                    else
                    {
                        minAngle = currentAngle;
                    }

                }
                else
                {
                    if (!isTopDown)
                    {
                        minAngle = currentAngle;
                    }
                    else
                    {
                        maxAngle = currentAngle;
                    }

                }

                if (Math.Abs(maxAngle - minAngle) < angleTolerance)
                {
                    break;
                }

                ++attemptCount;
            }

            double maxError = 20;

            if (Math.Abs(px - zeroRange) > maxError)
            {
                return (0, 0, exitAngle, apex, px);
            }

            currentAngle += artillery.angleAdjustment;


            return (currentAngle, tof, exitAngle, apex, px);
        }

        static public (double, double, double, double) simulateForAngle2(double muzzleVelocity, double angle, Artillery artillery, ShellType shell, double altDiff = 0)
        {
            double deltaT = artillery.simulationStep;
            Vector3D speed = new Vector3D(0, Math.Cos(angle) * muzzleVelocity, Math.Sin(angle) * muzzleVelocity);
            Vector3D gravV = new Vector3D(0, 0, -gravity);
            Vector3D currentPos = getVectorBasedOnAngle(angle, artillery);

            Vector3D changeInVelocity = new Vector3D(0, 0, 0);

            double tof = 0.0f;

            double apex = 0.0f;

            while (currentPos.Z >= altDiff || speed.Z > 0)
            {
                currentPos += speed * deltaT;
                changeInVelocity = artillery.isAirFriction ? speed.Length * speed * shell.airFriction + gravV : gravV;

                speed += changeInVelocity * deltaT;
                tof += deltaT;

                if (apex < currentPos.Z) apex = currentPos.Z;

                tof += deltaT;
                angle = Math.Atan2(speed.Z, speed.Y);
            }
            double vyRatio = (altDiff - currentPos.Z) / speed.Z;
            double pxCorrection = Math.Abs(speed.Y * vyRatio);
            currentPos.Y -= pxCorrection;

            if (apex < altDiff)
            {
                currentPos.Y = 0;
                tof = 0;
            };

            return (currentPos.Y, tof, toDegrees(angle), apex);
        }

        static public Vector3D getVectorBasedOnAngle(double angle, Artillery artillery)
        {
            Vector3D vector = artillery.getBaseProjectileSpawnPoint(angle);

            if (artillery.angleSolutions != null)
            {
                double degAngle = toDegrees(angle);
                foreach (AngleSolution solution in artillery.angleSolutions)
                {
                    if (degAngle < solution.angle + 2.5)
                    {
                        vector = new Vector3D(0, -(Math.Cos(angle) * solution.Xmultiplier + solution.Xoffset), Math.Sin(angle) * solution.Ymultiplier + solution.Yoffset);
                        break;
                    }
                }
            }
            return vector;
        }
    }
}
