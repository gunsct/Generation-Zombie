using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// AI에 필요한 함수들
	/// <summary> 배치도 복사본 만들기 </summary>
	public void ViewCardClone()
	{
		for (int i = 0; i < m_ViewCard.Length; i++)
		{
			for (int j = 0; j < m_ViewCard[i].Length; j++)
			{
				m_VirtualCopyCard[i][j] = m_ViewCard[i][j];
				if (m_VirtualCopyCard[i][j] == null) continue;
				m_VirtualCopyCard[i][j].MovePositionInit();
			}
		}
	}
	/// <summary> 이동 라인 및 가상위치 초기화 </summary>
	public void VirtualMoveInit()
	{
		for (int i = 0; i < m_ViewCard.Length; i++)
		{
			for (int j = 0; j < m_ViewCard[i].Length; j++)
			{
				m_VirtualCopyCard[i][j] = m_ViewCard[i][j];
				if (m_ViewCard[i][j] == null) continue;
				m_ViewCard[i][j].MovePositionInit();
			}
		}
	}
	public void VirtualCopyCard_ChangePos(Item_Stage move1, Item_Stage move2)
	{
		int line = move1.m_Line;
		int pos = move1.m_Pos;

		m_VirtualCopyCard[move2.m_Line][move2.m_Pos] = move1;
		move1.SetVirtualPos(move2.m_Line, move2.m_Pos);
		move1.ISMove = true;

		m_VirtualCopyCard[line][pos] = move2;
		move2.SetVirtualPos(line, pos);
		move2.ISMove = true;
	}
	public void StageCard_ChangePos(Item_Stage move1, Item_Stage move2, Action<Item_Stage> actioncb)
	{
		int line = move1.m_Line;
		int pos = move1.m_Pos;

		m_ViewCard[move2.m_Line][move2.m_Pos] = move1;
		move1.ISMove = true;

		m_ViewCard[line][pos] = move2;
		move2.ISMove = true;

		move1.Action(EItem_Stage_Card_Action.MoveTarget, 0, actioncb, move2);
	}

	public bool IS_Block(StageCardInfo info)
	{
		switch (info.m_TData.m_Type)
		{
		case StageCardType.Wall:
		case StageCardType.Enemy:
		case StageCardType.OldMine:
			case StageCardType.Allymine:
			case StageCardType.Fire:
		case StageCardType.Hive:
		case StageCardType.Exit:
		case StageCardType.Roadblock:
		case StageCardType.AllRoadblock:
			return true;
		}
		return info.m_NowTData.m_IsEndType;
	}

	/// <summary> 몬스터 리스트 찾기 </summary>
	/// <param name="MaxLine">최대 검색 라인</param>
	/// <returns> 범위안의 몬스터들 </returns>
	List<Item_Stage> GetEnemyStageCards(int MaxLine)
	{
		List<Item_Stage> enemys = new List<Item_Stage>();

		for (int i = 1; i <= MaxLine; i++)
		{
			for (int j = 0; j < m_ViewCard[i].Length; j++)
			{
				Item_Stage temp = m_ViewCard[i][j];
				if (temp == null || !temp.IS_AIEnemy()) continue;
				enemys.Add(temp);
			}
		}
		return enemys;
	}
	/// <summary> 몬스터 리스트 찾기 </summary>
	 /// <param name="MaxLine">최대 검색 라인</param>
	 /// <returns> 범위안의 몬스터들 </returns>
	List<Item_Stage> GetSpecialStageCards(int MaxLine, int _cardidx) {
		List<Item_Stage> enemys = new List<Item_Stage>();

		for (int i = 1; i <= MaxLine; i++) {
			for (int j = 0; j < m_ViewCard[i].Length; j++) {
				Item_Stage temp = m_ViewCard[i][j];
				if (temp == null) continue;
				if (temp.IS_Die()) continue;
				StageCardInfo info = temp.m_Info;
				if (info.IsDarkCard) continue;
				if (info.m_Idx != _cardidx) continue;
				enemys.Add(temp);
			}
		}
		return enemys;
	}
	/// <summary>
	/// 동물이 먹을 수 있는 카드만 추림
	/// </summary>
	/// <param name="MaxLine">최대 검색 라인</param>
	/// <returns></returns>
	List<Item_Stage> GetAnimalEatStageCards(int MaxLine) {
		List<Item_Stage> mats = new List<Item_Stage>();

		for (int i = 1; i <= MaxLine; i++) {
			for (int j = 0; j < m_ViewCard[i].Length; j++) {
				Item_Stage temp = m_ViewCard[i][j];
				if (temp == null) continue;
				if (temp.IS_Die()) continue;
				StageCardInfo info = temp.m_Info;
				if (info.IsDarkCard) continue;
				if (!info.IS_AnimalEatCard()) continue;
				mats.Add(temp);
			}
		}
		return mats;
	}
	/// <summary> 해당 위치에서 해당 방향에 있는 카드 알아내기 </summary>
	/// <param name="dir">이동 방향</param>
	/// <param name="line">현재 라인</param>
	/// <param name="pos">현재 위치</param>
	/// <returns>현재 위치에서 해당방향에 있는 카드</returns>
	Item_Stage GetDirStageCard(Item_Stage[][] tiles, EDIR dir, int dis, int line, int pos, bool isZeroLineCheck = false, bool isAIMaxLineCheck = true)
	{
		switch (dir)
		{
		case EDIR.UP:
			line += dis;
			pos += dis;
			break;
		case EDIR.LEFT:
			pos -= dis;
			break;
		case EDIR.DOWN:
			line -= dis;
			pos -= dis;
			break;
		case EDIR.RIGHT:
			pos += dis;
			break;
		}

		if (line < (isZeroLineCheck ? 0 : 1)) return null;
		if (line > (isAIMaxLineCheck ? AI_MAXLINE : m_ViewCard.Length - 1)) return null;
		if (pos < 0) return null;
		if (pos >= tiles[line].Length) return null;

		return tiles[line][pos];
	}

	List<Item_Stage> GetCrossCards(Item_Stage[][] tiles, Item_Stage main, int line, int pos, bool isZeroLineCheck = false, StageMaterialType type = StageMaterialType.None, bool isAIMaxline = false)
	{
		List<Item_Stage> re = new List<Item_Stage>();
		Item_Stage card = tiles[line][pos];
		if (card == null) return re;
		re.Add(tiles[line][pos]);
		if (type == StageMaterialType.None)
		{
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);

			if (re.Count > 4 && re.Contains(main)) return re;
		}
		else
		{
			if (!card.IS_MakingMaterial(type)) return re;
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);

			if (re.Count > 4 && re.Contains(main)) return re;
		}
		return new List<Item_Stage>();
	}

	List<Item_Stage> GetThreeWay4Cards(Item_Stage[][] tiles, Item_Stage main, int line, int pos, bool isZeroLineCheck = false, StageMaterialType type = StageMaterialType.None, bool isAIMaxline = false)
	{
		List<Item_Stage> re = new List<Item_Stage>();
		int checkcnt = 4;
		Item_Stage card = tiles[line][pos];
		if (card == null) return re;
		if (type == StageMaterialType.None)
		{
			// 0 0 0
			//   0
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);

			if (re.Count > checkcnt && re.Contains(main)) return re;

			//   0
			// 0 0
			//   0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			// 0
			// 0 0
			// 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			//   0
			// 0 0 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;
		}
		else
		{
			if (!card.IS_MakingMaterial(type)) return re;
			// 0 0 0
			//   0
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);

			if (re.Count > checkcnt && re.Contains(main)) return re;

			//   0
			// 0 0
			//   0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			// 0
			// 0 0
			// 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			//   0
			// 0 0 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;
		}
		return new List<Item_Stage>();
	}
	List<Item_Stage> GetThreeWay5Cards(Item_Stage[][] tiles, Item_Stage main, int line, int pos, bool isZeroLineCheck = false, StageMaterialType type = StageMaterialType.None, bool isAIMaxline = false)
	{
		List<Item_Stage> re = new List<Item_Stage>();
		int checkcnt = 4;
		Item_Stage card = tiles[line][pos];
		if (card == null) return re;
		if (type == StageMaterialType.None)
		{
			// 0 0 0
			//   0
			//   0
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 2, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);

			if (re.Count > checkcnt && re.Contains(main)) return re;

			//     0
			// 0 0 0
			//     0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 2, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			// 0
			// 0 0 0
			// 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 2, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			//   0
			//   0
			// 0 0 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 2, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;
		}
		else
		{
			if (!card.IS_MakingMaterial(type)) return re;
			// 0 0 0
			//   0
			//   0
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 2, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);

			if (re.Count > checkcnt && re.Contains(main)) return re;

			//     0
			// 0 0 0
			//     0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 2, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			// 0
			// 0 0 0
			// 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 2, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.DOWN, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;

			//   0
			//   0
			// 0 0 0
			re.Clear();
			re.Add(tiles[line][pos]);
			card = GetDirStageCard(tiles, EDIR.UP, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.UP, 2, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.LEFT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			card = GetDirStageCard(tiles, EDIR.RIGHT, 1, line, pos, false, isAIMaxline);
			if (card != null && card.IS_MakingMaterial(type)) re.Add(card);
			if (re.Count > checkcnt && re.Contains(main)) return re;
		}
		return new List<Item_Stage>();
	}

	List<Item_Stage> GetCurveCards(Item_Stage[][] tiles, Item_Stage main, int line, int pos, int Cnt, bool isZeroLineCheck = false, StageMaterialType type = StageMaterialType.None, bool isAIMaxline = false)
	{
		List<Item_Stage> re = new List<Item_Stage>();
		Item_Stage card = tiles[line][pos];
		if (card == null) return re;

		if (type == StageMaterialType.None)
		{
			#region DOWN
			List<Item_Stage> down = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.DOWN, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				down.Add(card);
			}
			#endregion

			#region LEFT
			List<Item_Stage> left = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.LEFT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				left.Add(card);
			}
			#endregion

			#region UP
			List<Item_Stage> up = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.UP, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				up.Add(card);
			}
			#endregion

			#region RIGHT
			List<Item_Stage> right = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.RIGHT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				right.Add(card);
			}
			#endregion

			if (Cnt < 2)
			{
				re.Add(card);
				return re;
			}
			Cnt--;

			if (down.Count + left.Count >= Cnt)
			{
				re.AddRange(down);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(left[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (down.Count + right.Count >= Cnt)
			{
				re.AddRange(down);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(right[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (left.Count + up.Count >= Cnt)
			{
				re.AddRange(left);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(up[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (right.Count + up.Count >= Cnt)
			{
				re.AddRange(right);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(up[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
		}
		else
		{
			if (!card.IS_MakingMaterial(type)) return re;
			#region DOWN
			List<Item_Stage> down = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.DOWN, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				down.Add(card);
			}
			#endregion

			#region LEFT
			List<Item_Stage> left = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.LEFT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				left.Add(card);
			}
			#endregion

			#region UP
			List<Item_Stage> up = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.UP, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				up.Add(card);
			}
			#endregion

			#region RIGHT
			List<Item_Stage> right = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.RIGHT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				right.Add(card);
			}
			#endregion

			if (Cnt < 2)
			{
				re.Add(card);
				return re;
			}
			Cnt--;
			if (down.Count + left.Count >= Cnt)
			{
				re.AddRange(down);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(left[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (down.Count + right.Count >= Cnt)
			{
				re.AddRange(down);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(right[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (left.Count + up.Count >= Cnt)
			{
				re.AddRange(left);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(up[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (right.Count + up.Count >= Cnt)
			{
				re.AddRange(right);
				re.Add(tiles[line][pos]);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(up[i]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
		}
		return new List<Item_Stage>();
	}

	List<Item_Stage> GetMatchCards(Item_Stage[][] tiles, Item_Stage main, int line, int pos, int Cnt, bool isZeroLineCheck = false, StageMaterialType type = StageMaterialType.None, bool isAIMaxline = false)
	{
		List<Item_Stage> re = new List<Item_Stage>();
		Item_Stage card = tiles[line][pos];
		if (card == null) return re;
		if (type == StageMaterialType.None)
		{
			#region DOWN
			List<Item_Stage> down = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.DOWN, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				down.Add(card);
			}
			#endregion

			#region LEFT
			List<Item_Stage> left = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.LEFT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				left.Add(card);
			}
			#endregion

			#region UP
			List<Item_Stage> up = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.UP, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				up.Add(card);
			}
			#endregion

			#region RIGHT
			List<Item_Stage> right = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.RIGHT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				right.Add(card);
			}
			#endregion

			Cnt--;
			if (down.Count + up.Count >= Cnt - 1)
			{
				re.AddRange(down);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(up[i]);
				re.Add(tiles[line][pos]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (left.Count + right.Count >= Cnt - 1)
			{
				re.AddRange(left);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(right[i]);
				re.Add(tiles[line][pos]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
		}
		else
		{
			if (!card.IS_MakingMaterial(type)) return re;
			#region DOWN
			List<Item_Stage> down = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.DOWN, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				down.Add(card);
			}
			#endregion

			#region LEFT
			List<Item_Stage> left = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.LEFT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				left.Add(card);
			}
			#endregion

			#region UP
			List<Item_Stage> up = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.UP, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				up.Add(card);
			}
			#endregion

			#region RIGHT
			List<Item_Stage> right = new List<Item_Stage>();
			for (int i = 1; i < Cnt; i++)
			{
				card = GetDirStageCard(tiles, EDIR.RIGHT, i, line, pos, false, isAIMaxline);
				if (card == null) break;
				if (!card.IS_MakingMaterial(type)) break;
				right.Add(card);
			}
			#endregion
			Cnt--;
			if (down.Count + up.Count >= Cnt)
			{
				re.AddRange(down);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(up[i]);
				re.Add(tiles[line][pos]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
			if (left.Count + right.Count >= Cnt)
			{
				re.AddRange(left);
				for (int i = 0, imax = Cnt - re.Count; i < imax; i++) re.Add(right[i]);
				re.Add(tiles[line][pos]);
				if (re.Contains(main)) return re;
				re.Clear();
			}
		}
		return new List<Item_Stage>();
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// AI 타겟 셋팅
	// 타겟 지정은 가상 배치를 이용해서 하므로 Item_Stage 의 m_Line, m_Pos 사용하면 안됨 이동한놈이 있어서 값이 달라짐
	// 체크가 끝난후에는 항상 VirtualMoveInit 호출 해주어야 원래의 라인과 위치를 알려줌
	/// <summary> 가장 가까운 자신의 Hostility에 등록된 대상을 추적 (동일한 거리에 있는 대상이 다수 일 경우 랜덤으로 추적) </summary>
	public Item_Stage Search_Target_Tracker(Item_Stage card)
	{
		List<Item_Stage> enemys = new List<Item_Stage>();
		enemys = GetEnemyStageCards(AI_MAXLINE);
		enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj));
		enemys.Remove(card);
		enemys.Sort((b, a) => card.GetDis(b).CompareTo(card.GetDis(a)));

		StageCardInfo cardinfo = card.m_Info;
		TEnemyTable cardtdata = cardinfo.m_TEnemyData;
		Item_Stage target = null;
		List<Navipos> navi = null;
		// 가까운 적 셋팅
		for (int i = 0; i < enemys.Count; i++)
		{
			Item_Stage temp = enemys[i];
			if (temp == null) continue;
			StageCardInfo tempinfo = temp.m_Info;

			TEnemyTable temptdata = tempinfo.m_TEnemyData;
			// 공격 가능 대상인지 확인
			if (!TDATA.ISEnemyAtkRelation(cardtdata.m_Type, temptdata.m_Type)) continue;
			// 이동 경로가 없으면 패스
			navi = Navigation(card, temp);
			// 0번째는 자신위치
			if (navi.Count < 2) continue;
			target = temp;
			break;
		}

		if (target != null && !card.CheckTargetAtkDis(target))
		{
			// 이동 위치료 변경해줌
			// 0번째는 자신위치
			VirtualCopyCard_ChangePos(card, m_VirtualCopyCard[navi[1].L][navi[1].P]);
		}
		return target;
	}
	public Item_Stage Search_SpecialTarget_Tracker(Item_Stage card) {
		List<Item_Stage> enemys = new List<Item_Stage>();
		enemys = GetSpecialStageCards(AI_MAXLINE, Mathf.RoundToInt(card.m_Info.m_TData.m_Value2));
		//enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj));
		enemys.Remove(card);
		enemys.Sort((b, a) => card.GetDis(b).CompareTo(card.GetDis(a)));

		StageCardInfo cardinfo = card.m_Info;
		TEnemyTable cardtdata = cardinfo.m_TEnemyData;
		Item_Stage target = null;
		List<Navipos> navi = null;
		// 가까운 적 셋팅
		for (int i = 0; i < enemys.Count; i++) {
			Item_Stage temp = enemys[i];
			if (temp == null) continue;
			StageCardInfo tempinfo = temp.m_Info;

			TEnemyTable temptdata = tempinfo.m_TEnemyData;
			// 이동 경로가 없으면 패스
			navi = Navigation(card, temp);
			// 0번째는 자신위치
			if (navi.Count < 2) continue;
			target = temp;
			break;
		}

		if (target != null && !card.CheckTargetAtkDis(target)) {
			// 이동 위치료 변경해줌
			// 0번째는 자신위치
			VirtualCopyCard_ChangePos(card, m_VirtualCopyCard[navi[1].L][navi[1].P]);
		}
		return target;
	}
	/// <summary> 가장 가까운 자신의 Hostility에 등록된 대상을 추적 (동일한 거리에 있는 대상이 다수 일 경우 랜덤으로 추적) 
	/// value02가 0이면 기존의 먹을 수 있는 카드 타겟, 0보다 크면 해당 스테이지 카드 인덱스 가진 카드 타겟
	/// </summary>
	public Item_Stage Search_Target_EatTracker(Item_Stage card) {
		List<Item_Stage> enemys = new List<Item_Stage>();
		enemys = GetAnimalEatStageCards(AI_MAXLINE);
		enemys.Sort((b, a) => card.GetDis(b).CompareTo(card.GetDis(a)));

		StageCardInfo cardinfo = card.m_Info;
		TEnemyTable cardtdata = cardinfo.m_TEnemyData;
		Item_Stage target = null;
		List<Navipos> navi = null;
		// 가까운 적 셋팅
		for (int i = 0; i < enemys.Count; i++) {
			Item_Stage temp = enemys[i];
			if (temp == null) continue;
			// 이동 경로가 없으면 패스
			navi = Navigation(card, temp);
			// 0번째는 자신위치
			if (navi.Count < 2) continue;
			target = temp;
			break;
		}

		if (target != null && !card.CheckTargetAtkDis(target)) {
			// 이동 위치료 변경해줌
			// 0번째는 자신위치
			VirtualCopyCard_ChangePos(card, m_VirtualCopyCard[navi[1].L][navi[1].P]);
		}
		return target;
	}
	/// <summary> 자신을 공격 가능한 대상이 상하좌우 위치에 있을 경우 반대 방향으로 이동거리 만큼 이동 </summary>
	public Item_Stage Search_Target_Coward(Item_Stage card)
	{
		TEnemyTable tdata = card.m_Info.m_TEnemyData;
		int Move = tdata.m_MoveAI.m_Values[0];
		List<Item_Stage> enemys = GetEnemyStageCards(AI_MAXLINE);
		enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj));
		enemys.Remove(card);
		List<EDIR> dirs = new List<EDIR> { EDIR.UP, EDIR.LEFT, EDIR.DOWN, EDIR.RIGHT };

		// 이동 불가 방향 제거
		if (card.m_Pos - 1 < 0) dirs.Remove(EDIR.LEFT);
		if (card.m_Pos + 1 >= m_ViewCard[card.m_Line].Length) dirs.Remove(EDIR.RIGHT);
		if (card.m_Line - 1 < 1) dirs.Remove(EDIR.DOWN);
		if (card.m_Line + 1 > AI_MAXLINE) dirs.Remove(EDIR.UP);

		// 자신을 공격하려고 하는 몬스터 리스트
		List<Item_Stage> targetOnEnemy = enemys.FindAll((obj) => obj.m_Target == card && obj.CheckTargetAtkDis(card));
		if (targetOnEnemy.Count < 1) return null;
		for (int i = targetOnEnemy.Count - 1; i > -1; i--)
		{
			// 해당 몬스터가 있는 방향 제거
			Item_Stage temp = targetOnEnemy[i];
			EDIR dir = card.GetTargetDir(temp);
			if (dirs.Contains(dir)) dirs.Remove(dir);
		}

		Item_Stage moveposcard = null;
		// 이동 가능 방향이 남아있다면 이동 거리중 어디까지 갈 수 있는지 체크
		if (dirs.Count > 0)
		{
			int dis = 0;
			// 방향중 가장 멀리 떨어진 곳으로 타겟 지정
			for (int i = dirs.Count - 1; i > -1; i--)
			{
				for(int j = Move; j > 0; j--)
				{
					Item_Stage temp = GetDirStageCard(m_VirtualCopyCard, dirs[i], j, card.m_Line, card.m_Pos);
					if (temp == null) continue;
					if (temp == card) continue;
					if (IS_Block(temp.m_Info)) continue;
					if (temp.ISMove) continue;
					List<Navipos> navi = Navigation(card, temp);
					// 0번째는 자신위치
					if (navi.Count - 1 == j && j > dis)
					{
						moveposcard = m_VirtualCopyCard[navi[1].L][navi[1].P];
						dis = navi.Count;
					}
				}
			}
		}
		if(moveposcard != null)
		{
			// 이동 위치료 변경해줌
			VirtualCopyCard_ChangePos(card, moveposcard);
		}
		return moveposcard;
	}


	/// <summary> 랜덤한 방향으로 Value01값 만큼 이동합니다. </summary>
	public Item_Stage Search_Target_Wanderer(Item_Stage card)
	{
		if (card.m_Target == null || !card.m_Target.m_Info.IS_EnemyCard)
		{
			// 몬스터들 공격 범위에 공격 가능한 대상이 있을대는 공격대상을 타겟으로 변경해준다.
			List<Item_Stage> enemys = GetEnemyStageCards(AI_MAXLINE);
			enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj));
			enemys.Remove(card);
			for (int i = enemys.Count - 1; i > -1; i--)
			{
				Item_Stage temp = enemys[i];
				if (temp == null) continue;
				if (!TDATA.ISEnemyAtkRelation(card.m_Info.m_TEnemyData.m_Type, temp.m_Info.m_TEnemyData.m_Type)) continue;
				if (!card.CheckTargetAtkDis(temp)) continue;
				return temp;
			}
		}


		if (card.m_Target != null) return card.m_Target;
		// 이동 가능한 전체 범위중 랜덤을 타겟을 설정한다.
		List<Item_Stage> moveposs = new List<Item_Stage>();
		for(int i = 0; i <= AI_MAXLINE; i++)
		{
			for(int j = 0; j < m_ViewCard[i].Length; j++)
			{
				Item_Stage temp = m_VirtualCopyCard[i][j];
				if (temp == null) continue;
				if (temp == card) continue;
				// 몬스터 및 벽위치는 어짜피 이동이 불가능 경로 검색이 필요없으므로 패스
				if (IS_Block(temp.m_Info)) continue;
				if (temp.ISMove) continue;
				// 경로가 검색 되었다면
				// 도착지점까지 이동이므로 개수가 1이상이면 이동
				if (Navigation(card, temp).Count > 1) moveposs.Add(m_ViewCard[i][j]);
			}
		}
		// 0번째는 자신위치
		if (moveposs.Count > 1)
		{
			Item_Stage temp = moveposs[UTILE.Get_Random(0, moveposs.Count)];
			// 이동 위치료 변경해줌
			VirtualCopyCard_ChangePos(card, temp);
			return temp;
		}
		return null;
	}

	/// <summary> 플레이어의 반대 방향으로 이동 </summary>
	public Item_Stage Search_Target_Runer(Item_Stage card)
	{
		if (card.m_Target == null || !card.m_Target.m_Info.IS_EnemyCard)
		{
			// 몬스터들 공격 범위에 공격 가능한 대상이 있을대는 공격대상을 타겟으로 변경해준다.
			List<Item_Stage> enemys = GetEnemyStageCards(AI_MAXLINE);
			enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj));
			enemys.Remove(card);
			for (int i = enemys.Count - 1; i > -1; i--)
			{
				Item_Stage temp = enemys[i];
				if (temp == null) continue;
				if (!TDATA.ISEnemyAtkRelation(card.m_Info.m_TEnemyData.m_Type, temp.m_Info.m_TEnemyData.m_Type)) continue;
				if (!card.CheckTargetAtkDis(temp)) continue;
				return temp;
			}
		}

		if (card.m_Target != null) return card.m_Target;
		// 가장 윗줄중 갈 수 있는 위치를 찾아서 이동
		List<Item_Stage> moveposs = new List<Item_Stage>();
		for(int i = AI_MAXLINE; i > 0; i--)
		{
			for (int j = 0; j < m_ViewCard[i].Length; j++)
			{
				Item_Stage temp = m_VirtualCopyCard[i][j];
				if (temp == null) continue;
				if (temp == card) continue;
				// 몬스터 및 벽위치는 어짜피 이동이 불가능 경로 검색이 필요없으므로 패스
				if (IS_Block(temp.m_Info)) continue;
				if (temp.ISMove) continue;
				// 경로가 검색 되었다면
				// 도착지점까지 이동이므로 개수가 1이상이면 이동
				if (Navigation(card, temp).Count > 1) moveposs.Add(m_ViewCard[i][j]);
			}

			if (moveposs.Count > 0) break;
		}
		// 0번째는 자신위치
		if (moveposs.Count > 1)
		{
			moveposs.Sort((b, a) => card.GetDis(b).CompareTo(card.GetDis(a)));
			Item_Stage temp = moveposs[0];
			// 이동 위치료 변경해줌
			VirtualCopyCard_ChangePos(card, temp);
			return temp;
		}
		return null;
	}/// <summary> 플레이어의 방향으로 이동 </summary>
	public Item_Stage Search_Target_Stalker(Item_Stage card) {
		if (card.m_Line == 0) return null;

		//if ((card.m_Target == null || !card.m_Target.m_Info.IS_EnemyCard)) {
		//	// 몬스터들 공격 범위에 공격 가능한 대상이 있을대는 공격대상을 타겟으로 변경해준다.
		//	List<Item_Stage> enemys = GetEnemyStageCards(AI_MAXLINE);
		//	enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj));
		//	enemys.Remove(card);
		//	for (int i = enemys.Count - 1; i > -1; i--) {
		//		Item_Stage temp = enemys[i];
		//		if (temp == null) continue;
		//		if (!TDATA.ISEnemyAtkRelation(card.m_Info.m_TEnemyData.m_Type, temp.m_Info.m_TEnemyData.m_Type)) continue;
		//		if (!card.CheckTargetAtkDis(temp)) continue;
		//		return temp;
		//	}
		//}

		if (card.m_Target != null) return card.m_Target; 
		// 가장 윗줄중 갈 수 있는 위치를 찾아서 이동
		List<Item_Stage> moveposs = new List<Item_Stage>();
		for (int i = 0; i < AI_MAXLINE; i++) {
			for (int j = 0; j < m_ViewCard[i].Length; j++) {
				Item_Stage temp = m_VirtualCopyCard[i][j];
				if (temp == null) continue;
				if (temp == card) continue;
				// 몬스터 및 벽위치는 어짜피 이동이 불가능 경로 검색이 필요없으므로 패스
				if (IS_Block(temp.m_Info)) continue;
				if (temp.ISMove) continue;
				// 경로가 검색 되었다면
				// 도착지점까지 이동이므로 개수가 1이상이면 이동
				if (Navigation(card, temp).Count > 1) moveposs.Add(m_ViewCard[i][j]);
			}

			if (moveposs.Count > 0) break;
		}
		// 0번째는 자신위치
		if (moveposs.Count > 1) {
			moveposs.Sort((b, a) => card.GetDis(b).CompareTo(card.GetDis(a)));
			Item_Stage temp = moveposs[0];
			// 이동 위치료 변경해줌
			VirtualCopyCard_ChangePos(card, temp);
			return temp;
		}
		return null;
	}
	/// <summary> 플레이어의 반대 방향으로 이동, 이동 안할때는 3*3 내 랜덤하게 1장을 value02로 바꿈 </summary>
	public Item_Stage Search_Target_Arsonist01(Item_Stage card) {
		if (card.m_Target != null) return card.m_Target;
		// 가장 윗줄중 갈 수 있는 위치를 찾아서 이동
		List<Item_Stage> moveposs = new List<Item_Stage>();
		for (int i = AI_MAXLINE; i > 0; i--) {
			for (int j = 0; j < m_ViewCard[i].Length; j++) {
				Item_Stage temp = m_VirtualCopyCard[i][j];
				if (temp == null) continue;
				if (temp == card) continue;
				if (IS_Block(temp.m_Info)) continue;
				if (temp.m_Info.m_TData.m_IsEndType) continue;
				if (temp.m_Info.IS_Boss) continue;
				if (temp.m_Info.IsDark) continue;
				if (!temp.m_Info.m_TData.IS_CanChangeType(TDATA.GetStageCardTable(Mathf.RoundToInt(card.m_Info.m_TData.m_Value2)).m_Type)) continue;
				if (temp.m_Info.IS_EnemyCard && card.m_Info.IS_EnemyCard && temp.m_Info.m_EnemyIdx == card.m_Info.m_EnemyIdx) continue;
				// 몬스터 및 벽위치는 어짜피 이동이 불가능 경로 검색이 필요없으므로 패스
				if (temp.ISMove) continue;
				// 경로가 검색 되었다면
				// 도착지점까지 이동이므로 개수가 1이상이면 이동
				if (Navigation(card, temp).Count > 1) moveposs.Add(m_ViewCard[i][j]);
			}

			if (moveposs.Count > 0) break;
		}
		// 0번째는 자신위치
		if (moveposs.Count > 1) {
			moveposs.Sort((b, a) => card.GetDis(b).CompareTo(card.GetDis(a)));
			Item_Stage temp = moveposs[0];
			// 이동 위치료 변경해줌
			VirtualCopyCard_ChangePos(card, temp);
			return temp;
		}
		return null;
	}
	/// <summary> 가상 이동 지점 찾기 및 타겟 설정 </summary>
	public void SetTarget()
	{
		// 이동및 공격을 가상으로 체크해준다.
		ViewCardClone();
		List<Item_Stage> enemys = GetEnemyStageCards(AI_MAXLINE);
		Dictionary<Item_Stage, EDIR> moveDir = new Dictionary<Item_Stage, EDIR>();
		List<Item_Stage> movelist = new List<Item_Stage>();

		for (int i = enemys.Count - 1; i > -1; i--)
		{
			Item_Stage enemy = enemys[i];
			if (IS_AIStopInfoCard(enemy)) enemy.SetTarget(null);
		}

		for (int i = enemys.Count - 1; i > -1; i--)
		{
			Item_Stage enemy = enemys[i];
			StageCardInfo info = enemy.m_Info;
			// 타겟이 죽었으면 초기화
			if (enemy.m_Target != null && enemy.m_Target.IS_Die())	enemy.m_Target = null;
			// 해당 위치까지 길이 있으면 패스
			// 0번째는 자신위치 마지막은 도착지점
			if (Navigation(enemy, enemy.m_Target).Count < 1) enemy.m_Target = null;

			if(IS_AIStopInfoCard(enemy)) continue;
			// 타겟을 이동 타입에 맞춰 셋팅해준다.
			// 공격후 이동이지만 같이해도 상관없다.
			// 공격후 죽었으면 해당 위치는 null이되기때문에 이동 경로에서 제외되면서 해당 AI는 이동하지 않는다.
			// 몬스터끼리는 위치 이동을 하지 못하고 피해가야하므로 순서대로만 이동해준다면 문제되지 않음
			EEnemyAIMoveType movetype = info.m_TEnemyData.GetMoveAIType();
			switch (movetype) {
				case EEnemyAIMoveType.Coward:
					enemy.SetTarget(Search_Target_Coward(enemy));
					break;
				case EEnemyAIMoveType.Tracker:
					enemy.SetTarget(Search_Target_Tracker(enemy));
					break;
				case EEnemyAIMoveType.EatTracker:
					enemy.SetTarget(Search_Target_EatTracker(enemy));
					break;
				case EEnemyAIMoveType.Wanderer:
					enemy.SetTarget(Search_Target_Wanderer(enemy));
					break;
				case EEnemyAIMoveType.Runer:
					enemy.SetTarget(Search_Target_Runer(enemy));
					break;
				case EEnemyAIMoveType.SpecialTracker:
					enemy.SetTarget(Search_SpecialTarget_Tracker(enemy));
					break;
				case EEnemyAIMoveType.Arsonist01:
					enemy.SetTarget(Search_Target_Arsonist01(enemy));
					break;
				case EEnemyAIMoveType.Stalker:
					enemy.SetTarget(Search_Target_Stalker(enemy));
					break;
			}
		}
		// 타겟 지정이 끝났으므로 길찾기 방향으로 이동 표시해준다.
		for (int i = enemys.Count - 1; i > -1; i--) {
			enemys[i].ViewTargetPos();
			enemys[i].SetEnemyType();
		}
		//이동 라인 및 가상위치 초기화
		VirtualMoveInit();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Card AI Action
	/// <summary> 3단계 : 카드 AI 발동 </summary>
	IEnumerator SelectAction_StageCardAI() {
		AutoCamPosInit = false;
		// 타겟 다시 검토
		SetTarget();
		float delaytime = 0f;
		List<Item_Stage> enemys = GetEnemyStageCards(AI_MAXLINE);
		enemys.RemoveAll((obj) => IS_AIStopInfoCard(obj) || IS_AiBlockAtkInfoCard(obj));
		List<Item_Stage> actioncheck = new List<Item_Stage>();
		List<Item_Stage> atkcard = new List<Item_Stage>();
		// 공격
		// EnemyTeamwork 디버프면 서로 공격 x
		if (!STAGE_USERINFO.ISBuff(StageCardType.ConEnemyTeamwork)) {
			for (int i = 0; i < enemys.Count; i++) {
				Item_Stage enemy = enemys[i];
				StageCardInfo info = enemy.m_Info;
				if (info.m_RepeatCnt[0] > 0) {
					info.m_RepeatCnt[0]--;
					continue;
				}
				if (enemy.m_Target == null) continue;
				if (enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.EatTracker && enemy.m_Target.m_Info.IS_EnemyCard) continue;
				if (enemy.m_Info.m_TEnemyData.GetMoveAIType() != EEnemyAIMoveType.EatTracker && enemy.m_Info.m_TEnemyData.GetMoveAIType() != EEnemyAIMoveType.SpecialTracker && !enemy.m_Target.m_Info.IS_EnemyCard) continue;
				// 타겟의 위치가 공격 거리에 있는지 체크
				if (!enemy.CheckTargetAtkDis(enemy.m_Target)) continue;
				TEnemyTable tenemy = info.m_TEnemyData;
				EnemySkillTable skill = tenemy.m_Skill.GetStageAISkill();
				if (skill == null) continue;
				info.m_RepeatCnt[0] = info.m_TEnemyData.m_AtkAI.GetRepeatCnt();
				enemy.ISAtk = true;

				delaytime = Mathf.Max(delaytime, 1f);
				atkcard.Add(enemy);
				actioncheck.Add(enemy);

				if(enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.EatTracker)
					PlayEffSound(SND_IDX.SFX_0799);
				else
					PlayEffSound(UTILE.Get_Random(0, 2) == 0 ? SND_IDX.SFX_0421 : SND_IDX.SFX_0422);

				Item_Stage targettemp = enemy.m_Target;
				enemy.Action(EItem_Stage_Card_Action.Atk, 0, (obj) => {
					actioncheck.Remove(obj);
					//대상이 에너미가 사망판정이 없기에 제거
					if (enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.EatTracker || enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.SpecialTracker) {
						RemoveStage(targettemp);
						m_ViewCard[targettemp.m_Line][targettemp.m_Pos] = null;
					}
				}, enemy.m_Target, skill);
				//대상이 에너미가 아니라 사망 판정이 없기에 공격과 동시에 모든 타켓에서 제외
				if (enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.EatTracker || enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.SpecialTracker) {
					RemoveTarget(enemy.m_Target);
				}
			}
			yield return new WaitWhile(() => actioncheck.Count > 0);
		}
		// 죽은 놈이 있어서 다시 검색
		SetTarget();
		enemys = GetEnemyStageCards(AI_MAXLINE);

		delaytime = 0f;
		List<Item_Stage> movecard = new List<Item_Stage>();
		List<Item_Stage> arsonist01s = new List<Item_Stage>();

		//이동 중간 다른걸 변환등 같이하는것, Arsonist01
		for (int i = 0; i < enemys.Count; i++) {
			Item_Stage enemy = enemys[i];
			if (enemy == null || !enemy.IS_AIEnemy()) continue;
			if (IS_AIStopInfoCard(enemy)) continue;
			StageCardInfo info = enemy.m_Info;

			if (info.m_RepeatCnt[1] > 0) {
				info.m_RepeatCnt[1]--;
				if (enemy.m_Info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.Arsonist01) {
					yield return Action_ArsonistAI(enemy, arsonist01s);
				}
				continue;
			}
		}

		if (arsonist01s.Count > 0) yield return new WaitForSeconds(0.5f);
		yield return new WaitWhile(() => arsonist01s.Count > 0);

		// 죽은 놈이 있어서 다시 검색
		SetTarget();
		enemys = GetEnemyStageCards(AI_MAXLINE);
		// 이동만 하는것
		for (int i = 0; i < enemys.Count; i++) {
			Item_Stage enemy = enemys[i];
			// 중간 AI 타입에 따라 카드를 변경하는 경우가 있어 몬스터 다시 체크
			// ex) EEnemyAIMoveType.Arsonist01 다음 움직일 대상을 변경하게될 확률이 있음
			if (enemy == null || !enemy.IS_AIEnemy()) continue;
			StageCardInfo info = enemy.m_Info;

			if (IS_AIStopInfoCard(enemy)) continue;
			// 공격을 한 몬스터는 패스
			if (enemy.ISAtk) continue;
			// 이동한 대상이라면 패스( 이동 연출을 해야하므로 )
			if (enemy.ISMove) continue;

			Item_Stage target = enemy.m_Target;
			if (target == null) continue;

			List<Navipos> movenavis = Navigation(enemy, target);
			// 다음 지점은 몬스터인지 아닌지로 체크해서 이동할지 안할지를 정한다.
			if (movenavis.Count < 2) continue;
			// 이동 방향으로 이동 거리만큼 하고 이동시켜준다.
			EDIR dir = EDIR.NONE;
			target = null;
			// 이동 고정 1로 변경
			Navipos afterpos = movenavis[1];
			target = m_ViewCard[afterpos.L][afterpos.P];
			// 이동 위치가 몬스터면 패스(공겨 대상)
			if (target.m_Info.IS_EnemyCard) continue;
			dir = target.GetTargetDir(enemy);
			// 이동할 위치가 없으면 패스
			if (target == null) continue;

			int movecnt = info.m_TEnemyData.m_MoveAI.GetRepeatCnt();
			if (enemy.m_Info.m_TEnemyData.ISGoEnemyDebuff()) movecnt = 0;
			info.m_RepeatCnt[1] = movecnt;

			actioncheck.Add(enemy);
			StageCard_ChangePos(enemy, target, (obj) => {
				actioncheck.Remove(obj);
			});

			if (info.m_TEnemyData.GetMoveAIType() == EEnemyAIMoveType.Coward) enemy.m_Target = null;
			movecard.Add(target);
			movecard.Add(enemy);
		}
		yield return new WaitWhile(() => actioncheck.Count > 0);

		// 죽은 놈이 있어서 다시 검색
		SetTarget();
		enemys = GetEnemyStageCards(AI_MAXLINE);

		//몬스터가 플레이어 스킬 공격
		/////////////////////////////
		Item_Stage_Char atktarget = null;
		for (int i = 0; i < enemys.Count; i++) {
			Item_Stage enemy = enemys[i];
			if (STAGE_USERINFO.GetBuffValue(StageCardType.Hide) > 0f) continue;
			if (IS_AiBlockRangeAtkInfoCard(enemy)) continue;
			if (IS_AiBlockAtkInfoCard(enemy)) continue;
			if (enemy.IS_Die()) continue;
			if (enemy.m_Info.IsDark) continue;
			StageCardInfo info = enemy.m_Info;
			TEnemyStageSkillTable table = TDATA.GetEnemyStageSkillTableGroup(info.m_TEnemyData.m_Skill.m_GroupID);
			if (table == null) continue;
			if (info.IS_Boss) continue;
			if (info.ISRefugee) continue;
			if (enemy.m_Line > table.m_Range || table.m_Range == 0) continue;//공격 거리 넘어감
			if (enemy.m_Pos - enemy.m_Line < m_SelectStage.m_Pos - 1 || enemy.m_Pos - enemy.m_Line > m_SelectStage.m_Pos + 1) continue;//정면 중앙 0-1-2 3개열
			if (table.m_Prob >= UTILE.Get_Random(0, 1000)) continue;    //확률 넘어감

			actioncheck.Add(enemy);
			int atkcnt = UTILE.Get_Random(table.m_AtkCnt[0], table.m_AtkCnt[1] + 1);//스킬 횟수

			//연출용 변수들
			atktarget = m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
			Vector3 prepos = enemy.gameObject.transform.position;
			Vector3 prescale = enemy.gameObject.transform.localScale;
			Vector3 targetpos = atktarget.gameObject.transform.position;
			Vector3 targetfxpos = atktarget.transform.position + new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
			int prelayer = enemy.m_Sort.sortingOrder;
			float movehalftime = 0.5f;
			bool dodge = UTILE.Get_Random(0f, 1f) < atktarget.m_Info.GetDNABuff(OptionType.HitDodge);

			enemy.SetLockOnOff(false);
			iTween.ScaleTo(enemy.gameObject, iTween.Hash("scale", prescale * 1.1f, "time", movehalftime, "easetype", "easeinquart"));
			yield return new WaitForSeconds(movehalftime);

			for (int j = 0; j < atkcnt; j++) {
				//적 확대 및 카드락 이미지 제거

				//모션
				enemy.m_Sort.sortingOrder = 4;
				switch (table.m_ATKType) {
					//근접
					case ESkillATKType.None:
					case ESkillATKType.Hit:
					case ESkillATKType.Move:
					case ESkillATKType.Bite:
					case ESkillATKType.Attack:
					case ESkillATKType.Slash:
					case ESkillATKType.Scratch:
					case ESkillATKType.MultiHit:
					case ESkillATKType.MultiBite:
					case ESkillATKType.MultiAttack:
					case ESkillATKType.MultiSlash:
					case ESkillATKType.MultiScratch:
					case ESkillATKType.ZombieBite:
					case ESkillATKType.ZombieMultiBite:
						movehalftime = 0.15f;
						Vector3 pluspos = (targetpos - prepos) * 0.35f;
						pluspos.z = 0f;
						iTween.MoveTo(enemy.gameObject, iTween.Hash("position", prepos + pluspos, "time", movehalftime, "easetype", "easeinquart"));
						yield return new WaitForSeconds(movehalftime);
						iTween.MoveTo(enemy.gameObject, iTween.Hash("position", prepos, "time", movehalftime, "easetype", "easeoutquart"));
						break;
					//원거리
					case ESkillATKType.Spit:
					case ESkillATKType.ZombieMultiSpit:
					case ESkillATKType.Continuous:
						movehalftime = 0.125f;
						iTween.MoveTo(enemy.gameObject, iTween.Hash("position", prepos + new Vector3(0f, 0.75f, 0f), "time", movehalftime, "easetype", "easeinquart"));
						yield return new WaitForSeconds(movehalftime);
						iTween.MoveTo(enemy.gameObject, iTween.Hash("position", prepos, "time", movehalftime, "easetype", "easeoutquart"));
						break;
				}
				//사운드
				switch (table.m_ATKType) {
					case ESkillATKType.Hit:
					case ESkillATKType.Attack:
					case ESkillATKType.Move:
					case ESkillATKType.MultiAttack:
					case ESkillATKType.MultiHit:
						PlayEffSound(SND_IDX.SFX_0440);
						break;
					case ESkillATKType.Bite:
						PlayEffSound(SND_IDX.SFX_0701);
						break;
					case ESkillATKType.Slash:
					case ESkillATKType.Scratch:
						PlayEffSound(SND_IDX.SFX_0420);
						break;
					case ESkillATKType.Spit:
					case ESkillATKType.ZombieMultiSpit:
						PlayEffSound(SND_IDX.SFX_0732);
						break;
					case ESkillATKType.MultiBite:
						PlayEffSound(SND_IDX.SFX_0701);
						break;
					case ESkillATKType.MultiSlash:
					case ESkillATKType.MultiScratch:
						PlayEffSound(SND_IDX.SFX_0420);
						break;
					case ESkillATKType.ZombieBite:
					case ESkillATKType.ZombieMultiBite:
						PlayEffSound(SND_IDX.SFX_0702);
						break;
					case ESkillATKType.Continuous:
						PlayEffSound(SND_IDX.SFX_0400);
						break;
				}
				//이펙트
				string fxname = table.m_FXNames.Count > 0 ? string.Format("Effect/Stage/Atk/{0}", table.m_FXNames[UTILE.Get_Random(0, table.m_FXNames.Count)]) : "Effect/Stage/Atk/Eff_EnemyAtk_Hit_01";
				int fxcnt = 0;
				switch (table.m_ATKType) {
					case ESkillATKType.MultiHit:
						fxcnt = UTILE.Get_Random(2, 4);
						break;
					case ESkillATKType.MultiAttack:
						fxcnt = UTILE.Get_Random(2, 4);
						break;
					case ESkillATKType.MultiBite:
						fxcnt = UTILE.Get_Random(4, 6);
						break;
					case ESkillATKType.MultiSlash:
						fxcnt = UTILE.Get_Random(2, 5);
						break;
					case ESkillATKType.MultiScratch:
						fxcnt = UTILE.Get_Random(2, 5);
						break;
					case ESkillATKType.ZombieMultiBite:
						fxcnt = UTILE.Get_Random(2, 3);
						break;
					case ESkillATKType.ZombieMultiSpit:
						fxcnt = UTILE.Get_Random(2, 4);
						break;
					default:
						fxcnt = 1;
						break;
				}
				for (int k = 0; k < fxcnt; k++) {
					float delay = 0f;
					switch (table.m_ATKType) {
						case ESkillATKType.MultiHit:
							delay = UTILE.Get_Random(10, 21) * 0.01f;
							break;
						case ESkillATKType.MultiAttack:
							delay = UTILE.Get_Random(10, 21) * 0.01f;
							break;
						case ESkillATKType.MultiBite:
							delay = UTILE.Get_Random(0, 11) * 0.01f;
							break;
						case ESkillATKType.MultiSlash:
							delay = UTILE.Get_Random(10, 21) * 0.01f;
							break;
						case ESkillATKType.MultiScratch:
							delay = UTILE.Get_Random(10, 21) * 0.01f;
							break;
						case ESkillATKType.ZombieMultiBite:
							delay = UTILE.Get_Random(10, 21) * 0.01f;
							break;
						case ESkillATKType.ZombieMultiSpit:
							delay = UTILE.Get_Random(10, 21) * 0.01f;
							break;
					}

					if (fxcnt > 1)
						targetfxpos = m_CenterChar.transform.position + new Vector3(UTILE.Get_Random(-3f, 3f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
					else
						targetfxpos = atktarget.transform.position + new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);

					StartEff(targetfxpos, fxname);
					//플레이어 피격 카메라 쉐이크
					CamAction(CamActionType.Pos_Player);
					CamActionType normalhittype = UTILE.Get_Random(0f, 1f) < 0.7f ? CamActionType.Battle_Hit : CamActionType.Battle_Hit_2;
					CamAction(normalhittype);

					if (dodge) {
						DNAAlarm(atktarget.m_Info, OptionType.HitDodge);
					}
					else {
						switch (table.m_SkillType) {
							case ESkillType.None://일반 공격
								float dna = atktarget.m_Info.GetDNABuff(OptionType.HitDefUp);
								if (dna > 0) DNAAlarm(atktarget.m_Info, OptionType.HitDefUp);
								float MyDamage = BaseValue.GetDamage(info.GetStat(EEnemyStat.ATK), 
									info.m_LV,
									Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Def) * (1f + dna + m_User.GetDefSkillVal(info.m_TEnemyData))), 
									1, ENoteHitState.End);
								//권장전투력 데미지 배율
								MyDamage *= BaseValue.GetCPDmgRatio(false, false, true);
								//원거리 데미지 비율
								MyDamage *= table.m_DmgRatio;
								dna = atktarget.m_Info.GetDNABuff(OptionType.HitDmgDown);
								if (dna > 0) DNAAlarm(atktarget.m_Info, OptionType.HitDmgDown);
								MyDamage -= Mathf.RoundToInt(MyDamage * Mathf.Clamp(dna, 0f, 1f));
								if (info.m_TEnemyData.m_Deadly) MyDamage = STAGE_USERINFO.GetMaxStat(StatType.HP);
								//데미지 연출
								DuelDamageFontFX(targetfxpos, -Mathf.RoundToInt(MyDamage / fxcnt));
								break;
						}
					}

					yield return new WaitForSeconds(delay);
				}

				if (dodge) {
					//회피
				}
				else {
					float dna1 = atktarget.m_Info.GetDNABuff(OptionType.HitDefUp);
					if (dna1 > 0) DNAAlarm(atktarget.m_Info, OptionType.HitDefUp);
					float MyDamage = BaseValue.GetDamage(info.GetStat(EEnemyStat.ATK),
						info.m_LV,
						Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Def) * (1f + dna1 + m_User.GetDefSkillVal(info.m_TEnemyData))),
						1, ENoteHitState.End
					);
					//권장전투력 데미지 배율
					MyDamage *= BaseValue.GetCPDmgRatio(false, false, true);
					//원거리 데미지 비율
					MyDamage *= table.m_DmgRatio;
					if (MyDamage >= Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * 0.01f) && UTILE.Get_Random(0, 100) < 30) {
						SND_IDX hitvocidx = atktarget.m_Info.m_TData.GetHitVoice(atktarget.m_TransverseAtkMode);
						PlayEffSound(hitvocidx);
					}
					//플레이어 피격 카메라 쉐이크
					yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.2f, Vector3.one * 0.01f);
					//공격 적용
					switch (table.m_SkillType) {
						case ESkillType.None://일반 공격
							dna1 = atktarget.m_Info.GetDNABuff(OptionType.HitDmgDown);
							if (dna1 > 0) DNAAlarm(atktarget.m_Info, OptionType.HitDmgDown);
							MyDamage -= Mathf.RoundToInt(MyDamage * Mathf.Clamp(dna1, 0f, 1f));
							if (info.m_TEnemyData.m_Deadly) MyDamage = STAGE_USERINFO.GetMaxStat(StatType.HP);
							AddStat(StatType.HP, -Mathf.RoundToInt(MyDamage), -Mathf.RoundToInt(MyDamage) > 0);

							//DNA 반사 데미지
							dna1 = DNACheck(atktarget, OptionType.HitThorn);
							int reflectdmg = Mathf.RoundToInt(dna1 * MyDamage);
							if (atktarget.m_Info.GetDNABuff(OptionType.DeathThorn) > 0) {
								DNAAlarm(atktarget.m_Info, OptionType.DeathThorn);
								reflectdmg = enemy.m_Info.GetMaxStat(EEnemyStat.HP);
							}
							if (reflectdmg > 0) {
								CamActionType normalhittype = UTILE.Get_Random(0f, 1f) < 0.7f ? CamActionType.Battle_Hit : CamActionType.Battle_Hit_2;
								CamAction(normalhittype);
								enemy.SetDamage(false, reflectdmg);
							}
							break;
						case ESkillType.Steal://강탈(Steal) - Value02의 확률로 Value01의 개수만큼 플레이어가  가지고 있는 재료를 제거
							m_MainUI.RandMatDiscard(table.m_SkillVal[1], table.m_SkillVal[0]);
							yield return new WaitForSeconds(0.5f);
							break;
					}
					//쿨타임 감소
					DNACheck(atktarget, OptionType.HitCoolDown);
					//랜덤 재료 획득
					DNACheck(atktarget, OptionType.HitMaterialAdd);
				}

				yield return new WaitForSeconds(Math.Max(0f, movehalftime - 0.2f));
				enemy.m_Sort.sortingOrder = prelayer;

				//DNA반사 데미지로 사망시 체크
				if (enemy.m_Info.GetStat(EEnemyStat.HP) < 1) {
					BATTLEINFO.m_Result = EBattleResult.WIN;
					if (!CheckEnd() && !enemy.m_Info.m_TEnemyData.ISRefugee()) {
						m_Check.Check(StageCheckType.KillEnemy, enemy.m_Info.m_TEnemyData.m_Idx);
						m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemy.m_Info.m_TEnemyData.m_Type);
						m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemy.m_Info.m_TEnemyData.m_Tribe);
						m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemy.m_Info.m_TEnemyData.m_Grade);
						m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
						CheckEnd();
					}

					//실패조건이 전투횟수일 경우 유아이 갱신
					STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);

					yield break;
				}
			}
			//생존스텟 알람은 뭉쳐서
			switch (table.m_SkillType) {
				case ESkillType.None://일반 공격
					//신부 시너지
					float? synergyFT2 = m_User.GetSynergeValue(JobType.Father, 1);
					for (StatType j = StatType.Men; j < StatType.SurvEnd; j++) {
						if (!STAGE_USERINFO.Is_UseStat(j)) continue;
						float val = table.Get_SrvDmg(j) * enemy.m_Info.m_TEnemyLvData.GetStat(EEnemyStat.ATKSURVSTAT) * atkcnt;

						if (synergyFT2 != null) {
							val *= 1f - (float)synergyFT2;
							STAGE_USERINFO.ActivateSynergy(JobType.Father);
						}
						yield return AddStat_Action(m_CenterChar.transform, j, -Mathf.RoundToInt(val));
					}
					break;
			}
			//적 축소
			iTween.ScaleTo(enemy.gameObject, iTween.Hash("scale", prescale, "time", movehalftime, "easetype", "easeinquart"));
			yield return new WaitForSeconds(movehalftime);
			actioncheck.Remove(enemy);

			//대사
			STAGE_USERINFO.CharSpeech(DialogueConditionType.HitShot, atktarget);
		}
		yield return new WaitWhile(() => actioncheck.Count > 0);

		// 이동과 공격으로 죽은 카드 연출을 동시 발동 해주어야됨
		// 죽었다는건 타입이 몬스터이므로 이동하는 위치가 아님
		// 죽을때 연출중 빈 공간을 채우는 것이 있는데 이것 때문에 이동해야되는 방향표시가 틀어지게 되므로 이동을 시키면서 죽는 연출을 같이해줌
		// ※ 주의 : 죽음 연출이 이동연출보다 시간이 짧을경우 문제가 되므로 죽음 연출시간이 더 길게 잡혔는지 체크해야됨
		// Die => 1초, 이동 => 0.5f
		yield return Check_DieCardAction();

		for (int i = 0; i < atkcard.Count; i++) atkcard[i].ISAtk = false;
		for (int i = 0; i < movecard.Count; i++) movecard[i].ISMove = false;


		// Hive and fire AI
		yield return SelectAction_StageCardAI_Hive();
		//StageMng.Mode.Mode_FireSpread 로 옮김
		//yield return SelectAction_StageCardAI_Fire();
		yield return SelectAction_StageCardAI_Burn();

		AutoCamPosInit = true;
	}
	/// <summary> 3*3 내 랜덤하게 하나를 value02 값으로 변경 </summary>
	IEnumerator Action_ArsonistAI(Item_Stage _item, List<Item_Stage> _targets) {
		List<Item_Stage> ChangeCards = new List<Item_Stage>();
		for (int j = _item.m_Line - 1, jMax = j + 3, Start = _item.m_Pos - 2; j < jMax; j++, Start++) {
			if (j < 1) continue;
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 3, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item == _item) continue;
				if (item.IS_Die()) continue;
				if (_targets.Contains(item)) continue;
				if (item.m_Info.IS_EnemyCard && _item.m_Info.IS_EnemyCard && item.m_Info.m_EnemyIdx == _item.m_Info.m_EnemyIdx) continue;
				if (item.m_Info.IS_RoadBlock) continue;
				if (item.m_Info.IS_Boss) continue;
				if (item.m_Info.m_TData.m_IsEndType) continue;
				if (item.m_Info.IsDark) continue;
				if (!item.m_Info.m_TData.IS_CanChangeType(TDATA.GetStageCardTable(Mathf.RoundToInt(_item.m_Info.m_TData.m_Value2)).m_Type)) continue;
				ChangeCards.Add(item);
			}
		}
		if (ChangeCards.Count < 1) yield break;

		Item_Stage target = ChangeCards[UTILE.Get_Random(0, ChangeCards.Count)];
		ChangeCards.Clear();
		target.SetCardChange(Mathf.RoundToInt(_item.m_Info.m_TData.m_Value2));
		Vector3 SPos = GetStage_EffPos(_item.transform.position);
		Vector3 EPos = GetStage_EffPos(target.transform.position);
		_targets.Add(target);

		TStageCardTable tdata = TDATA.GetStageCardTable(Mathf.RoundToInt(_item.m_Info.m_TData.m_Value2));
		switch (tdata.m_Type) {
			case StageCardType.Fire:
				PlayEffSound(SND_IDX.SFX_0621);
				EFF_MoveCardChange fx = (EFF_MoveCardChange)StartEff(SPos, "Effect/Stage/Eff_Trail", true).GetComponent<EFF_Trail>();
				((EFF_Trail)fx).SetTrail(UTILE.LoadPrefab("Effect/Stage/Eff_FireBug_01", false));
				target.Action(EItem_Stage_Card_Action.MoveChange, 0f, (obj) => {
					_targets.Remove(obj);
				}, _item, SPos, EPos, fx);
				break;
			default:
				target.Action(EItem_Stage_Card_Action.MoveChange, 0f, (obj) =>
				{
					_targets.Remove(obj);
				}, _item, SPos, EPos, (EFF_MoveCardChange)StartEff(SPos, "Effect/Stage/Eff_Walking", true).GetComponent<EFF_Walking>());
				break;
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Hive
	/// <summary> Hive 몬스터 리스트 찾기 </summary>
	/// <param name="MaxLine">최대 검색 라인</param>
	/// <returns> 범위안의 몬스터들 </returns>
	List<Item_Stage> GetHiveStageCards(int MaxLine)
	{
		List<Item_Stage> enemys = new List<Item_Stage>();

		for (int i = 1; i <= MaxLine; i++)
		{
			for (int j = 0; j < m_ViewCard[i].Length; j++)
			{
				Item_Stage temp = m_ViewCard[i][j];
				if (temp == null) continue;
				StageCardInfo info = temp.m_Info;
				if (temp.IS_Die()) continue;
				if (info.m_TData.m_Type != StageCardType.Hive) continue;
				if (IS_AIStopInfoCard(temp)) continue;
				enemys.Add(temp);
			}
		}
		return enemys;
	}

	IEnumerator SelectAction_StageCardAI_Hive()
	{
		List<Item_Stage> hives = GetHiveStageCards(AI_MAXLINE);
		if (hives.Count < 1) yield break;
		List<Item_Stage> ChangeCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		Item_Stage item;
		for (int k = hives.Count - 1; k > -1; k--)
		{
			Item_Stage atk = hives[k];
			List<Item_Stage> TargetCards = new List<Item_Stage>();
			// 타겟 데미지 주기
			// 타겟 지점에서부터 3*3 위치
			//	4,5		4,6		4,7
			//	3,4		3,5		3,6
			//	2,3		2,4		2,5
			for (int j = atk.m_Line - 1, jMax = j + 3, Start = atk.m_Pos - 2; j < jMax; j++, Start++)
			{
				if (j < 1) continue;
				if (j > AI_MAXLINE) break;
				int End = Math.Min(Start + 3, m_ViewCard[j].Length);
				for (int i = Start; i < End; i++)
				{
					if (i < 0) continue;
					item = m_ViewCard[j][i];
					if (item == null) continue;
					if (item.IS_ChangeCard()) continue;
					StageCardInfo info = item.m_Info;
					if (info.m_TData.m_IsEndType) continue;
					if (info.IS_EnemyCard) continue;
					if (info.m_TData.m_Type == StageCardType.Fire) continue;
					if (info.IS_RoadBlock) continue;
					TargetCards.Add(item);
				}
			}
			if (TargetCards.Count < 1) continue;
			item = TargetCards[UTILE.Get_Random(0, TargetCards.Count)];
			AreaCards.Add(atk);
			AreaCards.Add(item);
			ChangeCards.Add(item);
			item.SetCardChange(Mathf.RoundToInt(atk.m_Info.m_TData.m_Value2));
			Vector3 SPos = GetStage_EffPos(atk.transform.position);
			Vector3 EPos = GetStage_EffPos(item.transform.position);

			EFF_MoveCardChange fx = (EFF_MoveCardChange)StartEff(SPos, "Effect/Stage/Eff_Trail", true).GetComponent<EFF_Trail>();
			((EFF_Trail)fx).SetTrail(UTILE.LoadPrefab("Effect/Stage/Eff_Hive_01", false), 1.5f, 0f, 0.1f); 
			item.Action(EItem_Stage_Card_Action.MoveChange, 0f, (obj) => {
				ChangeCards.Remove(obj);
			}, atk, SPos, EPos, fx);
		}
		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => ChangeCards.Count > 0);
		yield return new WaitForSeconds(0.5f);//2 -> 1, EFF_Walking 타이밍 수정
		ShowArea(false);
	}


	List<Item_Stage> GetFireStageCards(int MaxLine)
	{
		List<Item_Stage> enemys = new List<Item_Stage>();

		for (int i = 1; i <= MaxLine; i++)
		{
			for (int j = 0; j < m_ViewCard[i].Length; j++)
			{
				Item_Stage temp = m_ViewCard[i][j];
				if (temp == null) continue;
				StageCardInfo info = temp.m_Info;
				if (temp.IS_Die()) continue;
				if (info.m_TData.m_Type != StageCardType.Fire) continue;
				enemys.Add(temp);
			}
		}
		return enemys;
	}
	List<Item_Stage> GetBurnStageCards(int MaxLine) {
		List<Item_Stage> enemys = new List<Item_Stage>();

		for (int i = 1; i <= MaxLine; i++) {
			for (int j = 0; j < m_ViewCard[i].Length; j++) {
				Item_Stage temp = m_ViewCard[i][j];
				if (temp == null) continue;
				StageCardInfo info = temp.m_Info;
				if (temp.IS_Die()) continue;
				if (!IS_BurnInfoCard(temp)) continue;
				enemys.Add(temp);
			}
		}
		return enemys;
	}
	IEnumerator SelectAction_StageCardAI_Fire()
	{
		List<Item_Stage> fires = GetFireStageCards(AI_MAXLINE);
		if (fires.Count < 1) yield break;
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> ChangeCards = new List<Item_Stage>();
		Item_Stage item;
		for (int k = fires.Count - 1; k > -1; k--)
		{
			Item_Stage atk = fires[k];
			List<Item_Stage> TargetCards = new List<Item_Stage>();

			if (atk == null) continue;
			if (m_ViewCard[atk.m_Line][atk.m_Pos] == null) continue;

			Item_Stage UP = GetDirStageCard(m_ViewCard, EDIR.UP, 1, atk.m_Line, atk.m_Pos);
			if(UP && UP.m_Info.IS_FireTarget() && !UP.IS_ChangeCard()) TargetCards.Add(UP);
			Item_Stage LEFT = GetDirStageCard(m_ViewCard, EDIR.LEFT, 1, atk.m_Line, atk.m_Pos);
			if (LEFT && LEFT.m_Info.IS_FireTarget() && !LEFT.IS_ChangeCard()) TargetCards.Add(LEFT);
			Item_Stage DOWN = GetDirStageCard(m_ViewCard, EDIR.DOWN, 1, atk.m_Line, atk.m_Pos);
			if (DOWN && DOWN.m_Info.IS_FireTarget() && !DOWN.IS_ChangeCard()) TargetCards.Add(DOWN);
			Item_Stage RIGHT = GetDirStageCard(m_ViewCard, EDIR.RIGHT, 1, atk.m_Line, atk.m_Pos);
			if (RIGHT && RIGHT.m_Info.IS_FireTarget() && !RIGHT.IS_ChangeCard()) TargetCards.Add(RIGHT);
			if (TargetCards.Count < 1) continue;

			item = TargetCards[UTILE.Get_Random(0, TargetCards.Count)];
			EDIR dir = item.GetTargetDir(atk);
			Vector3 Pos = GetStage_EffPos(item.transform.position);
			GameObject obj = null;
			switch (dir)
			{
			case EDIR.UP: obj = StartEff(Pos, "Effect/Stage/Eff_StageCard_Fire_Top", true); break;
			case EDIR.LEFT: obj = StartEff(Pos, "Effect/Stage/Eff_StageCard_Fire_Left", true); break;
			case EDIR.DOWN: obj = StartEff(Pos, "Effect/Stage/Eff_StageCard_Fire_Bottom", true); break;
			default: obj = StartEff(Pos, "Effect/Stage/Eff_StageCard_Fire_Right", true); break;
			}
			PlayEffSound(SND_IDX.SFX_0621);

			AreaCards.Add(atk);
			AreaCards.Add(item);
			if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { TargetCards, ChangeCards }, false);
			}
			else {
				ChangeCards.Add(item);
				item.SetCardChange(atk.m_Info.m_Idx);
				Vector3 SPos = GetStage_EffPos(atk.transform.position);
				item.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => { ChangeCards.Remove(obj); }, obj.GetComponent<AutoAciveOff>().GetTime() - 0.5f);
			}
		} 
		//ShowArea(true, AreaCards);
		yield return new WaitWhile(() => ChangeCards.FindAll(o=>o != null).Count > 0);
		//yield return new WaitForSeconds(1f);//2 -> 1, 이팩트 AutoDestroyTime 2.5 -> 1.5
		//ShowArea(false);
	}

	IEnumerator SelectAction_StageCardAI_Burn() {
		List<Item_Stage> burns = GetBurnStageCards(AI_MAXLINE);
		if (burns.Count < 1) yield break;
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> ChangeCards = new List<Item_Stage>();
		int fireidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Fire);

		for (int i = burns.Count - 1; i >= 0 ; i--) {
			Item_Stage item = burns[i];
			AreaCards.Add(item);
			if (!item.m_Info.IS_EnemyCard) continue; 

			item.SetDamage(false, Mathf.RoundToInt(item.m_Info.GetMaxStat(EEnemyStat.HP) * 0.1f));
			if (item.IS_Die()) {
				TEnemyTable enemy = item.m_Info.m_TEnemyData;
				if (!item.m_Info.m_TEnemyData.ISRefugee()) {
					m_Check.Check(StageCheckType.KillEnemy, enemy.m_Idx);
					m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemy.m_Type);
					m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemy.m_Tribe);
					m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemy.m_Grade);
				}
				m_Check.Check(StageCheckType.Fire_Enemy, (int)item.m_Info.m_TEnemyData.m_Type, 1);
				m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);

				ChangeCards.Add(item);
				if (item.m_Info.m_TEnemyData.m_Tribe == EEnemyTribe.Animal) {
					fireidx = UTILE.Get_Random(0f, 1f) >= 0.6f ? BURNMEAT_CARDIDX : RIPEMEAT_CARDIDX;
				}
				item.SetCardChange(fireidx);//잿더미 인덱스
				item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { ChangeCards.Remove(obj); }); 
			}
		}

		PlayEffSound(SND_IDX.SFX_0621);

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => ChangeCards.Count > 0);
		ShowArea(false);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Die Card Action
	IEnumerator Check_DieCardAction()
	{
		int diecnt = 0;
		List<Item_Stage> actioncards = new List<Item_Stage>();
		// 새로운 라인 추가, 선택한 카드가 있으면 1번라인부터, 없으면 0번라인부터
		//습격받으면 첫줄은 제외
		for (int j = 0, jmax = m_ViewCard.Length; j < jmax; j++)//m_IS_AutoBattle ? 1 : 0
		{
			for (int i = 0; i < m_ViewCard[j].Length; i++)
			{
				Item_Stage TempCard = m_ViewCard[j][i];
				if (TempCard != null && TempCard.IS_Die() && !TempCard.IS_KilledDuel)//일대일 결투로 죽은게 아닌 경우만 처리
				{
					diecnt++;
					//사망 이펙트
					GameObject fx = UTILE.LoadPrefab(string.Format("Effect/Stage/{0}", TempCard.m_Info.ISRefugee ? "Eff_StageCard_HumanDeath" : "Eff_StageCard_JombieDeath"), true, m_Damage.Panel);
					fx.GetComponent<AutoAciveOff>()?.SetTrackTarget(TempCard.transform);
					fx.transform.localScale /= m_Damage.Panel.localScale.x;
					PlayEffSound(SND_IDX.SFX_0220);

					actioncards.Add(TempCard);
					if (TempCard.IS_ChangeCard())
					{
						if(TempCard.m_Info.IS_EnemyCard && TempCard.IS_ChangeMatDieEnemyCard())
							TempCard.Action(EItem_Stage_Card_Action.ChangeEnemyMat, 0, (obj) => {
								actioncards.Remove(obj);
							});
						else
							TempCard.Action(EItem_Stage_Card_Action.Change, 0, (obj) => {
								actioncards.Remove(obj);
							});
					}
					else
					{
						TempCard.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
							// 카드 pool 이동
							RemoveStage(obj);
							actioncards.Remove(obj);
						});
						m_ViewCard[j][i] = null;
					}
				}
			}
		}
		if(diecnt > 1) UTILE.LoadPrefab("Effect/Stage/Eff_StageCard_JombieDeath_PP", true, m_Damage.Panel).transform.position = Utile_Class.GetWorldPosition(new Vector2(UTILE.Get_Random(-Screen.width, Screen.width) / 3, UTILE.Get_Random(-Screen.height, Screen.height)) / 3, -5f);
	
		yield return new WaitWhile(() => actioncards.Count > 0);
		yield return Check_NullCardAction();
	}

	bool IsAutoGetCard(Item_Stage card)
	{
		StageCardInfo info = card.m_Info;
		return card.m_Info.IS_AutoGet();
		//TStageCardTable tdata = info.m_TData;
		//switch(tdata.m_Type)
		//{
		//	case StageCardType.BigSupplyBox:	// 아이템 자동 획득
		//	case StageCardType.TornBody:        // 정신력 감소
		//	case StageCardType.Garbage:         // 포만감 감소
		//	case StageCardType.Pit:             // 청결도 감소
		//	/// <summary> 사슬 : 한 라인 전체(행)가 사슬 카드로만 형성되어 턴이 지날때마다 내려오며, 
		//	/// <para>기믹으로 '사슬 카드' 하나라도 제거 시 해당 라인의 사슬 카드는 모두 사라진다.</para>
		//	/// <para>*전방 3장의 카드에 사슬이 올 경우(3장 전부 사슬로 채워질 경우의 수 밖에 없음) 데미지를 입고 다음 턴으로.</para>
		//	/// <para>(턴 소모 + 데미지)</para>
		//	/// <para>*데미지는 [현재 파티 최대 체력] * [지정된 비율] 이다.</para>
		//	/// </summary>
		//	case StageCardType.Chain:
		//	case StageCardType.Fire:
		//	case StageCardType.CountBox:
		//	case StageCardType.Item_RewardBox:
		//		return true;
		//	case StageCardType.AtkUp:
		//	case StageCardType.DefUp:
		//	case StageCardType.HpUp:
		//	case StageCardType.RecoveryHp:
		//	case StageCardType.RecoveryAP:
		//	case StageCardType.SpeedUp:
		//	case StageCardType.CriticalUp:
		//	case StageCardType.CriticalDmgUp:
		//	case StageCardType.HeadShotUp:
		//	case StageCardType.LimitTurnUp:
		//	case StageCardType.TimePlus:
		//	case StageCardType.APRecoveryUp:
		//	case StageCardType.APConsumDown:
		//	case StageCardType.HealUp:
		//	case StageCardType.LevelUp:
		//	case StageCardType.RecoverySat:
		//	case StageCardType.RecoveryHyg:
		//	case StageCardType.RecoveryMen:
		//	case StageCardType.MergeSlotCount:
		//	case StageCardType.BanAirStrike:
		//		return tdata.m_Value1 < 0f || tdata.m_AutoGetBuff;
		//}
		//return info.ISRefugee;
	}
	bool IsAutoBattleCard(Item_Stage card) {
		if (card.m_Info.m_TData.m_Type != StageCardType.Enemy) return false;
		if (STAGEINFO.m_StageContentType != StageContentType.Stage && STAGEINFO.m_StageContentType != StageContentType.Cemetery) return false;
		return true;
	}

	IEnumerator Check_NullCardAction(bool Start = false)
	{
		List<Item_Stage> actioncards = new List<Item_Stage>();
		// 빈공간 채워주기
		// 1003901

		// 새로운 라인 추가
		float interverX = BaseValue.STAGE_INTERVER.x;
		float interverY = BaseValue.STAGE_INTERVER.y;
		//float CreateY = BaseValue.STAGE_INTERVER.y * (float)((m_CardLastLine > -1 ? m_CardLastLine : BaseValue.STAGE_LINE + 6));
		float y = interverY * 6f;
		int cnt = 3;

		actioncards.Clear();
		bool empty = false;
		for (int j = 0, jmax = m_ViewCard.Length; j < jmax; j++, y += interverY, cnt += 2)
		{
			if (!ISEndLine(j)) break;
			float x = ((cnt - 1) * interverX * -0.5f);
			for (int i = 0; i < m_ViewCard[j].Length; i++, x += interverX) {
				bool lineblock = false;
				float CreateY = y;
				if (m_ViewCard[j][i] == null)
				{
					// 위쪽 카드르 찾아땡겨준다.
					for (int k = j + 1, l = i + 1; k < jmax; k++, l++, CreateY += interverY)
					{
						if (!ISEndLine(k))
						{
							CreateY = k * interverY;
							break;
						}
						if (m_ViewCard[k][l] != null)
						{
							//위가 라인카드면 생성해서 당겨줌
							if (m_ViewCard[k][l].m_Info.m_TData.IS_LineCard()) {
								CreateY = k * interverY;
								Item_Stage item = CreateStageCard(j, i, new Vector3(x, CreateY, 0), 0f, (obj) => {
									actioncards.Remove(obj);
								}, 0, false, new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
								actioncards.Add(item);
								break;
							}
							//위가 블록 카드면 블록카드 좌우 체크
							else if (m_ViewCard[k][l].m_Info.IS_RoadBlock || lineblock) {
								lineblock = true;
								int lpos = l - 1, rpos = l + 1, pos = -1;
								if(lpos >= 0 && m_ViewCard[k][lpos] != null && !m_ViewCard[k][lpos].m_Info.IS_RoadBlock) {//좌측이 있고 장애물 아닐때
									pos = lpos;
								}
								else if(rpos < m_ViewCard[k].Length && m_ViewCard[k][rpos] != null && !m_ViewCard[k][rpos].m_Info.IS_RoadBlock) {//우측 있고 장애물 아닐때
									pos = rpos;
								}

								if(pos != -1) {
									m_ViewCard[j][i] = m_ViewCard[k][pos];
									m_ViewCard[k][pos] = null;
									actioncards.Add(m_ViewCard[j][i]);
									m_ViewCard[j][i].SetPos(j, i);
									m_ViewCard[j][i].Action(EItem_Stage_Card_Action.Move, 0f, (obj) => {
										actioncards.Remove(obj);
									}, Math.Abs(k - j), Math.Abs(pos - (i + Math.Abs(k - j))));
								}
								break;
							}
							else {
								m_ViewCard[j][i] = m_ViewCard[k][l];
								m_ViewCard[k][l] = null;
								actioncards.Add(m_ViewCard[j][i]);
								m_ViewCard[j][i].SetPos(j, i);
								m_ViewCard[j][i].Action(EItem_Stage_Card_Action.Move, 0f, (obj) => {
									actioncards.Remove(obj);
								}, Math.Abs(k - j), Math.Abs(l - (i + Math.Abs(k - j))));
							}
							break;
						}
					}
				}
				//위가 없으면 생성해서 채워줌
				if (m_ViewCard[j][i] == null && !lineblock)
				{
					Item_Stage item = CreateStageCard(j, i, new Vector3(x, CreateY, 0), 0f, (obj) => {
						actioncards.Remove(obj);
					}, 0, false, new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
					actioncards.Add(item);
				}
				//여전히 null이면 블럭라인이라 빈거고 아래서 NullCheck 반복시킴
				if (m_ViewCard[j][i] == null) empty = true;
				//빈칸 끌어내리고 에너미카드 타입 체크
				if (m_ViewCard[j][i] != null) m_ViewCard[j][i].SetEnemyType();
			}
		}

		yield return new WaitWhile(() => actioncards.Count > 0);

		if (empty) {
			yield return Check_NullCardAction(Start);
			yield break;
		}

		yield return Check_LightOnOff();

		bool UseChainCard = false;
		List<Item_Stage> AutoBattleEnemy = new List<Item_Stage>();
		// 앞라인 체크후 피난민이거나 BigSupplyBox 면 케릭터에 습득 표현해준후 다시 호출해준다.
		for (int i = 0; i < m_ViewCard[0].Length; i++)
		{
			Item_Stage card = m_ViewCard[0][i];
			if (card == null) continue;
			if (!card.IS_FadeIn) continue;
			if (m_IS_Jumping) continue;
			if (IsAutoGetCard(card))
			{
				yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);
				actioncards.Add(card);

				m_ViewCard[0][i] = null;

				card.m_Sort.sortingOrder = 4;
				switch (card.m_Info.m_TData.m_Type)
				{
				// 바로 데미지를 주어야한다.
				case StageCardType.Chain:
					UseChainCard = true;
					StartCoroutine(SelectAction_Chain(card, (obj) => {
							actioncards.Remove(obj);
						})
					);
					break;
				case StageCardType.Fire:
					// 데미지를 주고 제거
					StartCoroutine(SelectAction_Fire(card, (obj) => {
						actioncards.Remove(obj);
						RemoveStage(obj);
						m_Check.Check(StageCheckType.CardUse, (int)obj.m_Info.m_RealTData.m_Type);
					}));
					break;
				case StageCardType.BigSupplyBox:
					card.Action(EItem_Stage_Card_Action.Get, 0f, (obj) => {
						StageCardInfo info = obj.m_Info;
						m_Check.Check(StageCheckType.GetBox, 0, Mathf.Max(1, (int)info.m_RealTData.m_Value1));
						m_Check.Check(StageCheckType.CardUse, (int)info.m_RealTData.m_Type);
						actioncards.Remove(obj);
						RemoveStage(obj);
						CheckEnd();
					}, m_CenterChar.transform.position);
					yield return new WaitForEndOfFrame();
					yield return new WaitForSeconds(0.2f);//순차로 들어오게 수정
					break;
				case StageCardType.AtkUp:
				case StageCardType.DefUp:
				case StageCardType.HpUp:
				case StageCardType.SpeedUp:
				case StageCardType.CriticalUp:
				case StageCardType.CriticalDmgUp:
				case StageCardType.HeadShotUp:
				case StageCardType.TimePlus:
				case StageCardType.APRecoveryUp:
				case StageCardType.APConsumDown:
				case StageCardType.HealUp:
				case StageCardType.LevelUp:
				case StageCardType.MergeSlotCount:
					yield return SelectAction_StageCardProc(card, false);//루틴 기다려서 getaction 타임 알아서 딜레이줌
					m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_RealTData.m_Type);
					CheckEnd();
					actioncards.Remove(card);
					RemoveStage(card);
					break;
				case StageCardType.LimitTurnUp:
				case StageCardType.RecoveryAP:
				case StageCardType.RecoveryHp:
				case StageCardType.RecoverySat:
				case StageCardType.RecoveryHyg:
				case StageCardType.RecoveryMen:
					yield return SelectAction_StageCardProc(card, false);//루틴 기다려서 getaction 타임 알아서 딜레이줌
					card.Action(EItem_Stage_Card_Action.Get, 0f, (obj) => {
					}, m_CenterChar.transform.position);
					yield return new WaitWhile(() => !card.IS_NoAction);
					m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_RealTData.m_Type);
					CheckEnd();
					actioncards.Remove(card);
					RemoveStage(card);
					break;
				case StageCardType.Material:
					yield return SelectAction_Material(card);
					actioncards.Remove(card);
					RemoveStage(card);
					break;
				default:
					card.Action(EItem_Stage_Card_Action.Get, 0f, (obj) => {
						StageCardInfo info = obj.m_Info;
						TStageCardTable tdata = info.m_TData;
						float statratio = STAGEINFO.m_TStage.m_LV > 0 ? TDATA.GetEnemyLevelTable(STAGEINFO.m_TStage.m_LV).m_DebuffCardStatRatio : 1f;
						switch (tdata.m_Type) {
							case StageCardType.TornBody:
								//디버프 카드 밸류 스테이지 내 레벨 보정 반영
								StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, -Mathf.RoundToInt(info.m_TData.m_Value1 * statratio)));
								m_Check.Check(StageCheckType.CardUse, (int)info.m_RealTData.m_Type);
								break;
							case StageCardType.Garbage:
								StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, -Mathf.RoundToInt(info.m_TData.m_Value1 * statratio)));
								m_Check.Check(StageCheckType.CardUse, (int)info.m_RealTData.m_Type);
								break;
							case StageCardType.Pit:
								StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, -Mathf.RoundToInt(info.m_TData.m_Value1 * statratio)));
								m_Check.Check(StageCheckType.CardUse, (int)info.m_RealTData.m_Type);
								break;
							case StageCardType.CountBox:
								m_Check.Check(StageCheckType.CardUse, (int)info.m_RealTData.m_Type);
								break;
							case StageCardType.BanAirStrike:
								//비워둬 아래에서 해
								break;
							default:
								if(info.ISRefugee) GetRefugee(obj);
								break;
						}

						actioncards.Remove(obj);
						RemoveStage(obj);
						CheckEnd();
					}, m_CenterChar.transform.position);

					//이건 프로세스 들어가야해서 따로 빼냄, 획득 연출이랑 별개로 체크
					StageCardInfo info = card.m_Info;
					TStageCardTable tdata = info.m_TData;
					switch (tdata.m_Type) {
						case StageCardType.BanAirStrike:
							yield return SelectAction_StageCardProc_BanAirStrike(card);
							break;
					}

					yield return new WaitForEndOfFrame();
					yield return new WaitForSeconds(0.1f);//순차로 들어오게 수정
					break;
				}
			}
		}
		 
		for(int i = 0; i < 3; i++) {
			if (m_ViewCard[0][i] == null) empty = true;
		}
		// NullAction이 무한루프로 체크되는것을 막기위해 셋팅
		bool IsNullAction = false;
		if(actioncards.Count > 0)
		{
			IsNullAction = true;
			yield return new WaitWhile(() => actioncards.Count > 0);

			if (UseChainCard)
			{
				// 체인 연출
				yield return SelectAction_ChainUI();
				StageFail(StageFailKind.Turn);
				yield break;
			}
		}

		// 지뢰 체크
		yield return SelectAction_StageCardProc_OldMine();

		if (IsNullAction || empty) yield return Check_NullCardAction();

		SetTarget();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Navigation
	public class Navipos : ClassMng
	{
		/// <summary> 라인 </summary>
		public int L;
		/// <summary> 위치 </summary>
		public int P;
		/// <summary> 거리 </summary>
		public int DIS;
		/// <summary> 잔여 거리 </summary>
		public int REDIS;
		/// <summary> 전체 거리 </summary>
		public int TOTDIS { get { return DIS + REDIS; } }

		public Navipos Parent;

		public Navipos(int line, int pos) { L = line; P = pos; }

		/// <summary> 실행하기 </summary>
		/// <param name="PPos">부모 타일</param>
		/// <param name="TPos">도착지 타일</param>
		public void Execute(Navipos PPos, Navipos TPos)
		{
			Parent = PPos;

			DIS = PPos.DIS + STAGE.GetViewDis(PPos, this);

			REDIS = STAGE.GetViewDis(this, TPos);
		}

		public bool Equals(Navipos pos)
		{
			return L == pos.L && P == pos.P;
		}
		public bool Equals(int line, int pos)
		{
			return L == line && P == pos;
		}
	}

	public int GetViewDis(Navipos Base, Navipos Target)
	{
		return m_ViewCard[Base.L][Base.P].GetDis(m_ViewCard[Target.L][Target.P]);
	}

	void Set_Navi(EDIR dir, Navipos NowPos, Navipos StartPos, Navipos EndPos, List<Navipos> OpenPoss, List<Navipos> ClosePoss)
	{
		int line = NowPos.L;
		int pos = NowPos.P;
		switch (dir)
		{
		case EDIR.UP:
			pos++;
			line++;
			break;
		case EDIR.LEFT:
			pos--;
			break;
		case EDIR.DOWN:
			pos--;
			line--;
			break;
		case EDIR.RIGHT:
			pos++;
			break;
		}

		if (line < 1) return;
		if (line > AI_MAXLINE) return;
		if (pos < 0) return;
		if (pos >= m_VirtualCopyCard[line].Length) return;
		Item_Stage Target = m_VirtualCopyCard[line][pos];
		if (Target == null) return;
		// 이번에 이동을 한 타겟이면 패스
		if (Target.ISMove) return;
		// 도착지점에 몬스터이기때문에 벽으로 인식을 막기위해 예외처리
		if (!EndPos.Equals(line, pos))
		{
			StageCardInfo TargetInfo = Target.m_Info;

			if (IS_Block(TargetInfo)) return;
			// 제외 블럭이면 패스
			if (ClosePoss.Find(t => t.Equals(line, pos)) != null) return;
		}

		Navipos open = OpenPoss.Find(t => t.Equals(line, pos));
		if (open == null)
		{
			open = new Navipos(line, pos);
			OpenPoss.Add(open);
			open.Execute(NowPos, EndPos);
		}
		else if (NowPos.DIS + GetViewDis(NowPos, open) < open.DIS)
		{
			open.Execute(NowPos, EndPos);
		}

	}

	/// <summary> 경로 셋팅 </summary>
	/// <param name="card">시작 카드</param>
	/// <param name="target">도착 카드</param>
	/// <returns>현재 지점부터 도착지점까지의 경로 0번째는 현재위치, 마지막번째는 도착지점 개수가 0이면 경로 검색 실패</returns>
	public List<Navipos> Navigation(Item_Stage card, Item_Stage target)
	{
		List<Navipos> Poss = new List<Navipos>();
		if (target == null) return Poss;
		List<Navipos> OpenPoss = new List<Navipos>();
		List<Navipos> ClosePoss = new List<Navipos>();

		Navipos StartPos = new Navipos(card.m_Line, card.m_Pos);
		Navipos EndPos = new Navipos(target.m_Line, target.m_Pos);

		OpenPoss.Add(StartPos);

		Navipos NowPos = null;

		while (OpenPoss.Count > 0)
		{
			OpenPoss.Sort((b, a) => b.TOTDIS.CompareTo(a.TOTDIS));
			NowPos = OpenPoss[0];

			OpenPoss.Remove(NowPos);
			ClosePoss.Add(NowPos);
			Item_Stage NowCard = m_VirtualCopyCard[NowPos.L][NowPos.P];

			if (NowPos.Equals(EndPos)) break;

			for (EDIR dir = EDIR.UP; dir < EDIR.NONE; dir++)
			{
				Set_Navi(dir, NowPos, StartPos, EndPos, OpenPoss, ClosePoss);
			}
		}

		if (!NowPos.Equals(EndPos))
		{
			return Poss;
		}

		do
		{
			Poss.Add(NowPos);

			NowPos = NowPos.Parent;
		}
		while (NowPos != null);

		// 반전해줌
		Poss.Reverse();
		return Poss;
	}
}
