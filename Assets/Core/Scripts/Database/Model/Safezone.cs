using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Safezone
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Base { get; set; }
    [MaxLength(1000000)]
    public string Objects { get; set; }
    [MaxLength(100)]
    public string Music { get; set; }

    [Indexed]
    public int PatientId { get; set; }
}
