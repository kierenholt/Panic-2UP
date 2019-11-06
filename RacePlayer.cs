using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Panic
{
    public class CheckPoint : Tile
    {
        public CheckPoint(List<WriteableBitmap> paramBitmap, double[] paramworldXY, double[] paramWorldWidthHeight, string paramString) 
            : base(paramBitmap, paramworldXY, paramWorldWidthHeight, paramString)
        {
        }

        public TileCollisionType collisionType { get; set; }
        public CheckPoint next { get; set; }
    }

    public class RacePlayer : Player
    {
        public FixedSprite goHereArrow { get; set; }
        public CheckPoint checkPoint { get; set; }

        public TileCollisionChecker<RacePlayer> tc;

        public Bearing bearingToCheckPoint { get; set; }

        public void setNextCheckPoint(CheckPoint optional)
        {
            if (checkPoint != null)
            {
                checkPoint.removeChild(goHereArrow);
                tc.clearTriggers(checkPoint.collisionType);
            }
            //next checkpoint
            checkPoint = optional ?? checkPoint.next;
            checkPoint.addChild(goHereArrow);
            tc.addTrigger(checkPoint.collisionType, (player, tile) =>
                {
                    setNextCheckPoint(null);
                },
                (tile) => { return tile.tag == checkPoint.tag; }
            );
        }

        public RacePlayer(Color paramColor,
            List<WriteableBitmap> paramBitmap,
            double[] paramXY,
            int paramLives,
            string paramCodeString,
            Key[] paramKeyActions,
            bool paramIsPuter)
            : base(paramColor, paramBitmap, paramXY, new[] { 32d, 32d }, paramLives, paramCodeString, paramIsPuter, paramKeyActions, -12d, 12d)
        {
            leftCollisionOffset = 3;
            rightCollisionOffset = -3;
            topCollisionOffset = 8;
            bottomCollisionOffset = -8;
            goHereArrow = new FixedSprite(Properties.Resources.A.ToFrame().colorized(color), new[] { 0d, 4d }, new[] { 20d, 20d });
            frameOffset = 2;
        }


        public override void move(Key[] keysDownMomentary)
        {
            xVelocity *= 0.995;
            yVelocity *= 0.995;

            worldX += xVelocity;
            worldY += yVelocity;

            if (!isPuter)
                foreach (Key key in keysDownMomentary)
                {
                    if (key == keyActions[0]) accelerate();
                    if (key == keyActions[1]) turnleft();
                    if (key == keyActions[2]) brake();
                    if (key == keyActions[3]) turnright();
                }

            if (script != null && playerScriptON)
            {
                double[] checkPointTop = checkPoint.getCornerCoords(Face.front);
                double[] thisTop = getCornerCoords(Face.front);
                bearingToCheckPoint = bearingToCheckPoint.toPointUpdateOnce(checkPointTop[0] - thisTop[0], checkPointTop[1] - thisTop[1], -xVelocity, -yVelocity);
                double distanceToCheckPoint = Math.Sqrt((checkPointTop[0] - thisTop[0]) * (checkPointTop[0] - thisTop[0]) + (checkPointTop[1] - thisTop[1]) * (checkPointTop[1] - thisTop[1]));

                string input = bearingToCheckPoint.ToString() + "," + Math.Floor(distanceToCheckPoint);
                string output = string.Empty;

                try
                {
                    output = script(input);
                }
                catch
                {
                    playerScriptON = false;
                }

                if (!string.IsNullOrEmpty(output))
                {
                    if (output.Contains("L")) turnleft();
                    if (output.Contains("R")) turnright();
                    if (output.Contains("A")) accelerate();
                    if (output.Contains("B")) brake();
                }
            }

            updateRotation();
        }

        public void accelerate()
        {
            double speed = xVelocity * xVelocity + yVelocity * yVelocity;
            if (speed < 0.01)
            {
                switch (rotation)
                {
                    case Bearing.N:
                        yVelocity = -1;
                        break;
                    case Bearing.NE:
                        yVelocity = -0.3;
                        xVelocity = 0.3;
                        break;
                    case Bearing.E:
                        xVelocity = 0.3;
                        break;
                    case Bearing.SE:
                        yVelocity = 0.3;
                        xVelocity = 0.3;
                        break;
                    case Bearing.S:
                        yVelocity = 0.3;
                        break;
                    case Bearing.SW:
                        yVelocity = 0.3;
                        xVelocity = -0.3;
                        break;
                    case Bearing.W:
                        xVelocity = -0.3;
                        break;
                    case Bearing.NW:
                        yVelocity = -0.3;
                        xVelocity = -0.3;
                        break;
                }
            }
            else
            {
                xVelocity *= (1.03 - speed / 250);
                yVelocity *= (1.03 - speed / 250);
            }
        }

        public void turnright()
        {
            xVelocity = xVelocity - 0.03 * yVelocity;
            yVelocity = yVelocity + 0.02 * xVelocity;
        }

        public void turnleft()
        {
            xVelocity = xVelocity + 0.02 * yVelocity;
            yVelocity = yVelocity - 0.03 * xVelocity;
        }

        public void brake()
        {
            xVelocity *= 0.95;
            yVelocity *= 0.95;
        }
    }
}
