using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FloatParameter : Parameter
{
    public float Min { get; set; }
    public float Max { get; set; }
    public List<float> IntensityDefault { get; set; }
}
