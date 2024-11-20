using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class AuctionInfo : ClassMng
{
	public List<AuctionItem> Items = new List<AuctionItem>();

	bool IsLoading = false;
	public void Load(Action CB, bool IsJoin)
	{
		if (IsLoading) return;
		IsLoading = true;
		WEB.SEND_REQ_AUCTION_INFO((res) => {
			IsLoading = false;
			if(res.IsSuccess()) Init(res);
			CB?.Invoke();
		}, IsJoin);
	}

	public void Init(RES_AUCTION_INFO data)
	{
		Items.Clear();
		for (int i = data.Infos.Count - 1; i > -1; i--) SetDATA(data.Infos[i]);
	}

	public void SetDATA(RES_AUCTION_ITEM data)
	{
		if (data == null) return;
		AuctionItem info = Items.Find(o => o.m_Uid == data.UID);
		if (info == null)
		{
			info = new AuctionItem();
			Items.Add(info);
			info.SetDATA(data);
		}
		else info.SetDATA(data);
	}


}

