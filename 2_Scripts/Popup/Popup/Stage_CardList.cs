using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

public class Stage_CardList : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Title;
		public Transform[] Buckets;		//0:new,1:enemy,2:all
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		if(STAGEINFO.m_StageContentType == StageContentType.Stage)
			m_SUI.Title.text = string.Format("{0} {1} - {2} {3}", TDATA.GetString(162), STAGEINFO.m_Idx / 100, TDATA.GetString(83), STAGEINFO.m_Idx % 100);
		else
			m_SUI.Title.text = string.Format("{0} {1}", TDATA.GetString(83), STAGEINFO.m_Idx % 100); 
		Item_Stage_CardListElement element = null;
		//등장한 모드
		List<PlayType> PlayTypes = new List<PlayType>();
		PlayTypes.AddRange(STAGEINFO.m_TStage.m_PlayType.Select(o => o.m_Type));
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.TurmoilCount) PlayTypes.Add(PlayType.TurmoilCount);
		if (STAGEINFO.m_TStage.m_APRecovery == 0) PlayTypes.Add(PlayType.APRecvZero);

		for (int i = 0;i< PlayTypes.Count; i++) {
			bool first = TDATA.GetStageFirstPlayType(PlayTypes[i], STAGEINFO.m_TStage.m_DifficultyType, STAGEINFO.m_Idx);
			element = UTILE.LoadPrefab("Item/Item_Stage_CardListElement", true, m_SUI.Buckets[first ? 0 : 2]).GetComponent<Item_Stage_CardListElement>();
			BaseValue.StagePlayTypeInfo info = BaseValue.GetStagePlayTypeInfo(PlayTypes[i]);
			element.SetData(info.m_Icon, info.m_Name, new List<string>() { info.m_Desc }, first);
		}
		//스테이지에 등장하는 모드
		//새로 등장한 카드
		List<TStageGuideTable> newdatas = STAGEINFO.m_TStage.m_DifficultyType == StageDifficultyType.Normal ? TDATA.GetStageGuideTable(STAGEINFO.m_Idx) : null;
		if (newdatas != null) {
			for (int i = 0; i < newdatas.Count; i++) {
				element = UTILE.LoadPrefab("Item/Item_Stage_CardListElement", true, m_SUI.Buckets[0]).GetComponent<Item_Stage_CardListElement>();
				element.SetData(newdatas[i].m_CardIdx, true);
			}
		}
		//스테이지에 등장하는 카드
		List<TStageCardTable> alldata = TDATA.GetStageCardGroup(STAGEINFO.m_StageContentType == StageContentType.Subway ? 1 : STAGEINFO.m_LV).FindAll(o=> (newdatas == null ? true : newdatas.Find(r=>r.m_CardIdx != o.m_Idx) != null) && (o.m_Prob > 0 || o.m_DarkProb > 0));
		alldata.Sort((TStageCardTable _b, TStageCardTable _a) => {
			if(_b.m_Name != _a.m_Name) return _b.m_Name.CompareTo(_a.m_Name);
			if (_b.m_Value1 == _a.m_Value1) return _b.m_Value2.CompareTo(_a.m_Value2);
			if (_b.m_Type == StageCardType.Enemy && _a.m_Type == StageCardType.Enemy) return _b.m_Value1.CompareTo(_a.m_Value1);
			return _b.m_Type.CompareTo(_a.m_Type);
		});
		List<TStageCardTable> containdata = new List<TStageCardTable>();
		for (int i = 0; i < alldata.Count; i++) {
			switch (alldata[i].m_Type) {
				//case StageCardType.SupplyBox:
				case StageCardType.Supplybox02:
				case StageCardType.Item_RewardBox:
				case StageCardType.Ash:
				case StageCardType.Fire:
				case StageCardType.Hive:
				case StageCardType.OldMine:
				case StageCardType.Allymine:
				case StageCardType.TimeBomb:
				case StageCardType.TornBody:
				case StageCardType.Garbage:
				case StageCardType.Pit:
					if (containdata.Find(o => o.m_Type == alldata[i].m_Type) != null) continue;
					break;
			}
			if(alldata[i].m_Type == StageCardType.SupplyBox) {
				if (containdata.Find(o => o.m_Name == alldata[i].m_Name && o.m_Info == alldata[i].m_Info) != null) continue;
			}
			else if (containdata.Find(o => o.m_Name == alldata[i].m_Name && o.m_Info == alldata[i].m_Info && o.m_Type == alldata[i].m_Type && o.m_Value1 == alldata[i].m_Value1 && (IS_NotUseVal2(o) ? true : o.m_Value2 == alldata[i].m_Value2)) != null) continue;
			element = UTILE.LoadPrefab("Item/Item_Stage_CardListElement", true, m_SUI.Buckets[alldata[i].m_Type == StageCardType.Enemy ? 1 : 2]).GetComponent<Item_Stage_CardListElement>();
			element.SetData(alldata[i].m_Idx, false);
			containdata.Add(alldata[i]);
		}
		for(int i = 0;i< m_SUI.Buckets.Length;i++) m_SUI.Buckets[i].gameObject.SetActive(m_SUI.Buckets[i].childCount > 1);
	}
	bool IS_NotUseVal2(TStageCardTable _tdata) {
		switch (_tdata.m_Type) {
			case StageCardType.Oil:
			case StageCardType.GasStation:
				return true;
			case StageCardType.Enemy:
				TEnemyTable edata = TDATA.GetEnemyTable((int)_tdata.m_Value1);
				if (edata.m_MoveAI.m_Type == EEnemyAIMoveType.Arsonist01 && TDATA.GetStageCardTable((int)_tdata.m_Value2)?.m_Type == StageCardType.Fire) {
					return true;
				}
				else return false;
			default:return false;
		}
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
