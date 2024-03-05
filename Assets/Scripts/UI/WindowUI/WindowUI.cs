using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowUI : BaseUI, IDragHandler, IPointerDownHandler
{
	protected override void Awake()
	{
		base.Awake();

		//buttons["CloseButton"].onClick.AddListener(() => { CloseUI(); });
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position += (Vector3)eventData.delta;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		GameManager.UI.SelectWindowUI(this);
	}

	public override void CloseUI()
	{
		base.CloseUI();

		GameManager.UI.CloseWindowUI(this);
	}
}
