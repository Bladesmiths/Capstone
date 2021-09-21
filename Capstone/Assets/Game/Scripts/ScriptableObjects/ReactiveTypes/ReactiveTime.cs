using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Source: https://answers.unity.com/questions/1234096/is-it-possible-to-pick-reference-variables-from-on.html
/// </summary>

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "ReactiveTime", menuName = "ScriptableObjects/ReactiveTypes/ReactiveTime")]
    public class ReactiveTime : ReactiveType<TimeSpan>
    {

    }
}

