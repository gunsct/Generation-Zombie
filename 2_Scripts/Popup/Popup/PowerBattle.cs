using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerBattle : PopupBase
{
	enum EState
	{
		Create = 0,
		Play,
		End
	}
#pragma warning disable 0649
	[System.Serializable]
	struct STuto
	{
		public GameObject Active;
		public float DelayTime;
	}

	[System.Serializable]
	struct STimer
	{
		public GameObject Active;
		public Animator Ani;
		public Image Gage;
	}

	[SerializeField] Animator m_Ani;
	[SerializeField] STuto m_Tuto;
	[SerializeField] STimer m_Timer;
	bool m_Presscheck;
	[SerializeField] Vector2[] m_ChainPoss;
	[SerializeField] Vector3[] m_ChainRots;
	[SerializeField] GameObject m_ChainPrefab;
	[SerializeField] RectTransform m_ChainPanel;
	List<Item_PB_Chain> m_Chains = new List<Item_PB_Chain>();
	public Enemy_AtkInfo m_AtkInfo;
	EState m_State = EState.Create;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		m_AtkInfo = (Enemy_AtkInfo)aobjValue[0];
		m_Tuto.Active.SetActive(false);
		StartCoroutine(CreateChain(UTILE.Get_Random(3, 6)));
		CheckTouchAni();
	}

	IEnumerator CreateChain(int Cnt)
	{
		m_State = EState.Create;
		m_Timer.Active.SetActive(false);
		POPUP.TouchLock(true);
		yield return new WaitForSeconds(0.3f);
		// 하트연출 시작
		m_Timer.Ani.speed = 1f;
		m_Timer.Active.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		float delayTime = 0.8f / (float)Cnt;
		float AddNext = 0;
		Dictionary<int, List<int>> CreateCheck = new Dictionary<int, List<int>>();
		EnemySkillTable skill = BATTLEINFO.GetSkill(m_AtkInfo.m_Atk);
		SND.PlayEffSound(SND_IDX.SFX_0950);
		for (int i = Cnt; i > 0; i--)
		{
			float delay = UTILE.Get_Random(AddNext, delayTime + AddNext);
			Item_PB_Chain item = Utile_Class.Instantiate(m_ChainPrefab, m_ChainPanel).GetComponent<Item_PB_Chain>();
			m_Chains.Add(item);
			item.SetData(UTILE.Get_Random((int)skill.m_SkillValues[1], (int)skill.m_SkillValues[2]), ChainEndCB);
			item.PressCheckCB(OnChainPressed);
			RectTransform rtf = (RectTransform)item.transform;
			int Pos = 0;
			int Rot = 0;
			while(true)
			{
				Pos = UTILE.Get_Random(0, m_ChainPoss.Length);
				if (!CreateCheck.ContainsKey(Pos)) CreateCheck.Add(Pos, new List<int>());
				Rot = UTILE.Get_Random(0, m_ChainPoss.Length);
				if (CreateCheck[Pos].Find((t) => t == Rot + 1) < 1) break;
			}
			CreateCheck[Pos].Add(Rot + 1);
			rtf.anchoredPosition = m_ChainPoss[Pos];
			rtf.eulerAngles = m_ChainRots[Rot];
			yield return new WaitForSeconds(delay);
			AddNext = delayTime + AddNext - delay;
		}
		yield return new WaitWhile(()=> Utile_Class.IsAniPlay(m_Ani));
		POPUP.TouchLock(false);

		StartCoroutine("Timer");
	}

	void SetTimeGage(float value)
	{
		m_Timer.Gage.fillAmount = value;
	}
	
	IEnumerator Timer()
	{
		m_State = EState.Play;
		EnemySkillTable skill = BATTLEINFO.GetSkill(m_AtkInfo.m_Atk);
		float maxtime = skill.m_SkillValues[0];
		float time = maxtime;
		SetTimeGage(1f);
		m_Timer.Ani.speed = ((581f - 60f) / 60f) / maxtime;
		while (m_State == EState.Play && time > 0f)
		{
			yield return new WaitForEndOfFrame();
			time -= Time.deltaTime;
			SetTimeGage(time / maxtime);
		}
		
		if (m_State == EState.Play)
		{
			m_Timer.Ani.speed = 1f;
			SetTimeGage(0f);
			BATTLE.GetEnemy().AtkActionStart(m_AtkInfo, 2);
			BATTLE.UserDamage(Mathf.RoundToInt(BATTLEINFO.m_User.GetStat(StatType.HP)), skill, false);
			m_Ani.SetTrigger("Fail");
			BATTLE.m_MainUI.Invoke("StartFadeIn", 180f / 60f);
		}
	}

	void ChainEndCB(Item_PB_Chain chain)
	{
		Enemy_Controller enemy = BATTLE.GetEnemy();
		enemy.StartAni(EEnemyAni.Hit5);
		m_Chains.Remove(chain);
		if (m_Chains.Count < 1)
		{
			m_State = EState.End;
			StopCoroutine("Timer");
			m_Timer.Ani.speed = 1f;
			Utile_Class.AniSkip(m_Timer.Ani, 581f);
			enemy.AtkActionStart(m_AtkInfo, 1);
			m_Ani.SetTrigger("Succ");
		}
	}

	void CheckTouchAni()
	{
		m_Tuto.Active.SetActive(false);
		if (PlayerPrefs.GetInt($"VIEW_PB_Noti_{USERINFO.m_UID}", 0) == 0)
		{
			StartCoroutine(VIEW_PB_Noti());
			PlayerPrefs.SetInt($"VIEW_PB_Noti_{USERINFO.m_UID}", 1);
			PlayerPrefs.Save();
		}
	}

	void OnChainPressed()
	{
		m_Presscheck = true;
	}

	IEnumerator VIEW_PB_Noti()
	{
		yield return new WaitForSeconds(1.3f);
		m_Tuto.Active.SetActive(true);
		RectTransform rtf = (RectTransform)m_Tuto.Active.transform;
		m_Tuto.Active.transform.position = m_Chains[m_Chains.Count - 1].transform.position;
		m_Presscheck = false;
		yield return new WaitWhile(() => !m_Presscheck);
		m_Tuto.Active.SetActive(false);
	}


	protected void AniEnd(int mode)
	{
		Close(mode);
	}
}
