using System;
using System.Collections.Generic;
using UnityEngine;

public static class ThethaStar<T>
{
    //public delegate bool DoesSatisfy(T current); //Es para preguntar si este nodo te sirve
    //public delegate List<T> GetNeighbours(T current); //El diccionario que vamos a usar para obtener nodos y valores
    //public delegate float GetCost(T father, T child); //Obtener el value segun padre-hijo
    //public delegate float Heuristic(T current); //Variable en base a nuestro nodo que queremos que modifique (es lo que hace el AStar sea eso)
    //public delegate bool CanSkip(T child, T grandparent);

    public static List<T> Run(
        T start,
        Func<T, bool> satisfy,
        Func<T, List<T>> neighbours,
        Func<T, T, float> getCost,
        Func<T, float> heuristic,
        Func<T, T, bool> canSkip,
        int antiLoop = 100
        )
    {
        Dictionary<T, float> cost = new Dictionary<T, float>(); //Valores de los nodos
        Dictionary<T, T> parents = new Dictionary<T, T>(); //Relación entre padres hijos
        PriorityQueue<T> pending = new PriorityQueue<T>(); //Nodos pendientes
        HashSet<T> visited = new HashSet<T>(); //Los nodos que ya se visitaron

        pending.Enqueue(start, 0); //El nodo de start sin valor, lo ponemos en pending 

        cost.Add(start, 0); //Agregamos el start con valor 0 porque ya estamos ahí

        while (!pending.IsEmpty)
        {
            T current = pending.Dequeue();

            #region DONG
            antiLoop--;  //Esto es el DONG, watch mine
            if (antiLoop <= 0) return new List<T>();
            #endregion

            if (satisfy(current))
            {
                //Debug.Log($"Found a path with {parents.Count} nodes");
                return GetPath(current, parents);
            }

            visited.Add(current);

            List<T> localNeighbours = neighbours(current);

            for (int i = 0; i < localNeighbours.Count; i++)
            {

            }


            //Debug.Log($"Checked Neighbours {localNeighbours.Count}");
            foreach (var item in localNeighbours)
            {
                T node = item;
                if (visited.Contains(node)) 
                    continue;


                T currentParent;
                if (parents.ContainsKey(current) && canSkip(parents[current], node))
                {
                    currentParent = parents[current];
                }
                else {
                    currentParent = current;
                }

                float nodeValue = getCost.Invoke(currentParent, node);
                float totalValue = cost[currentParent] + nodeValue; //La suma de todos los values
                if (cost.ContainsKey(node) && cost[node] < totalValue) 
                    continue;

                cost[node] = totalValue;
                parents[node] = currentParent;
                pending.Enqueue(node, totalValue + heuristic(node));
            }
        }
        return new List<T>();
    }

    private static List<T> GetPath(T end, Dictionary<T, T> parents) //Generar el path
    {
        //Debug.Log("Path Obtained");

        var path = new List<T>();
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