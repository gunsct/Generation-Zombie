using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;
public class CollectionInfo : ClassMng
{
	/// <summary> 수집한 정보 (컬렉션 타입, 타입별 인덱스, 등급) </summary>
	public Dictionary<CollectionType, Dictionary<int, int>> Collection = new Dictionary<CollectionType, Dictionary<int, int>>();
	/// <summary> 완료한 컬렉션의 레벨 정보 (컬렉션 인덱스, 레벨) </summary>
	public Dictionary<int, int> CollectionLV = new Dictionary<int, int>();

	public List<TCollectionTable> NowCheckTable = new List<TCollectionTable>();
	public bool IsAlram() => NowCheckTable.Count(o =>o.m_Type != CollectionType.Character && IsSuccess(o)) > 0;

	public void Init(RES_COLLECTION_INFO res = null)
	{
		Collection.Clear();
		CollectionLV.Clear();
		if (res == null)
		{
			ResetCheckCollection();
			return;
		}

		for(int i = 0; i < res.Datas.Count; i++)
		{
			var info = res.Datas[i];
			if (!Collection.ContainsKey(info.Type)) Collection.Add(info.Type, new Dictionary<int, int>());
			if (!Collection[info.Type].ContainsKey(info.Idx)) Collection[info.Type].Add(info.Idx, 0);
			Collection[info.Type][info.Idx] = info.Value;
		}

		for (int i = 0; i < res.LV.Count; i++)
		{
			var info = res.LV[i];
			if (CollectionLV.ContainsKey(info.Idx)) continue;
			CollectionLV.Add(info.Idx, info.LV);
		}
		
		ResetCheckCollection();
	}

	public void Check(CollectionType type, int idx, int grade)
	{
		if (!Collection.ContainsKey(type)) Collection.Add(type, new Dictionary<int, int>());
		if (!Collection[type].ContainsKey(idx)) Collection[type].Add(idx, 0);
		if (Collection[type][idx] < grade) {
			Collection[type][idx] = grade;

			//컬렉션 완료될때 알림, 현재는 안씀
			//if (CollectionLV.Count > 0) {
			//	List<TCollectionGroup> group = TDATA.GetCollectionTypeGroups(type);
			//	List<TCollectionTable> checktables = new List<TCollectionTable>();
			//	for (int i = 0; i < group.Count; i++) {
			//		List<TCollectionTable> tables = group[i].GetCheckList(idx, grade);
			//		if (tables != null) checktables.AddRange(tables);
			//	}
			//	for (int i = 0; i < checktables.Count; i++) {
			//		if (IsSuccess(checktables[i])) {
			//			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(586), checktables[i].GetName(), checktables[i].m_LV + 1));
			//			break;
			//		}
			//	}
			//}
		}
	}

	public List<TCollectionTable> GetList(CollectionType type)
	{
		return NowCheckTable.FindAll(o => o.m_Type == type);
	}
	public List<TCollectionTable> GetSucList(CollectionType tab = CollectionType.END)
	{
		if (tab == CollectionType.END) return NowCheckTable.FindAll(o => o.m_Type != CollectionType.Character && IsSuccess(o));
		return NowCheckTable.FindAll(o => o.m_Type == tab && IsSuccess(o));
	}
	public List<TCollectionTable> GetBuffList(CollectionType type)
	{
		return NowCheckTable.FindAll(o =>
		{
			if (o.m_Type != type) return false;
			if (!CollectionLV.ContainsKey(o.m_Idx)) return false;
			return CollectionLV[o.m_Idx] > 0;
		});
	}

	public int GetMaxLV(int Idx)
	{
		var Group = TDATA.GetCollectionGroup(Idx);
		if (Group == null) return 0;
		return Group.m_MaxLV;
	}

