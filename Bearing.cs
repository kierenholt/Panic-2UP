using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panic
{
    public enum Bearing
    {
        N = 0, NE = 1, E = 2, SE = 3, S = 4, SW = 5, W = 6, NW = 7
    }

    public static class BearingExtensions
    {

        public static int toDegrees(this Bearing b)
        {
            return (int)b * 45;
        }


        public static Bearing updateOnce(this Bearing rotation, double xVelocity, double yVelocity)
        {
            switch (rotation)
            {
                case Bearing.S: //xvelocity =  0, yvelocity > 0
                    if (-2 * xVelocity > Math.Abs(yVelocity)) return Bearing.SW;
                    if (2 * xVelocity > Math.Abs(yVelocity)) return Bearing.SE;
                    break;
                case Bearing.SE:
                    if (2 * xVelocity < Math.Abs(yVelocity)) return Bearing.S;
                    if (2 * yVelocity < Math.Abs(xVelocity)) return Bearing.E;
                    break;
                case Bearing.E:
                    if (2 * yVelocity > Math.Abs(xVelocity)) return Bearing.SE;
                    if (-2 * yVelocity > Math.Abs(xVelocity)) return Bearing.NE;
                    break;
                case Bearing.NE:
                    if (-2 * yVelocity < Math.Abs(xVelocity)) return Bearing.E;
                    if (2 * xVelocity < Math.Abs(yVelocity)) return Bearing.N;
                    break;
                case Bearing.N:
                    if (2 * xVelocity > Math.Abs(yVelocity)) return Bearing.NE;
                    if (-2 * xVelocity > Math.Abs(yVelocity)) return Bearing.NW;
                    break;
                case Bearing.NW:
                    if (-2 * xVelocity < Math.Abs(yVelocity)) return Bearing.N;
                    if (-2 * yVelocity < Math.Abs(xVelocity)) return Bearing.W;
                    break;
                case Bearing.W:
                    if (-2 * yVelocity > Math.Abs(xVelocity)) return Bearing.NW;
                    if (2 * yVelocity > Math.Abs(xVelocity)) return Bearing.SW;
                    break;
                case Bearing.SW:
                    if (2 * yVelocity < Math.Abs(xVelocity)) return Bearing.W;
                    if (-2 * xVelocity < Math.Abs(yVelocity)) return Bearing.S;
                    break;
            }
            return rotation;
        }


        public static Bearing toPointUpdateOnce(this Bearing rotation, double pointX, double pointY, double xVelocity, double yVelocity)
        {
            switch (rotation)
            {
                case Bearing.S: //xvelocity =  0, yvelocity > 0
                    if (-2 * (pointX*yVelocity-pointY*xVelocity) > Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.SW;
                    if (2 * (pointX*yVelocity-pointY*xVelocity) > Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.SE;
                    break;
                case Bearing.SE:
                    if (2 * (pointX*yVelocity-pointY*xVelocity) < Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.S;
                    if (2 * (pointY*yVelocity+pointX*xVelocity) < Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.E;
                    break;
                case Bearing.E:
                    if (2 * (pointY*yVelocity+pointX*xVelocity) > Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.SE;
                    if (-2 * (pointY*yVelocity+pointX*xVelocity) > Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.NE;
                    break;
                case Bearing.NE:
                    if (-2 * (pointY*yVelocity+pointX*xVelocity) < Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.E;
                    if (2 * (pointX*yVelocity-pointY*xVelocity) < Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.N;
                    break;
                case Bearing.N:
                    if (2 * (pointX*yVelocity-pointY*xVelocity) > Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.NE;
                    if (-2 * (pointX*yVelocity-pointY*xVelocity) > Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.NW;
                    break;
                case Bearing.NW:
                    if (-2 * (pointX*yVelocity-pointY*xVelocity) < Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.N;
                    if (-2 * (pointY*yVelocity+pointX*xVelocity) < Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.W;
                    break;
                case Bearing.W:
                    if (-2 * (pointY*yVelocity+pointX*xVelocity) > Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.NW;
                    if (2 * (pointY*yVelocity+pointX*xVelocity) > Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.SW;
                    break;
                case Bearing.SW:
                    if (2 * (pointY*yVelocity+pointX*xVelocity) < Math.Abs(pointX*yVelocity-pointY*xVelocity)) return Bearing.W;
                    if (-2 * (pointX*yVelocity-pointY*xVelocity) < Math.Abs(pointY*yVelocity+pointX*xVelocity)) return Bearing.S;
                    break;
            }
            return rotation;
        }
    }
}
