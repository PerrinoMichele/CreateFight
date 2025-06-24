using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {

    }

    void Update()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        int count = 0;
        foreach (GameObject obj in interactables)
        {
            if (obj.transform.position.y == -1f)
            {
                count++;
            }
        }

        scoreText.text = $"Terrain covered:\n{count} / 900";
    }
}
