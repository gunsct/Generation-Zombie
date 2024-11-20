using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShopAdviceConditionValue
{
	/// <summary> 조건 타입 </summary>
	public ShopAdviceConditionType m_Type;
	/// <summary> 조건 값</summary>
	public int m_Val;
}
public class TShopAdviceConditionTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 형태 (팝업 일지, 상점 추천 일지) </summary>
	public ShopAdviceCondition m_Type;
	/// <summary> 배너 프리팹 이름 </summary>
	public string m_BannerPrefab;
	/// <summary> 조건 타입과 값 </summary>
	public List<ShopAdviceConditionValue> m_Val = new List<ShopAdviceConditionValue>();
	/// <summary> 확률 </summary>
	public float m_Prob;
	/// <summary> 추천 상품 Index (ShopTable의 Index 참조) </summary>
	public int m_GoodsIdx;
	/// <summary> 판매 종료 타입 </summary>
	public ShopAdviceCloseType m_CloseType;
	/// <summary> 판매 종료 값 </summary>
	public int m_CloseVal;

	public TShopAdviceConditionTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<ShopAdviceCondition>();
		m_BannerPrefab = pResult.Get_String();
		for (int i = 0; i < 3; i++) {
			ShopAdviceConditionType conditype = pResult.Get_Enum<ShopAdviceConditionType>();
			int val = pResult.Get_Int32();
			m_Val.Add(new ShopAdviceConditionValue() { m_Type = conditype, m_Val = val }); 
		}
		m_Prob = pResult.Get_Float();
		m_GoodsIdx = pResult.Get_Int32();
		m_CloseType = pResult.Get_Enum<ShopAdviceCloseType>();
		m_CloseVal = pResult.Get_Int32();
	}
	public bool IS_Condition() {
#if PACKAGE_ALLVIEW
		return true;
#endif
		bool can = true;
		CharInfo charinfo = null;

		for (int i = 0; i < m_Val.Count; i++) {
			switch (m_Val[i].m_Type) {
				///// <summary> Value 값에 해당 하는 계정 레벨 도달 시 </summary>
				case ShopAdviceConditionType.UserLevel:
					if(m_Val[i].m_Val > USERINFO.m_LV) can = false;
					break;
				///// <summary> Value값에 해당 Index의 캐릭터가 미획득 상태일 경우 </summary>
				case ShopAdviceConditionType.NotHaveChar:
					charinfo = USERINFO.GetChar(m_Val[i].m_Val);
					if (charinfo != null) can = false;
					break;
				///// <summary> Value값에 해당 하는 Index의 캐릭터가 획득 상태일 경우 </summary>
				case ShopAdviceConditionType.HaveChar:
					charinfo = USERINFO.GetChar(m_Val[i].m_Val);
					if (charinfo == null) can = false;
					break;
				///// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 Level이 Value값에 도달 했을 경우</summary>
				case ShopAdviceConditionType.CharLevel:
					if (charinfo == null || m_Val[i].m_Val > charinfo.m_LV) can = false;
					break;
				///// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 등급이 Value값에 도달 했을 경우</summary>
				case ShopAdviceConditionType.CharGrade:
					if (charinfo == null || m_Val[i].m_Val > charinfo.m_Grade) can = false;
					break;
				///// <summary> (앞선 획득한 캐릭터/등급과 함께 사용) 목표 캐릭터의 목표 등급에서 최고 레벨 달성 시</summary>
				case ShopAdviceConditionType.MaxLevel:
					if (charinfo == null || charinfo.GradeMaxLv(m_Val[i].m_Val) > charinfo.m_LV) can = false;
					break;
				///// <summary> Value값에 해당하는 스테이지에 진입 </summary>
				case ShopAdviceConditionType.NormalStage:
					if (USERINFO.GetDifficulty() != 0 || USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != m_Val[i].m_Val) can = false;
					break;
				case ShopAdviceConditionType.HardStage:
					if (USERINFO.GetDifficulty() != 1 || USERINFO.m_Stage[StageContentType.Stage].Idxs[1].Idx != m_Val[i].m_Val) can = false;
					break;
				case ShopAdviceConditionType.NightmareStage:
					if (USERINFO.GetDifficulty() != 2 || USERINFO.m_Stage[StageContentType.Stage].Idxs[2].Idx != m_Val[i].m_Val) can = false;
					break;
				///// <summary> Value값에 해당하는 스테이지 클리어 </summary>
				case ShopAdviceConditionType.StageClear:
					if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear < m_Val[i].m_Val) can = false;
					break;
				case ShopAdviceConditionType.HardStageClear:
					if (USERINFO.m_Stage[StageContentType.Stage].Idxs[1].Clear < m_Val[i].m_Val) can = false;
					break;
				case ShopAdviceConditionType.NightmareStageClear:
					if (USERINFO.m_Stage[StageContentType.Stage].Idxs[2].Clear < m_Val[i].m_Val) can = false;
					break;
				///// <summary> Value값에 해당하는 연속 실패 시 </summary>
				case ShopAdviceConditionType.FailCount:
					if (m_Val[i].m_Val > USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount) can = false;
					break;
				///// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 혈청이 Value값에 해당하는 페이지에 도달 시</summary>
				case ShopAdviceConditionType.SerumBlock:
					if (charinfo == null || m_Val[i].m_Val > charinfo.GetSerumBlockCnt()) can = false;
					break;
				///// <summary> 매일 첫 접속 시 </summary>
				case ShopAdviceConditionType.FirstConnect:
					MyFAEvent myevent = USERINFO.m_Event.Datas.Find(o => o.Type == LS_Web.FAEventType.Rot_Attendance);
					if (myevent == null || !myevent.IsReward()) can = false;
					break;
				/// <summary>목표 ShopIndex구매 했을 경우 활성화 </summary>
				case ShopAdviceConditionType.BuyItem:
					if (USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_Val[i].m_Val) == null) can = false;
					break;
				case ShopAdviceConditionType.BuyOneMoreItem:
					var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_Val[i].m_Val);
					if (buyinfo == null || (buyinfo != null && buyinfo.Cnt < 1)) can = false;
					break;
				case ShopAdviceConditionType.Event:
					can = false;
					List<LS_Web.FAEventType> events = BaseValue.EVENT_LIST;
					for (int j = 0; j < events.Count; j++) {
						MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => o.Type == events[i]);
						if(evt != null) {
							switch (events[i]) {
								case LS_Web.FAEventType.Stage_Minigame:
									FAEventData_Stage_Minigame mini = (FAEventData_Stage_Minigame)evt.RealData;
									if (mini.ShopItems.Contains(m_GoodsIdx)) can = true;
									break;
							}
						}
					}
					break;
			}
		}

		switch (m_CloseType) {
			/// <summary> Value 값에 해당 하는 계정 레벨 도달 시 </summary>
			case ShopAdviceCloseType.UserLevel:
				if (m_CloseVal <= USERINFO.m_LV) can = false;
				break;
			/// <summary> Value값에 해당 Index의 캐릭터가 미획득 상태일 경우 </summary>
			case ShopAdviceCloseType.NotHaveChar:
				charinfo = USERINFO.GetChar(m_CloseVal);
				if (charinfo == null) can = false;
				break;
			/// <summary> Value값에 해당 하는 Index의 캐릭터가 획득 상태일 경우 </summary>
			case ShopAdviceCloseType.HaveChar:
				charinfo = USERINFO.GetChar(m_CloseVal);
				if (charinfo != null) can = false;
				break;
			/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 Level이 Value값에 도달 했을 경우 </summary>
			case ShopAdviceCloseType.CharLevel:
				if (charinfo == null || m_CloseVal <= charinfo.m_LV) can = false;
				break;
			/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 등급이 Value값에 도달 했을 경우 </summary>
			case ShopAdviceCloseType.CharGrade:
				if (charinfo == null || m_CloseVal <= charinfo.m_Grade) can = false;
				break;
			/// <summary> (앞선 획득한 캐릭터/등급과 함께 사용) 목표 캐릭터의 목표 등급에서 최고 레벨 달성 시 </summary>
			case ShopAdviceCloseType.MaxLevel:
				if (charinfo == null || charinfo.GradeMaxLv(m_CloseVal) <= charinfo.m_LV) can = false;
				break;
			/// <summary> Value값에 해당하는 노말 스테이지에 진입 </summary>
			case ShopAdviceCloseType.NormalStage:
				if (USERINFO.GetDifficulty() != 0 || USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx == m_CloseVal) can = false;
				break;
			/// <summary> Value값에 해당하는 나이트메어 스테이지에 진입 </summary>
			case ShopAdviceCloseType.HardStage:
				if (USERINFO.GetDifficulty() != 1 || USERINFO.m_Stage[StageContentType.Stage].Idxs[1].Idx == m_CloseVal) can = false;
				break;
			/// <summary> Value값에 해당하는 아포칼립스 스테이지에 진입 </summary>
			case ShopAdviceCloseType.NightmareStage:
				if (USERINFO.GetDifficulty() != 2 || USERINFO.m_Stage[StageContentType.Stage].Idxs[2].Idx == m_CloseVal) can = false;
				break;
			///// <summary> Value값에 해당하는 스테이지 클리어 </summary>
			case ShopAdviceCloseType.StageClear:
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear >= m_CloseVal) can = false;
				break;
			case ShopAdviceCloseType.HardStageClear:
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[1].Clear >= m_CloseVal) can = false;
				break;
			case ShopAdviceCloseType.NightmareStageClear:
				if ( USERINFO.m_Stage[StageContentType.Stage].Idxs[2].Clear >= m_CloseVal) can = false;
				break;
			/// <summary> Value값에 해당하는 연속 실패 시 </summary>
			case ShopAdviceCloseType.FailCount:
				if (m_CloseVal > USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount) can = false;
				break;
			/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 혈청이 Value값에 해당하는 페이지에 도달 시 </summary>
			case ShopAdviceCloseType.SerumBlock:
				if (charinfo == null || m_CloseVal <= charinfo.GetSerumBlockCnt()) can = false;
				break;
			case ShopAdviceCloseType.BuyOneMoreItem:
				var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_CloseVal);
				if (buyinfo != null && buyinfo.Cnt > 0) can = false;
				break;
		}
		if (can && UTILE.Get_Random(0f, 1f) > m_Prob) can = false;
		return can;
	}
}
public class TShopAdviceConditionTableMng : ToolFile
{
	public Dictionary<int, TShopAdviceConditionTable> DIC_Idx = new Dictionary<int, TShopAdviceConditionTable>();
	public List<TShopAdviceConditionTable> Datas = new List<TShopAdviceConditionTable>();

