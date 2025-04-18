using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class Inventory : MonoBehaviour
{
    public int[] itemsAmounts;
    public Button woodButton;
    public Button rockButton;
    public Button bombButton;
    public Button metalButton;

    public int currentMaterialAmount;

    private InputPlayer inputPlayer;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = rockButton.GetComponentInChildren<TextMeshProUGUI>().transform.localScale;
        inputPlayer = GetComponent<InputPlayer>();
        currentMaterialAmount = itemsAmounts[0];
        //UpdateBlockText(1);
        SwitchToWood();
    }

    void Update()
    {
        int rockAmount = itemsAmounts[1];
        if (rockAmount == 0 && rockButton.image.color == Color.white)
        {
            SwitchToWood();
        }
        int bombAmount = itemsAmounts[2];
        if (bombAmount == 0 && bombButton.image.color == Color.white)
        {
            SwitchToWood();
        }
    }

    public void SwitchToWood()
    {
        inputPlayer.isPressingButton = true;
        StartCoroutine(inputPlayer.ResettingButton());
        currentMaterialAmount = itemsAmounts[0];
        woodButton.image.color = Color.white;
        rockButton.image.color = Color.grey;
        bombButton.image.color = Color.grey;
        metalButton.image.color = Color.grey;
    }

    public void SwitchToRock()
    {
        inputPlayer.isPressingButton = true;
        StartCoroutine(inputPlayer.ResettingButton());
        currentMaterialAmount = itemsAmounts[1];
        rockButton.image.color = Color.white;
        woodButton.image.color = Color.grey;
        bombButton.image.color = Color.grey;
        metalButton.image.color = Color.grey;
    }

    public void SwitchToBomb()
    {
        inputPlayer.isPressingButton = true;
        StartCoroutine(inputPlayer.ResettingButton());
        currentMaterialAmount = itemsAmounts[2];
        woodButton.image.color = Color.grey;
        rockButton.image.color = Color.grey;
        bombButton.image.color = Color.white;
        metalButton.image.color = Color.grey;
    }




    public void UpdateBlockText(int itemIndex)
    {
        if(itemIndex == 1)
        {
            TextMeshProUGUI rockButtonText = rockButton.GetComponentInChildren<TextMeshProUGUI>();
            rockButtonText.text = itemsAmounts[1].ToString();
            StartCoroutine(PopEffect());
        }
        if (itemIndex == 2)
        {
            TextMeshProUGUI rockButtonText = bombButton.GetComponentInChildren<TextMeshProUGUI>();
            rockButtonText.text = itemsAmounts[2].ToString();
            //StartCoroutine(PopEffect());
        }

    }

    private IEnumerator PopEffect()
    {
        int currentMaterialIndex = System.Array.IndexOf(itemsAmounts, currentMaterialAmount);

        TextMeshProUGUI rockButtonText = rockButton.GetComponentInChildren<TextMeshProUGUI>();
        
        Vector3 targetScale = originalScale * 6;

        // Scale up
        float t = 0;
        while (t < .2f / 2f)
        {
            t += Time.deltaTime;
            rockButtonText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / (.2f / 2f));
            yield return null;
        }

        // Scale down
        t = 0;
        while (t < .2f / 2f)
        {
            t += Time.deltaTime;
            rockButtonText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / (.2f / 2f));
            yield return null;
        }

        rockButtonText.transform.localScale = originalScale;
    }
}
