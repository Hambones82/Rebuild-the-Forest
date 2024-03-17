using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int xIn, int yIn)
        {
            this.x = xIn;
            this.y = yIn;
        }

        public List<Vector2Int> GetNeighborsInBounds(int width, int height)
        {
            if(x < 0 || y < 0 || x >= width || y >= height)
            {
                throw new ArgumentOutOfRangeException($"{ToString()} is out of range of 0-{width}, 0-{height}");
            }
            List<Vector2Int> retval = new List<Vector2Int>();
            if (x > 0) retval.Add(new Vector2Int(x - 1, y));
            if (y > 0) retval.Add(new Vector2Int(x, y - 1));
            if (x < width - 1) retval.Add(new Vector2Int(x + 1, y));
            if (y < height - 1) retval.Add(new Vector2Int(x, y + 1));
            return retval;
        }

        public override string ToString()
        {
            return $"x:{x}, y:{y}";
        }
    }


}
