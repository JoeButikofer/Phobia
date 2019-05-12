using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PatientList : MonoBehaviour {

    public GameObject listItemPrefab;

    public InputField newPatientName;
    public InputField newPatientSurname;
    public GameObject newPatientPanel;
    public Button selectBtn;
    public Button updateBtn;

    public UpdatePatientPanel updatePanel;

    private int selectedPatientId = -1;

    // Use this for initialization
    void OnEnable()
    {
        updateBtn.interactable = false;
        selectBtn.interactable = false;
        RefreshPatientList();
    }

    public void RefreshPatientList()
    {
        ClearPatientList();

        var patients = DataService.Instance.GetPatients();

        int i = 0;
        foreach (var patient in patients)
        {
            var patientListItem = (GameObject)Instantiate(listItemPrefab, transform, false);

            patientListItem.transform.Find("Text").GetComponent<Text>().text = patient.Name + " " + patient.Surname;

            Patient currentPatient = patient;
            patientListItem.GetComponent<Button>().onClick.AddListener(() => {
                selectedPatientId = currentPatient.Id;
                selectBtn.interactable = true;
                updateBtn.interactable = true;
            });
            i++;
        }
    }

    void ClearPatientList()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

	
	public void SelectPatient()
    {
        GlobalVariables.SelectedPatientId = selectedPatientId;
    }

    public void AddPatient()
    {
        if(newPatientName.text != "" && newPatientSurname.text != "")
        {
            DataService.Instance.CreatePatient(newPatientName.text, newPatientSurname.text);
            RefreshPatientList();

            newPatientName.text = "";
            newPatientSurname.text = "";
            newPatientPanel.SetActive(false);
        }
    }

    public void DeletePatient()
    {
        DataService.Instance.DeletePatient(selectedPatientId);
        selectedPatientId = -1;
        GlobalVariables.SelectedPatientId = -1;
        RefreshPatientList();

        updateBtn.interactable = false;
        selectBtn.interactable = false;
    }

    public void ShowUpdatePanel()
    {
        if(selectedPatientId >= 0)
        {
            updatePanel.gameObject.SetActive(true);
            updatePanel.Init(selectedPatientId);
        }
    }
}
