using System;
using UnityEngine;

public class OneStoreReview : IFReview
{
	public string pid = "0000761771";
	public override void SetReview(Action<int> EndCB)
	{
		EndCB?.Invoke(SUCCESS);
		Application.OpenURL("onestore://common/product/" + pid);
	
	}
}

