using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class FormationListChangedEventData : EventBase
{
    public static readonly Type eventType = typeof(FormationListChangedEventData);

    public override Type EventType
    {
        get
        {
            return eventType;
        }
    }

    public override void Clear()
    {
    }
}
