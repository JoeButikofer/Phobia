using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScenarioDetail
{
    public string Name { get; set; }
    public string Phobia { get; set; }
    public string Description { get; set; }

    public int MinIntensityLevel { get; set; }
    public int MaxIntensityLevel { get; set; }

    public List<Parameter> Parameters { get; set; }
}
