using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    float timer;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= 0.5f) // update twice a second
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText.text = $"FPS: {fps}";
            timer = 0f;
        }
    }
}