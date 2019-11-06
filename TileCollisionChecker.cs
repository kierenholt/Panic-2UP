using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panic
{
    //public enum TileCollisionType
    //{
    //    none = 0, projectileLeftTop = 1, projectileRightBottom = 2, straddle = 4
    //}

    public class TileCollisionTrigger<T1>
        where T1: Projectile
    {
        public Action<T1, Tile> action;
        public Func<Tile, bool> condition;

        public TileCollisionTrigger(Action<T1, Tile> paramAction, Func<Tile, bool> paramCondition)
        {
            action = paramAction;
            condition = paramCondition;
        }
    }

    [Flags]
    public enum TileCollisionType
    {
        none = 0, tileLeft = 1, tileRight = 2, tileTop = 4, tileBottom = 8, intruding = 16, extruding = 32, including = 64, excluding = 128
    }

    public class TileCollisionChecker<T1>
        where T1 : Projectile
    {
        public TileCollisionChecker(T1 paramProj, TileGrid paramTileGrid)
        {
            projectile = paramProj;
            tileGrid = paramTileGrid;

            topRow = tileGrid.getRowNum(projectile.topCollision);
            bottomRow = tileGrid.getRowNum(projectile.bottomCollision);
            leftCol = tileGrid.getColNum(projectile.leftCollision);
            rightCol = tileGrid.getColNum(projectile.rightCollision);

            verticalStraddle = topRow != bottomRow;
            horizontalStraddle = leftCol != rightCol;

            triggers = new List<TileCollisionTrigger<T1>>[256];
        }

        private List<TileCollisionTrigger<T1>>[] triggers { get; set; }
        public void addTrigger(TileCollisionType collisiontype,  Action<T1, Tile> action, Func<Tile, bool> condition)
        {
            if (triggers[(int)collisiontype] == null)
                triggers[(int)collisiontype] = new List<TileCollisionTrigger<T1>>();
            triggers[(int)collisiontype].Add(new TileCollisionTrigger<T1>(action, condition));
        }

        public void clearTriggers(TileCollisionType collisiontype)
        {
            triggers[(int)collisiontype] = null;
        }


        private bool verticalStraddle;
        private bool horizontalStraddle;
        //public TileCollisionType verticalResult;
        //public TileCollisionType horizontalResult;

        public int topRow { get; private set; }
        public int bottomRow { get; private set; }
        public int leftCol { get; private set; }
        public int rightCol { get; private set; }

        public T1 projectile { get; private set; }
        public TileGrid tileGrid { get; private set; }


        //public bool update()
        //{
        //    verticalResult = TileCollisionType.none;
        //    horizontalResult = TileCollisionType.none;
        //    return updateOnce();
        //}

        private int[] distinctInts(int a, int b)
        {
            return a != b ? new[] { a, b } : new[] { a };
        }

        public void update()
        {
            if (projectile.topCollision < tileGrid.topCollision || projectile.bottomCollision > tileGrid.bottomCollision ||
                projectile.leftCollision < tileGrid.leftCollision || projectile.rightCollision > tileGrid.rightCollision)
            {
            }
            else
            {
                //vertical
                if (topRow != bottomRow) //straddle => exclusion
                {
                    if (projectile.topCollision > tileGrid.rowBottoms[topRow])
                    {
                        if (triggers[(int)(TileCollisionType.tileBottom | TileCollisionType.excluding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileBottom | TileCollisionType.excluding)].ToArray())
                                foreach (int c in distinctInts(leftCol, rightCol))
                                    if (trigger.condition(tileGrid.tiles[c, topRow]))
                                        trigger.action(projectile, tileGrid.tiles[c, topRow]);
                        topRow++;
                    }
                    if (projectile.bottomCollision < tileGrid.rowTops[bottomRow])
                    {
                        if (triggers[(int)(TileCollisionType.tileTop | TileCollisionType.excluding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileTop | TileCollisionType.excluding)].ToArray())
                                foreach (int c in distinctInts(leftCol, rightCol))
                                    if (trigger.condition(tileGrid.tiles[c, bottomRow]))
                                        trigger.action(projectile, tileGrid.tiles[c, bottomRow]);
                        bottomRow--;
                    }
                }
                else //no straddle => extrusion
                {
                    if (projectile.topCollision < tileGrid.rowTops[topRow])
                    {
                        if (triggers[(int)(TileCollisionType.tileTop | TileCollisionType.extruding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileTop | TileCollisionType.extruding)].ToArray())
                                foreach (int c in distinctInts(leftCol, rightCol))
                                    if (trigger.condition(tileGrid.tiles[c, topRow]))
                                        trigger.action(projectile, tileGrid.tiles[c, topRow]);
                        topRow--;
                    }
                    if (projectile.bottomCollision > tileGrid.rowBottoms[bottomRow])
                    {
                        if (triggers[(int)(TileCollisionType.tileBottom | TileCollisionType.extruding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileBottom | TileCollisionType.extruding)].ToArray())
                                foreach (int c in distinctInts(leftCol, rightCol))
                                    if (trigger.condition(tileGrid.tiles[c, bottomRow]))
                                        trigger.action(projectile, tileGrid.tiles[c, bottomRow]);
                        bottomRow++;
                    }
                }


                if (leftCol != rightCol) //straddle => ecxlusion
                {
                    if (projectile.leftCollision > tileGrid.colRights[leftCol])
                    {
                        if (triggers[(int)(TileCollisionType.tileRight | TileCollisionType.excluding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileRight | TileCollisionType.excluding)].ToArray())
                                foreach (int r in distinctInts(topRow, bottomRow))
                                    if (trigger.condition(tileGrid.tiles[leftCol, r]))
                                        trigger.action(projectile, tileGrid.tiles[leftCol, r]);
                        leftCol++;
                    }
                    if (projectile.rightCollision < tileGrid.colLefts[rightCol])
                    {
                        if (triggers[(int)(TileCollisionType.tileLeft | TileCollisionType.excluding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileLeft | TileCollisionType.excluding)].ToArray())
                                foreach (int r in distinctInts(topRow, bottomRow))
                                    if (trigger.condition(tileGrid.tiles[rightCol, r]))
                                        trigger.action(projectile, tileGrid.tiles[rightCol, r]);
                        rightCol--;
                    }
                }
                else //no straddle => extrusion
                {
                    if (projectile.leftCollision < tileGrid.colLefts[leftCol])
                    {
                        if (triggers[(int)(TileCollisionType.tileLeft | TileCollisionType.extruding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileLeft | TileCollisionType.extruding)].ToArray())
                                foreach (int r in distinctInts(topRow, bottomRow))
                                    if (trigger.condition(tileGrid.tiles[leftCol, r]))
                                        trigger.action(projectile, tileGrid.tiles[leftCol, r]);
                        leftCol--;
                    }
                    if (projectile.rightCollision > tileGrid.colRights[rightCol])
                    {
                        if (triggers[(int)(TileCollisionType.tileRight | TileCollisionType.extruding)] != null)
                            foreach (TileCollisionTrigger<T1> trigger in triggers[(int)(TileCollisionType.tileRight | TileCollisionType.extruding)].ToArray())
                                foreach (int r in distinctInts(topRow, bottomRow))
                                    if (trigger.condition(tileGrid.tiles[rightCol, r]))
                                        trigger.action(projectile, tileGrid.tiles[rightCol, r]);
                        rightCol++;
                    }
                }
            }

            //return collisionHappened;
        }

    }
}
