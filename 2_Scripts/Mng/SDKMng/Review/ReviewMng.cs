using System;


public abstract class IFReview : ClassMng
{
	public const int IDLE = 0x0000; // 대기중
	public const int SUCCESS = 0x0001;  // 성공


	public const int ERROR_REQ = 0x0002;  // 요청 실패
	public const int ERROR_LAUNCHING = 0x0002;  // 시작 오류
	public const int ERROR_NOTFOUND = 0x0002;  // 스토어 또는 공식앱 아님

	public abstract void SetReview(Action<int> EndCB);
}

public class ReviewMng
{
	IFReview m_pReview;
	public ReviewMng()
	{
#if UNITY_ANDROID
#	if ONE_STORE
		m_pReview = new OneStoreReview();
#	else
		m_pReview = new GoogleReview();
#	endif
#else
		m_pReview = new AppleReview();
#endif
	}

	public void SetReview(Action<int> EndCB)
	{
		m_pReview?.SetReview(EndCB);
	}
}

