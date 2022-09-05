using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamBuilderItem : MonoBehaviour
{
    public UnitModel model;

    public Vector3 targetPos;

    [SerializeField] Image panel;
    [SerializeField] Color blueColor;
    [SerializeField] Color redColor;

    [SerializeField] SliderControl sliderStr;
    [SerializeField] SliderControl sliderCon;
    [SerializeField] SliderControl sliderReg;
    [SerializeField] SliderControl sliderBrb;
    [SerializeField] SliderControl sliderSpd;
    [SerializeField] SliderControl sliderLck;

    //[SerializeField] Toggle etoggle;

    private void Awake()
    {
        if (model && panel)
        {
            GetComponent<Image>().color =
                model.Team == Team.Blue ?
                blueColor
                : redColor;
            //SetModel(false);
        }
        //etoggle.onValueChanged.AddListener(SetModel);
    }

    public void SetModel(bool active)
    {
        model.gameObject.SetActive(active);
    }
    private void Update()
    {
        if (model)
        {
            model.STR = sliderStr.Value;
            model.CON = sliderCon.Value;
            model.REG = sliderReg.Value;
            model.BRB = sliderBrb.Value;
            model.SPD = sliderSpd.Value;
            model.LCK = sliderLck.Value;
        }
    }
}
