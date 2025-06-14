using UnityEngine;
using UnityEngine.UIElements;

public class TableHeader : VisualElement {
	private readonly Label value = new() { name = "Value" };

	public TableHeader(string val) {
		value.text = val;
		value.pickingMode = PickingMode.Ignore;
		style.width = Length.Percent(100);
		style.maxHeight = 50;
		Add(value);
	}

	public void UpdateValue(string val) {
		value.text = val;
	}

	public void SetAsColumn(Color border, float size) {
		style.borderBottomColor = border;
		style.borderBottomWidth = size;
	}

	public string Value => value.text;
}
