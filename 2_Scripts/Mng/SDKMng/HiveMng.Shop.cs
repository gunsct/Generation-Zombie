using hive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static hive.AuthV4;
using static hive.IAPV4;
using static LS_Web;

public partial class HiveMng : ClassMng
{
	Dictionary<string, IAPV4Product> m_DicProductInfoList;
	List<IAPV4Product> m_ProductInfoList;
	public IAPV4Product GetProductInfo(string PID)
	{
		if (m_DicProductInfoList == null) return null;
		if (!m_DicProductInfoList.ContainsKey(PID)) return null;
		return m_DicProductInfoList[PID];
	}

	public void ShopInit(Action CB)
	{
		// Hive IAP v4의 초기화 요청
		hive.IAPV4.marketConnect((ResultAPI result, List<IAPV4Type> marketIdList) =>
		{
			Utile_Class.DebugLog($"ShopInit result : {result.toString()}\nmarketIdList : {marketIdList}");
			if (result.isSuccess())
			{
				// 소모성 상품 목록 조회 요청
				hive.IAPV4.getProductInfo(onIAPV4ProductInfoCB);
			}
			else
			{
				// Error Handling
			}
			CB?.Invoke();
		});
	}

	// 소모성 상품 목록 조회 결과 콜백 핸들러
	public void onIAPV4ProductInfoCB(ResultAPI result, List<IAPV4Product> productInfoList, int balance)
	{
		Utile_Class.DebugLog($"ShopInit result : {result.toString()}\nproductInfoList : {productInfoList}\nbalance : {balance}");
		if (result.isSuccess())
		{
			if(productInfoList != null && productInfoList.Count > 0)
			{
				m_DicProductInfoList = productInfoList.ToDictionary(o => o.marketPid);

				//foreach (var info in m_DicProductInfoList)
				//{
				//	Utile_Class.DebugLog($"!!!!!!!!!!!!!!!!!!!!!!!!  {info.Key} : {info.Value}");

				//}
			}
			//foreach (hive.IAPV4.IAPV4Product productInfo in productInfoList)
			//{

			//	if(productInfo == null) Utile_Class.DebugLog($"!!!!!!!!!!!!!!!!!!!!!!!!  productInfo NULL !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			//	else Utile_Class.DebugLog($"!!!!!!!!!!!!!!!!!!!!!!!!  {productInfo.marketPid} : {productInfo.price}");

			//}
		}
		else
		{
			// Error Handling
		}

	}


	public void ShopBuy(int idx, string PID, Action<RES_BASE> CB)
	{
		if (!WEB.CheckNetState(true)) return;
		POPUP.LockConnecting(true);
		WEB.SEND_REQ_SHOP_PUID((res) =>
		{
			if(!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			ShopBuy(res.PayLoad, PID, (result, receipt) =>
			{
				if (result.isSuccess())
				{
					WEB.SEND_REQ_SHOP_BUY_INAPP((res) => {
						if (res.IsSuccess())
						{
							// 지급 완료된 아이템의 완료 요청
							FinishBuy(PID, (finishres, pid) =>
							{
								CB?.Invoke(res);
							});
						}
						else
						{
							POPUP.LockConnecting(false);
							CB?.Invoke(res);
						}
					}, APPINFO.Market, receipt == null ? "" : receipt.bypassInfo, res.PayLoad);
				}
				else
				{

					MainMng.Instance.StartCoroutine(ErrorMsg(result, CB));
					// Error Handling
					//NEED_INITIALIZE 초기화 진행 필요
					//NETWORK 네트워크 에러
					//NOT_SUPPORTED   구매 불가 상태(기기 앱 내 구입 차단 등), 지원되지 않는 마켓 설정 시
					//INVALID_SESSION 구매를 진행할 수 없는 사용자 세션
					//INVALID_PARAM 파라미터 오류
					//IN_PROGRESS purchase API가 이미 호출됨
					//ITEM_PENDING 오프라인 결제 시도
					//CANCELED 사용자에 의한 취소
					//NEED_RESTORE restore API 호출 필요
					//RESPONSE_FAIL   기타 오류. Hive IAP 서버 오류
				}
			});
		}, idx);
	}

	IEnumerator ErrorMsg(ResultAPI result, Action<RES_BASE> CB)
	{
		// 랜더링 프로세스 맞추기 위해 한프레임 쉬어줌
		yield return new WaitForEndOfFrame();
		POPUP.LockConnecting(false);
		Analytics_add_to_cart();
		switch (result.errorCode)
		{
		case ResultAPI.ErrorCode.NOT_SUPPORTED:
			POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(2113));
			break;
		case ResultAPI.ErrorCode.CANCELED:
			break;
		default:
			//MAIN.StartCoroutine(Restore(CB));
			//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, result.message);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2114));
			break;
		}
		CB?.Invoke(new RES_BASE() { result_code = EResultCode.ERROR_SHOP_BUY_MARKET_ERROR });
	}

