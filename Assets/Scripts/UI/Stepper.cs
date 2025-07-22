using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class Stepper : MonoBehaviour
{
    public TextMeshProUGUI StepperText;
    public GameObject StepDown;
    public GameObject StepUp;

    [SerializeField]
    public string StepperType = "None";

    [SerializeField]
    private string[] options = { "Low", "Medium", "High" };

    [SerializeField]
    public int Index = 2;


    void Start()
    {
        if (StepperType!="None")
    {
            switch (StepperType) {
                case "Fullscreen":
                    Index = PlayerPrefs.GetInt("Fullscreen", 3) == 2 ? 1 : 0;

                    break;
            }
    }
        StepperText.text = options[Index];
        UpdateStepperButtons();
    }

    public void CheckStepperType()
    {
        switch (StepperType)
        {
            case "None":
                Debug.Log("Please specify the stepper type!");
                break;

            case "InvertY":
                GlobalVariables.InvertY = Index;
                break;
                
            case "AutocompleteQTE":
                GlobalVariables.AutocompleteQTE = Index;
                break;
                

            case "GraphicsQuality":
                switch (Index)
                {
                    //Lowest
                    case 0:
                        GlobalVariables.GraphicsQuality = 1;
                        break;
                    //Low
                    case 1:
                        GlobalVariables.GraphicsQuality = 2;
                        break;
                    //Medium
                    case 2:
                        GlobalVariables.GraphicsQuality = 3;
                        break;
                    //High
                    case 3:
                        GlobalVariables.GraphicsQuality = 4;
                        break;
                    //Highest
                    case 4:
                        GlobalVariables.GraphicsQuality = 5;
                        break;
                }
                QualitySettings.SetQualityLevel(GlobalVariables.GraphicsQuality);
                break;


            case "ShadowQuality":
                switch (Index)
                {
                    //Low
                    case 0:
                        GlobalVariables.ShadowQuality = 0;
                        break;
                    //Medium
                    case 1:
                        GlobalVariables.ShadowQuality = 1;
                        break;
                    //High
                    case 2:
                        GlobalVariables.ShadowQuality = 2;
                        break;
                    //VeryHigh
                    case 3:
                        GlobalVariables.ShadowQuality = 3;
                        break;
                }
                QualitySettings.shadowResolution = (ShadowResolution)GlobalVariables.ShadowQuality;
                break;

            case "Fullscreen":
                switch (Index)
                {
                    //Windowed
                    case 0:
                        GlobalVariables.Fullscreen = 3;
                        break;
                     //Fullscreen
                    case 1:
                        GlobalVariables.Fullscreen = 2;
                        break;
                }
                Screen.fullScreenMode = (FullScreenMode)GlobalVariables.Fullscreen;
                break;

            case "VSYNC":
                switch (Index)
                {
                    //Vsync Off
                    case 0:
                        GlobalVariables.Vsync = 0;
                        break;

                    //Vsync On
                    case 1:
                        GlobalVariables.Vsync = 1;
                        break;
                }
                QualitySettings.vSyncCount = GlobalVariables.Vsync;
                break;

            case "antiAliasing":
                switch (Index)
                {
                    //Antialiasing Off
                    case 0:
                        GlobalVariables.antiAliasing = 0;
                        break;

                    //Antialiasing On
                    case 1:
                        GlobalVariables.antiAliasing = 1;
                        break;
                }
                QualitySettings.antiAliasing = GlobalVariables.antiAliasing;
                break;

            case "Resolution":
                switch (Index)
                {
                    //1920x1080
                    case 6:
                GlobalVariables.Resolution = "1920x1080";
                break;
                    case 5:
                    //1600x900
                 GlobalVariables.Resolution = "1600x900";
                break;
                    case 4:
                    //1366x768
                GlobalVariables.Resolution = "1366x768";
                break;
                    case 3:
                    //1280x720
                GlobalVariables.Resolution = "1280x720";
                break;
                    case 2:
                    //800x600
                GlobalVariables.Resolution = "800x600";
                break;
                    case 1:
                    //640x480
                GlobalVariables.Resolution = "640x480";
                break;
                    case 0:
                        //300x200
                GlobalVariables.Resolution = "300x200";
                break;
                }
            break;

            case "Subtitles":
                GlobalVariables.Subtitles = Index;
                break;
        }
    }

public void OnStepUp()
{
    if (Index != options.Length - 1)
    {
        Index = Mathf.Clamp(Index + 1, 0, options.Length - 1);
    }
    else
    {
        Index = 0;
    }
    
    StepperText.text = options[Index];
    CheckStepperType();
    UpdateStepperButtons();
}

    public void OnStepDown()
    {
        Index = Mathf.Clamp(Index - 1, 0, options.Length - 1);
        StepperText.text = options[Index];
        CheckStepperType();
        UpdateStepperButtons();
    }

    public void UpdateStepperButtons()
    {
        int previousIndex = Index;
        
        switch (StepperType)
        {
            case "VSYNC":
                Index = GlobalVariables.Vsync;
                break;

            case "antiAliasing":
                Index = GlobalVariables.antiAliasing;
                break;

            case "Fullscreen":
                Index = GlobalVariables.Fullscreen == 2 ? 1 : 0;
                break;

            case "Resolution":
                Index = System.Array.IndexOf(options, GlobalVariables.Resolution);
                break;

            case "GraphicsQuality":
                Index = GlobalVariables.GraphicsQuality - 1;
                break;

            case "ShadowQuality":
                Index = GlobalVariables.ShadowQuality;
                break;

            case "InvertY":
                Index = GlobalVariables.InvertY;
                break;

            case "AutocompleteQTE":
                Index = GlobalVariables.AutocompleteQTE;
                break;

            case "Subtitles":
                Index = GlobalVariables.Subtitles;
                break;
        }

        // Clamp the index to valid range
        Index = Mathf.Clamp(Index, 0, options.Length - 1);
        
        // Only update if the index is valid
        if (Index >= 0 && Index < options.Length)
        {
            StepperText.text = options[Index];
        }
        else
        {
            Debug.LogWarning($"Invalid index {Index} for stepper {StepperType}. Options length: {options.Length}");
            Index = 0;
            StepperText.text = options[Index];
        }

        // Update button visibility
        StepDown.SetActive(Index != 0);
        StepUp.SetActive(Index != options.Length - 1);
    }
}
