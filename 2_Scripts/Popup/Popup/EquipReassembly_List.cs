using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

public class EquipReassembly_List : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Title;
		public Item_Reassembly_List_Element[] Elements;	//0~4 normal , 5~9 special
		public Transform Element;		//Item_Reassembly_List_Element
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;
	ItemInfo m_Info;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (ItemInfo)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		m_SUI.Title.text = string.Format(TDATA.GetString(815), TDATA.GetString(112));
		TReassemblyTable reassemblytable = TDATA.GetReassemblyTable(m_Info.m_TData.m_Type);
		Dictionary<ItemType, List<TItemTable>> tdatas = new Dictionary<ItemType, List<TItemTable>>();
		TReassemblyTable rtdata = TDATA.GetReassemblyTable(m_Info.m_TData.m_Type);
		int probtypesum = rtdata.m_EquipProbs.Sum();// - rtdata.m_EquipProbs.Length;
		int probsum = 0;
		//Ÿ�Ժ��� ��޺� ����Ʈ��, ��üȮ��
		for(ItemType i = ItemType.Blunt;i <= ItemType.Accessory; i++) {
			List<TItemTable> typedata = TDATA.GetItemTypeGroupTable(i).FindAll(o=>o.m_Grade == m_Info.m_Grade);
			probsum += typedata.Sum(o => o.m_ReassemblyProb * rtdata.m_EquipProbs[(int)o.GetEquipType()]);
			ItemType type = i <= ItemType.Rifle ? ItemType.Weapon : i;
			if (!tdatas.ContainsKey(type)) tdatas.Add(type, new List<TItemTable>());
			tdatas[type].AddRange(typedata);
		}
		//probsum /= probtypesum;
		for (int i = 0;i< tdatas.Count;i++) {
			ItemType type = tdatas.ElementAt(i).Key;
			List<TItemTable> datas = tdatas[type];
			List<TItemTable> nmdatas = datas.FindAll(o => o.m_Value == 0);
			List<TItemTable> spdatas = datas.FindAll(o => o.m_Value != 0);

			float[] probs = new float[2];//0:�Ϲ�,1:����
			probs[0] = nmdatas.Sum(o => o.m_ReassemblyProb * rtdata.m_EquipProbs[(int)o.GetEquipType()]);// * rtdata.m_EquipProbs[i] / probtypesum;
			probs[1] = spdatas.Sum(o => o.m_ReassemblyProb * rtdata.m_EquipProbs[(int)o.GetEquipType()]);// * rtdata.m_EquipProbs[i] / probtypesum;

			m_SUI.Elements[i].SetData(type, m_Info.m_Grade, (float)(probs[0] / probsum), true);
			m_SUI.Elements[i + 5].SetData(type, m_Info.m_Grade, (float)(probs[1] / probsum), false);
		}

			//��޺� ���� ������ Ȯ���� Ÿ�Ժ� Ȯ�� �����ְ�
			//������� 5����, �Ϲ���� ���� 7���� �� 4�� �� 16��
			base.SetUI();
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
