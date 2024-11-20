public enum PriceType
{
    /// <summary> 캐시 </summary>
    Cash,
    /// <summary> 달러 </summary>
    Money,
    /// <summary> 현금 </summary>
    Pay,
    /// <summary> 광고 </summary>
    AD,
    /// <summary> 에너지 </summary>
    Energy,
    /// <summary> 기간 </summary>
    Time,
    /// <summary> 아이템 </summary>
    Item,
    /// <summary> Price 0 : 서버시간 00:00:00 에 초기화, 1~ : 서버시간 00:00:00 부터 Price 간격으로 초기화(분단위)</summary>
    AD_InitTime,
    /// <summary> 구매후 Price 시간(분) 에 초기화 </summary>
    AD_AddTime,
    /// <summary> pvp 재화 </summary>
    PVPCoin,
	/// <summary> 길드 코인 </summary>
	GuildCoin,
    /// <summary> 마일리지 </summary>
    Mileage,
}

public enum ShopResetType
{
    /// <summary> 없음 </summary>
    None = 0,
    /// <summary> 구매한 시간 00시 + Time </summary>
    ZeroTime,
    /// <summary> 구매한 시간 + Time </summary>
    AddTime,
    /// <summary> 매주 월요일 00시  </summary>
    DayOfWeek,
    /// <summary> 시즌에 맞추기  </summary>
    Season,
    /// <summary>  </summary>
    End
}