	public bool IsSuccess(TCollectionTable tdata)
	{
		if (tdata.m_Colloets.Count < 1) return false;
		if (CollectionLV.ContainsKey(tdata.m_Idx) && CollectionLV[tdata.m_Idx] >= GetMaxLV(tdata.m_Idx)) return false;
		return !tdata.m_Colloets.Any(o => GetCollectionValue(tdata.m_Type, o) < tdata.m_Grade);
	}
	public void SuccessAlarm(CollectionType _type, int _idx, int _grade) {
		List<TCollectionGroup> group = TDATA.GetCollectionTypeGroups(_type);
		List<TCollectionTable> checktables = new List<TCollectionTable>();
		for(int i = 0; i < group.Count; i++) {
			List<TCollectionTable> tables = group[i].GetCheckList(_idx, _grade);
			if(tables != null) checktables.AddRange(tables);
		}
		for(int i = 0;i< checktables.Count; i++) {
			if(CollectionLV[checktables[i].m_Idx] > checktables[i].m_LV && IsSuccess(checktables[i])) {

				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(568), checktables[i].GetName(), checktables[i].m_Grade));
				break;
			}
		}
	}
	public float Rate(TCollectionTable tdata)
	{
		if (tdata.m_Colloets.Count < 1) return 0f;
		int Cnt = tdata.m_Colloets.Count(o => GetCollectionValue(tdata.m_Type, o) >= tdata.m_Grade);
		return (float)Cnt / (float)tdata.m_Colloets.Count;
	}

	public int Sort(TCollectionTable befor, TCollectionTable after)
	{
		bool bisSuc = IsSuccess(befor);
		bool aisSuc = IsSuccess(after);
		// 성공 순
		if (bisSuc != aisSuc) return aisSuc.CompareTo(bisSuc);
		else if (bisSuc == aisSuc)
		{
			// 달성률 순
			float bRate = Rate(befor);
			float aRate = Rate(after);
			if (bRate != aRate) return aRate.CompareTo(bRate);
		}
		// 인덱스순
		return befor.m_Idx.CompareTo(after.m_Idx);
	}

	public void ResetCheckCollection()
	{
		// 해당 그룹데이터
		List<TCollectionGroup> Group = TDATA.GetCollectionGroups();
		NowCheckTable = Group.Select(o =>
		{
			if (!CollectionLV.ContainsKey(o.m_Idx)) return o.m_List[0];
			var lv = Mathf.Min(o.m_MaxLV, CollectionLV[o.m_Idx]);
			if (!o.m_List.ContainsKey(lv)) return o.m_List[0];
			return o.m_List[lv];
		}).ToList();
	}
	public List<TCollectionTable> GetCompCollection(CollectionType _type) {
		List<TCollectionTable> Group = TDATA.GetCollectionTypeGroups(_type).SelectMany(o=>o.m_List.Values).ToList();
		List<TCollectionTable> nowcheck = GetList(_type);
		for(int i = 0; i < nowcheck.Count; i++) {
			Group.RemoveAll(o => o.m_Idx == nowcheck[i].m_Idx && o.m_LV >= nowcheck[i].m_LV);
		}
		return Group;
	}
	public int GetCollectionValue(CollectionType type, int idx)
	{
		if (!Collection.ContainsKey(type)) return 0;
		if (!Collection[type].ContainsKey(idx)) return 0;
		return Collection[type][idx];
	}

	public float GetStatValue(StatType _stattype, CollectionType _coltype = CollectionType.END)
	{
		List<int> keys = new List<int>(CollectionLV.Keys);
		float re = 0f;
		for(int i = keys.Count - 1; i > -1; i--)
		{
			int key = keys[i];
			if (CollectionLV[key] < 1) continue;
			var tdata = TDATA.GetCollectionTable(key, CollectionLV[key]);
			if (_coltype != CollectionType.END && tdata.m_Type != _coltype) continue;
			if (tdata == null) continue;
			if (tdata.m_Stat == null) continue;
			if (tdata.m_Stat.m_Type != _stattype) continue;
			re += tdata.m_Stat.m_Value;
		}
		return re;
	}

	public void SetEnd(TCollectionTable data)
	{
		if (TDATA.GetCollectionGroup(data.m_Idx).m_MaxLV <= data.m_LV) return;
		if (!CollectionLV.ContainsKey(data.m_Idx)) CollectionLV.Add(data.m_Idx, data.m_LV + 1);
		else CollectionLV[data.m_Idx] = data.m_LV + 1;
	}
}

