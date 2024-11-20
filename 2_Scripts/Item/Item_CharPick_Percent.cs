using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_CharPick_Percent : ObjMng
{
    [Serializable]
    public struct SRankUI
    {
        public GameObject[] ActivePanels;
        public float[] Scale;
        public Color[] BGColors;
        public Image BG;
        public Color[] TextColors;
        public TextMeshProUGUI Text;
    }

    [Serializable]
    public struct SPercentUI
    {
        public Color[] TextColors;
        public TextMeshProUGUI Text;
    }

    [Serializable]
    public struct SUI
    {
        public Image Face;
        public SRankUI Rank;
        public SPercentUI Percent;
    }

    [SerializeField] private SUI m_SUI;
    int m_Idx;

    public void SetData(int Rank, int Idx, double Percnet)
    {
        m_Idx = Idx;
        int pos = Rank == 1 ? 0 : 1;
        TCharacterTable tchar = TDATA.GetCharacterTable(Idx);
        m_SUI.Face.sprite = tchar.GetPortrait();

        m_SUI.Rank.BG.color = m_SUI.Rank.BGColors[pos];
        m_SUI.Rank.Text.color = m_SUI.Rank.TextColors[pos];
        m_SUI.Rank.Text.text = Rank.ToString();

        for (int i = 0; i < m_SUI.Rank.ActivePanels.Length; i++) m_SUI.Rank.ActivePanels[i].SetActive(pos == 0);

        m_SUI.Percent.Text.color = m_SUI.Percent.TextColors[pos];
        m_SUI.Percent.Text.text = $"{Percnet.ToString("0.#")}%";

        ((RectTransform)transform).localScale = Vector3.one * m_SUI.Rank.Scale[pos];
    }
    public void ViewInfo() {
        POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_NotGet, null, m_Idx, Info_Character_NotGet.State.Normal);
    }
}