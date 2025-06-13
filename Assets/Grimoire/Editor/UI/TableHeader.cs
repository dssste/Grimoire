using UnityEngine;
using UnityEngine.UIElements;

public class TableHeader : VisualElement
{
    private readonly Label value = new() { name = "Value" };

    public TableHeader(string val, Color background, Color textColor, float textSize, bool rtl, bool bold)
    {
        value.text = val;
        value.pickingMode = PickingMode.Ignore;
        value.style.color = textColor;
        value.style.fontSize = textSize;
        if(bold)
            value.style.unityFontStyleAndWeight = FontStyle.Bold;
        style.backgroundColor = background;
        style.width = Length.Percent(100);
        style.maxHeight = 50;
        if(rtl)
        {
            style.flexDirection = FlexDirection.RowReverse;
            value.style.marginRight = 10;
        }
        else
        {
            style.flexDirection = FlexDirection.Row;
            value.style.marginLeft = 10;
        }
        Add(value);
    }

    public void UpdateValue(string val)
    {
        value.text = val;
    }

    public void SetAsColumn(Color border, float size)
    {
        style.borderBottomColor = border;
        style.borderBottomWidth = size;
    }

    public string Value => value.text;
}
