using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRandEvent : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public GameObject CardPrefab;//Item_Tower_RandEventCard
		public Transform Bucket;
		public Transform PickBucket;
	}
	[SerializeField] SUI m_SUI;
	Transform m_LastCard;
	[SerializeField] float m_XInterval = 420f;
	[SerializeField] float m_CardWidth = 375f;
	bool m_IsRoll;
	Transform m_PickTrans;
	float[] m_PickCardSize = new float[3] { 1f, 1.2f, 2f };

	private void Update() {
		if (m_IsRoll) {
			for (int i = 0; i < m_SUI.Bucket.childCount; i++) {
				Transform first = m_SUI.Bucket.GetChild(i);
				if (first.position.x >= Screen.width + m_CardWidth * 0.5f) {
					first.localPosition = m_LastCard.localPosition - new Vector3(m_XInterval, 0f, 0f);
					m_LastCard = first;
				}
				//카드 가운데쯤 오면 자동으로 사이즈
				//x == 0 1.2f 
				float distx = Mathf.Clamp(Mathf.Abs(first.position.x - Screen.width * 0.5f), 0f, m_XInterval);
				float val = 1f - distx / m_XInterval;
				first.localScale = Vector3.one * Mathf.Clamp(m_PickCardSize[0] + val * (m_PickCardSize[1] - m_PickCardSize[0]), m_PickCardSize[0], m_PickCardSize[1]);
			}
		}
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		TStageCardTable pickcard = (TStageCardTable)aobjValue[0];
		TTowerMapTable maptable = (TTowerMapTable)aobjValue[1];
		TowerSOType sotype = (TowerSOType)aobjValue[2];

		m_SUI.Bucket.localPosition = new Vector3(-Screen.width, m_SUI.Bucket.localPosition.y, 0f);

		Item_Tower_RandEventCard  card = Utile_Class.Instantiate(m_SUI.CardPrefab, m_SUI.Bucket).GetComponent<Item_Tower_RandEventCard>();
		card.transform.localPosition = Vector3.zero;
		m_PickTrans = card.transform;

		Sprite img = null;
		switch (maptable.m_EventType) {
			case TowerEventType.NormalEnemy:
			case TowerEventType.EliteEnemy:
			case TowerEventType.Boss:
				TEnemyTable enemy = TDATA.GetEnemyTable(Mathf.RoundToInt(pickcard.m_Value1));
				img = UTILE.LoadImg(string.Format("Card/Tower/TowerCard_{0}", (int)enemy.m_Type), "png");
				break;
			case TowerEventType.OpenEvent:
			case TowerEventType.SecrectEvent:
				switch (pickcard.m_Type) {
					case StageCardType.Enemy:
						enemy = TDATA.GetEnemyTable(Mathf.RoundToInt(pickcard.m_Value1));
						img = UTILE.LoadImg(string.Format("Card/Tower/TowerCard_{0}", (int)enemy.m_Type), "png");
						break;
					case StageCardType.Tower_Refugee: img = UTILE.LoadImg("Card/Tower/TowerCard_13", "png"); break;
					case StageCardType.Tower_Rest: img = UTILE.LoadImg("Card/Tower/TowerCard_12", "png"); break;
					case StageCardType.RecoveryMen:
					case StageCardType.RecoveryHyg:
					case StageCardType.RecoverySat:
					case StageCardType.PerRecoveryMen:
					case StageCardType.PerRecoveryHyg:
					case StageCardType.PerRecoverySat:
					case StageCardType.RecoveryHp:
						img = pickcard.GetImg(); break;
					case StageCardType.Tower_SupplyBox:
						switch (pickcard.m_Value2) {
							case 0: img = UTILE.LoadImg("Card/Tower/TowerCard_11", "png"); break;
							case 2: img = UTILE.LoadImg("Card/Tower/TowerCard_10", "png"); break;
							default: img = UTILE.LoadImg("Card/Tower/TowerCard_9", "png"); break;
						}
						break;
				}
				break;
		}
		card.SetData(img);
		////////////////
		// open => 보급상자 3,피난민 1, 휴식 1,, secret => 적8, 보급상자 3, 스탯 8 
		List<int> randlist = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };//sotype == TowerSOType.Open ? new List<int>() { 9, 10, 11, 12, 13, 9, 10, 11, 12, 13 } : new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20 };
		for (int i = randlist.Count - 1, cnt = 1; i > -1; i--, cnt++) {
			int idx = UTILE.Get_Random(0, i);
			int num = randlist[idx];
			randlist.RemoveAt(idx);

			card = Utile_Class.Instantiate(m_SUI.CardPrefab, m_SUI.Bucket).GetComponent<Item_Tower_RandEventCard>();
			card.name = i.ToString();
			card.transform.localPosition = new Vector3(-m_XInterval * cnt, 0f, 0f);
			switch (num) {
				case 13:
					card.SetData(UTILE.LoadImg("Card/Stage/Buff_1001", "png"));
					break;
				case 14:
					card.SetData(UTILE.LoadImg("Card/Stage/Buff_1022", "png"));
					break;
				case 15:
					card.SetData(UTILE.LoadImg("Card/Stage/Buff_1019", "png"));
					break;
				case 16:
					card.SetData(UTILE.LoadImg("Card/Stage/Buff_1016", "png"));
					break;
				case 17:
					card.SetData(UTILE.LoadImg("Card/Stage/Debuff_1005", "png"));
					break;
				case 18:
					card.SetData(UTILE.LoadImg("Card/Stage/Debuff_1008", "png"));
					break;
				case 19:
					card.SetData(UTILE.LoadImg("Card/Stage/Debuff_1007", "png"));
					break;
				case 20:
					card.SetData(UTILE.LoadImg("Card/Stage/Debuff_1006", "png"));
					break;
				default:
					card.SetData(UTILE.LoadImg(string.Format("Card/Tower/TowerCard_{0}", num), "png"));
					break;
			}
		}
		m_LastCard = card.transform;

		m_IsRoll = true;
		int cardcnt = m_SUI.Bucket.childCount;
		float startx = m_SUI.Bucket.localPosition.x;
		float endinterval = 20f;
		float movex = Screen.width + cardcnt * 2 * m_XInterval;
		//아니면 천천히 돌아가다가 선택 카드가 2바퀴 돌고 0지나면 바로 트위너 멈추고 센터로 돌리기
		iTween.ValueTo(gameObject, iTween.Hash("from", startx, "to", startx + movex + endinterval, "time", 2.5f, "onupdate", "TW_BucketX", "easetype", "easeoutcubic"));//+ endinterval
		iTween.ValueTo(gameObject, iTween.Hash("from", startx + movex + endinterval, "to", startx + movex, "time", 0.5f, "delay", 2.5f, "onupdate", "TW_BucketX", "oncomplete", "TW_RoulletEnd", "easetype", "easeincubic"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1f, "onupdate", "TW_CardBucketAlpha", "easetype", "linear"));

		PlayEffSound(SND_IDX.SFX_0193);
	}
	void TW_BucketX(float _amount) {
		m_SUI.Bucket.localPosition = new Vector3(_amount, m_SUI.Bucket.localPosition.y, 0f);
	}
	void TW_RoulletEnd() {
		StartCoroutine(RollEndAction());
	}
	void TW_PickCard(float _amount) {
		m_PickTrans.localScale = Vector3.one * _amount;
	}
	void TW_CardBucketAlpha(float _amount) {
		m_SUI.Bucket.GetComponent<CanvasGroup>().alpha = _amount;
	}
	void TW_PickBucketAlpha(float _amount) {
		m_SUI.PickBucket.GetComponent<CanvasGroup>().alpha = _amount;
	}
	IEnumerator RollEndAction() {
		m_IsRoll = false;
		m_PickTrans.parent = m_SUI.PickBucket;
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 0.85f, "delay", 0.65f, "onupdate", "TW_PickBucketAlpha", "easetype", "easeoutcubic"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_PickCardSize[1], "to", m_PickCardSize[2], "time", 1.1f, "delay", 0.4f, "onupdate", "TW_PickCard", "easetype", "easeoutcubic"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 0.5f, "onupdate", "TW_CardBucketAlpha", "easetype", "linear"));

		yield return new WaitForSeconds(1.5f);
		Close(0);
	}
}
