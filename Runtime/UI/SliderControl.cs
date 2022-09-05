using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    [SerializeField]
    Text text;
    [SerializeField]
    Slider slider;

    [SerializeField]
    string stat = "STR";

    public int Value => (int)slider.value;

    private void Update()
    {
        text.text = $"{stat}: {Value}";
    }
}