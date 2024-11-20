using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecommendGoodsInfo : ClassMng
{
	public int m_SIdx;      //샵테이블 인덱스
	public double m_UTime;  //뽑힌 시간, UTILE.Get_ServerTime()

	[JsonIgnore] public TShopTable m_STData { get { return TDATA.GetShopTable(m_SIdx); } }
	[JsonIgnore] public TShopAdviceConditionTable m_SSACTData { get { return TDATA.GetShopAdviceTableToSIdx(m_SIdx); } }
	[JsonIgnore] public double GetRemainTime { get { return m_UTime + (m_SSACTData == null ? 0 : m_SSACTData.m_CloseVal) * 60 - UTILE.Get_ServerTime(); } }
	/// <summary> 기존에 없거나 하루 지났거나 구매 안했거나</summary>
	[JsonIgnore]
	public bool IS_CanListUp { get {
		return (m_UTime == 0 || !UTILE.IsSameDay((long)m_UTime))
			&& USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_SIdx && ((m_STData.m_LimitCnt != 0 ? o.Cnt >= m_STData.m_LimitCnt : false) || o.GetTime() > 0)) == null
			&& USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == m_SIdx && o.GetLastTime() * 0.001d > (o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE ? 86400 * 3 : 0)) == null;
		} }
}
