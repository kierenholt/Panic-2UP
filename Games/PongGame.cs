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
using Panic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Collections;

namespace Panic
{
    public class PongGame : Game
    {
        private Key[] upKeys = new Key[] { Key.W, Key.I };
        private Key[] downKeys = new Key[] { Key.S, Key.K };
        private Key[] leftKeys = new Key[] { Key.A, Key.J };
        private Key[] rightKeys = new Key[] { Key.D, Key.L };


        public const int PLAYERHEIGHT = 20;
        public const int PLAYERWIDTH = 80;

        public override int width { get { return 300; } }
        public override int height { get { return 400; } }
        public override string scoreNames { get { return "Score: "; } }
        
        public PongGame() : base()
        {
            keysToCheckPermanent = upKeys.Concat(downKeys).Concat(leftKeys).Concat(rightKeys).ToArray();
        }

        public override void movePlayer(object o)
        {
            //HumanPlayer p = (HumanPlayer)o;

            ////position update
            //p.xVelocity = 0;
            //if (keysDownMomentary.Contains(rightKeys[p.playerID])) { p.xVelocity = 2; }
            //if (keysDownMomentary.Contains(leftKeys[p.playerID])) { p.xVelocity = -2; }

            //if (p.script != null && p.playerScriptON && p.xVelocity == 0)
            //{
            //    string str = p.script(String.Join(",", Math.Floor(ball.x - PLAYERWIDTH / 2 - p.x), Math.Floor(ball.y - p.y)));
            //    if (str.Equals("L")) p.xVelocity = -2;
            //    if (str.Equals("R")) p.xVelocity = +2;
            //}

            //p.x += p.xVelocity;
            //p.y += p.yVelocity;
        }



        CollisionChecker<Box, Projectile> floorBall;
        CollisionChecker<Player, Projectile>[] playerBall;
        CollisionChecker<Box, Projectile>[] scoringZoneCollisions;

        public override void doCollisions()
        {
            //ball

            
            //position update
            ball.x += ball.xVelocity;
            ball.y += ball.yVelocity;

            //collisions

            if (floorBall.update())
                floorBall.reboundKeepInside();

            foreach (var pb in playerBall)
                if (pb.update())
                    pb.reboundKeepOutside();

            for (int i = 0; i < scoringZoneCollisions.Count(); i++ )
                if (scoringZoneCollisions[i].update() && scoringZoneCollisions[i].verticalResult.HasFlag(CollisionType.intruding))
                    players[i].scoreOrLives++;
        }

        public Projectile ball;
        public Box[] scoringZones;

        public FixedSprite floor;

        public override ObservableCollection<FixedSprite> sprites
        {
            get
            {
                if (_sprites == null)
                {
                    _sprites = new ObservableCollection<FixedSprite>();

                    //floor = new FixedSprite(Helpers.fillSquare(Color.Aqua, width, height).ToFrame(), 0, 0);
                    //_sprites.Add(floor);

                    ////ball
                    //ball = new Projectile(Helpers.fillSquare(System.Drawing.Color.Black, 10, 10).ToFrame(),
                    //   80, 80,
                    //    -3, -3);
                    //_sprites.Add(ball); 

                    ////scoringzones
                    //scoringZones = new Box[] { new Box(0, height - 90, width, 90), new Box(0, 0, width, 90)};

                    ////players
                    //foreach (HumanPlayer p in players)
                    //    _sprites.Add(p);

                    ////collisions
                    //floorBall = new CollisionChecker<Box,Projectile>(floor, ball);
                    //playerBall = new CollisionChecker<Player, Projectile>[] 
                    //{ 
                    //    new CollisionChecker<Player, Projectile>(players[0], ball),
                    //    new CollisionChecker<Player, Projectile>(players[1], ball) 
                    //};
                    //scoringZoneCollisions = new CollisionChecker<Box, Projectile>[] 
                    //{
                    //    new CollisionChecker<Box, Projectile>(scoringZones[1], ball),
                    //    new CollisionChecker<Box, Projectile>(scoringZones[0], ball)
                    //};
                }
                return _sprites;
            }
        }

        public override IEnumerable<Player> players
        {
            get
            {
                if (_players == null)
                {
                    _players = new List<Player>();

                        //_players[0] = new HumanPlayer(0, 0,
                        //    Helpers.fillSquare(HumanPlayer.colors[0], PLAYERWIDTH, PLAYERHEIGHT).ToFrame(),
                        //    (width - PLAYERWIDTH) / 2,
                        //    height - 100,
                        //    0, defaultPythonCode, null );
                    

                        //_players[1] = new HumanPlayer(1, 1,
                        //    Helpers.fillSquare(HumanPlayer.colors[1], PLAYERWIDTH, PLAYERHEIGHT).ToFrame(),
                        //    (width - PLAYERWIDTH) / 2,
                        //    100 - PLAYERHEIGHT,
                        //    0, defaultPythonCode, null);

                }
                return _players;
            }
        }

        public override string[] puterScripts
        {
            get
            {
                return new[] 
                {
                    @"
def move(input):
  coords = input.split("","")
  if (int(coords[0]) < 0):
    return ""L""
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
  coords = input.split("","")
  ballx = int(coords[0])
  bally = int(coords[1])
  return ""R""
";          }
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
s is a comma separated x,y coordinate for the position of the ball relative to the middle of the paddle 

<h3>e.g. 30, -50 means the ball is 30 pixels to the left and 50 pixels down</h3>

<h2>return values</h2>
your function should return a string.
""L"" moves the paddle left
""R"" moves the paddle right

</p>
";
            }
        }
    
    }

}
