using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarGame.ThreeModel
{
    public class Three<T>
    {
        public Vertex<T> Top { get; set; }

        public Three(Vertex<T> top)
        {
            Top = top;
        }

        public void OutAllElement(Vertex<T> point, List<Vertex<T>> lastList)
        {
            Console.WriteLine(point.Source);

            foreach (var child in point.Edges)
            {
                if (!lastList.Contains(child.Element))
                {
                    lastList.Add(child.Element);
                    OutAllElement(child.Element, lastList);
                }
            }
        }

        public Dictionary<Vertex<T>, double> AStar(Vertex<T> from, Vertex<T> to, Func<T, T, double> dist)
        {
            Dictionary<Vertex<T>, double> open = new Dictionary<Vertex<T>, double>(),
                closed = new Dictionary<Vertex<T>, double>();
            open.Add(from, 0);

            while (open.Count > 0)
            {
                var newVert = open.OrderBy(el => el.Value + dist(to.Source, el.Key.Source)).First();
                open.Remove(newVert.Key);
                if (closed.ContainsKey(newVert.Key))
                {
                    if (closed[newVert.Key] > newVert.Value)
                        closed[newVert.Key] = newVert.Value;
                }
                else
                    closed.Add(newVert.Key, newVert.Value);

                if (newVert.Key == to)
                    continue;

                foreach (var ch in newVert.Key.Edges)
                {
                    if (!closed.ContainsKey(ch.Element) || closed[ch.Element] > newVert.Value + ch.Length)
                    {
                        if (open.ContainsKey(ch.Element))
                        {
                            if (open[ch.Element] > newVert.Value + ch.Length)
                                open[ch.Element] = newVert.Value + ch.Length;
                        }
                        else
                            open.Add(ch.Element, newVert.Value + ch.Length);
                    }
                }

            }

            if (!closed.ContainsKey(to))
                return null;
            return closed;
        }
    }
}
