using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Reassembly_Result : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public Animator Anim;
        public Item_Item_Card ItemCard;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI GradeName;
        public TextMeshProUGUI CP;
        public GameObject StatPrefab;//item_infostat
        public Transform StatBucket;
        public GameObject OptionPrefab;//Item_EquipOption
        public Transform OptionBucket;
        public Image PrivateChar;
        public GameObject CharGroup;
    }
    [SerializeField]
    SUI m_SUI;

    Action<ItemInfo> m_CB;
    ItemInfo m_SelectItem;

    public void SetData(ItemInfo _item, Action<ItemInfo> _cb) {
        m_SelectItem = _item;
        m_SUI.ItemCard.SetData(_item);
        m_SUI.GradeName.color = BaseValue.GradeColor(_item.m_Grade);
        m_SUI.GradeName.text = BaseValue.GradeName(_item.m_Grade);
        m_SUI.Name.text = _item.m_TData.GetName();
        m_SUI.CP.text = Utile_Class.CommaValue(_item.GetCombatPower());
        for(int i = 0; i < _item.m_TData.m_Stat.Count; i++) {
            Item_InfoStat stat = Utile_Class.Instantiate(m_SUI.StatPrefab, m_SUI.StatBucket).GetComponent<Item_InfoStat>();
            StatType stattype = _item.m_TData.m_Stat[i].m_Stat;
            stat.SetData(stattype, Mathf.RoundToInt(_item.GetStat(stattype)));
        }
        for(int i = 0; i < _item.m_AddStat.Count; i++) {
            Item_EquipOption option = Utile_Class.Instantiate(m_SUI.OptionPrefab, m_SUI.OptionBucket).GetComponent<Item_EquipOption>();
            option.SetData(_item.m_AddStat[i].ToString());
		}
        TEquipSpecialStat setstat = _item.m_TSpecialStat;
        m_SUI.CharGroup.SetActive(setstat != null);
        if (setstat != null) {
            TCharacterTable tchar = TDATA.GetCharacterTable(setstat.m_Char);
            m_SUI.PrivateChar.sprite = tchar.GetPortrait();
        }
       m_CB = _cb;
    }

    public void ClickCard() {
        m_SUI.Anim.SetTrigger("Select");
        m_CB?.Invoke(m_SelectItem);
    }
    public void NonSelect(ItemInfo _info) {
        if (m_SelectItem == _info) return;
        m_SUI.Anim.SetTrigger("Normal");
    }
}
