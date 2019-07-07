using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarGame.ThreeModel
{
    class RectangleCoor
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public RectangleCoor(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
    }
}
