using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public int[] itemsAmounts;
    public GameObject[] bulletPreviews;
    public Button woodButton;
    public Button rockButton;
    public Button bombButton;
    public Button metalButton;
    public Image rightHandle;
    public Image cubeButton;
    public int currentMaterialAmount;
    public List<Sprite> sprites;
    public AudioClip clickSound;

    private Image handleChildImage;
    private Image cubeButtonChildImage;
    private InputPlayer inputPlayer;
    private Vector3 originalScale;
    private AudioSource audioSource;

    void Start()
    {
        score = 0;
        audioSource = FindFirstObjectByType<AudioSource>();
        originalScale = rockButton.GetComponentInChildren<TextMeshProUGUI>().transform.localScale;
        inputPlayer = GetComponent<InputPlayer>();
        currentMaterialAmount = itemsAmounts[0];
        UpdateBlockText(0);
        UpdateBlockText(1);
        UpdateBlockText(2);

        woodButton.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        rockButton.transform.localScale = Vector3.one;
        bombButton.transform.localScale = Vector3.one;
        metalButton.transform.localScale = Vector3.one;
        SwitchToWood();

        handleChildImage = rightHandle.transform.Find("BlockImage").GetComponent<Image>();
        cubeButtonChildImage = cubeButton.transform.Find("BlockImage").GetComponent<Image>();
        SetBlockButtonImage(0);
    }

    public void SetBlockButtonImage(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < sprites.Count)
        {
            handleChildImage.sprite = sprites[itemIndex];
            cubeButtonChildImage.sprite = sprites[itemIndex + 4];           
        }
    }

    public void SwitchToWood()
    {
        bulletPreviews[0].SetActive(true);
        bulletPreviews[1].SetActive(false);
        bulletPreviews[2].SetActive(false);

        if (itemsAmounts[0] > 0)
        {
            if (woodButton.transform.localScale != new Vector3(1.3f, 1.3f, 1f))
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton(.5f));
                currentMaterialAmount = itemsAmounts[0];

                woodButton.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                rockButton.transform.localScale = Vector3.one;
                bombButton.transform.localScale = Vector3.one;
                metalButton.transform.localScale = Vector3.one;
                SetBlockButtonImage(0);
            }
            else
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton(.5f));
            }
        }
        else
        {
            // only switch if there is something else to use
            if (itemsAmounts[1] > 0 || itemsAmounts[2] > 0)
                SwitchToRock();
            else
                currentMaterialAmount = 0;  // nothing left
                bulletPreviews[0].SetActive(false);
                bulletPreviews[1].SetActive(false);
                bulletPreviews[2].SetActive(false);
        }
    }

    public void SwitchToRock()
    {
        bulletPreviews[0].SetActive(false);
        bulletPreviews[1].SetActive(true);
        bulletPreviews[2].SetActive(false);

        if (itemsAmounts[1] > 0)
        {
            if (rockButton.transform.localScale != new Vector3(1.3f, 1.3f, 1f))
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton(.5f));
                currentMaterialAmount = itemsAmounts[1];

                rockButton.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                woodButton.transform.localScale = Vector3.one;
                bombButton.transform.localScale = Vector3.one;
                metalButton.transform.localScale = Vector3.one;
                SetBlockButtonImage(1);
            }
            else
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton(.5f));
            }
        }
        else
        {
            if (itemsAmounts[2] > 0 || itemsAmounts[0] > 0)
                SwitchToBomb();
            else
                currentMaterialAmount = 0;
                bulletPreviews[0].SetActive(false);
                bulletPreviews[1].SetActive(false);
                bulletPreviews[2].SetActive(false);
        }
    }

    public void SwitchToBomb()
    {
        bulletPreviews[0].SetActive(false);
        bulletPreviews[1].SetActive(false);
        bulletPreviews[2].SetActive(true);
        //bulletPreviews[2].transform.rotation = Quaternion.identity;

        if (itemsAmounts[2] > 0)
        {
            if (bombButton.transform.localScale != new Vector3(1.3f, 1.3f, 1f))
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton(.5f));
                currentMaterialAmount = itemsAmounts[2];

                woodButton.transform.localScale = Vector3.one;
                rockButton.transform.localScale = Vector3.one;
                bombButton.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                metalButton.transform.localScale = Vector3.one;
                SetBlockButtonImage(2);
            }
            else
            {
                audioSource.PlayOneShot(clickSound);
                inputPlayer.isPressingButton = true;
                StartCoroutine(inputPlayer.ResettingButton(.5f));
            }
        }
        else
        {
            if (itemsAmounts[0] > 0 || itemsAmounts[1] > 0)
                SwitchToWood();
            else
                currentMaterialAmount = 0;
                bulletPreviews[0].SetActive(false);
                bulletPreviews[1].SetActive(false);
                bulletPreviews[2].SetActive(false);
        }
    }

    public void CollectPickup(int materialInventoryNumber)
    {
        itemsAmounts[materialInventoryNumber]++;
        if(itemsAmounts[materialInventoryNumber] == 1 && !bulletPreviews[0].activeInHierarchy && !bulletPreviews[1].activeInHierarchy && !bulletPreviews[2].activeInHierarchy)
        {
            bulletPreviews[materialInventoryNumber].SetActive(true);
        }
        UpdateBlockText(materialInventoryNumber);
    }


    public void UpdateBlockText(int itemIndex)
    {
        if (itemIndex == 0)
        {
            TextMeshProUGUI rockButtonText = woodButton.GetComponentInChildren<TextMeshProUGUI>();

            rockButtonText.text = itemsAmounts[0].ToString();
            if(itemsAmounts[1] == 0 && itemsAmounts[2] == 0) { SwitchToWood(); }       
            //StartCoroutine(PopEffect());
        }

        //UPDATE ROCK COUNTER
        if (itemIndex == 1 && rockButton != null)
        {
            TextMeshProUGUI rockButtonText = rockButton.GetComponentInChildren<TextMeshProUGUI>();
            
            rockButtonText.text = itemsAmounts[1].ToString();
            //StartCoroutine(PopEffect());
        }

        //UPDATE BOMBS NUMBER
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
