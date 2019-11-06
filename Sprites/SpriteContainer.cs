using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Panic
{
    public interface IBlittable
    {
        public WriteableBitmap bitmap;
        public Rect screenRect;
    }


    public class SpriteContainer<T1>: Box, IBlittable
        where T1 : IBlittable
    {

        public SpriteContainer(Rect paramCollisionRect, Point paramLocation)
            : base(paramCollisionRect)
        {

            
        }

        public Rect screenRect { get; private set; }
        protected WriteableBitmap _cachedBitmap;
        public List<T1> children = new List<T1>();
        public Action<T1, int> childModifier;

        public void addChild(T1 child)
        {
            children.Add(child);
            if (childModifier != null)
                for (int i = 0; i < children.Count; i++)
                    childModifier(children[i], i);
        }

        public void removeChild(T1 child)
        {
            children.Remove(child);
            if (childModifier != null)
                for (int i = 0; i < children.Count; i++)
                    childModifier(children[i], i);
        }


        public virtual WriteableBitmap bitmap
        {
            get
            {
                return _cachedBitmap;
            }
        }

        public void blit()
        {
            if (_cachedBitmap == null)
            {
                double minLeft = children.Min(c => c.screenRect.Left);
                double maxRight = children.Min(c => c.screenRect.Right);
                double minTop = children.Min(c => c.screenRect.Top);
                double maxBottom = children.Min(c => c.screenRect.Bottom);
                    
                screenRect = new Rect(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
                    
                _cachedBitmap = new WriteableBitmap((int)screenRect.Width, (int)screenRect.Height,
                    300, 300, System.Windows.Media.PixelFormats.Bgra32, null);

                foreach (var c in children)
                    _cachedBitmap.Blit(c.screenRect.Location.minus(screenRect.Location), 
                        c.bitmap, 
                        new Rect(0,0,c.screenRect.Width, c.screenRect.Height),
                        System.Windows.Media.Colors.White, WriteableBitmapExtensions.BlendMode.Alpha);
            }
        }
    }
}
