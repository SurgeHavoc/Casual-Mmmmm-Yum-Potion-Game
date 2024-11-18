using UnityEngine;
using System.Collections.Generic;

public class Mortar : MonoBehaviour
{
    public string[] MinigameIngredientList; // List of all nine minigame ingredients.
    public string[] CurrentRandomSequence = new string[3];
    public SpriteRenderer[] SequenceDisplay;
    public GameObject pestle;
    public Dictionary<string, Color> IngredientColors; // Temporary mapping to colors.

    private List<string> DraggedIngredients = new List<string>();
    private int PestleCrushCount = 0;
    private bool IsPestleDraggable = false;
    private Vector3 PestleOriginalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PestleOriginalPosition = pestle.transform.position;

        InitializeColors();
        GenerateRandomSequence();

        SetPestleDraggable(false);
    }

    // Temporary way of matching ingredients to a sequence.
    private void InitializeColors()
    {
        IngredientColors = new Dictionary<string, Color>
        {
            { "Apple", Color.red },
            { "Banana", Color.yellow },
            { "Cherry", new Color(0.5f, 0f, 0f) },
            { "Blueberry", Color.blue },
            { "Grape", new Color(0.5f, 0f, 0.5f) },
            { "Turnip", Color.magenta },
            { "Cabbage", Color.green },
            { "Carrot", new Color(1f, 1f, 0f) },
            { "Orange", new Color(1f, 0.5f, 0f) }
        };
    }

    private void GenerateRandomSequence()
    {
        for (int i = 0; i < CurrentRandomSequence.Length; i++)
        {
            CurrentRandomSequence[i] = MinigameIngredientList[Random.Range(0, MinigameIngredientList.Length)];
        }

        Debug.Log("Generated Sequence: " + string.Join(", ", CurrentRandomSequence));
        UpdateSequenceDisplay();
        SetPestleDraggable(false);
        ResetPestlePosition();
        PestleCrushCount = 0;
    }

    // Temporarily update the colors of the sequence display squares.
    private void UpdateSequenceDisplay()
    {
        for (int i = 0; i < SequenceDisplay.Length; i++)
        {
            if (i < CurrentRandomSequence.Length && IngredientColors.ContainsKey(CurrentRandomSequence[i]))
            {
                SequenceDisplay[i].color = IngredientColors[CurrentRandomSequence[i]];
            }
            else
            {
                SequenceDisplay[i].color = Color.clear;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingredient"))
        {
            MortarIngredient ingredient = collision.GetComponent<MortarIngredient>();
            if (ingredient != null)
            {
                DraggedIngredients.Add(ingredient.IngredientName);
                Debug.Log($"Added {ingredient.IngredientName}. Current list: {string.Join(", ", DraggedIngredients)}");

                collision.transform.position = ingredient.OriginalPosition;

                if (DraggedIngredients.Count == CurrentRandomSequence.Length)
                {
                    CheckSequence();
                }
            }
        }
        else if (collision.CompareTag("Pestle") && IsPestleDraggable)
        {
            PestleCrushCount++;
            Debug.Log($"Crush count: {PestleCrushCount}");

            if (PestleCrushCount >= 5)
            {
                Debug.Log("5 ingredients crushed! Generating new sequence.");
                GenerateRandomSequence();
            }
        }
    }

    private void CheckSequence()
    {
        bool IsMatch = true;
        for (int i = 0; i < CurrentRandomSequence.Length; i++)
        {
            if (DraggedIngredients[i] != CurrentRandomSequence[i])
            {
                IsMatch = false;
                break;
            }
        }

        if (IsMatch)
        {
            Debug.Log("Sequence matched! Pestle now draggable.");
            SetPestleDraggable(true);
        }
        else
        {
            Debug.Log("Sequence did not match. Try again.");
        }

        DraggedIngredients.Clear();
    }

    private void SetPestleDraggable(bool CanDrag)
    {
        IsPestleDraggable = CanDrag;
        var PestleScript = pestle.GetComponent<Pestle>();
        if (PestleScript != null)
        {
            PestleScript.enabled = CanDrag;
        }
    }

    private void ResetPestlePosition()
    {
        pestle.transform.position = PestleOriginalPosition;
    }
}
