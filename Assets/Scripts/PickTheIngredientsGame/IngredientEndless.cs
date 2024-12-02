using UnityEngine;
using UnityEngine.UI;

public class IngredientEndless : MonoBehaviour
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
        GameManagerEndless.Instance.IngredientCollected();
    }
}