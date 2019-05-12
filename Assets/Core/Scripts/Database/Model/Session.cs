using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Session
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public long StartTime { get; set; }
    public long EndTime { get; set; }

    public string ScenarioName { get; set; }

    [MaxLength(100)]
    public string Video { get; set; }

    [Indexed]
    public int PatientId { get; set; }

    public string GetAbsoluteVideoPath()
    {
        return Path.Combine(Application.persistentDataPath, Video);
    }

}
