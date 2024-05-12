using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Positions
{
    public List<CharacterPosition> positions;

    public Positions() {
        positions = new List<CharacterPosition>();
    }
}
