using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Panic
{
    public static class Screen
    {
        public static System.Windows.Point toScreen(this Point point)
        {
            return new System.Windows.Point(point.X + point.Y, (point.Y - point.X) / 2);
            //return new[] { worldCoords[0] + worldCoords[1], (worldCoords[1] - worldCoords[0]) / 2 };
        }


        public static Rect toScreen(this Rect rect)
        {
            double TLx = rect.Top + rect.Left;
            double TRy = (rect.Top - rect.Right)/2;
            double BLy = (rect.Bottom - rect.Left)/2;
            double BRx = rect.Right + rect.Bottom;

            return new Rect(TLx, TRy, BRx - TLx, BLy - TRy);
        }

        public static Point minus(this Point point, Point another)
        {
            return new Point(point.X - another.X, point.Y - another.Y);
        }

        public static Point plus(this Point point, Point another)
        {
            return new Point(point.X + another.X, point.Y + another.Y);
        }
    }

    public class Tile: FixedSprite
    {
        public string tag;

        public Tile(List<WriteableBitmap> paramBitmap, Rect paramCollisionRect, Rect paramScreenRectRelativeToCollisionTopLeft, string paramString)
            : base(paramBitmap, paramCollisionRect, paramScreenRectRelativeToCollisionTopLeft)
        {
            tag = paramString;
        }
    }

    public class TileGrid : SpriteContainer<Tile>
    {
        public const double worldTileHeight = 64d;

        public static TileGrid create(string tileCodes, Dictionary<char, Func<double[], string, Tile>> tileDict)
        {
            var query = tileCodes.Replace(" ","").Split(new[]  {'\n','\r'}, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(new[] {'\t'}));
            int rows = query.Count();
            int cols = query.First().Count();

            double x = -0.5 * (cols * worldTileHeight);
            double y = 0.5 * (rows * worldTileHeight);

            return new TileGrid(query, tileDict, new[] { x, y }, new[] { cols * worldTileHeight, rows * worldTileHeight })
        }

        public TileGrid(IEnumerable<IEnumerable<string>> query, Dictionary<char, Func<double[], string, Tile>> tileDict, double[] paramXY, double[] paramWorldWidthHeight)
            : base(paramXY, paramWorldWidthHeight)
        {
            numRows = query.Count();
            numCols = query.First().Count();

            tiles = new Tile[numCols, numRows];
            allTiles = new List<Tile>();

            int rows = query.Count();
            int cols = query.First().Count();
            

            rowTops = Enumerable.Range(0, numRows).Select(n => worldTileHeight * n).ToArray();
            colLefts = Enumerable.Range(0, numCols).Select(n => worldTileHeight * n ).ToArray();
            rowBottoms = Enumerable.Range(0, numRows).Select(n => worldTileHeight * (n + 1)).ToArray();
            colRights = Enumerable.Range(0, numCols).Select(n => worldTileHeight * (n + 1)).ToArray();

            int r = 0; 
            foreach (var row in query)
            {
                int c = 0;
                foreach (var str in row)
                {
                    Tile tile = tileDict[str[0]](new[] { colLefts[c], rowTops[r]  }, str);
                    tiles[c, r] = tile;
                    allTiles.Add(tile);
                    c++;
                }
                r++;
            }

        }

        public Tile[,] tiles;
        public List<Tile> allTiles { get; private set; }

        public int numRows { get; private set; }
        public int numCols { get; private set; }
        public double[] rowTops { get; private set; }
        public double[] colLefts { get; private set; }
        public double[] rowBottoms { get; private set; }
        public double[] colRights { get; private set; }

        public int getColNum(double x)
        {
            int retVal = (int)Math.Floor((x - colLefts[0]) / worldTileHeight);
            if (retVal < 0 || retVal > numCols)
                return -1;
            return retVal;
        }

        public int getRowNum(double y)
        {
            int retVal = (int)Math.Floor((y - rowTops[0]) / worldTileHeight);
            if (retVal < 0 || retVal > numCols)
                return -1;
            return retVal;
            
        }

        public Tile getTile(double x, double y)
        {
            int col = getColNum(x);
            int row = getRowNum(y);
            if (col == -1 || row == -1)
                return null;
            return tiles[col, row];
        }

    }
}