	/// <summary> 구매 </summary>
	public void ShopBuy(string PayLoad, string PID, Action<ResultAPI, IAPV4Receipt> CB)
	{
		IAPV4.purchase(PID, PayLoad, (ResultAPI result, IAPV4Receipt receipt) =>
		{
			if (receipt != null) Utile_Class.DebugLog($"purchase result : {result.toString()}\nreceipt : {receipt}\npayload : {receipt.iapPayload}");
			else Utile_Class.DebugLog($"purchase result : {result.toString()}\nreceipt : {receipt}");
			CB?.Invoke(result, receipt);
		});
	}

	void FinishBuy(string PID, Action<ResultAPI, String> CB = null)
	{
		// 지급 완료된 아이템의 완료 요청
		hive.IAPV4.transactionFinish(PID, (ResultAPI result, String marketPid) => {
			Utile_Class.DebugLog($"transactionFinish result : {result.toString()}\nreceipt : {marketPid}");
			POPUP.LockConnecting(false);
			CB?.Invoke(result, marketPid);
		});
	}

	void FinishBuyMulty(List<string> PID, Action<List<ResultAPI>, List<String>> CB = null)
	{
		// 지급 완료된 아이템의 완료 요청
		hive.IAPV4.transactionMultiFinish(PID, (List<ResultAPI> resultList, List<String> marketPidList) => {
			Utile_Class.DebugLog($"transactionMultiFinish result : {resultList}\nreceipt : {marketPidList}");
			CB?.Invoke(resultList, marketPidList);
		});
	}

	public IEnumerator Restore(Action<List<RES_REWARD_BASE>> CB = null)
	{
		List<IAPV4Receipt> resList = null;
		bool LoadEnd = false;
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		yield return CallRenderThread();
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		int finishcnt = 0;
		// 미 지급된 소모성 아이템의 영수증 정보 요청
		hive.IAPV4.restore((ResultAPI result, List<IAPV4Receipt> receiptList) => {
			POPUP.SetConnecting(false);
			if (result.isSuccess())
			{
				resList = receiptList;
				foreach (hive.IAPV4.IAPV4Receipt receipt in receiptList)
				{
					// 전달받은 영수증으로 영수증 검증 요청
					WEB.SEND_REQ_SHOP_BUY_INAPP((res) => {
						if(!res.IsSuccess())
						{
							finishcnt++;
							return;
						}
						// 지급 완료된 아이템의 완료 요청
						FinishBuy(receipt.product.marketPid, (finishres, pid) => {
							finishcnt++;
						});
						rewards.AddRange(res.GetRewards());
					}, APPINFO.Market, receipt.bypassInfo, string.Empty);
				}
			}
			else if (result.errorCode == ResultAPI.ErrorCode.NOT_OWNED)
			{
				// 미지급된 아이템 없음
			}
			else
			{
				// Error Handling
				//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, result.message);
			}
			LoadEnd = true;
		});

		yield return new WaitUntil(() => LoadEnd);
		yield return new WaitWhile(() => resList != null && resList.Count != finishcnt);
		CB?.Invoke(rewards);
		//if(resList != null)
		//{
		//	foreach (var receipt in resList)
		//	{
		//		WEB.SEND_REQ_SHOP_BUY_INAPP((res) => {
		//			 지급 완료된 아이템의 완료 요청
		//			FinishBuy(PID);
		//		}, idx, receipt.iapPayload, APPINFO.Market, receipt.bypassInfo);
		//	}
		//}
	}
}

