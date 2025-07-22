using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorMapper : MonoBehaviour
{
    public List<Mapper> mapperList;
}

[Serializable]
public class Mapper
{
    public Color top, bottom;
}