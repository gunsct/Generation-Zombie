//#if UNITY_ANDROID
//using Google.Play.Review;
//using UnityEngine;
//#endif
using System;
using System.Collections;

public class GoogleReview : IFReview
{
	//#if UNITY_ANDROID
	//	private ReviewManager mng;
	//#endif
	public override void SetReview(Action<int> EndCB)
	{
		//MAIN.StartCoroutine(Proc(EndCB));
	}

	//	private IEnumerator Proc(Action<int> EndCB)
	//	{
	//#if UNITY_ANDROID
	//		var mng = new ReviewManager();

	//		// 객체 요청
	//		var requestFlowOperation = mng.RequestReviewFlow();
	//		yield return requestFlowOperation;
	//		if (requestFlowOperation.Error != ReviewErrorCode.NoError)
	//		{
	//			// Log error. For example, using requestFlowOperation.Error.ToString().
	//			EndCB?.Invoke(ERROR_REQ);
	//			yield break;
	//		}
	//		var ReviewInfo = requestFlowOperation.GetResult();

	//		// 리뷰 흐름 시작
	//		var launchFlowOperation = mng.LaunchReviewFlow(ReviewInfo);
	//		yield return launchFlowOperation;
	//		ReviewInfo = null; // Reset the object
	//		if (launchFlowOperation.Error != ReviewErrorCode.NoError)
	//		{
	//			// Log error. For example, using requestFlowOperation.Error.ToString().
	//			EndCB?.Invoke(ERROR_LAUNCHING);
	//			yield break;
	//		}
	//		EndCB?.Invoke(SUCCESS);
	//#else
	//		EndCB?.Invoke(SUCCESS);
	//		yield break;
	//#endif
	//	}
}

