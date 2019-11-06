using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panic
{
    [Flags]
    public enum CollisionType
    {
        none = 0, leftTop = 1, rightBottom = 2, intruding = 4, extruding = 8, including = 16, excluding = 32
    }

    public enum CollisionSide
    {
        none, left, right, top, bottom
    }


    public class CollisionChecker<T1, T2> 
        where T1: Box
        where T2: Projectile 
    {
        enum edgeOrder
        {
            ppbb, pbpb, bppb, pbbp, bpbp, bbpp
        }

        public CollisionChecker(
            T1 paramBox,
            T2 paramProjectile)
        {
            vertical = edgeOrder.ppbb;
            horizontal = edgeOrder.ppbb;
            _box = paramBox;
            _proj = paramProjectile;
            verticalResult = CollisionType.none;
            horizontalResult = CollisionType.none;

            bool dummy = false;
            while (updateOnce(ref dummy)) { }//sets vertical and horizontal to correct edgeOrder;

        }

        private edgeOrder vertical;
        private edgeOrder horizontal;
        public CollisionType verticalResult;
        public CollisionType horizontalResult;
        private T1 _box;
        private T2 _proj;
        public T1 box 
        { 
            get { return _box; }
            set
            {
                _box = value;
                bool dummy = false;
                while (updateOnce(ref dummy)) { } //sets vertical and horizontal to correct edgeOrder;
            }
        }
        public T2 proj
        {
            get { return _proj; }
            set 
            {
                _proj = value; 
                bool dummy = false;
                while (updateOnce(ref dummy)) { } //sets vertical and horizontal to correct edgeOrder;
            }
        }

        public bool VorHasFlag(CollisionType flag)
        {
            return horizontalResult.HasFlag(flag) || verticalResult.HasFlag(flag);
        }


        public CollisionSide side
        {
            get
            {
                if (vertical.HasFlag(CollisionType.leftTop)) return CollisionSide.top;
                if (vertical.HasFlag(CollisionType.rightBottom)) return CollisionSide.bottom;
                if (horizontal.HasFlag(CollisionType.leftTop)) return CollisionSide.left;
                if (horizontal.HasFlag(CollisionType.rightBottom)) return CollisionSide.right;
                return CollisionSide.none;
            }
        }

        public bool update()
        {
            verticalResult = CollisionType.none;
            horizontalResult = CollisionType.none;
            bool collisionHappened = false;
            while (updateOnce(ref collisionHappened)) { }
            return collisionHappened;
        }

        private bool updateOnce(ref bool collisionHappened)
        {
            bool verticalChange = false;
            bool outsideVertical = false;
            bool horizontalChange = false;
            bool outsideHorizontal = false;
            
            //check only for changes
            switch (horizontal) 
            {
                case (edgeOrder.ppbb):
                {
                    if (_proj.rightCollision > _box.leftCollision)
                    {
                        horizontal = edgeOrder.pbpb;
                        horizontalResult |= CollisionType.leftTop | CollisionType.intruding;
                        horizontalChange = true;
                    }
                    outsideHorizontal = true;
                    break;
                }
                case (edgeOrder.pbpb):
                {
                    if (_proj.rightCollision < _box.leftCollision)
                    {
                        horizontal = edgeOrder.ppbb;
                        horizontalResult |= CollisionType.leftTop | CollisionType.excluding;
                        horizontalChange = true;
                    }
                    if (_proj.leftCollision > _box.leftCollision)
                    {
                        horizontal = edgeOrder.bppb;
                        horizontalResult |= CollisionType.leftTop | CollisionType.including;
                        horizontalChange = true;
                    }
                    if (_proj.rightCollision > _box.rightCollision)
                    {
                        horizontal = edgeOrder.pbbp;
                        horizontalResult |= CollisionType.rightBottom | CollisionType.extruding;
                        horizontalChange = true;
                    }
                    break;
                }
                case (edgeOrder.bppb):
                {
                    if (_proj.leftCollision < _box.leftCollision)
                    {
                        horizontal = edgeOrder.pbpb;
                        horizontalResult |= CollisionType.leftTop | CollisionType.extruding;
                        horizontalChange = true;
                    }
                    if (_proj.rightCollision > _box.rightCollision)
                    {
                        horizontal = edgeOrder.bpbp;
                        horizontalResult |= CollisionType.rightBottom | CollisionType.extruding;
                        horizontalChange = true;
                    }
                    break;
                }
                case (edgeOrder.pbbp):
                {
                    if (_proj.rightCollision < _box.rightCollision)
                    {
                        horizontal = edgeOrder.pbpb;
                        horizontalResult |= CollisionType.rightBottom | CollisionType.including;
                        horizontalChange = true;
                    }
                    if (_proj.leftCollision > _box.leftCollision)
                    {
                        horizontal = edgeOrder.bpbp;
                        horizontalResult |= CollisionType.leftTop | CollisionType.including;
                        horizontalChange = true;
                    }
                    break;
                }
                case (edgeOrder.bpbp):
                {
                    if (_proj.rightCollision < _box.rightCollision)
                    {
                        horizontal = edgeOrder.bppb;
                        horizontalResult |= CollisionType.rightBottom | CollisionType.including;
                        horizontalChange = true;
                    }
                    if (_proj.leftCollision < _box.leftCollision)
                    {
                        horizontal = edgeOrder.pbbp;
                        horizontalResult |= CollisionType.leftTop | CollisionType.extruding;
                        horizontalChange = true;
                    }
                    if (_proj.leftCollision > _box.rightCollision)
                    {
                        horizontal = edgeOrder.bbpp;
                        horizontalResult |= CollisionType.rightBottom | CollisionType.excluding;
                        horizontalChange = true;
                    }
                    break;
                }
                case (edgeOrder.bbpp):
                {
                    if (_proj.leftCollision < _box.rightCollision)
                    {
                        horizontal = edgeOrder.bpbp;
                        horizontalResult |= CollisionType.rightBottom | CollisionType.intruding;
                        horizontalChange = true;
                    }
                    outsideHorizontal = true;
                    break;
                }
            } //end select


            switch (vertical)
            {
                case (edgeOrder.ppbb):
                    {
                        if (_proj.bottomCollision > _box.topCollision)
                        {
                            vertical = edgeOrder.pbpb;
                            verticalResult |= CollisionType.leftTop | CollisionType.intruding;
                            verticalChange = true;
                        }
                        outsideVertical = true;
                        break;
                    }
                case (edgeOrder.pbpb):
                    {
                        if (_proj.bottomCollision < _box.topCollision)
                        {
                            vertical = edgeOrder.ppbb;
                            verticalResult |= CollisionType.leftTop | CollisionType.excluding;
                            verticalChange = true;
                        }
                        if (_proj.topCollision > _box.topCollision)
                        {
                            vertical = edgeOrder.bppb;
                            verticalResult |= CollisionType.leftTop | CollisionType.including;
                            verticalChange = true;
                        }
                        if (_proj.bottomCollision > _box.bottomCollision)
                        {
                            vertical = edgeOrder.pbbp;
                            verticalResult |= CollisionType.rightBottom | CollisionType.extruding;
                            verticalChange = true;
                        }
                        break;
                    }
                case (edgeOrder.bppb):
                    {
                        if (_proj.topCollision < _box.topCollision)
                        {
                            vertical = edgeOrder.pbpb;
                            verticalResult |= CollisionType.leftTop | CollisionType.extruding;
                            verticalChange = true;
                        }
                        if (_proj.bottomCollision > _box.bottomCollision)
                        {
                            vertical = edgeOrder.bpbp;
                            verticalResult |= CollisionType.rightBottom | CollisionType.extruding;
                            verticalChange = true;
                        }
                        break;
                    }
                case (edgeOrder.pbbp):
                    {
                        if (_proj.bottomCollision < _box.bottomCollision)
                        {
                            vertical = edgeOrder.pbpb;
                            verticalResult |= CollisionType.rightBottom | CollisionType.including;
                            verticalChange = true;
                        }
                        if (_proj.topCollision > _box.topCollision)
                        {
                            vertical = edgeOrder.bpbp;
                            verticalResult |= CollisionType.leftTop | CollisionType.including;
                            verticalChange = true;
                        }
                        break;
                    }
                case (edgeOrder.bpbp):
                    {
                        if (_proj.bottomCollision < _box.bottomCollision)
                        {
                            vertical = edgeOrder.bppb;
                            verticalResult |= CollisionType.rightBottom | CollisionType.including;
                            verticalChange = true;
                        }
                        if (_proj.topCollision < _box.topCollision)
                        {
                            vertical = edgeOrder.pbbp;
                            verticalResult |= CollisionType.leftTop | CollisionType.extruding;
                            verticalChange = true;
                        }
                        if (_proj.topCollision > _box.bottomCollision)
                        {
                            vertical = edgeOrder.bbpp;
                            verticalResult |= CollisionType.rightBottom | CollisionType.excluding;
                            verticalChange = true;
                        }
                        break;
                    }
                case (edgeOrder.bbpp):
                    {
                        if (_proj.topCollision < _box.bottomCollision)
                        {
                            vertical = edgeOrder.bpbp;
                            verticalResult |= CollisionType.rightBottom | CollisionType.intruding;
                            verticalChange = true;
                        }
                        outsideVertical = true;
                        break;
                    }
            } //end select

            collisionHappened |= 
                (
                    (verticalChange & !outsideHorizontal) || (horizontalChange && !outsideVertical)
                );
            return verticalChange || horizontalChange;
        }


        public void reboundKeepInside()
        {
            if (verticalResult.HasFlag(CollisionType.extruding))
                _proj.yVelocity *= -1;
            if (horizontalResult.HasFlag(CollisionType.extruding))
                _proj.xVelocity *= -1;
        }


        public void reboundKeepOutside()
        {
            if (verticalResult.HasFlag(CollisionType.intruding))
                _proj.yVelocity *= -1;
            if (horizontalResult.HasFlag(CollisionType.intruding))
                _proj.xVelocity *= -1;
        }

    }

    public static class collisionCheckerExtensions
    {

        public static void reboundEqualMasses(this CollisionChecker<RacePlayer, RacePlayer> checker)
        {
            double xCOMvelocity = (checker.proj.xVelocity - checker.box.xVelocity);
            double yCOMvelocity = (checker.proj.yVelocity - checker.box.yVelocity);
            if (checker.horizontalResult.HasFlag(CollisionType.intruding)) { checker.proj.xVelocity -= xCOMvelocity; checker.box.xVelocity += xCOMvelocity; }
            if (checker.verticalResult.HasFlag(CollisionType.intruding)) { checker.proj.yVelocity -= yCOMvelocity; checker.box.yVelocity += yCOMvelocity; }
        }

        public static void reboundEqualMasses(this CollisionChecker<Player, Player> checker)
        {
            double xCOMvelocity = (checker.proj.xVelocity - checker.box.xVelocity);
            double yCOMvelocity = (checker.proj.yVelocity - checker.box.yVelocity);
            if (checker.horizontalResult.HasFlag(CollisionType.intruding)) { checker.proj.xVelocity -= xCOMvelocity; checker.box.xVelocity += xCOMvelocity; }
            if (checker.verticalResult.HasFlag(CollisionType.intruding)) { checker.proj.yVelocity -= yCOMvelocity; checker.box.yVelocity += yCOMvelocity; }
        }
    }





}
