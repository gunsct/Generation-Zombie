using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_101
{
	DelayStart = 1,
	DelayStartEnd,
	SceneChange,
	StageStart_Loading,
	DL_101,
	Focus_Line_0_1_4,
	Focus_Line_0_0_1,
	FREE_Play_1,
	DL_108,
	Focus_Line_1_0_2,
	FREE_Play_2,
	Focus_MissionGuide,
	DL_113,
	FREE_Play_3,
	DL_110,
	Select_0_1021Char,
	Char_0_1021_SkillEnd,
	DL_155,
	Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_101()
	{
		TutoType_Stage_101 type = GetTutoState<TutoType_Stage_101>();

		switch (type) {
			case TutoType_Stage_101.FREE_Play_1:
			case TutoType_Stage_101.FREE_Play_2:
			case TutoType_Stage_101.FREE_Play_3:
			case TutoType_Stage_101.Delay_Clear:
				return true;
		}
		return false;
	}
	void PlayTuto_Stage_101(int no, object[] args)
	{
		TutoType_Stage_101 type = (TutoType_Stage_101)no;
		TutoUI ui;
		RectTransform rtf;
		Item_Stage card;
		Vector3 pos, temp;
		Item_Stage_Char charcard;
		Main_Stage stageui;
		DeckSetting decksetting;
		List<GameObject> fxs = new List<GameObject>();

		switch (type)
		{
			case TutoType_Stage_101.DelayStart:
				POPUP.RemoveTutoUI();
				USERINFO.m_PlayDeck.SetChar(0, USERINFO.GetChar(1021).m_UID);
				WEB.SEND_REQ_DECK_SET((res) =>
				{
					if (!res.IsSuccess()) {
						WEB.StartErrorMsg(res.result_code, (btn, obj) => {
						});
						return;
					}

					USERINFO.m_PlayDeck.IsChange = false;
					UserInfo.StageIdx sidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
					WEB.SEND_REQ_STAGE_START((res) =>
					{
						if (!res.IsSuccess()) {
							WEB.StartErrorMsg(res.result_code);
							return;
						}
						STAGEINFO.SetStage(StagePlayType.Stage, StageModeType.Stage, sidx.Idx, 1, sidx.Week, USERINFO.GetDifficulty());
						Next();
					}, USERINFO.m_Stage[StageContentType.Stage].UID, sidx.Week, sidx.Pos, sidx.Idx, sidx.DeckPos, false, 0);
				});
				break;
			case TutoType_Stage_101.DelayStartEnd:
				break;
			case TutoType_Stage_101.SceneChange:
				UserInfo.StageIdx sidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
				AsyncOperation pAsync = null;
				pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () =>
				{
					MAIN.ActiveScene(() => {
						switch (STAGEINFO.m_StageModeType) {
							case StageModeType.NoteBattle:
								BATTLE.Init(EBattleMode.Normal, STAGEINFO.GetCreateEnemyIdx(), STAGEINFO.GetCreateEnemyLV(0, false), 0, null, true);
								break;
						}
					});
				});
				USERINFO.CheckClearUserPickInfo(StageContentType.Stage, sidx.Week, sidx.Pos, sidx.Idx);
				Next();
				break;
			case TutoType_Stage_101.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_101.DL_101:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(101, () => { Next(); });
				break;
			case TutoType_Stage_101.Focus_Line_0_1_4:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[1][4];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, false);
				POPUP.StartTutoTimer(() => {
					ui.StartDlg(106, () => { Next(); });
				}, 1f);
				break;
			case TutoType_Stage_101.Focus_Line_0_0_1:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][1].transform, STAGE.m_ViewCard[1][1].transform, STAGE.m_ViewCard[1][2].transform, STAGE.m_ViewCard[1][3].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
				for (int i = 0; i < fxs.Count; i++) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Able" : "Loop");
				POPUP.StartTutoTimer(() => {
					ui.RemoveFX();
				}, 2f);
				break;
			case TutoType_Stage_101.FREE_Play_1:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_101.DL_108:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(108, () => { Next(); });
				break;
			case TutoType_Stage_101.Focus_Line_1_0_2:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				fxs = ui.SetFX(new List<Transform>() { STAGE.m_ViewCard[0][2].transform, STAGE.m_ViewCard[1][2].transform, STAGE.m_ViewCard[1][3].transform, STAGE.m_ViewCard[1][4].transform }, "Item/Tuto/Item_FX_Card_Tuto", Vector3.one * 4.45f);
				for (int i = 0; i < fxs.Count; i++) fxs[i].GetComponent<Animator>().SetTrigger(i == 0 ? "Able" : "Loop");
				POPUP.StartTutoTimer(() => {
					ui.RemoveFX();
				}, 2f);
				break;
			case TutoType_Stage_101.FREE_Play_2:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_101.Focus_MissionGuide:
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1); ;
				stageui.GetGuideParentObj.SetActive(true);
				rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
				ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(401, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_101.DL_113:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(113, () => { Next(); });
				break;
			case TutoType_Stage_101.FREE_Play_3:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_101.DL_110:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(110, () => { Next(); });
				break;
			case TutoType_Stage_101.Select_0_1021Char:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[0];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				Start_ClickDelay();
				break;
			case TutoType_Stage_101.Char_0_1021_SkillEnd:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_101.DL_155:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(155, () => { Next(); });
				break;
			case TutoType_Stage_101.Delay_Clear:
				ui = POPUP.ShowTutoUI();
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_101.End:
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_Stage_101(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_101 type = GetTutoState<TutoType_Stage_101>();
		Item_Stage stagecard;
		Item_Stage_Char charcard;
		switch (type)
		{
			case TutoType_Stage_101.DelayStartEnd:
			case TutoType_Stage_101.StageStart_Loading:
			case TutoType_Stage_101.FREE_Play_1:
			case TutoType_Stage_101.FREE_Play_2:
				return false;
			case TutoType_Stage_101.FREE_Play_3:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0;
			case TutoType_Stage_101.Focus_Line_0_0_1:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0 || stagecard.m_Pos != 1;
			case TutoType_Stage_101.Focus_Line_1_0_2:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0 || stagecard.m_Pos != 2;
			case TutoType_Stage_101.Select_0_1021Char:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1021;
			case TutoType_Stage_101.Char_0_1021_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
				return false;
		}
		return true;
	}
}
