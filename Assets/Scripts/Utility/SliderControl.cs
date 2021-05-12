using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour {
    [SerializeField] private InputField inputField = null;

    private Slider slider;
    
    public enum SliderSetting { Volume};
    public SliderSetting menu;

    void Start() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate {
            UpdateSliderValue();
        });
        inputField.onValueChanged.AddListener(delegate {
            UpdateSliderPosition();
        });

        switch (menu.ToString()) {
            case "Volume":
                slider.value = Options.volume;
                inputField.text = Options.volume.ToString();
                break;
        }
    }

    void UpdateSliderValue() {
        inputField.text = slider.value.ToString();
    }

    void UpdateSliderPosition() {
        //TO DO: Make value numbers only and maybe catch error
        slider.value = int.Parse(inputField.text);
    }
}
