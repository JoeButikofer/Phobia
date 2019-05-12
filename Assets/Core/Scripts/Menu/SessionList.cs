using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class SessionList : MonoBehaviour {

    public GameObject listItemPrefab;
    public SessionDetailPanel detailPanel;

    private Session selectedSession;

    // Use this for initialization
    void OnEnable ()
    {
        RefreshSessionList();
    }

    private void RefreshSessionList()
    {
        ClearSessionList();

        detailPanel.Clear();

        var sessions = new List<Session>(DataService.Instance.GetPatientSessions(GlobalVariables.SelectedPatientId));

        foreach (var session in sessions)
        {
            var sessionListItem = (GameObject)Instantiate(listItemPrefab, transform, false);

            sessionListItem.transform.Find("Text").GetComponent<Text>().text = session.ScenarioName + ", " + EpochTools.ConvertEpochToHumanReadableDate(session.StartTime);

            Session currentSession = session;
            sessionListItem.GetComponent<Button>().onClick.AddListener(() => {
                detailPanel.ShowSessionDetail(currentSession);
                selectedSession = currentSession;
            });
        }

        if (sessions.Count > 0)
            detailPanel.ShowSessionDetail(sessions[0]);
    }

    void ClearSessionList()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void LaunchHistoryTool()
    {
        if(selectedSession != null)
        {
            GlobalVariables.SelectedSessionId = selectedSession.Id;
        }
    }

    public void DeleteSession()
    {
        if (selectedSession != null)
        {
            DataService.Instance.DeleteSession(selectedSession.Id);

            RefreshSessionList();
        }
    }
}
