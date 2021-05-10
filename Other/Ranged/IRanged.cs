using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IRanged<T>
{
    public T MinimalValue { get; set; }

    public T MaximalValue { get; set; }

    public T RandomValue { get; }
}
