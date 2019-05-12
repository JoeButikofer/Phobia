using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SQLite4Unity3d;

public class DataService
{
    private static DataService instance;
    public static DataService Instance
    {
        get
        {
            if (instance == null)
                instance = new DataService();

            return instance;
        }

    }


    private SQLiteConnection _connection;

    private static string DATABASE_NAME = "PhobiaDB.db";

    public DataService()
    {

        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DATABASE_NAME);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->
	        var loadDb = Application.dataPath + "/StreamingAssets/" + DATABASE_NAME;
	        // then save to Application.persistentDataPath
	        File.Copy(loadDb, filepath);
            Debug.Log("Database written");
        }
        var dbPath = filepath;
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _connection.Execute("PRAGMA foreign_keys = ON;"); // Enable foreign keys support
        Debug.Log("Database final PATH: " + dbPath);

    }

    public IEnumerable<Patient> GetPatients()
    {
        return _connection.Table<Patient>();
    }

    public Patient CreatePatient(string name, string surname)
    {
        var p = new Patient
        {
            Name = name,
            Surname = surname
        };
        _connection.Insert(p);

        //Create default safezone (a patient MUST have a safezone)
        var sz = new Safezone
        {
            Base = "sz_empty",
            Objects = "",
            Music = "",
            PatientId = p.Id
        };

        _connection.Insert(sz);
        return p;
    }

    public Patient GetPatient(int id)
    {
        return _connection.Table<Patient>().Where(x => x.Id == id).First();
    }

    public void UpdatePatient(Patient patient)
    {
        _connection.Update(patient);
    }

    public void DeletePatient(int id)
    {
        var session = GetPatientSessions(id).GetEnumerator();
        session.MoveNext();
        if (session.Current != null)
        {
            string patientSessionsFolder = Directory.GetParent(session.Current.GetAbsoluteVideoPath()).Parent.FullName;
            //Delete session captures folder
            Directory.Delete(patientSessionsFolder, true);

            var note = GetSessionNotes(session.Current.Id).GetEnumerator();
            note.MoveNext();

            if (note.Current != null)
            {
                string patientNotesFolder = Directory.GetParent(note.Current.GetAbsoluteDataPath()).Parent.FullName;
                //Delete notes folder
                Directory.Delete(patientNotesFolder, true);
            }
        }

        _connection.Delete(GetPatient(id)); // Foreign Key Constraints will take care of deleting every information about the patient in the database
    }

    public Safezone GetPatientSafeZone(int patientId)
    {
        return _connection.Table<Safezone>().Where(x => x.PatientId == patientId).First();
    }

    public Safezone GetPatientSafeZone(Patient patient)
    {
        return GetPatientSafeZone(patient.Id);
    }

    public IEnumerable<Session> GetPatientSessions(int patientId)
    {
        return _connection.Table<Session>().Where(x => x.PatientId == patientId);
    }

    public IEnumerable<Session> GetPatientSessions(Patient patient)
    {
        return GetPatientSessions(patient.Id);
    }

    public void CreateSession(Session s)
    {
        _connection.Insert(s);
    }

    public Session GetSession(int id)
    {
        return _connection.Table<Session>().Where(x => x.Id == id).First();
    }

    public void DeleteSession(int id)
    {
        var note = GetSessionNotes(id).GetEnumerator();
        note.MoveNext();

        if (note.Current != null)
        {
            string noteSessionFolder = Directory.GetParent(note.Current.GetAbsoluteDataPath()).FullName;
            //Delete notes folder
            Directory.Delete(noteSessionFolder, true);
        }

        string captureSessionFolder = Directory.GetParent(GetSession(id).GetAbsoluteVideoPath()).FullName;
        //Delete capture
        Directory.Delete(captureSessionFolder, true);

        _connection.Delete(GetSession(id));
    }

    public IEnumerable<Note> GetSessionNotes(int sessionId)
    {
        return _connection.Table<Note>().Where(x => x.SessionId == sessionId);
    }

    public void CreateNote(Note note)
    {
        _connection.Insert(note);
    }

    public void UpdateSession(Session s)
    {
        _connection.Update(s);
    }

    public void UpdateSafezone(Safezone sz)
    {
        _connection.Update(sz);
    }
}
