using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public void PickUp()
    {
        GameManager.Instance.IngredientCollected();
    }
}
