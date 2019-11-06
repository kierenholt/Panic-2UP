using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Panic
{

    public class RaceGame : Game<RacePlayer>
    {

        private int _width;
        private int _height;
        public override int width { get { return _width; } }
        public override int height { get { return _height; } }
        public override string scoreNames { get { return " Laps: "; } }


        public RaceGame() : base()
        {
            keyActions = new Stack<Key[]>(new []
            {
                new[] { Key.OemOpenBrackets, Key.OemSemicolon, Key.OemComma, Key.OemTilde } ,
                new[] { Key.T, Key.F, Key.G, Key.H } ,
                new[] { Key.I, Key.J, Key.K, Key.L } ,
                new[] { Key.NumPad8, Key.NumPad4, Key.NumPad5, Key.NumPad6 },
                new[] { Key.Up, Key.Left, Key.Down, Key.Right },
                new[] { Key.W, Key.A, Key.S, Key.D }
            });

            keysToCheckPermanent = keyActions.SelectMany(g => g).ToArray();

            //players = new ObservableCollection<RacePlayer>(); //ALREADY INSTANTIATED IN BASE

            sprites.Count();

            playerPlayer = new List<CollisionChecker<RacePlayer, RacePlayer>>();

            //human
            enrolPlayer(new RacePlayer(Color.Red,
                Panic.Properties.Resources.black_vehicles.toFrames(0, 0, 32, 32, 8).colorized(Color.Red),
            playerStartingPoint(players.Count),
            5,
            defaultPythonCode,
            keyActions.Pop(),
            false));

            //puter
            enrolPlayer(new RacePlayer(Color.Black,
            Panic.Properties.Resources.black_vehicles.toFrames(0, 0, 32, 32, 8).colorized(Color.Black),
            playerStartingPoint(players.Count),
            5,
            puterScripts[0],
            null,
            true));
        }

        public List<CheckPoint> checkPoints;
        public List<TileCollisionChecker<RacePlayer>> tileCollisionCheckers;

        public TileGrid thingy;

        
        public override ObservableCollection<FixedSprite> sprites
        {
            get
            {
                if (_sprites == null)
                {
                    List<WriteableBitmap> A = Properties.Resources.A.ToFrame();
                    List<WriteableBitmap> B = Properties.Resources.B.ToFrame();
                    List<WriteableBitmap> C = Properties.Resources.C.ToFrame();
                    List<WriteableBitmap> D = Properties.Resources.D.ToFrame();
                    List<WriteableBitmap> E = Properties.Resources.E.ToFrame();
                    List<WriteableBitmap> F = Properties.Resources.F.ToFrame();
                    List<WriteableBitmap> G = Properties.Resources.G.ToFrame();
                    List<WriteableBitmap> H = Properties.Resources.H.ToFrame();
                    List<WriteableBitmap> I = Properties.Resources.I.ToFrame();
                    List<WriteableBitmap> J = Properties.Resources.J.ToFrame();
                    List<WriteableBitmap> K = Properties.Resources.K.ToFrame();
                    List<WriteableBitmap> L = Properties.Resources.L.ToFrame();

                    _sprites = new ObservableCollection<FixedSprite >();

                    Dictionary<char, Func<double[], string, Tile>> tileDict = new Dictionary<char, Func<double[], string, Tile>>()
                    {
                        {'A', (xy,str) => { return new CheckPoint(A, xy, new[] {64d, 64d}, str)
                            { faceAtNorth = Face.left, collisionType = TileCollisionType.tileRight | TileCollisionType.extruding  }; }}, //E checkpoint
                        {'B', (xy,str) => { return new CheckPoint(B, xy, new[] {64d, 64d}, str)
                            { faceAtNorth = Face.front, collisionType = TileCollisionType.tileTop | TileCollisionType.extruding  };}}, //N checkpoint
                        {'C', (xy,str) => { return new CheckPoint(C, xy, new[] {64d, 64d}, str)
                            { faceAtNorth = Face.rear, collisionType = TileCollisionType.tileBottom   | TileCollisionType.extruding }; }}, //S checkpoint
                        {'D', (xy,str) => { return new CheckPoint(D, xy, new[] {64d, 64d}, str)
                            { faceAtNorth = Face.right, collisionType = TileCollisionType.tileLeft  | TileCollisionType.extruding}; }}, //W checkpoint
                        {'E', (xy,str) => { return new Tile(E, xy, new[] {64d, 64d}, str)
                            { faceAtNorth = Face.right }; }}, //starting line
                        {'F', (xy,str) => { return new Tile(F, xy, new[] {64d, 64d}, str); }},
                        {'G', (xy,str) => { return new Tile(G, xy, new[] {64d, 64d}, str); }},
                        {'H', (xy,str) => { return new Tile(H, xy, new[] {64d, 64d}, str); }},
                        {'I', (xy,str) => { return new Tile(I, xy, new[] {64d, 64d}, str); }},
                        {'J', (xy,str) => { return new Tile(J, xy, new[] {64d, 64d}, str); }},
                        {'K', (xy,str) => { return new Tile(K, xy, new[] {64d, 64d}, str); }},
                        {'L', (xy,str) => { return new Tile(L, xy, new[] {64d, 64d}, str); }}
                    };

                    string map = @"
F	F	F	F	F	F	F	F	F	F	F	F
F	F	F	K	H	H	H	H	A5	I	F	F
F	F	F	B4	F	F	F	F	F	G	F	F
F	F	F	G	F	K	D9	I	F	G	F	F
F	F	F	G	F	G	F	B8	F	C6	F	F
F	F	F	G	F	G	F	J	D7	L	F	F
F	F	F	G	F	G	F	F	F	F	F	F
F	K	A3	L	F	C10	F	F	F	F	F	F
F	B2	F	F	F	J	H	H	A11	I	F	F
F	G	F	F	F	F	F	F	F	C12	F	F
F	J	D1	H	H	E	H	H	H	L	F	F
F	F	F	F	F	F	F	F	F	F	F	F
";

                    thingy = TileGrid.create(map, tileDict);
                    _sprites.Add(thingy);

                    char[] checkpointChars = new[] { 'A', 'B', 'C', 'D' };
                    checkPoints = thingy.allTiles.OfType<CheckPoint>().OrderBy(t2 => Int32.Parse(t2.tag.Substring(1))).ToList();
                    for (int c = 0; c < checkPoints.Count - 1; c++)
                        checkPoints[c].next = checkPoints[c + 1];
                    checkPoints.Last().next = checkPoints.First();


                    tileCollisionCheckers = new List<TileCollisionChecker<RacePlayer>>();

                    _width = (int)thingy.sourceRect.Right;
                    _height = (int)thingy.sourceRect.Bottom;

                }
                return _sprites;
            }
        }


        public override RacePlayer createPlayerFromSelection(PlayerSelectionVM playerSelectionVM)
        {
            bool isPuter = playerSelectionVM.selectedPuterLevelZeroIsHuman > 0;
            string script = isPuter ? puterScripts[playerSelectionVM.selectedPuterLevelZeroIsHuman - 1] : string.Empty;

            if (!isPuter && !keyActions.Any())
                return null;

            Key[] keys = isPuter ? null : keyActions.Pop();

            RacePlayer newPlayer = new RacePlayer(playerSelectionVM.selectedColor.todrawingcolor(), 
                playerSelectionVM.rotatingSprite.bitmapList.ToList() , 
                playerStartingPoint(players.Count), 5,
                script, keys, isPuter);

            return newPlayer;
        }

        public override void enrolPlayer(RacePlayer p)
        {
            ////players

            foreach(RacePlayer rp in players.Except(new[] {p}))
                playerPlayer.Add(new CollisionChecker<RacePlayer, RacePlayer>(rp, p));

            players.Add(p);

            p.tc = new TileCollisionChecker<RacePlayer>(p, thingy);
            tileCollisionCheckers.Add(p.tc);

            resetPlayer(p);


            p.setNextCheckPoint(checkPoints.First());
        }

        /// <summary>
        /// player needs to be in *players* for this to work. resets player properties not other sprites
        /// </summary>
        /// <param name="p"></param>
        public override void resetPlayer(RacePlayer p)
        {
            p.setNextCheckPoint(checkPoints.First());
            double[] xy = playerStartingPoint(players.IndexOf(p));
            p.worldX = xy[0]; p.worldY = xy[1];
            p.xVelocity = 0; p.yVelocity = 0;
            p.rotation = Bearing.W;
            p._currentFrameNum = 0; //0 == West
            if (p.isPuter)
                p.playerScriptON = true;
            p.RaisePropertyChanged("");


        }

        public override double[] playerStartingPoint(int index)
        {
            Tile starttile = thingy.allTiles.First(t => t.tag[0] == 'E');
            return starttile.getCornerCoords(Face.front);
        }

        public void expelPlayer(RacePlayer p)
        {
            foreach (CollisionChecker<RacePlayer, RacePlayer> rp in playerPlayer.Where(pp => pp.proj == p || pp.box == p))
                playerPlayer.Remove(rp);

            //remove collisions
        }

        List<CollisionChecker<RacePlayer, RacePlayer>> playerPlayer;

        public override void doCollisions()
        {

            foreach (var pp in playerPlayer)
                if (pp.update())
                    pp.reboundEqualMasses();

            foreach (var t in tileCollisionCheckers)
                t.update();

            //add tile effects 
            foreach (var p in players)
            {
                Tile current = thingy.getTile(p.worldX, p.worldY);
                if (current != null && current.tag[0] == 'F')
                {
                    p.xVelocity *= 0.98;
                    p.yVelocity *= 0.98;
                }
            }
        }


        public override void movePlayer(object o)
        {
            //save state
            Player p = (Player)o;
            p.move(keysDownMomentary);
        }

        public override string[] puterScripts
        {
            get
            {
                return new[] 
                {
                    @"
def move(input):
  distance = int(input.split("","")[1])
  bearing = input.split("","")[0]
  if (bearing == ""E""):
    return ""RA""
  if (bearing == ""SE""):
    return ""RA""
  if (bearing == ""NE""):
    return ""RA""
  if (bearing == ""N""):
    return ""A""
  if (bearing == ""W""):
    return ""LA""
  if (bearing == ""SW""):
    return ""LA""
  if (bearing == ""NW""):
    return ""LA""
  if (bearing == ""S""):
    return ""R""


"
                };
            }
        }

        public override string defaultPythonCode
        {
            get
            {
                return @"

def move(input):
  output = """"
  coords = input.split("","")
  
  return output

";
            }
        }


        public override string HTMLInstructions 
        {
            get
            {
                return @"

<p>

<h1>move(s)</h1>

Only python function is required: move(s)

<h2>first input parameter: s</h2>
s is the x,y coordinates for the position of the next checkpoint relative to the middle of the paddle

<h3>e.g. 30, -50, means the next gate to aim for is 30 pixels to the left and 50 pixels down</h3>

<h2>return values</h2>
your function should return a string which may contain:
""L"" moves the car left
""R"" moves the car right
""U"" moves the car up
""D"" moves the car down

</p>
";
            }
        }
    }

}
