using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public float TransitionTime = 1f;
    public GameObject MenuManager;
    public GameObject 
        MainMenu, 
        SettingsMenu, 
        AudioVideoMenu, 
        GameplayMenu, 
        ControlsMenu, 
        GraphicsMenu;

    public GameObject QuitGamePopup;
    public GameObject ResolutionPopupMenu;
    private float TimeRemaining;

    public Animator Transition;

    public GameObject StepperPrefab;
    private GameObject Transitions;

    public Slider VolSlider;
    public Slider MusicVolSlider;
    public Slider SpeakerVolSlider;
    public Slider MouseSensitivitySlider;

    public void DisableMenuButtons()
    {
        foreach (var btn in MenuManager.GetComponentsInChildren<Button>(true))
        {
            btn.interactable = false;
        }
    }

    public void EnableMenuButtons()
    {
        foreach (var btn in MenuManager.GetComponentsInChildren<Button>(true))
        {
            btn.interactable = true;
        }
    }

    public void ApplyGraphicalChanges()
    {
        PlayerPrefs.SetString("Resolution", GlobalVariables.Resolution);
        ResolutionPopupMenu.SetActive(false);
        ApplyChanges();
    }
    public void DenyGraphicalChanges() 
    {
        Debug.Log("Cancelled! Reverting Resolution...");
        ResolutionPopupMenu.SetActive(false);

        GlobalVariables.Resolution = PlayerPrefs.GetString("Resolution");

        string[] dimensions = PlayerPrefs.GetString("Resolution").Split('x'); // apply the resolution
        Screen.SetResolution(int.Parse(dimensions[0]), int.Parse(dimensions[1]), (FullScreenMode)GlobalVariables.Fullscreen);

        Stepper[] steppers = FindObjectsByType<Stepper>(FindObjectsSortMode.None);
        foreach (Stepper stepper in steppers)
        {stepper.UpdateStepperButtons();}

        EnableMenuButtons();

    }

    public void Start()
    {
        ResolutionPopupMenu.SetActive(false);
        QuitGamePopup.SetActive(false);
        
        GlobalVariables.MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        VolSlider.value = GlobalVariables.MasterVolume;

        GlobalVariables.SpeakerVolume = PlayerPrefs.GetFloat("SpeakerVolume");
        SpeakerVolSlider.value = GlobalVariables.SpeakerVolume;
        
        GlobalVariables.MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        MusicVolSlider.value = GlobalVariables.MusicVolume;

        GlobalVariables.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        MouseSensitivitySlider.value = GlobalVariables.MouseSensitivity;
    }

    private void DeactivateMenus()
    {
        AudioVideoMenu.SetActive(false);
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        GraphicsMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        GameplayMenu.SetActive(false);
    }

    IEnumerator SwitchMenu(GameObject targetMenu)
    {
        Transition.SetTrigger("TRStart");
        DisableMenuButtons();

        yield return new WaitForSeconds(TransitionTime);

        DeactivateMenus();
        targetMenu.SetActive(true);

        Transition.SetTrigger("TREnd");
        EnableMenuButtons();
    }

    public void SwitchMenuWithoutTransitions(GameObject targetMenu)
    {
        DeactivateMenus();
        targetMenu.SetActive(true);
        EnableMenuButtons();
    }

    // Go to Menu
    public void GoToMenu(string menuName)
    {
        switch (menuName)
        {
            case "AudioVideo":
                SwitchMenuWithoutTransitions(AudioVideoMenu);
                break;
            case "Gameplay":
                SwitchMenuWithoutTransitions(GameplayMenu);
                break;
            case "Graphics":
                SwitchMenuWithoutTransitions(GraphicsMenu);
                break;
            case "Controls":
                SwitchMenuWithoutTransitions(ControlsMenu);
                break;
            case "ReturnSettings":
                SwitchMenuWithoutTransitions(SettingsMenu);
                break;
            case "Settings":
                StartCoroutine(SwitchMenu(SettingsMenu));
                break;
            case "Main":
                StartCoroutine(SwitchMenu(MainMenu));
                break;
            default:
                Debug.LogWarning("Menu not recognized: " + menuName);
                break;
        }
    }

    public void ApplyChanges() 
    {
        if (GlobalVariables.Resolution != PlayerPrefs.GetString("Resolution")) //okay so if the resolution isnt already set to player preferences
        {
            string[] dimensions = GlobalVariables.Resolution.Split('x'); // apply the resolution
            Screen.SetResolution(int.Parse(dimensions[0]), int.Parse(dimensions[1]), (FullScreenMode)GlobalVariables.Fullscreen);

            Debug.Log(int.Parse(dimensions[0]).ToString() + "x" + int.Parse(dimensions[1]).ToString());

            ResolutionPopupMenu.SetActive(true); //then ask them if they wanna keep it (cont.)
            TimeRemaining = ResolutionPopupMenu.GetComponent<Popup>().Timer;
        }
        else
        {
            //GRAPHICS
            PlayerPrefs.SetInt("Fullscreen", GlobalVariables.Fullscreen); //if their resolution is already player prefs, ignore this
            PlayerPrefs.SetInt("VSYNC", GlobalVariables.Vsync);
            PlayerPrefs.SetInt("GraphicsQuality", GlobalVariables.GraphicsQuality);
            PlayerPrefs.SetInt("ShadowQuality", GlobalVariables.ShadowQuality);
            PlayerPrefs.SetInt("antiAliasing", GlobalVariables.antiAliasing);

            //AUDIO/VIDEO            
            PlayerPrefs.SetInt("Subtitles", GlobalVariables.Subtitles);
            GlobalVariables.MasterVolume = VolSlider.value;
            PlayerPrefs.SetFloat("MasterVolume", GlobalVariables.MasterVolume);
            GlobalVariables.SpeakerVolume = SpeakerVolSlider.value;
            PlayerPrefs.SetFloat("SpeakerVolume", GlobalVariables.SpeakerVolume);
            GlobalVariables.MusicVolume = MusicVolSlider.value;
            PlayerPrefs.SetFloat("MusicVolume", GlobalVariables.MusicVolume);

            //GAMEPLAY
            GlobalVariables.MouseSensitivity = MouseSensitivitySlider.value;
            PlayerPrefs.SetFloat("MouseSensitivity", GlobalVariables.MouseSensitivity);
            PlayerPrefs.SetInt("InvertY", GlobalVariables.InvertY);
            PlayerPrefs.SetInt("AutocompleteQTE", GlobalVariables.AutocompleteQTE);

            PlayerPrefs.Save();
            SwitchMenuWithoutTransitions(SettingsMenu); //go back to settings
        }
    }

    public void CancelSettings()
    {
        Debug.Log("Cancelled! Reverting all settings...");

        Stepper[] steppers = FindObjectsByType<Stepper>(FindObjectsSortMode.None);

        foreach (Stepper stepper in steppers)
        {
            switch (stepper.StepperType)
            {
                //GRAPHICS
                case "Fullscreen":
                    GlobalVariables.Fullscreen = PlayerPrefs.GetInt("Fullscreen");
                    Screen.fullScreenMode = (FullScreenMode)GlobalVariables.Fullscreen;
                    break;

                case "VSYNC":
                    GlobalVariables.Vsync = PlayerPrefs.GetInt("VSYNC");
                    QualitySettings.vSyncCount = GlobalVariables.Vsync;
                    break;

                case "antiAliasing":
                    GlobalVariables.antiAliasing = PlayerPrefs.GetInt("antiAliasing");
                    QualitySettings.antiAliasing = GlobalVariables.antiAliasing;
                    break;

                case "Resolution":
                    GlobalVariables.Resolution = PlayerPrefs.GetString("Resolution");
                    break;

                case "GraphicsQuality":
                    GlobalVariables.GraphicsQuality = PlayerPrefs.GetInt("GraphicsQuality");
                    QualitySettings.SetQualityLevel(GlobalVariables.GraphicsQuality);
                    break;

                case "ShadowQuality":
                    GlobalVariables.ShadowQuality = PlayerPrefs.GetInt("ShadowQuality");
                    QualitySettings.shadowResolution = (ShadowResolution)GlobalVariables.ShadowQuality;
                    break;

                //GAMEPLAY
                case "InvertY":
                    GlobalVariables.InvertY = PlayerPrefs.GetInt("InvertY");
                    break;

                case "AutocompleteQTE":
                    GlobalVariables.AutocompleteQTE = PlayerPrefs.GetInt("AutocompleteQTE");
                    break;

                case "Subtitles":
                    GlobalVariables.Subtitles = PlayerPrefs.GetInt("Subtitles");
                    break;

                default:
                    Debug.Log("stepper type: " + stepper.StepperType +" is not clarified. add the function in MenuController script, you idiot!");
                    break;
            }
            stepper.UpdateStepperButtons();
            
        GlobalVariables.MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        VolSlider.value = GlobalVariables.MasterVolume;

        GlobalVariables.SpeakerVolume = PlayerPrefs.GetFloat("SpeakerVolume");
        SpeakerVolSlider.value = GlobalVariables.SpeakerVolume;
        
        GlobalVariables.MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        MusicVolSlider.value = GlobalVariables.MusicVolume;

        GlobalVariables.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        MouseSensitivitySlider.value = GlobalVariables.MouseSensitivity;
        }

        SwitchMenuWithoutTransitions(SettingsMenu);
    }

    public void DenyQuitGame()
    {
        QuitGamePopup.SetActive(false);
    }

        public void QuitGame()
    {
        QuitGamePopup.SetActive(true);
    }

// Quit 
public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (TimeRemaining > 0)
        {
            TimeRemaining -= Time.deltaTime;  //decrease timer
            ResolutionPopupMenu.GetComponent<Popup>().TimerText.text = Mathf.RoundToInt(TimeRemaining).ToString();
        }
        else //basically you took too long lol
        {
        if (ResolutionPopupMenu.activeSelf)
            {
                DenyGraphicalChanges();
            }
        }
    }

    public IEnumerator BeginGame()
    {
        Transition.SetTrigger("TRStart");
        DisableMenuButtons();

        yield return new WaitForSeconds(TransitionTime);
        
        SceneManager.LoadScene("game");
        
        EnableMenuButtons();
        Transition.SetTrigger("TREnd");
        MainMenu.SetActive(false);

    }

    public void OnBeginGame()
    {
        StartCoroutine(BeginGame());
    }
}