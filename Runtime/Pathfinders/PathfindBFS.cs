using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathfindBFS<T>
{

    //public delegate bool Satisfies(T curr);

    //public delegate List<T> GetNeighbours(T curr);

    public static List<T> Run(T start, Func<T, bool> satisfies, Func<T, List<T>> getNeighbours, int watchDong = 500)
    {
        Dictionary<T, T> parents = new Dictionary<T, T>();
        Queue<T> pending = new Queue<T>();
        HashSet<T> visited = new HashSet<T>();

        //Debug.Log("PathfindBFS Start");

        pending.Enqueue(start);
        int counter = 0;
        while (pending.Any()) {
            counter++;
            if (counter > watchDong)
                return null;

            T current = pending.Dequeue();
            if (satisfies(current)) {
                //Debug.Log("Path Found!");
                return ConstructPath(current, parents);
            }
            visited.Add(current);
            List<T> neighbours = getNeighbours(current);

            foreach (var item in neighbours)
            {
                if (visited.Contains(item) || pending.Contains(item)) 
                    continue;

                pending.Enqueue(item);

                parents[item] = current;
            }

        }
        return null;
    }


    private static List<T> ConstructPath(T end, Dictionary<T,T> parents)
    {
        List<T> path = new List<T>();
        path.Add(end);

        while (parents.ContainsKey(path[path.Count - 1]))
        {
            var lastNode = path[path.Count - 1];
            path.Add(parents[lastNode]);
        }
        path.Reverse();

        return path;
    }
}
