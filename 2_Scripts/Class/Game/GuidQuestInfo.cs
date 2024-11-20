public class GuidQuestInfo
{
	public enum InfoType
	{
		/// <summary> 미션 </summary>
		Mission = 0,
		/// <summary> 업적 </summary>
		Achieve,
		End
	}

	public InfoType type;
	public object Data;

	public bool IsComple()
	{
		switch (type)
		{
		case GuidQuestInfo.InfoType.Mission: return ((MissionData)Data).IS_Complete();
		case GuidQuestInfo.InfoType.Achieve: return true;
		}
		return false;
	}
}


