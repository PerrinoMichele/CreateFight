using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI text;
    float elapsedTime = 0;
    private GameObject player;
    private bool timerStarted = false;

    void Start()
    {
        player = FindFirstObjectByType<InputPlayer>().gameObject;
    }

    void FixedUpdate()
    {
        if(gameObject.name == "Lives")
        {
            int lives = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>().hitPoints;

            if(lives == 3) { text.text = "♥♥♥"; }
            else if (lives == 2) { text.text = "♥♥"; }
            else if (lives == 1) { text.text = "♥"; }
            //livesText.text = $"♥: {lives}";
        }

        if (gameObject.name == "Time")
        {
            if (player.GetComponent<Inventory>().itemsAmounts[1] >= 3) { timerStarted = true; }
            
            if(timerStarted == true )
            {
                elapsedTime += Time.deltaTime;

                int minutes = Mathf.FloorToInt(elapsedTime / 60f);
                int seconds = Mathf.FloorToInt(elapsedTime % 60f);

                text.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                int numberOfRocks = player.GetComponent<Inventory>().itemsAmounts[1];
                int numberOfWood = player.GetComponent<Inventory>().itemsAmounts[0];

                if (numberOfWood == 0 && numberOfRocks == 0)
                {
                    // completely empty: teach them to start with wood
                    text.text = "GET WOOD";
                }

                else
                {
                    // some wood or some rock, tell them to grab rocks
                    text.text = "GET " + (3 - numberOfRocks) + " ROCKS";
                }
            }

        }


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
