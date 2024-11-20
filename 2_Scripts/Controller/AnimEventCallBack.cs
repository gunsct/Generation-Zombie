using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventCallBack : ObjMng
{
    public List<Action> m_CB = new List<Action>();

    public void CallBack(int _idx) {
        if (m_CB.Count < 1) return;
        m_CB[_idx]?.Invoke();
	}
}
