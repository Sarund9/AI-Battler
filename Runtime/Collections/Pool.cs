using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pool<T>
{
    public delegate T FactoryMethod();
    private FactoryMethod _factoryMethod;

    private bool _isDynamic;
    private List<T> _quantity;
    private Action<T> _turnOn, _turnOff;

    //Primeros 2 te los da el Spawner, los 2dos 2 el Spawneado a través del Spawner
    public Pool(FactoryMethod factoryMethod, Action<T> turnOn, Action<T> turnOff, int initialStock = 0, bool isDynamic = true)
    {
        _factoryMethod = factoryMethod;
        _isDynamic = isDynamic;

        _turnOn = turnOn;
        _turnOff = turnOff;

        _quantity = new List<T>();

        for (int i = 0; i < initialStock; i++)
        {
            var o = _factoryMethod();
            _turnOff(o);
            _quantity.Add(o);
        }
    }
    
    //Al nacer
    public T SendFromPool()
    {
        var result = default(T);
        if (_quantity.Count > 0)
        {
            result = _quantity[0];
            _quantity.RemoveAt(0);
        }
        else if (_isDynamic)
            result = _factoryMethod();
        _turnOn(result);
        return result;
    }

    //Al morir
    public void ReturnToPool(T o)
    {
        _turnOff(o);
        _quantity.Add(o);
    }
}
