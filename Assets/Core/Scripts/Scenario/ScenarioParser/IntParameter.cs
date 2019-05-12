using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class IntParameter : Parameter
{
    public int Min { get; set; }
    public int Max { get; set; }
    public List<int> IntensityDefault { get; set; }
}

