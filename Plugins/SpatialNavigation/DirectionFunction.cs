using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIAssistant.Plugin.SpatialNavigation
{
    class DirectionFunction
    {
        public InDirectionOfFunction.InDirectionOf IsInDirection { get; private set; }
        public DistanceCaluculatorFunction.DistanceCaluculator DistanceCaluculator { get; private set; }

        public DirectionFunction(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    IsInDirection = InDirectionOfFunction.Up;
                    DistanceCaluculator = DistanceCaluculatorFunction.Up;
                    break;
                case Direction.Down:
                    IsInDirection = InDirectionOfFunction.Down;
                    DistanceCaluculator = DistanceCaluculatorFunction.Down;
                    break;
                case Direction.Left:
                    IsInDirection = InDirectionOfFunction.Left;
                    DistanceCaluculator = DistanceCaluculatorFunction.Left;
                    break;
                case Direction.Right:
                default:
                    IsInDirection = InDirectionOfFunction.Right;
                    DistanceCaluculator = DistanceCaluculatorFunction.Right;
                    break;
            }
        }
    }
}
