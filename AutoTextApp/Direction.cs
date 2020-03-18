using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTextApp
{
    public interface IDirection
    {
        DirectionType GetDirection(bool isIncreasePostive, DirectionType direction);

        string GetDirectionText(DirectionType direction);
    }

    public class Direction : IDirection
    {
        private Synonyms _synonyms;
        private Random _random = new Random(381654729);

        public Direction(Synonyms synonyms)
        {
            _synonyms = synonyms;
        }

        public DirectionType GetDirection(bool isIncreasePostive, DirectionType direction)
        {
            if (direction != DirectionType.FLAT && !isIncreasePostive)
            {
                if (direction == DirectionType.NEGATIVE)
                {
                    return DirectionType.POSITIVE;
                }
                return DirectionType.NEGATIVE;
            }

            return direction;
        }

        public string GetDirectionText(DirectionType direction)
        {
            if (direction == DirectionType.FLAT)
            {
                var index = _random.Next(_synonyms.Flat.Length);
                return _synonyms.Flat[index];
            }
            else if (direction == DirectionType.POSITIVE)
            {
                var index = _random.Next(_synonyms.Positive.Length);
                return _synonyms.Positive[index];
            }
            else // direction == DirectionType.NEGATIVE
            {
                var index = _random.Next(_synonyms.Negative.Length);
                return _synonyms.Negative[index];
            }
        }
    }
}
