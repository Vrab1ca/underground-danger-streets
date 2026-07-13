using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    public enum DayPhase
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    [Header("Light References")]
    public Light sunLight;
    public Light moonLight;

    [Tooltip("Usually this is the Sun Directional Light transform.")]
    public Transform sunTransform;

    [Tooltip("Usually this is the Moon Directional Light transform.")]
    public Transform moonTransform;

    [Header("Night Objects")]
    [Tooltip(
        "Stars, street lights and other objects " +
        "that should be enabled during the night."
    )]
    public GameObject[] nightOnlyObjects;

    [Header("Phase Duration - Real Minutes")]
    [Min(0.1f)]
    public float morningMinutes = 3f;

    [Min(0.1f)]
    public float afternoonMinutes = 9f;

    [Min(0.1f)]
    public float eveningMinutes = 3f;

    [Min(0.1f)]
    public float nightMinutes = 15f;

    [Header("Starting Time")]
    public DayPhase startPhase = DayPhase.Morning;

    [Range(0f, 1f)]
    public float startPhaseProgress = 0f;

    public bool cycleIsRunning = true;

    [Header("Sun Rotation")]
    public float sunYaw = -30f;

    [Header("Sun Colors")]
    public Color morningSunColor =
        new Color(1f, 0.68f, 0.42f);

    public Color afternoonSunColor =
        new Color(1f, 0.95f, 0.82f);

    public Color eveningSunColor =
        new Color(1f, 0.35f, 0.15f);

    public Color nightSunColor =
        new Color(0.25f, 0.32f, 0.5f);

    [Header("Sun Intensity")]
    [Range(0f, 2f)]
    public float morningSunIntensity = 0.45f;

    [Range(0f, 2f)]
    public float afternoonSunIntensity = 1.15f;

    [Range(0f, 2f)]
    public float eveningSunIntensity = 0.3f;

    [Range(0f, 2f)]
    public float nightSunIntensity = 0f;

    [Header("Moon")]
    public Color moonColor =
        new Color(0.55f, 0.65f, 1f);

    [Range(0f, 2f)]
    public float morningMoonIntensity = 0f;

    [Range(0f, 2f)]
    public float afternoonMoonIntensity = 0f;

    [Range(0f, 2f)]
    public float eveningMoonIntensity = 0.15f;

    [Range(0f, 2f)]
    public float nightMoonIntensity = 0.45f;

    [Header("Ambient Colors")]
    public Color morningAmbientColor =
        new Color(0.42f, 0.35f, 0.32f);

    public Color afternoonAmbientColor =
        new Color(0.7f, 0.72f, 0.75f);

    public Color eveningAmbientColor =
        new Color(0.35f, 0.22f, 0.25f);

    public Color nightAmbientColor =
        new Color(0.035f, 0.045f, 0.09f);

    [Header("Fog")]
    public bool useFog = true;

    public Color morningFogColor =
        new Color(0.65f, 0.5f, 0.42f);

    public Color afternoonFogColor =
        new Color(0.7f, 0.8f, 0.9f);

    public Color eveningFogColor =
        new Color(0.45f, 0.2f, 0.22f);

    public Color nightFogColor =
        new Color(0.025f, 0.035f, 0.075f);

    [Range(0f, 0.1f)]
    public float morningFogDensity = 0.003f;

    [Range(0f, 0.1f)]
    public float afternoonFogDensity = 0.001f;

    [Range(0f, 0.1f)]
    public float eveningFogDensity = 0.004f;

    [Range(0f, 0.1f)]
    public float nightFogDensity = 0.008f;

    [Header("Skybox")]
    public bool changeSkyboxExposure = true;

    [Range(0f, 3f)]
    public float morningSkyExposure = 0.8f;

    [Range(0f, 3f)]
    public float afternoonSkyExposure = 1.2f;

    [Range(0f, 3f)]
    public float eveningSkyExposure = 0.6f;

    [Range(0f, 3f)]
    public float nightSkyExposure = 0.15f;

    [Header("Current Time - Read Only")]
    [SerializeField]
    private DayPhase currentPhase;

    [SerializeField]
    [Range(0f, 24f)]
    private float currentHour;

    [SerializeField]
    private float phaseProgress;

    private float cycleTimerSeconds;

    public DayPhase CurrentPhase
    {
        get { return currentPhase; }
    }

    public float CurrentHour
    {
        get { return currentHour; }
    }

    private float MorningSeconds
    {
        get { return morningMinutes * 60f; }
    }

    private float AfternoonSeconds
    {
        get { return afternoonMinutes * 60f; }
    }

    private float EveningSeconds
    {
        get { return eveningMinutes * 60f; }
    }

    private float NightSeconds
    {
        get { return nightMinutes * 60f; }
    }

    private float TotalCycleSeconds
    {
        get
        {
            return MorningSeconds +
                   AfternoonSeconds +
                   EveningSeconds +
                   NightSeconds;
        }
    }

    private void OnValidate()
    {
        morningMinutes =
            Mathf.Max(0.1f, morningMinutes);

        afternoonMinutes =
            Mathf.Max(0.1f, afternoonMinutes);

        eveningMinutes =
            Mathf.Max(0.1f, eveningMinutes);

        nightMinutes =
            Mathf.Max(0.1f, nightMinutes);
    }

    private void Awake()
    {
        FindReferences();

        RenderSettings.ambientMode =
            AmbientMode.Flat;

        RenderSettings.fog = useFog;

        if (useFog)
        {
            RenderSettings.fogMode =
                FogMode.ExponentialSquared;
        }

        SetStartingTime();
        UpdateCycleInformation();
        ApplyEnvironment();
    }

    private void Update()
    {
        if (cycleIsRunning)
        {
            cycleTimerSeconds +=
                Time.deltaTime;

            cycleTimerSeconds =
                Mathf.Repeat(
                    cycleTimerSeconds,
                    TotalCycleSeconds
                );
        }

        UpdateCycleInformation();
        ApplyEnvironment();
    }

    private void FindReferences()
    {
        if (sunLight != null &&
            sunTransform == null)
        {
            sunTransform =
                sunLight.transform;
        }

        if (moonLight != null &&
            moonTransform == null)
        {
            moonTransform =
                moonLight.transform;
        }
    }

    private void SetStartingTime()
    {
        float startOffset = 0f;

        switch (startPhase)
        {
            case DayPhase.Morning:
                startOffset = 0f;
                break;

            case DayPhase.Afternoon:
                startOffset =
                    MorningSeconds;
                break;

            case DayPhase.Evening:
                startOffset =
                    MorningSeconds +
                    AfternoonSeconds;
                break;

            case DayPhase.Night:
                startOffset =
                    MorningSeconds +
                    AfternoonSeconds +
                    EveningSeconds;
                break;
        }

        cycleTimerSeconds =
            startOffset +
            GetPhaseDuration(startPhase) *
            startPhaseProgress;
    }

    private void UpdateCycleInformation()
    {
        float morningEnd =
            MorningSeconds;

        float afternoonEnd =
            morningEnd +
            AfternoonSeconds;

        float eveningEnd =
            afternoonEnd +
            EveningSeconds;

        if (cycleTimerSeconds < morningEnd)
        {
            currentPhase =
                DayPhase.Morning;

            phaseProgress =
                cycleTimerSeconds /
                MorningSeconds;

            currentHour =
                Mathf.Lerp(
                    6f,
                    12f,
                    phaseProgress
                );

            return;
        }

        if (cycleTimerSeconds < afternoonEnd)
        {
            currentPhase =
                DayPhase.Afternoon;

            phaseProgress =
                (cycleTimerSeconds -
                 morningEnd) /
                AfternoonSeconds;

            currentHour =
                Mathf.Lerp(
                    12f,
                    18f,
                    phaseProgress
                );

            return;
        }

        if (cycleTimerSeconds < eveningEnd)
        {
            currentPhase =
                DayPhase.Evening;

            phaseProgress =
                (cycleTimerSeconds -
                 afternoonEnd) /
                EveningSeconds;

            currentHour =
                Mathf.Lerp(
                    18f,
                    21f,
                    phaseProgress
                );

            return;
        }

        currentPhase =
            DayPhase.Night;

        phaseProgress =
            (cycleTimerSeconds -
             eveningEnd) /
            NightSeconds;

        // Night moves from 21:00 to 06:00.
        float nightHour =
            Mathf.Lerp(
                21f,
                30f,
                phaseProgress
            );

        currentHour =
            Mathf.Repeat(
                nightHour,
                24f
            );
    }

    private void ApplyEnvironment()
    {
        DayPhase nextPhase =
            GetNextPhase(currentPhase);

        float smoothProgress =
            Mathf.SmoothStep(
                0f,
                1f,
                phaseProgress
            );

        UpdateSun(
            nextPhase,
            smoothProgress
        );

        UpdateMoon(
            nextPhase,
            smoothProgress
        );

        UpdateAmbient(
            nextPhase,
            smoothProgress
        );

        UpdateFog(
            nextPhase,
            smoothProgress
        );

        UpdateSkybox(
            nextPhase,
            smoothProgress
        );

        UpdateNightObjects();
    }

    private void UpdateSun(
        DayPhase nextPhase,
        float blend
    )
    {
        float sunAngle =
            currentHour / 24f *
            360f -
            90f;

        if (sunTransform != null)
        {
            sunTransform.rotation =
                Quaternion.Euler(
                    sunAngle,
                    sunYaw,
                    0f
                );
        }

        if (sunLight == null)
            return;

        sunLight.color =
            Color.Lerp(
                GetSunColor(currentPhase),
                GetSunColor(nextPhase),
                blend
            );

        sunLight.intensity =
            Mathf.Lerp(
                GetSunIntensity(currentPhase),
                GetSunIntensity(nextPhase),
                blend
            );

        sunLight.enabled =
            sunLight.intensity > 0.001f;
    }

    private void UpdateMoon(
        DayPhase nextPhase,
        float blend
    )
    {
        if (moonTransform != null)
        {
            float moonAngle =
                currentHour / 24f *
                360f +
                90f;

            moonTransform.rotation =
                Quaternion.Euler(
                    moonAngle,
                    sunYaw,
                    0f
                );
        }

        if (moonLight == null)
            return;

        moonLight.color =
            moonColor;

        moonLight.intensity =
            Mathf.Lerp(
                GetMoonIntensity(currentPhase),
                GetMoonIntensity(nextPhase),
                blend
            );

        moonLight.enabled =
            moonLight.intensity > 0.001f;
    }

    private void UpdateAmbient(
        DayPhase nextPhase,
        float blend
    )
    {
        RenderSettings.ambientLight =
            Color.Lerp(
                GetAmbientColor(currentPhase),
                GetAmbientColor(nextPhase),
                blend
            );
    }

    private void UpdateFog(
        DayPhase nextPhase,
        float blend
    )
    {
        RenderSettings.fog =
            useFog;

        if (!useFog)
            return;

        RenderSettings.fogColor =
            Color.Lerp(
                GetFogColor(currentPhase),
                GetFogColor(nextPhase),
                blend
            );

        RenderSettings.fogDensity =
            Mathf.Lerp(
                GetFogDensity(currentPhase),
                GetFogDensity(nextPhase),
                blend
            );
    }

    private void UpdateSkybox(
        DayPhase nextPhase,
        float blend
    )
    {
        if (!changeSkyboxExposure)
            return;

        if (RenderSettings.skybox == null)
            return;

        if (!RenderSettings.skybox.HasProperty(
                "_Exposure"
            ))
        {
            return;
        }

        float exposure =
            Mathf.Lerp(
                GetSkyExposure(currentPhase),
                GetSkyExposure(nextPhase),
                blend
            );

        RenderSettings.skybox.SetFloat(
            "_Exposure",
            exposure
        );
    }

    private void UpdateNightObjects()
    {
        bool shouldBeActive = false;

        if (currentPhase == DayPhase.Night)
        {
            shouldBeActive = true;
        }
        else if (currentPhase ==
                 DayPhase.Evening &&
                 phaseProgress >= 0.35f)
        {
            shouldBeActive = true;
        }
        else if (currentPhase ==
                 DayPhase.Morning &&
                 phaseProgress <= 0.15f)
        {
            shouldBeActive = true;
        }

        if (nightOnlyObjects == null)
            return;

        for (int i = 0;
             i < nightOnlyObjects.Length;
             i++)
        {
            GameObject nightObject =
                nightOnlyObjects[i];

            if (nightObject == null)
                continue;

            if (nightObject.activeSelf !=
                shouldBeActive)
            {
                nightObject.SetActive(
                    shouldBeActive
                );
            }
        }
    }

    private DayPhase GetNextPhase(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return DayPhase.Afternoon;

            case DayPhase.Afternoon:
                return DayPhase.Evening;

            case DayPhase.Evening:
                return DayPhase.Night;

            default:
                return DayPhase.Morning;
        }
    }

    private float GetPhaseDuration(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return MorningSeconds;

            case DayPhase.Afternoon:
                return AfternoonSeconds;

            case DayPhase.Evening:
                return EveningSeconds;

            default:
                return NightSeconds;
        }
    }

    private Color GetSunColor(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningSunColor;

            case DayPhase.Afternoon:
                return afternoonSunColor;

            case DayPhase.Evening:
                return eveningSunColor;

            default:
                return nightSunColor;
        }
    }

    private float GetSunIntensity(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningSunIntensity;

            case DayPhase.Afternoon:
                return afternoonSunIntensity;

            case DayPhase.Evening:
                return eveningSunIntensity;

            default:
                return nightSunIntensity;
        }
    }

    private float GetMoonIntensity(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningMoonIntensity;

            case DayPhase.Afternoon:
                return afternoonMoonIntensity;

            case DayPhase.Evening:
                return eveningMoonIntensity;

            default:
                return nightMoonIntensity;
        }
    }

    private Color GetAmbientColor(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningAmbientColor;

            case DayPhase.Afternoon:
                return afternoonAmbientColor;

            case DayPhase.Evening:
                return eveningAmbientColor;

            default:
                return nightAmbientColor;
        }
    }

    private Color GetFogColor(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningFogColor;

            case DayPhase.Afternoon:
                return afternoonFogColor;

            case DayPhase.Evening:
                return eveningFogColor;

            default:
                return nightFogColor;
        }
    }

    private float GetFogDensity(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningFogDensity;

            case DayPhase.Afternoon:
                return afternoonFogDensity;

            case DayPhase.Evening:
                return eveningFogDensity;

            default:
                return nightFogDensity;
        }
    }

    private float GetSkyExposure(
        DayPhase phase
    )
    {
        switch (phase)
        {
            case DayPhase.Morning:
                return morningSkyExposure;

            case DayPhase.Afternoon:
                return afternoonSkyExposure;

            case DayPhase.Evening:
                return eveningSkyExposure;

            default:
                return nightSkyExposure;
        }
    }

    public string GetFormattedTime()
    {
        int hours =
            Mathf.FloorToInt(
                currentHour
            );

        int minutes =
            Mathf.FloorToInt(
                (currentHour - hours) *
                60f
            );

        return hours.ToString("00") +
               ":" +
               minutes.ToString("00");
    }

    public void PauseCycle()
    {
        cycleIsRunning = false;
    }

    public void ContinueCycle()
    {
        cycleIsRunning = true;
    }

    public void SetMorning()
    {
        cycleTimerSeconds = 0f;
    }

    public void SetAfternoon()
    {
        cycleTimerSeconds =
            MorningSeconds;
    }

    public void SetEvening()
    {
        cycleTimerSeconds =
            MorningSeconds +
            AfternoonSeconds;
    }

    public void SetNight()
    {
        cycleTimerSeconds =
            MorningSeconds +
            AfternoonSeconds +
            EveningSeconds;
    }
}