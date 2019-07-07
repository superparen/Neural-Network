using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarGame.ThreeModel
{
    public class Vertex<T>
    {
        public T Source { get; set; }
        public List<Edge<T>> Edges { get; set; }

        public Vertex(T element)
        {
            Source = element;
            Edges = new List<Edge<T>>();
        }
    }
}
