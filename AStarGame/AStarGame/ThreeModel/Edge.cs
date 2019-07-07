using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarGame.ThreeModel
{
    public class Edge<T>
    {
        public int Length { get; set; }
        public Vertex<T> Element { get; set; }

        public Edge(Vertex<T> element, int length = 1)
        {
            Element = element;
            Length = length;
        }
    }
}
