using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A Collection that Caches the result of expensive operations
/// </summary>
/// <typeparam name="TInput"> The Input of the Cached function </typeparam>
/// <typeparam name="TResult"> The Return type of the Cached function </typeparam>
public class LookupCache<TInput, TResult>
{
	private Dictionary<TInput, TResult> _values = new Dictionary<TInput, TResult>();
	private Func<TInput, TResult> _process;

	private int _capacity;

	public LookupCache(Func<TInput, TResult> process, int capacity = 100)
	{
		_process = process;
		_capacity = capacity;
	}
	public TResult Get(TInput key)
	{
		if (!_values.ContainsKey(key)) {
			_values[key] = _process(key);
			if (_values.Count > _capacity)
				_values.Remove(_values.First().Key);
		}
		return _values[key];
	}

}