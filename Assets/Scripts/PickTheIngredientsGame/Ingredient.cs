using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    public Sprite IngredientSprite;

    private void Start()
    {
        GetComponent<Image>().sprite = IngredientSprite;
    }
    public void PickUp()
    {
        GameManager.Instance.IngredientCollected();
    }
}
