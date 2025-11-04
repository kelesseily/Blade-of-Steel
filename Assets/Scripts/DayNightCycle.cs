using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Tooltip("The main Directional Light in your scene (acts as the sun).")]
    public Light sun;

    [Tooltip("How many real-world seconds a full in-game day should last.")]
    public float dayDurationInSeconds = 120f; // e.g., 2 minutes for a full day/night

    [Tooltip("Controls the current time. 0 = midnight, 0.25 = sunrise, 0.5 = midday, 0.75 = sunset, 1 = midnight")]
    [Range(0, 1)]
    public float timeOfDay = 0.25f; // Start at sunrise (0.25)

    void Update()
    {
        if (sun == null)
            return; // Do nothing if no sun is assigned

        // 1. ADVANCE THE TIME OF DAY
        // Time.deltaTime is the time passed since the last frame.
        // Dividing by dayDurationInSeconds gives us how much 'timeOfDay' (0-1) has passed.
        timeOfDay += Time.deltaTime / dayDurationInSeconds;

        // 2. LOOP THE TIME OF DAY
        // If timeOfDay goes past 1 (midnight), loop it back to 0.
        if (timeOfDay >= 1f)
        {
            timeOfDay -= 1f;
        }

        // 3. ROTATE THE SUN
        // We use the timeOfDay (0-1) to calculate the sun's rotation (0-360 degrees).
        // 0.5 (midday) = 180 degrees (pointing straight down).
        // 0 or 1 (midnight) = 0 or 360 degrees (pointing straight up).
        // 0.25 (sunrise) = 90 degrees (on the horizon).
        sun.transform.rotation = Quaternion.Euler(timeOfDay * 360f, -30f, 0f);
    }
}
