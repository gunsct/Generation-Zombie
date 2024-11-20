using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Store_Select : PopupBase
{
    public enum State
	{
        Shop,
        Post
	}
    [Serializable]
    public struct SUI
	{
        public TextMeshProUGUI TItle;
        public Image[] TitleIcon;
        public Transform Element;           //Item_Store_GachaMileage_Select_Element
        public Transform Bucket;
	}
    [SerializeField] SUI m_SUI;
    int m_Idx;
    State m_State;
    Action<int, List<int>> m_CB;

    TShopTable m_TData;
    TItemTable m_ItData;
    Item_Store_Select_ListElement m_SelectElement;

    public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
        m_Idx = (int)aobjValue[0];
        m_State = (State)aobjValue[1];
        if(m_State == State.Shop) {
            m_TData = TDATA.GetShopTable(m_Idx);
            m_ItData = TDATA.GetItemTable(m_TData.m_Rewards[0].m_ItemIdx);
        }
		else {
            m_ItData = TDATA.GetItemTable(m_Idx);
        }
        m_CB = (Action<int, List<int>>)aobjValue[2];

        base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
        m_SUI.TItle.text = m_ItData.GetName();

        GachaGroup gachagroup = TDATA.GetGachaGroup(m_ItData.m_Value);
        List<TGachaGroupTable> gdatas = gachagroup.m_List;

        for (int i = 0; i < m_SUI.TitleIcon.Length; i++) {
            Sprite icon = null;
            switch (gdatas[0].MRewardKind) {
                case RewardKind.Character:
                    icon = UTILE.LoadImg(string.Format("Card/Item/Item_Mile_Select_Surv{0}", m_ItData.m_Grade), "png");
                    if (icon == null) icon = UTILE.LoadImg("Card/Item/Item_Mile_Select_Surv", "png");
                    break;
                case RewardKind.Item:
                    switch (TDATA.GetItemTable(gdatas[0].m_RewardIdx).GetGroupType()) {
                        case ItemType.Helmet:
                            icon = UTILE.LoadImg(string.Format("Card/Item/Item_Mile_Select_Helmet{0}", m_ItData.m_Grade), "png");
                            if (icon == null) icon = UTILE.LoadImg("Card/Item/Item_Mile_Select_Helmet", "png");
                            break;
                        case ItemType.Costume:
                            icon = UTILE.LoadImg(string.Format("Card/Item/Item_Mile_Select_Costume{0}", m_ItData.m_Grade), "png");
                            if (icon == null) icon = UTILE.LoadImg("Card/Item/Item_Mile_Select_Costume", "png");
                            break;
                        case ItemType.Shoes:
                            icon = UTILE.LoadImg(string.Format("Card/Item/Item_Mile_Select_Shoes{0}", m_ItData.m_Grade), "png");
                            if (icon == null) icon = UTILE.LoadImg("Card/Item/Item_Mile_Select_Shoes", "png");
                            break;
                        case ItemType.Accessory:
                            icon = UTILE.LoadImg(string.Format("Card/Item/Item_Mile_Select_Accessory{0}", m_ItData.m_Grade), "png");
                            if (icon == null) icon = UTILE.LoadImg("Card/Item/Item_Mile_Select_Accessory", "png");
                            break;
                        case ItemType.Weapon:
                            icon = UTILE.LoadImg(string.Format("Card/Item/Item_Mile_Select_Weapon{0}", m_ItData.m_Grade), "png");
                            if (icon == null) icon = UTILE.LoadImg("Card/Item/Item_Mile_Select_Weapon", "png");
                            break;
                    }
                    break;

            }
            m_SUI.TitleIcon[i].sprite = icon;
        }

        gdatas.Sort((TGachaGroupTable _b, TGachaGroupTable _a) => {
            int bcidx = _b.MRewardKind == RewardKind.Character ? _b.m_RewardIdx : TDATA.GetItemTable(_b.m_RewardIdx).m_Value;
            int bgrade = TDATA.GetCharacterTable(bcidx).m_Grade;
            int acidx = _a.MRewardKind == RewardKind.Character ? _a.m_RewardIdx : TDATA.GetItemTable(_a.m_RewardIdx).m_Value;
            int agrade = TDATA.GetCharacterTable(acidx).m_Grade;
            if(bgrade != agrade) return agrade.CompareTo(bgrade);
            return _b.m_RewardIdx.CompareTo(_a.m_RewardIdx);
        });

        UTILE.Load_Prefab_List(gdatas.Count, m_SUI.Bucket, m_SUI.Element);
        for(int i = 0; i < gdatas.Count; i++) {
            Item_Store_Select_ListElement element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Store_Select_ListElement>();
            element.SetData(gdatas[i], SetSelect);
        }

        base.SetUI();
	}
    public void SetSelect(Item_Store_Select_ListElement _element) {
        m_SelectElement = _element;;

        TGachaGroupTable gdata = m_SelectElement.m_TData;
        if (gdata.MRewardKind == RewardKind.Item) {     
            POPUP.ViewItemInfo((result, obj) => {
                if (result == gdata.m_RewardIdx) {
                    Click_Buy(gdata.m_RewardIdx);
                }
            }, new object[] { new ItemInfo() { m_Idx = gdata.m_RewardIdx, m_Grade = gdata.m_RewardGrade, m_Lv = 1 }, m_Popup, null, m_State == State.Shop ? m_TData : null }).OnlyInfo();
        }
        else if (gdata.MRewardKind == RewardKind.Character) {
            POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_NotGet, (res, obj)=> { 
                if(res == gdata.m_RewardIdx) {
                    Click_Buy(gdata.m_RewardIdx);
                }
            }, gdata.m_RewardIdx, Info_Character_NotGet.State.Select, m_State == State.Shop ? m_TData.m_Idx : 0);
        }
    }
    /// <summary>
    /// 상점통해 들어와서 구매
    /// </summary>
    /// <param name="_idx"></param>
	public void Click_Buy(int _idx) {
        m_CB?.Invoke(m_Idx, new List<int>() { _idx });
        Close(1);
    }
}