	public TShopAdviceConditionTableMng() : base("Datas/ShopAdviceConditionTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TShopAdviceConditionTable data = new TShopAdviceConditionTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ShopAdviceConditionTable
	TShopAdviceConditionTableMng m_ShopAdiveCondtion = new TShopAdviceConditionTableMng();

	public TShopAdviceConditionTable GetShopAdviceConditionTable(int _idx)
	{
		if (!m_ShopAdiveCondtion.DIC_Idx.ContainsKey(_idx)) return null;
		return m_ShopAdiveCondtion.DIC_Idx[_idx];
	}

	public Dictionary<int, TShopAdviceConditionTable> GetAllShopAdviceConditionTable()
	{
		return m_ShopAdiveCondtion.DIC_Idx;
	}

	public List<TShopAdviceConditionTable> GetCanAdviceTables(ShopAdviceCondition _type) {
		//condition 조건 되고 구매정보 없거나 카운트 0인것들만
		return m_ShopAdiveCondtion.Datas.FindAll(o => o.m_Type == _type && o.IS_Condition() && (USERINFO.m_ShopInfo.BUYs.Find(t=>t.Idx == o.m_GoodsIdx && (TDATA.GetShopTable(o.m_GoodsIdx).m_LimitCnt != 0 ? t.Cnt >= TDATA.GetShopTable(o.m_GoodsIdx).m_LimitCnt : false)) == null));
	}
	public TShopAdviceConditionTable GetShopAdviceTableToSIdx(int _sidx) {
		return m_ShopAdiveCondtion.Datas.Find(o => o.m_GoodsIdx == _sidx);
	}
}