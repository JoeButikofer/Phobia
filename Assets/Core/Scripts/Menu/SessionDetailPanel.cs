using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SessionDetailPanel : MonoBehaviour {

    public Text scenarioNameTxt;
    public Text startTimeTxt;
    public Text endTimeTxt;
    public Text durationTxt;

    public void ShowSessionDetail(Session session)
    {
        scenarioNameTxt.text = session.ScenarioName;
        startTimeTxt.text = EpochTools.ConvertEpochToHumanReadableTime(session.StartTime, true);
        endTimeTxt.text = EpochTools.ConvertEpochToHumanReadableTime(session.EndTime, true);
        durationTxt.text = EpochTools.ConvertDurationToHumanReadableString(session.EndTime - session.StartTime);
    }

    public void Clear()
    {
        scenarioNameTxt.text = "-";
        startTimeTxt.text = "-";
        endTimeTxt.text = "-";
        durationTxt.text = "-";
    }
}
