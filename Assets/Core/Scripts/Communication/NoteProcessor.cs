using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

class NoteProcessor
{
    private string rootNotePath;
    private Session session;
    private string sessionFolderName;

    public NoteProcessor(string rootNotePath, Session session)
    {
        this.rootNotePath = rootNotePath;
        this.session = session;

        sessionFolderName = EpochTools.ConvertEpochToSortableDateTimeString(session.StartTime);
    }

    public void Process(Note note)
    {
        new Thread(() => {

            switch(note.Type)
            {
                case "IMAGE":
                    ProcessImage(note);
                    break;
                case "TEXT":
                    ProcessText(note);
                    break;
            }

        }).Start();
    }

    private void ProcessImage(Note note)
    {
        Debug.Log("Image Received\nDecoding...");
        byte[] data = DecodeImage(note.Data);
        Debug.Log("Decoding complete\nSaving on disk...");
        string filePath = SaveNoteInFileSystem(data, ".png");
        Debug.Log("Saving on disk complete\nSaving on database...");
        SaveNoteInDatabase(note, filePath);
        Debug.Log("Saving on database complete");
    }

    private void ProcessText(Note note)
    {
        Debug.Log("Text Received\nSaving on disk...");
        string filePath = SaveNoteInFileSystem(Encoding.UTF8.GetBytes(note.Data), ".txt");
        Debug.Log("Saving on disk complete\nSaving on database...");
        SaveNoteInDatabase(note, filePath);
        Debug.Log("Saving on database complete");
    }

    private byte[] DecodeImage(string encodedImage)
    {
        return Convert.FromBase64String(encodedImage);
    }

    private String SaveNoteInFileSystem(byte[] data, string extension)
    {
        var patient = DataService.Instance.GetPatient(GlobalVariables.SelectedPatientId);

        string path = "notes";

        if (patient != null)
        {
            path = Path.Combine(path, Path.Combine(patient.Name + patient.Surname + patient.Id, sessionFolderName));
        }
        else
        {
            path = Path.Combine(path, Path.Combine("Default", sessionFolderName));
            Debug.Log("No patient selected, save note to default folder");
        }

        System.IO.Directory.CreateDirectory(Path.Combine(rootNotePath, path));

        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

        string noteName = "note_" + timestamp + extension;

        var finalPath = Path.Combine(rootNotePath, Path.Combine(path, noteName));

        int postfix = 1;
        while(Directory.Exists(finalPath)) //It should not happen but in case, do a check if a nte with this exact same name already exist.
        {
            postfix++;
            noteName = "note_" + timestamp + "_" + postfix + extension;
            finalPath = Path.Combine(rootNotePath, Path.Combine(path, noteName));
        }

        File.WriteAllBytes(finalPath, data);

        return Path.Combine(path, noteName);
    }

    private void SaveNoteInDatabase(Note note, string notePath)
    {
        note.Data = notePath; //Replace the encoded image or text by the path
        note.SessionId = session.Id; //Assign the note to the current session

        DataService.Instance.CreateNote(note);
    }
}

