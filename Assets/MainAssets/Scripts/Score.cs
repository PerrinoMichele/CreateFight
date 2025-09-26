using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI livesText;

    void Start()
    {

    }

    void Update()
    {
        int lives = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>().hitPoints;

        livesText.text = $"Lives: {lives}";
        //GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        //int count = 0;
        //foreach (GameObject obj in interactables)
        //{
        //    if (obj.transform.position.y == -1f)
        //    {
        //        count++;
        //    }
        //}

        //scoreText.text = $"Terrain covered:\n{count} / 900";
    }
}
