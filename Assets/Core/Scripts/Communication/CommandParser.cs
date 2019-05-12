using System.Collections;
using SimpleJSON;
using UnityEngine;
using System;

public delegate void PlayReceived();
public delegate void PauseReceived();
public delegate void ResetReceived();
public delegate void LoadSafezoneReceived();
public delegate void LoadScenarioReceived();

public delegate void NoteReceived(Note note);

public class CommandParser
{
    public event PlayReceived playReceived;
    public event PauseReceived pauseReceived;
    public event ResetReceived resetReceived;
    public event LoadSafezoneReceived loadSafezoneReceived;
    public event LoadScenarioReceived loadScenarioReceived;

    public event NoteReceived noteReceived;

    public void Parse(string data)
    {
        var parsedData = JSON.Parse(data);

        switch (parsedData["type"].Value)
        {
            case "TEXT":
            case "IMAGE":
                ProcessNote(parsedData);
                break;
            case "COMMAND":
                ParseCommand(parsedData["data"].Value);
                break;
            default:
                Debug.Log("Parser : Unknown type received");
                break;
        }
    }

    private void ParseCommand(string command)
    {
        switch (command)
        {
            case "PLAY":
                playReceived();
                break;
            case "PAUSE":
                pauseReceived();
                break;
            case "RESET":
                resetReceived();
                break;
            case "SAFEZONE":
                loadSafezoneReceived();
                break;
            case "SCENARIO":
                loadScenarioReceived();
                break;
            default:
                Debug.Log("Parser : Unrecognized command received");
                break;
        }
    }

    private void ProcessNote(JSONNode parsedData)
    {
        Note note = new Note();
        note.Type = parsedData["type"].Value;
        note.Data = parsedData["data"].Value;

        note.StartTime = long.Parse(parsedData["startTime"].Value);
        note.EndTime = long.Parse(parsedData["endTime"].Value);

        noteReceived(note);
    }
}
