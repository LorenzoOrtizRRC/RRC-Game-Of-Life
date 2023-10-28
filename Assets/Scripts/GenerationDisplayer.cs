using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerationDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textDisplay;

    public void DisplayCurrentGeneration(int generationNumber)
    {
        _textDisplay.text = $"Generation: {generationNumber}";
    }
}
