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
        /*
        static public (double, double, double) getAngleSolutionForRange(double zeroRange, double muzzleVelocity, double altDiff, Artillery artillery)
        {
            double zeroAngle = 0.0f;
            double deltaT = artillery.simulationStep;
            double tof = 0.0f;
            double exitAngle = 0.0f;
            double minAngle = artillery.minAngle;
            double maxAngle = artillery.maxAngle;

            for (int i = 0; i < 20; i++)
            {
                double lx = 0.0f;
                double ly = 0.0f;

                //double px = artillery.muzzleLength * Math.Cos(zeroAngle);
                double px = 0;
                double py = artillery.muzzleLength * Math.Cos(zeroAngle) - altDiff / 100.0f;

                double gx = 0;
                double gy = -gravity;

                double vx = Math.Cos(zeroAngle) * muzzleVelocity;
                double vy = Math.Sin(zeroAngle) * muzzleVelocity;

                tof = 0.0f;
                double v = 0.0f;

                while (px < zeroRange && tof <= 100.0f)
                {
                    lx = px;
                    ly = py;

                    v = Math.Sqrt(vx * vx + vy * vy);

                    double ax = vx * v * artillery.airFriction;
                    double ay = vy * v * artillery.airFriction;
                    ax += gx;
                    ay += gy;

                    px += vx * deltaT * 0.5;
                    py += vy * deltaT * 0.5;
                    vx += ax * deltaT;
                    vy += ay * deltaT;
                    px += vx * deltaT * 0.5;
                    py += vy * deltaT * 0.5;

                    tof += deltaT;
                    exitAngle = Math.Atan2(vy, vx);
                    //Console.WriteLine($"tof: {tof}, px:{px}, py:{py}, vx{vx}, vy:{vy}");
                }

                double y = ly + (zeroRange - lx) * (py - ly) / (px - lx);
                double offset = -Math.Atan(y / zeroRange);
                zeroAngle += offset;

                if (Math.Abs(offset) < 0.00001f)
                {
                    break;
                }
            }

            return (zeroAngle, tof, exitAngle);
        }
        */
        static public (double, double, double, double, double) getAngleSolutionForRange2(double zeroRange, double muzzleVelocity, double altDiff, Artillery artillery, ShellType shell, bool isTopDown)
        {
            double angleTolerance = toRadians(0.1);
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
                    Console.WriteLine("Max Attempts reached");
                    break;
                }

                Console.WriteLine($"currentAngle:{currentAngle:f3}, maxAngle: {maxAngle:f3}, minAngle: {minAngle:f3}\n");
                ++attemptCount;
            }

            double maxError = 20;

            if (Math.Abs(px - zeroRange) > maxError)
            {
                return (0, 0, exitAngle, apex, px);
            }

            return (currentAngle, tof, exitAngle, apex, px);
        }
        static public (double, double, double, double) simulateForAngle2(double muzzleVelocity, double angle, Artillery artillery, ShellType shell, double altDiff = 0)
        {
            double deltaT = simulationStep;
            Vector3D speed = new Vector3D(0, Math.Cos(angle) * muzzleVelocity, Math.Sin(angle) * muzzleVelocity);
            Vector3D gravV = new Vector3D(0, 0, -gravity);
            Vector3D currentPos = getVectorBasedOnAngle(angle, artillery);

            Vector3D changeInVelocity = new Vector3D(0, 0, 0);

            double tof = 0.0f;

            double apex = 0.0f;

            while (currentPos.Z >= altDiff || speed.Z > 0)
            {
                currentPos += speed * deltaT;
                if (artillery.isAirFriction)
                {
                    changeInVelocity = speed.Length * speed * shell.airFriction + gravV;
                } else
                {
                    changeInVelocity = gravV;
                }
                speed += changeInVelocity * deltaT;
                tof += deltaT;

                if (apex < currentPos.Z) apex = currentPos.Z;

                tof += deltaT;
                angle = Math.Atan2(speed.Z, speed.Y);
                //Console.WriteLine($"py: {currentPos.Z:F3}, px:{currentPos.Y:F3}, tof:{tof:F3}, V:{speed:F3}, Vx: {speed.Y:F3}, Vy: {speed.Z:F3}, Angle: {toDegrees(angle):F6}, changeInVelocity: {changeInVelocity}");
            }
            Console.WriteLine($"px:{currentPos.Y:F3}, tof:{tof:F3}, V:{speed:F3}, exitAngle:{toDegrees(angle):F3}, py:{currentPos.Z:F3}");
            double vyRatio = (altDiff - currentPos.Z) / speed.Z;
            Console.WriteLine($"altDiff:{altDiff:f3}, py:{currentPos.Z:f3}, vy:{speed.Z:f3}");
            double pxCorrection = Math.Abs(speed.Y * vyRatio);
            currentPos.Y -= pxCorrection;
            Console.WriteLine($"vyRatio:{vyRatio:F3}, pxCorrection:{pxCorrection:F3}, pxAfter:{currentPos.Y:F3}, apex:{apex:f3}");

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
                    if( degAngle < solution.angle + 2.5)
                    {
                        vector = new Vector3D(0, -(Math.Cos(angle) * solution.Xmultiplier + solution.Xoffset), Math.Sin(angle) * solution.Ymultiplier + solution.Yoffset);
                        break;
                    }
                }
            }
            Console.WriteLine($"Returned vector: {vector}");
            return vector;
        }

    }
}
