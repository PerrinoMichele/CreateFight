using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int[] itemsAmounts;
    public Button woodButton;
    public Button rockButton;
    public Button bombButton;
    public Button metalButton;
    public Image rightHandle;
    public int currentMaterialAmount;
    public List<Sprite> sprites;
    public AudioClip clickSound;

    private Image childImage;
    private InputPlayer inputPlayer;
    private Vector3 originalScale;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = FindFirstObjectByType<AudioSource>();
        originalScale = rockButton.GetComponentInChildren<TextMeshProUGUI>().transform.localScale;
        inputPlayer = GetComponent<InputPlayer>();
        currentMaterialAmount = itemsAmounts[0];
        UpdateBlockText(1);
        UpdateBlockText(2);

        woodButton.image.color = Color.white;
        rockButton.image.color = Color.grey;
        bombButton.image.color = Color.grey;
        metalButton.image.color = Color.grey;
        SwitchToWood();

        childImage = rightHandle.transform.Find("BlockImage").GetComponent<Image>();
        SetBlockButtonImage(0);
    }

    public void SetBlockButtonImage(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < sprites.Count)
        {
            childImage.sprite = sprites[itemIndex];
        }
    }

    public void SwitchToWood()
    {
        inputPlayer.isPressingButton = true;
        StartCoroutine(inputPlayer.ResettingButton());
        currentMaterialAmount = itemsAmounts[0];
        if(woodButton.image.color != Color.white)
        {
            audioSource.PlayOneShot(clickSound);
            woodButton.image.color = Color.white;
            rockButton.image.color = Color.grey;
            bombButton.image.color = Color.grey;
            metalButton.image.color = Color.grey;
            SetBlockButtonImage(0);
        }
        else
        {
            inputPlayer.buildBlock();
        }
    }

    public void SwitchToRock()
    {
        if (itemsAmounts[1] > 0)
        {
            if (rockButton.image.color != Color.white)
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton());
                currentMaterialAmount = itemsAmounts[1];
                rockButton.image.color = Color.white;
                woodButton.image.color = Color.grey;
                bombButton.image.color = Color.grey;
                metalButton.image.color = Color.grey;
                SetBlockButtonImage(1);
            }
            else
            {
                inputPlayer.buildBlock();
            }
        }
    }

    public void SwitchToBomb()
    {
        if (itemsAmounts[2] > 0)
        {
            if (bombButton.image.color != Color.white)
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton());
                currentMaterialAmount = itemsAmounts[2];
                woodButton.image.color = Color.grey;
                rockButton.image.color = Color.grey;
                bombButton.image.color = Color.white;
                metalButton.image.color = Color.grey;
                SetBlockButtonImage(2);
            }
            else
            {
                inputPlayer.buildBlock();
            }
        }
    }




    public void UpdateBlockText(int itemIndex)
    {
        if(itemIndex == 1 && rockButton != null)
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
        
        Vector3 targetScale = originalScale * 3;

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
