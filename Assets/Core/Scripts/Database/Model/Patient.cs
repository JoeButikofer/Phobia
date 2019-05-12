using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Patient
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public override string ToString()
    {
        return string.Format("[Patient: Id={0}, Name={1},  Surname={2}]", Id, Name, Surname);
    }
}
