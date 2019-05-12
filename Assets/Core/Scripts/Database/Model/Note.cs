using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Type { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    [MaxLength(100)]
    public string Data { get; set; }
    [Indexed]
    public int SessionId { get; set; }

    public string GetAbsoluteDataPath()
    {
        return Path.Combine(Application.persistentDataPath, Data);
    }
}

