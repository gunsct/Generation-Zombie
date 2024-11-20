using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EFF_MoveCardChange : ObjMng
{
	public virtual void SetData(Vector3 SPos, Vector3 EPos, Action<GameObject> EndCB)
	{
		StartCoroutine(Play(SPos, EPos, EndCB));
	}

	protected virtual IEnumerator Play(Vector3 SPos, Vector3 EPos, Action<GameObject> EndCB)
	{
		EndCB?.Invoke(gameObject);
		Destroy(gameObject);
		yield break;
	}
}
