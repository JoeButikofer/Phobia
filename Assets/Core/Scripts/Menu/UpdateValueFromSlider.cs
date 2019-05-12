using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UpdateValueFromSlider : MonoBehaviour {

    public Slider slider;

    private Text textEdit;

    void Awake()
    {
        textEdit = GetComponent<Text>();
        slider.onValueChanged.AddListener((value) => { textEdit.text = "" + value; });
    }
}
