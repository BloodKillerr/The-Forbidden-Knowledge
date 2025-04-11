using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI optionsLabel;

    [SerializeField] private GameObject options;
    [SerializeField] private GameObject graphics;
    [SerializeField] private GameObject audio;
    [SerializeField] private GameObject controls;

    [SerializeField] private TextMeshProUGUI fpsLabel;
    [SerializeField] private Slider fpsMax;

    [SerializeField] private TextMeshProUGUI masterLabel;
    [SerializeField] private Slider masterVolume;

    [SerializeField] private TextMeshProUGUI musicLabel;
    [SerializeField] private Slider musicVolume;

    [SerializeField] private TextMeshProUGUI sfxLabel;
    [SerializeField] private Slider sfxVolume;

    public void GraphicsButton()
    {
        optionsLabel.text = "Graphics";
        options.SetActive(false);
        graphics.SetActive(true);
        Debug.Log("Graphics settings");
    }

    public void AudioButton()
    {
        optionsLabel.text = "Audio";
        options.SetActive(false);
        audio.SetActive(true);
        Debug.Log("Audio settings");
    }

    public void ControlsButton()
    {
        optionsLabel.text = "Controls";
        options.SetActive(false);
        controls.SetActive(true);
        Debug.Log("Controls settings");
    }

    public void BackToOptions()
    {
        options.SetActive(true);
        graphics.SetActive(false);
        audio.SetActive(false);
        controls.SetActive(false);
        optionsLabel.text = "Options";
    }

    public void FpsChange()
    {
        fpsLabel.text = fpsMax.value.ToString();
    }

    public void MasterChange()
    {
        masterLabel.text = masterVolume.value.ToString();
    }

    public void MusicChange()
    {
        musicLabel.text = musicVolume.value.ToString();
    }

    public void SfxChange()
    {
        sfxLabel.text = sfxVolume.value.ToString();
    }
}
