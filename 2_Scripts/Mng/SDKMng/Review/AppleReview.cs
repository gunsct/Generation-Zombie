using System;

public class AppleReview : IFReview
{
	public override void SetReview(Action<int> EndCB)
	{
#if UNITY_IOS
		UnityEngine.iOS.Device.RequestStoreReview();
#endif
		EndCB?.Invoke(SUCCESS);
	}
}

