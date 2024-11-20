using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_DungeonLvBtn : MonoBehaviour
{
    public TextMeshProUGUI m_Txt;
    public GameObject m_LockIcon;
    public void SetData(string _txt) {
        m_Txt.text = _txt;
    }
    public void SetTxtColor(Color _color, bool _lock = false) {
        m_Txt.color = _color;
        m_LockIcon.SetActive(_lock);
    }
}
