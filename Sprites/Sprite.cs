using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Panic
{
    public enum Face { frontLeft = 0, front = 1, frontRight = 2, right = 3, rearRight = 4, rear = 5, rearLeft = 6, left = 7 }
    
    public class Box
    {
        public Box(Rect paramCollisionRect)
        {}

        public Rect collisionRect { get; protected set; }

        public void centreOnCanvas(double canvasWidth, double canvasHeight)
        {
            //worldY = (canvasHeight - worldHeight) / 2;
            //worldX = (canvasWidth - worldWidth) / 2; 
        }
        
        public double leftCollision { get { return collisionRect.Left; } }
        public double rightCollision { get { return collisionRect.Right; } }
        public double topCollision { get { return collisionRect.Top; } }
        public double bottomCollision { get { return collisionRect.Bottom; } }
        public double centreCollision { get { return collisionRect.Left + collisionRect.Width / 2; } }
        public double middleCollision { get { return collisionRect.Top + collisionRect.Height / 2; } }
        
    }

    public class FixedSprite : Box, IBlittable
    {
        public FixedSprite(List<WriteableBitmap> paramBitmap, Rect paramCollisionRect, Rect paramScreenRectRelativeToCollisionTopLeft)
            : base(paramCollisionRect)
        {
            bitmapList = paramBitmap;
            numFrames = bitmapList.Count;
            currentFrameNum = 0;

            faceAtNorth = 0;

            screenRectRelativeToCollisionTopLeft = paramScreenRectRelativeToCollisionTopLeft;
        }

        public FixedSprite(List<WriteableBitmap> paramBitmap, System.Windows.Point paramLocation)
            : this(paramBitmap, new Rect(paramLocation, 
                new System.Windows.Size(paramBitmap[0].Width, paramBitmap[0].Height)),
                new Rect(0,0,paramBitmap[0].Width, paramBitmap[0].Height))
        {
        }


        public Rect screenRectRelativeToCollisionTopLeft { get; protected set; }
        public virtual Rect screenRect 
        {
            get 
            {
                return new Rect(collisionRect.TopLeft.toScreen().plus(screenRectRelativeToCollisionTopLeft.TopLeft), screenRectRelativeToCollisionTopLeft.Size); 
            }
            protected set; 
        }

        public virtual Face faceAtNorth { get; set; }

        public double[] getCornerCoords(Face corner)
        {
            switch (((int)corner - (int)faceAtNorth + 1).mod(8))
            {
                case 0:
                    return new[] { leftCollision, topCollision }; //left top
                case 1:
                    return new[] { centreCollision, topCollision }; //top
                case 2:
                    return new[] { rightCollision, topCollision };
                case 3:
                    return new[] { rightCollision, middleCollision };
                case 4:
                    return new[] { rightCollision, bottomCollision };
                case 5:
                    return new[] { centreCollision, bottomCollision };
                case 6:
                    return new[] { leftCollision, bottomCollision };
                case 7:
                    return new[] { leftCollision, middleCollision };
            }
            throw new ArgumentNullException();
        }

        public int frameOffset;
        public int _currentFrameNum;
        public int currentFrameNum 
        {
            get { return _currentFrameNum; }
            set
            {
                int newFrameNum = value.mod(numFrames);
                if (_currentFrameNum != newFrameNum)
                {
                    _currentFrameNum = newFrameNum;
                    _bitmapChanged = true;
                }
            }
        }
        protected int numFrames;

        protected bool _bitmapChanged;
        protected WriteableBitmap _cachedBitmap;

        public List<WriteableBitmap> bitmapList { get; protected set; }
        public virtual WriteableBitmap bitmap
        {
            get 
            {
                return _cachedBitmap;
            }
        }

        public void blit()
        {
            if (_cachedBitmap == null || _bitmapChanged)
            {
                //has no children, no caching required
                _cachedBitmap = bitmapList[currentFrameNum];
                _bitmapChanged = false;
            }
        }
    }

}
