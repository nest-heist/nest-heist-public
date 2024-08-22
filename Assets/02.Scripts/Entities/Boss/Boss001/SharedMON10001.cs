using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedMON10001 : SharedVariable<MON10001>
{
    public static implicit operator SharedMON10001(MON10001 value) {return new SharedMON10001 { Value = value };}
}
