public enum StageContentType
{
	// 미션용
	All = -3,
	DownTown = -2,

	None = -1,
	/// <summary> 일반 스테이지 </summary>
	Stage = 0,
	/// <summary> 로지은행, 일일 던전 - 달러, 기존 Dollar, </summary>
	Bank,
	/// <summary> 보일 사관학교, 일일 던전 - 경험치, 기존 Exp </summary>
	Academy,
	/// <summary> 허버트 대학, 요일 던전, 기존 Week </summary>
	University,
	/// <summary> 고든 타워 </summary>
	Tower,
	/// <summary> 램버트 공동묘지, 기존 ZHorde </summary>
	Cemetery,
	/// <summary> 폐허가 된 공장, 기존 Melee </summary>
	Factory,
	/// <summary> 지하철 </summary>
	Subway,
	/// <summary> 긴급 임무 </summary>
	Replay,
	/// <summary> 긴급 임무(나이트메어)  </summary>
	ReplayHard,
	/// <summary> 긴급 임무(아포칼립스) </summary>
	ReplayNight,
	/// <summary> pvp </summary>
	PvP,
	End,
}