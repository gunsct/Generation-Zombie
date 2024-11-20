using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DNASelect : PopupBase
{
    public enum TabName
	{
        Befor = 0,
        After,
        End
	}
    [Serializable]
    public struct STab
    {
        [ReName("Off", "On")]
        public GameObject[] Active;
    }
    [Serializable]
    public struct SDNA
    {
        [ReName("Infor", "Empty")]
        public GameObject[] Active;
        public TextMeshProUGUI NameTxt;
        public TextMeshProUGUI DescTxt;
        public Item_RewardDNA_Card Item;
        public TextMeshProUGUI EmptyText;

        // DNA 부가 효과
        public Item_Info_Char_DNAStat_Sub[] Effs;
        public Item_Info_Char_DNAStat_Set[] Sets;
        public GameObject Empty;
    }

    [Serializable]
    public struct SSelectDNAList
    {
        public ScrollReck_ViewItemController ScrollController;
        public RectTransform Prefab;
        public GameObject Empty;
        public Item_SortingGroup Sorting;
        [ReName("All", "Red", "Blue", "Green", "Purple")]
        public Animator[] ColorBtnAnim;
    }

    [Serializable]
    public struct SUI
    {
        public Animator Anim;
        [ReName("Befor", "After")]
        public STab[] Tab;
        public SDNA DNAInfo;
        public SSelectDNAList ItemList;
        public Item_Button EquipBtn;
    }
    [SerializeField] SUI m_SUI;
    IEnumerator m_Action;

    private List<Item_RewardDNA_Card> m_DnaCardList = new List<Item_RewardDNA_Card>();
    private CharInfo m_Char;
    private int m_Pos;
    List<DNAInfo> m_AllDNA;
    List<DNAInfo> m_SortDNA;
    Dictionary<TabName, DNAInfo> m_DNAInfo = new Dictionary<TabName, DNAInfo>();
    DNABGType m_ColorSort = DNABGType.End;
    bool IS_AscendingSort = true;
    TabName m_Tab = TabName.End;
    public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
    {
        base.SetData(pos, popup, cb, aobjValue);
        m_SUI.ItemList.ScrollController.SetData(0, m_SUI.ItemList.Prefab, SetDNA);

        m_Pos = (int) aobjValue[0];
        m_Char = (CharInfo) aobjValue[1];
        m_DNAInfo.Clear();
        m_DNAInfo.Add(TabName.Befor, USERINFO.GetDNA(m_Char.m_EqDNAUID[m_Pos]));
        m_DNAInfo.Add(TabName.After, null);
        m_Tab = TabName.End;
        m_ColorSort = DNABGType.End;
        ClickTab((int)TabName.Befor);
        SetList();

        m_SUI.EquipBtn.SetActive(false, false, UIMng.BtnBG.Not);
        m_SUI.ItemList.Sorting.SetData(ClickAscending, Item_SortingGroup.Mode.Normal);

    }
    
    private void SetList()
    {
        m_AllDNA = USERINFO.m_DNAs.FindAll(o => !USERINFO.m_EqDNAs.Contains(o.m_UID));
        IS_AscendingSort = m_SUI.ItemList.Sorting.m_Ascending;
        ClickSort((int)DNABGType.None);
    }
    void SetDNA(ScrollReck_ViewItemController.RefreshMode mode) {

        var listcnt = m_SUI.ItemList.ScrollController.GetViewCnt();
        int offset = m_SUI.ItemList.ScrollController.GetViewLine() * m_SUI.ItemList.ScrollController.GetOneLineItemCnt();
        for (int i = 0; i < listcnt; i++, offset++) {
            Item_RewardDNA_Card item = m_SUI.ItemList.ScrollController.GetItem<Item_RewardDNA_Card>(i);
            if (item == null) break;
            if (offset > -1 && offset < m_SortDNA.Count) {
                DNAInfo info = m_SortDNA[offset];
                item.SetData(info.m_Idx, i, info.m_Lv, info.m_UID, OnDNACardClicked);
                item.SetCheckActive(m_DNAInfo[TabName.After] == info);
                item.gameObject.SetActive(true);
            }
            else item.gameObject.SetActive(false);
        }
        if (mode == ScrollReck_ViewItemController.RefreshMode.Add || mode == ScrollReck_ViewItemController.RefreshMode.Minus) {
            m_SUI.ItemList.ScrollController.SetItemCount(m_SortDNA != null ? m_SortDNA.Count : 0);
        }
        m_SUI.ItemList.Empty.SetActive(m_SortDNA == null || m_SortDNA?.Count < 1);
    }

    void SetDnaInfo()
	{
        DNAInfo info = m_DNAInfo[m_Tab];
        if (info == null)
        {
            m_SUI.DNAInfo.Active[0].SetActive(false);
            m_SUI.DNAInfo.Active[1].SetActive(true);
            m_SUI.DNAInfo.EmptyText.text = TDATA.GetString(m_Tab == TabName.Befor ? 1083 : 1084);
            return;
        }

        TDnaTable tdata = info.m_TData;

        m_SUI.DNAInfo.Active[0].SetActive(true);
        m_SUI.DNAInfo.Active[1].SetActive(false);

        m_SUI.DNAInfo.NameTxt.text = tdata.GetName();
        m_SUI.DNAInfo.DescTxt.text = tdata.GetDesc();
        m_SUI.DNAInfo.Item.SetData(info.m_Idx, -1, info.m_Lv, uid:info.m_UID);

        // 부가 효과
        for(int i = 0; i < m_SUI.DNAInfo.Effs.Length; i++)
		{
            if (i < info.m_AddStat.Count)
            {
                m_SUI.DNAInfo.Effs[i].gameObject.SetActive(true);
                m_SUI.DNAInfo.Effs[i].SetData(info.m_AddStat[i].m_Stat, info.m_AddStat[i].m_Val * 0.0001f);
            }
            else m_SUI.DNAInfo.Effs[i].gameObject.SetActive(false);
        }
        m_SUI.DNAInfo.Empty.SetActive(info.m_AddStat.Count < 1);

        List<DNAInfo> eqdnas;
        switch (m_Tab)
		{
        case TabName.Befor:
            eqdnas = USERINFO.m_DNAs.FindAll(o => m_Char.m_EqDNAUID.Contains(o.m_UID));
            break;
        default:
            if(m_DNAInfo[TabName.Befor] == null) eqdnas = USERINFO.m_DNAs.FindAll(o => m_Char.m_EqDNAUID.Contains(o.m_UID));
            else eqdnas = USERINFO.m_DNAs.FindAll(o => o.m_UID != m_DNAInfo[TabName.Befor].m_UID && m_Char.m_EqDNAUID.Contains(o.m_UID));
            eqdnas.Add(info);
            break;
        }
        int MyColor = eqdnas.Count(o => o.m_TData.m_BGType == tdata.m_BGType);

        m_SUI.DNAInfo.Sets[0].SetData_Info(tdata.m_BGType, MyColor, 0);
        m_SUI.DNAInfo.Sets[1].SetData_Info(tdata.m_BGType, MyColor, 1);
    }

    [EnumAction(typeof(TabName))]
    public void ClickTab(int Pos)
	{        
        m_Tab = (TabName)Pos;
        m_SUI.Tab[Pos].Active[0].SetActive(false);
        m_SUI.Tab[Pos].Active[1].SetActive(true);


        m_SUI.Tab[1 - Pos].Active[0].SetActive(true);
        m_SUI.Tab[1 - Pos].Active[1].SetActive(false);
        SetDnaInfo();

    }

    [EnumAction(typeof(DNABGType))]
    public void ClickSort(int _type) {
        DNABGType color = (DNABGType)_type;
        if (m_ColorSort == color) return;
        m_ColorSort = color;
        m_SortDNA = m_AllDNA.FindAll(o => o.m_TData.m_BGType == m_ColorSort || m_ColorSort == DNABGType.None);
        for (int i = 0; i < m_SUI.ItemList.ColorBtnAnim.Length; i++) {
            m_SUI.ItemList.ColorBtnAnim[i].SetTrigger(i == _type ? "On" : "Off");
        }
        m_SUI.ItemList.ScrollController.InitPosition();
        SetSort();
        m_SUI.ItemList.ScrollController.SetData(m_SortDNA.Count, m_SUI.ItemList.Prefab, SetDNA);
        m_DnaCardList.Clear();
        for (int i = 0; i < m_SUI.ItemList.ScrollController.GetViewCnt(); i++) m_DnaCardList.Add(m_SUI.ItemList.ScrollController.GetItem<Item_RewardDNA_Card>(i));

        //SetDNA(ScrollReck_ViewItemController.RefreshMode.Normal);
        //m_SUI.ItemList.Scroll.ScrollInit();
    }

    public void ClickAscending() {
        IS_AscendingSort = m_SUI.ItemList.Sorting.m_Ascending;
        m_SUI.ItemList.ScrollController.InitPosition();
        SetSort();
        //m_SUI.ItemList.Scroll.ScrollInit();
        SetDNA(ScrollReck_ViewItemController.RefreshMode.Normal);
    }
    void SetSort() {
        m_SortDNA.Sort((a, b) => {
            if (a.m_Grade != b.m_Grade) return a.m_Grade.CompareTo(b.m_Grade);
            if (a.m_Lv != b.m_Lv) return a.m_Lv.CompareTo(b.m_Lv);
            if (a.m_Idx != b.m_Idx) return a.m_Idx.CompareTo(b.m_Idx);
            return a.m_UID.CompareTo(b.m_UID);
        });
        if (!IS_AscendingSort) m_SortDNA.Reverse();
    }

    private void OnDNACardClicked(object sender, int pos) {
        if (m_Action != null) return;
        Item_RewardDNA_Card selectcard = sender as Item_RewardDNA_Card;
        for (int i = 0; i < m_DnaCardList.Count; i++)
        {
            Item_RewardDNA_Card card = m_DnaCardList[i];
            m_DnaCardList[i].SetCheckActive(m_DnaCardList[i] == selectcard);
        }
        m_DNAInfo[TabName.After] = selectcard.m_Info;
        ClickTab((int)TabName.After);
        
        m_SUI.EquipBtn.SetActive(true, false, UIMng.BtnBG.Normal);
    }

    public void Inject() {
        if (m_Action != null) return;
        if (m_DNAInfo[TabName.After] == null) return;
        long uid = m_DNAInfo[TabName.After].m_UID;
        if (uid == 0) return;
        Utile_Class.DebugLog(m_Char.m_UID + " / " + m_Pos + " / " + uid);

#if NOT_USE_NET
        m_Char.m_EqDNAUID[m_Pos] = uid;
        m_Char.CheckDNASetFX();
        MAIN.Save_UserInfo();
        PLAY.PlayEffSound(SND_IDX.SFX_1071);
        Close(1);
#else
        WEB.SEND_REQ_CHAR_DNA_SET((res) =>
        {
            if (!res.IsSuccess()) {
                switch (res.result_code) {
                    case EResultCode.ERROR_NOT_FOUND_CHAR:
                    case EResultCode.ERROR_NOT_FOUND_DNA:
                    case EResultCode.ERROR_POS:
                        WEB.SEND_REQ_ALL_INFO(res2 => { });
                        return;
                    default:
                        WEB.StartErrorMsg(res.result_code);
                        return;
                }
            }
            PLAY.PlayEffSound(SND_IDX.SFX_1071);
            m_Char.CheckDNASetFX();
            Close(1);
        }, m_Char.m_UID, m_Pos, uid);
#endif
    }

    public void ClickViewDetail() {
        if (m_Action != null) return;
        DNAInfo info = m_DNAInfo[m_Tab];
        if (info == null) return;
        POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_DNA, null, info);
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
