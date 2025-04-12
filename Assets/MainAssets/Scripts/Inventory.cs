using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public int[] itemsAmounts;
    public Button woodButton;
    public Button rockButton;

    public int currentMaterialAmount;

    private InputPlayer inputPlayer;

    void Start()
    {
        inputPlayer = GetComponent<InputPlayer>();
        currentMaterialAmount = itemsAmounts[0];
        UpdateBlockText();
        SwitchToWood();

    }


    void Update()
    {
        if (currentMaterialAmount == 0 && rockButton.image.color == Color.white)
        {
            SwitchToWood();
        }
    }

    public void SwitchToRock()
    {
        inputPlayer.isPressingButton = true;
        StartCoroutine(inputPlayer.ResettingButton());
        currentMaterialAmount = itemsAmounts[1];
        rockButton.image.color = Color.white;
        woodButton.image.color = Color.grey;
    }

    public void SwitchToWood()
    {
        inputPlayer.isPressingButton = true;
        StartCoroutine(inputPlayer.ResettingButton());
        currentMaterialAmount = itemsAmounts[0];
        woodButton.image.color = Color.white;
        rockButton.image.color = Color.grey;
    }




    public void UpdateBlockText()
    {
        TextMeshProUGUI rockButtonText = rockButton.GetComponentInChildren<TextMeshProUGUI>();
        rockButtonText.text = "ROCK\n" + itemsAmounts[1].ToString();
    }
}
