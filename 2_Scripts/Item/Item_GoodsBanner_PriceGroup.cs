using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_GoodsBanner_PriceGroup : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public TextMeshProUGUI Price;
        public TextMeshProUGUI Discount;
        public GameObject DiscoundGroup;
    }
    [SerializeField] SUI m_SUI;

    public void SetData(string _price, string _discount) {
        m_SUI.Price.text = _price;
        m_SUI.Discount.text = string.Format("{0}{1}", _discount, TDATA.GetString(5127));
        m_SUI.DiscoundGroup.SetActive(!string.IsNullOrEmpty(_discount));
    }
}
