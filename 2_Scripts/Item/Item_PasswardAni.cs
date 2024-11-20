using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_PasswardAni : MonoBehaviour
{
	public enum State
	{
		SetPass = 0,
		Play,
		Fail,
		End
	}
	public TextMeshProUGUI Passward;
	[SerializeField] int Length = 9;
	[SerializeField] bool ISAllRandomString;
	readonly char[] RandChar = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
	, 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
	, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
	[SerializeField, ReName("Fail Action Time", "Change Char Time")]
	float[] Times = new float[] { 5, 0.5f };
	[SerializeField, ReName("Action Time Min", "Action Time Max")]
	float[] TimeRandom = new float[] { 0.8f, 3};
	StringBuilder Pass = new StringBuilder();
	State Playstate = State.SetPass;
	int Offset = 0;
	float ChangeCheck = 0;
	float TimeCheck = 0;
	float GapTime = 0;
	float NextActionTime = 0;
	float ActionTime = 0;
	private void Awake()
	{
		StateChange(State.SetPass);
	}
	public void Update()
	{
		switch (Playstate)
		{
		case State.Play:
			if (ISAllRandomString) Ani_AllRandomString();
			else Ani_RandomString();
			break;
		case State.Fail:
			TimeCheck += Time.deltaTime;
			if(TimeCheck > Times[0]) StateChange(State.End);
			break;
		case State.End:
			TimeCheck += Time.deltaTime;
			if (TimeCheck > Times[0]) StateChange(State.SetPass);
			break;
		}
		//TimeCheck += Time.deltaTime;
		//string.Format()
		//if(TimeCheck > 1f)
		//{
		//    char random = (char)Utile_Class.Get_RandomStatic((int)'A', (int)'Z');
		//}
	}
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		// Ensure continuous Update calls.
		if (!Application.isPlaying)
		{
			UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
			UnityEditor.SceneView.RepaintAll();
		}
	}
#endif

	void Ani_AllRandomString()
	{
		TimeCheck += Time.deltaTime;
		ChangeCheck += Time.deltaTime;
		if (TimeCheck > ActionTime) StateChange(State.Fail);
		else if(ChangeCheck > Times[1])
		{
			StringBuilder temp = new StringBuilder();
			for (int i = 0; i < Length; i++) temp.Append(RandChar[Utile_Class.Get_RandomStatic(0, RandChar.Length)]);
			Passward.text = temp.ToString();
			ChangeCheck -= Times[1];
		}
	}

	void Ani_RandomString()
	{
		ChangeCheck += Time.deltaTime;
		TimeCheck += Time.deltaTime;

		if (TimeCheck > NextActionTime)
		{
			Offset++;
			NextActionTime += GapTime;
		}

		if (TimeCheck > ActionTime) StateChange(State.Fail);
		else if (ChangeCheck > Times[1])
		{
			StringBuilder temp = new StringBuilder(Length);
			if (Offset > 0) temp.Append(Pass.ToString(), 0, Offset);
			temp.Append(RandChar[Utile_Class.Get_RandomStatic(0, RandChar.Length)]);
			Passward.text = temp.ToString();
			ChangeCheck -= Times[1];
		}
	}

	public void StateChange(State state)
	{
		Playstate = state;
		TimeCheck = 0;
		switch (Playstate)
		{
		case State.SetPass:
			Pass.Clear();
			for (int i = 0; i < Length; i++) Pass.Append(RandChar[Utile_Class.Get_RandomStatic(0, RandChar.Length)]);
			Offset = 0;
			ChangeCheck = 0;
			Passward.fontStyle = FontStyles.Normal;
			Passward.text = "";
			ActionTime = Utile_Class.Get_RandomStatic(TimeRandom[0], TimeRandom[1]);
			GapTime = ActionTime / (float)Length;
			NextActionTime = GapTime;
			StateChange(State.Play);
			break;
		case State.Fail:
			Passward.text = Pass.ToString();
			break;
		case State.End:
			Passward.fontStyle = FontStyles.Underline;
			break;
		}

	}
}
