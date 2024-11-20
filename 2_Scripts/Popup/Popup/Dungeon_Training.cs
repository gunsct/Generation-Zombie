using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Dungeon_Training : PopupBase
{
    [Serializable]
    public struct SUI
    {
        public Animator Ani;
        public TextMeshProUGUI[] Names;
        public TextMeshProUGUI Cnt;
        public GameObject[] FXs;
    }

    private IEnumerator m_Action;
    
    [SerializeField]
    SUI m_SUI;

    public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
    {
        base.SetData(pos, popup, cb, aobjValue);
        if ((bool)aobjValue[1]) SetTraining(STAGEINFO.m_Pos);
        MainMenuType premenu = STAGEINFO.GetPreMenu();
        if (premenu != MainMenuType.Dungeon) SND.PlayEffSound(SND_IDX.SFX_0185);
    }
	public override void SetUI() {
		base.SetUI();
        for (int i = 0; i < 4; i++) {
            m_SUI.Names[i].text = (GetLv(i) + 1).ToString();
        }
        m_SUI.Cnt.text = string.Format(TDATA.GetString(739), USERINFO.m_Stage[StageContentType.Academy].GetItemCnt(false));
    }
    public int GetLv(int _pos) {
        List<TModeTable> modetdatas = TDATA.GetModeTable(StageContentType.Academy, DayOfWeek.Sunday, _pos);
        int limitstg = 0;
        int clearlv = USERINFO.m_Stage[StageContentType.Academy].Idxs.Find(t => t.Week == DayOfWeek.Sunday && t.Pos == _pos).Clear;
        int[] lvs = new int[3];
        lvs[1] = modetdatas.Count;
        for (int i = clearlv; i < lvs[1]; i++) {
            limitstg = modetdatas[i].m_StageLimit;
            if (limitstg > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)modetdatas[i].m_DiffType].Idx) {
                lvs[2] = i;
                break;
            }
        }
        return lvs[0] = Mathf.Clamp(lvs[2] == 0 ? clearlv : (clearlv < lvs[2] ? clearlv : clearlv - 1), 0, lvs[1] - 1);
    }
    public void ClickSelectTraining(int _pos) {
        if (Utile_Class.IsAniPlay(m_SUI.Ani)) return;
        if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Training, _pos, m_Popup)) return;

        SetTraining(_pos);
    }
    void SetTraining(int _pos) {
        if (m_Action != null) return;
        m_SUI.Ani.SetTrigger("Out");
        for(int i = 0; i < m_SUI.FXs.Length; i++) {
            m_SUI.FXs[i].SetActive(false);
        }
        POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Training_Detail, (result, obj) => {
            for (int i = 0; i < m_SUI.FXs.Length; i++) {
                m_SUI.FXs[i].SetActive(true);
            }
            SetUI();
        }, StageContentType.Academy, DayOfWeek.Sunday, _pos, new Action(InAnim));
    }
    void InAnim() {
        m_SUI.Ani.SetTrigger("In");
    }
    public override void Close(int Result = 0)
    {
        if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
        Utile_Class.DebugLog("Close");
        if(m_Action != null) return;
        m_Action = CloseAction(Result);
        StartCoroutine(m_Action);
    }

    IEnumerator CloseAction(int Result)
    {
        m_SUI.Ani.SetTrigger("Close");
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));
        base.Close(Result);
    }

}