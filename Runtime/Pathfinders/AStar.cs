using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar<T>
{
    public delegate bool DoesSatisfy(T current); //Es para preguntar si este nodo te sirve
    public delegate Dictionary<T, float> GetNeighbours(T current); //El diccionario que vamos a usar para obtener nodos y valores
    public delegate float GetValue(T father, T child); //Obtener el value segun padre-hijo
    
    public delegate float Heuristic(T current); //Variable en base a nuestro nodo que queremos que modifique (es lo que hace el AStar sea eso)
    
    public static List<T> Run(
        T start,
        Predicate<T> satisfy,
        Func<T, Dictionary<T,float>> neighbours,
        Func<T, float> heuristic,
        int antiLoop = 100)
    {
        Dictionary<T, float> value = new Dictionary<T, float>(); //Valores de los nodos
        Dictionary<T, T> parents = new Dictionary<T, T>(); //Relación entre padres hijos
        PriorityQueue<T> pending = new PriorityQueue<T>(); //Nodos pendientes
        HashSet<T> visited = new HashSet<T>(); //Los nodos que ya se visitaron
        
        pending.Enqueue(start, 0); //El nodo de start sin valor, lo ponemos en pending 
        
        value.Add(start, 0); //Agregamos el start con valor 0 porque ya estamos ahí
        
        while (!pending.IsEmpty)
        {
            T current = pending.Dequeue();

            #region DONG
            antiLoop--;  //Esto es el DONG, watch mine
            if (antiLoop <= 0) return new List<T>();
            #endregion

            if (satisfy(current))
            {
               return GetPath(current, parents);
            }
            
            visited.Add(current);
            
            Dictionary<T, float> localNeighbours = neighbours(current);
            
            foreach (var item in localNeighbours)
            {
                T node = item.Key;
                if (visited.Contains(node)) continue;
                float nodeValue = item.Value;
                float totalValue = value[current] + nodeValue; //La suma de todos los values
                if (value.ContainsKey(node) && value[node] < totalValue) continue;
                value[node] = totalValue;
                parents[node] = current;
                pending.Enqueue(node, totalValue + heuristic(node));
            }
        }
        return new List<T>();
    }
    
    static List<T> GetPath(T end, Dictionary<T, T> parents) //Generar el path
    {
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
