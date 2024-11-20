using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

[System.Serializable] public class DicImgNumber : SerializableDictionary<string, Sprite> { }
// EditMode Prefab으로 만들어서 사용할 경우 오브젝트 생성해서 사용하는 방식이므로 에러가 발생함(프리팹은 수정할 수없음 해당 프리팹을 열어 수정해야됨)
//[ExecuteInEditMode]
public class ImgNumber : ObjMng
{
    [Serializable] 
    public struct SUI
	{
		public RectTransform Panel;
		public DicImgNumber Img;

		public Color color;

		public int Value;
	}

	[SerializeField] SUI m_SUI;
	List<Image> m_NoList = new List<Image>();
#if UNITY_EDITOR
	void OnValidate()
	{
		UnityEditor.EditorApplication.delayCall += () =>
		{
			SetValue();
		};
	}
#endif

	private void Update()
	{
		SetColor(m_SUI.color);
	}
	public void SetColor(Color color)
	{
		for (int i = m_NoList.Count - 1; i > -1; i--)
		{
			m_NoList[i].color = color;
		}
	}

	public void SetValue(int Value)
	{
		m_SUI.Value = Value;
		SetValue();
	}

	public void SetValue()
	{
		if (m_SUI.Panel == null) return;
		m_NoList.Clear();
		string value = m_SUI.Value.ToString();
		int length = value.Length;
		LoadObj(length);
		int val = m_SUI.Value;
		for (int i = length - 1; i  > -1; i--)
		{
			var sub = value.Substring(i, 1);
			var img = m_SUI.Panel.GetChild(i).GetComponent<Image>();
			if(!m_SUI.Img.ContainsKey(sub))
			{
				img.gameObject.SetActive(false);
				continue;
			}
			m_NoList.Add(img);
			img.gameObject.SetActive(true);
			img.sprite = m_SUI.Img[sub];
			img.color = m_SUI.color;
			img.SetNativeSize();
		}
	}

	void LoadObj(int nCnt)
	{
		Transform tfList = m_SUI.Panel;
		if (tfList == null) return;
		int i;
		int nChildCnt = tfList.childCount;
		int nGap = nCnt - nChildCnt;
		if (nGap < 0)
		{
			// 차이 개수 만큼 차감
			// 처음 위치부터 바로 셋팅해야하는데 바로 삭제가 되지않아 셋팅이 잘못되는경우가 있어 뒤에서부터 삭제한다.
			for (i = nChildCnt - 1; i >= nChildCnt + nGap; i--) 
			{
				var obj = tfList.GetChild(i).gameObject;
#if UNITY_EDITOR
				if (Application.isPlaying) Destroy(obj);
				else DestroyImmediate(obj, true);
#else
				Destroy(obj);
#endif
			}
		}
		else
		{
			// 차이 개수만큼 생성
			for (i = 0; i < nGap; i++)
			{
				GameObject emptyGameObject = new GameObject($"Num_{nChildCnt + i}");
				Image img = emptyGameObject.AddComponent<Image>();
				img.raycastTarget = false;
				// SetParent() 호출시 Instantiate 하면서 프리팹생성에서 에러 발생하므로 사용 금지
				//emptyGameObject.transform.parent = tfList;
				emptyGameObject.transform.SetParent(tfList, false);
			}
		}
	}
}

