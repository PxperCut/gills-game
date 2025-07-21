using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static int Fullscreen;
    public static int Vsync;
    public static int GraphicsQuality;
    public static int ShadowQuality;
    public static int antiAliasing;
    public static string Resolution;
    public static int InvertY;
    public static int AutocompleteQTE;
    public static int Subtitles;
    public static float MasterVolume;
    public static float MusicVolume;
    public static float SpeakerVolume;
    public static float MouseSensitivity;

    // Start is called before the first frame update
    public void Awake()
    {
Debug.Log("GraphicsQuality: " + GraphicsQuality);
Debug.Log("ShadowQuality: " + ShadowQuality);
Debug.Log("QualitySettings.shadowResolution: " + QualitySettings.shadowResolution);

        DontDestroyOnLoad(this);
        
        //GRAPHICS
        //Fullscreen
        Fullscreen = PlayerPrefs.HasKey("Fullscreen") ///basically check if player pref exists 
            ? PlayerPrefs.GetInt("Fullscreen") //if so set it the player pref
            : 2; //otherwise if it doesnt exist set it to this
        Screen.fullScreenMode = (FullScreenMode)Fullscreen;

        //Vsync
        Vsync = PlayerPrefs.HasKey("VSYNC")
            ? QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSYNC")
            : QualitySettings.vSyncCount = 1;

        //AA
            antiAliasing = PlayerPrefs.HasKey("antiAliasing")
        ? QualitySettings.antiAliasing = PlayerPrefs.GetInt("antiAliasing")
        : QualitySettings.antiAliasing = 1;

        //Res
        Resolution = PlayerPrefs.HasKey("Resolution")
            ? PlayerPrefs.GetString("Resolution")
            : "1920x1080";  //default to 1920x1080
        // if there isnt any key store the default res
        if (!PlayerPrefs.HasKey("Resolution"))
        {
            PlayerPrefs.SetString("Resolution", "1920x1080");
        }

        //Graphics Quality
        if (PlayerPrefs.HasKey("GraphicsQuality"))
        {
            GraphicsQuality = PlayerPrefs.GetInt("GraphicsQuality");
        }
        else
        {
            QualitySettings.SetQualityLevel(5);
        }

        //Shadow Quality
        if (PlayerPrefs.HasKey("ShadowQuality"))
        {
            ShadowQuality = PlayerPrefs.GetInt("ShadowQuality");
        }
        else
        {
            QualitySettings.shadowResolution = (ShadowResolution)GlobalVariables.ShadowQuality;
        }

        //GAMEPLAY
        //InvertY
        if (PlayerPrefs.HasKey("InvertY"))
        {
            InvertY = PlayerPrefs.GetInt("InvertY");
        }
        else
        {
            InvertY = 0;
        }

        //Mouse Sensitivity
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        else
        {
            MouseSensitivity = 0.6f;
        }

        //Autocomplete QTE
        if (PlayerPrefs.HasKey("AutocompleteQTE"))
        {
            AutocompleteQTE = PlayerPrefs.GetInt("AutocompleteQTE");
        }
        else
        {
            AutocompleteQTE = 0;
        }

        //AUDIO/VIDEO
        //SUBTITLES
        if (PlayerPrefs.HasKey("Subtitles"))
        {
            Subtitles = PlayerPrefs.GetInt("Subtitles");
        }
        else
        {
            Subtitles = 0;
        }

        //MASTER VOLUME
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        else
        {
            MasterVolume = 1;
        }

        //MUSIC VOLUME
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            MusicVolume = 1f;  // Default to full volume
        }

        //SPEAKER VOLUME
        if (PlayerPrefs.HasKey("SpeakerVolume"))
        {
            SpeakerVolume = PlayerPrefs.GetFloat("SpeakerVolume");
        }
        else
        {
            SpeakerVolume = 1f;  // Default to full volume
        }
    }
}