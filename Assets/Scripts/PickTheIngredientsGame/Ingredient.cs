using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    private Image IngredientImage;

    private void Awake()
    {
        IngredientImage = GetComponent<Image>();
    }

    public void SetIngredientSprite(Sprite NewSprite)
    {
        if (IngredientImage != null)
        {
            IngredientImage.sprite = NewSprite;
        }
    }

    public void PickUp()
    {
        GameManager.Instance.IngredientCollected();
    }
}