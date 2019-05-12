using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePatientPanel : MonoBehaviour
{
    public InputField patientName;
    public InputField patientSurname;

    public PatientList patientList;

    private int patientId;

    // Use this for initialization
    void Start()
    {
        
    }

    public void Init(int patientId)
    {
        this.patientId = patientId;
        Patient patient = DataService.Instance.GetPatient(patientId);
        patientName.text = patient.Name;
        patientSurname.text = patient.Surname;
    }

    public void UpdatePatient()
    {
        if (patientName.text != "" && patientSurname.text != "")
        {
            Patient patient = DataService.Instance.GetPatient(patientId);
            patient.Name = patientName.text;
            patient.Surname = patientSurname.text;

            DataService.Instance.UpdatePatient(patient);

            patientList.RefreshPatientList();

            this.gameObject.SetActive(false);
        }
    }
}

