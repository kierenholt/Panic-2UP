using Panic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Panic
{
    public class Projectile : FixedSprite
    {
        public double xVelocity;
        public double yVelocity;

        public Projectile(List<WriteableBitmap> paramBitmap,
            double[] paramXY, double[] paramWorldWidthHeight,
            double paramXVelocity,
            double paramYVelocity,
            double paramWorldToScreenOffsetX,
            double paramWorldToScreenOffsetY
            )
            : base(paramBitmap, paramXY, paramWorldWidthHeight)
        {
            xVelocity = paramXVelocity;
            yVelocity = paramYVelocity;
            blitScreenOffsetX = paramWorldToScreenOffsetX;
            blitScreenOffsetY = paramWorldToScreenOffsetY;
        }

        public Bearing _rotation;
        public Bearing rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    currentFrameNum = (int)rotation + frameOffset;
                    _bitmapChanged = true;
                }
            }
        }


        public void updateRotation()
        {
            rotation = rotation.updateOnce(xVelocity, yVelocity);
        }

        public override Face faceAtNorth { get { return (Face)(1 - rotation); } set { } }

        //public static Bearing BearingTest(int x, int y)
        //{
        //    Projectile proj = new Projectile(Helpers.fillSquare(System.Drawing.Color.AliceBlue, 3, 3).ToFrame(), 3, 3, x, y);
        //    proj.updateRotation();
        //    return proj.rotation;
        //}

        //public static Bearing bearingTo()
        //{
        //    Bearing retVal = Bearing.N;
        //    Bearing nextVal = updateBearingTo();
        //    while (retVal != nextVal)
        //    {
        //        retVal = nextVal;
        //        nextVal = updateBearingTo();
        //    }
        //    return retVal;
        //}
        
    }
}
