using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    Image hpBar;
    [SerializeField]
    UnitModel model;

    new Transform camera;


    private void Awake()
    {
        camera = Camera.main.transform;
    }

    private void Update()
    {
        if (hpBar && model)
        {
            hpBar.fillAmount = model.CurrentHP / model.MaxHP;
        }
        transform.LookAt(camera, Vector3.up);
    }
}