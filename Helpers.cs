using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Panic
{
    public static class Helpers
    {

        public static T[] RightRotateArray<T>(T[] arr, int shift)
        {
            return LeftRotateArray<T>(arr, arr.Count() - shift);
        }

        public static T[] LeftRotateArray<T>(T[] arr, int shift)
        {
            //shift = 2
            //12345 => 34512
            shift = shift % arr.Length;
            T[] buffer = new T[shift];
            Array.Copy(arr, 0, buffer, arr.Length - shift, shift);
            Array.Copy(arr, shift, buffer, 0, arr.Length - shift);
            return buffer;
        }

        public static Bitmap fillSquare(Color color, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(color);
            return bmp;
        }


        public static System.Drawing.Color todrawingcolor(this System.Windows.Media.Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B); 
        }

        public static System.Windows.Media.Color tomediacolor(this Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static bool equals(this Color c, System.Windows.Media.Color m)
        {
            return c.R == m.R && c.G == m.G && c.B == m.B;
        }
    }

    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {

        readonly List<TElement> elements;


        public Grouping(IEnumerable<TElement> paramGroup, TKey paramKey)
        {
            Key = paramKey;
            elements = paramGroup.ToList();
        }

        public Grouping(IGrouping<TKey, TElement> grouping)
        {
            if (grouping == null)
                throw new ArgumentNullException("grouping");
            Key = grouping.Key;
            elements = grouping.ToList();
        }

        public TElement this[int index]
        {
            get { return elements[index]; }
        }

        public TKey Key { get; private set; }

        public IEnumerator<TElement> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    }
}
