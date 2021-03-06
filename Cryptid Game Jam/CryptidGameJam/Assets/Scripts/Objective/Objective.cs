﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Objective 
{
    [SerializeField]
    public bool isActive;
    [SerializeField]
    public string objectiveText;

    public Objective(string text)
    {
        objectiveText = text;
        isActive = false;
    }
}
