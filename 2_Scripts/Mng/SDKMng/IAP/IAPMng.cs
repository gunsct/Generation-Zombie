using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using static LS_Web;

public abstract class IFIAP : ClassMng
{
	/// <summary> 초기화 </summary>
	public abstract void Init();

	/// <summary> 아이템 타이틀 </summary>
	public abstract string Get_Title(string pid);
	/// <summary> 아이템 정보 </summary>
	public abstract string Get_Info(string pid);
	/// <summary> 아이템 가격 </summary>
	public abstract string Get_Price(string pid);

	/// <summary> Restore </summary>
	public abstract void Restore(Action _EndCallback);

	/// <summary> 아이템 구매 </summary>
	public abstract void ItemBuy(string puid, string pid, Action<int, string> _EndCallback);
}

public class IAPMng : ClassMng
{
	public const int SUCCESS = 0x0000;  // 성공
	public const int CANCEL = 0xFFFF;
	public const int ERROR = 0xFFFE;

	/// <summary> 가격 정보용
	/// <para> Android는 국가마다 추가로 연동되는 과금 SDK가 있지만</para>
	/// <para> 금액정보는 서버에서 입력하는 곳이 많음 기본 마켓에서 금액 정보를 가져와서 사용</para>
	/// </summary>
	IFIAP m_pPriceInfo;
	/// <summary> 실 구매 SDK 연동 </summary>
	IFIAP m_pBuy;

	static NumberFormatInfo m_NumberFormat;
	public IAPMng()
	{
		// TODO : 여기서 연결될 마켓을 디파인으로 정리할것
		//m_pPriceInfo = m_pBuy = new IAP_UNITY();
	}

	public void Init()
	{
		// 금액 정보 받아오기 초기화 해준다.
		m_pPriceInfo?.Init();
	}

	public string GetTitle(string pid)
	{
		var info = HIVE.GetProductInfo(pid);
		return info == null ? "" : info.title;
		//return m_pPriceInfo == null ? "" : m_pPriceInfo.Get_Title(pid);
	}

	public string GetInfo(string pid)
	{
		var info = HIVE.GetProductInfo(pid);
		return info == null ? "" : info.productDescription;
		//return m_pPriceInfo == null ? "" : m_pPriceInfo.Get_Info(pid);
	}

	public static NumberFormatInfo GetCultureInfoFromISOCurrencyCode(string code)
	{
		if(m_NumberFormat == null)
		{
			foreach (System.Globalization.CultureInfo ci in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
			{
				System.Globalization.RegionInfo ri = new System.Globalization.RegionInfo(ci.LCID);
				if (ri.ISOCurrencySymbol == code)
				{
					m_NumberFormat = ci.NumberFormat;
					break;
				}
			}
			m_NumberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
		}
		return m_NumberFormat;
	}

	public string GetOriginalPrice(RES_SHOP_PID_INFO PID)
	{
		// IAPV4Product.displayOriginalPrice 값은 구글만 제공되므로 사용 불가
		// 예상 문제
		// 통화 포멧을 발경하지 못하면 
		// 디바이스에 표기법으로 찾으므로
		// 실제 할인가격과 다르게 표기될 수 있음
		// 또한 못 찾은 화폐의 금액이 기본 통화 표기와 다를 수 있으므로 해당 문제해결을 위해 환률 계산을 해야될 수도이 있음
		// 가급적이면 사용을 하지 않는 것으로...
		if (PID == null) return "";
		if (string.IsNullOrEmpty(PID.SaleText)) return GetPrice(PID.PID);
		var info = HIVE.GetProductInfo(PID.PID);
		if (info == null) return "";
		var number = GetCultureInfoFromISOCurrencyCode(info.currency);
		double price = info.price;
		if(double.TryParse(PID.SaleText, out double sale)) price /= sale;
		return price.ToString("c", number);
	}

	public string GetPrice(string pid)
	{
		var info = HIVE.GetProductInfo(pid);
		return info == null ? "" : info.displayPrice;
		//return m_pPriceInfo == null ? "" : m_pPriceInfo.Get_Price(pid);
	}

	public void Restore(Action cb)
	{
		m_pBuy?.Restore(cb);
	}

	public void BuyItem(string strPUID, string strPID, Action<int, string> cb)
	{
		m_pBuy?.ItemBuy(strPUID, strPID, cb);
	}
}
