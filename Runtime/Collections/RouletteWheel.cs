using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// List base Collection that posesses a Get() function to obtain a random item based on a Weight value
/// </summary>
/// <typeparam name="T"></typeparam>
public class RouletteWheel<T>
{
    List<(T, float)> _options = new List<(T, float)>();
    private float _maxWeight;

    public RouletteWheel(params (T, float)[] options)
    {
        _options = options.ToList();

        _maxWeight = 0;
        foreach (var opt in _options) {
            _maxWeight += opt.Item2;
        }
    }
    public RouletteWheel(params T[] options)
    {
        for (int i = 0; i < options.Length; i++) {
            _options[i] = (options[i], 1.0f);
        }
        _maxWeight = 0;
        foreach (var opt in _options)
        {
            _maxWeight += opt.Item2;
        }
    }
    public RouletteWheel()
    {
        _maxWeight = 0;
    }
    public T Get()
    {
        float n = Random.Range(0,_maxWeight);

        float count = 0;
        foreach (var item in _options)
        {
            count += item.Item2;
            if (count > n)
                return item.Item1;
        }
        throw new Exception("Something Happened...");
    }
    public void Add(T item, float weight)
    {
        _options.Add((item, weight));
        _maxWeight += weight;
    }
    public void Add((T, float) item)
    {
        _options.Add(item);
        _maxWeight += item.Item2;
    }
    public void RemoveAt(int index)
    {
        _maxWeight -= _options[index].Item2;
        _options.RemoveAt(index);
    }
    public void Remove(T item)
    {
        for (int i = 0; i < _options.Count; i++) {
            var o = _options[i];
            if (o.Equals(item)) {
                float w = o.Item2;
                _maxWeight -= w;
                _options.RemoveAt(i);
            }
        }
    }
    public void AddRange((T, float)[] items_weight)
    {
        foreach (var item in items_weight)
        {
            _maxWeight += item.Item2;
        }
        _options.AddRange(items_weight);
    }
    public void Clear()
    {
        _maxWeight = 0;
        _options.Clear();
    }
    //private class ExampleR
    //{
    //    private void Test()
    //    {
    //        RouletteWheel<Action> _actions = new RouletteWheel<Action>(
    //            (() => { }, 5),
    //            (() => { }, 5)
    //            );
    //
    //    }
    //
    //}
}


