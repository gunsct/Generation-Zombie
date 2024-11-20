using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class Utile_Class
{
	static string LangPattern = new StringBuilder(1024)
		.Append("^[")
		.Append("0-9")     // 숫자
		.Append("a-zA-Z")   // 영어
		.Append("가-힣")  // 한글
		.Append("ぁ-ゔァ-ヴゝゞー々〆〤")   // 히라가나, 가타카나
		// CJK 통합 한자 (중국, 일본, 한국)
		//.Append("一-龥")  // 한문
		.Append("\u4E00-\u9FEA")  // 일반 (一-鿪)
		.Append("\u3400-\u4DB5")  // 확장 A (㐀-䶵)
		.Append("\uF900-\uFA6D")  // CJK 호환용 한자 (豈-舘)
		.Append("กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮฯะัาำิีึืฺุู฿เแโใไๅๆ็่้๊๋์ํ๎๏๐๑๒๓๔๕๖๗๘๙๚๛")  // 태국어
		.Append("ᜀ\u1700-\u171F")  // 필리핀
		.Append("أ-ي")  // 아랍어
		.Append("\uA980-\uA9DF")  // 자와어
		// 라틴어 확장(체코어, 덴마크어, 네덜란드어, 핀란드어, 프랑스어, 독일어, 헝가리어, 이탈리아어, 노르웨이어, 폴란드어, 포르투갈어, 루마니아어, 스페인어, 스웨덴어)
		.Append("\u00C0-\u00D6")  // 라틴어-1 보충 (À-Ö)
		.Append("\u00D8-\u00F6")  // 라틴어-1 보충 (Ø-ö)
		.Append("\u00F8-\u00FF")  // 라틴어-1 보충 (ø-ÿ)
		.Append("\u0100-\u017F")  // 라틴어 확장-A (Ā-ſ)
		.Append("\u0180-\u024F")  // 라틴어 확장-B (ƀ-ɏ)
		.Append("\u2C60-\u2C7F")  // 라틴어 확장-C (Ⱡ-Ɀ)
		.Append("]*$")
		.ToString();
	// 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 500, 1000
	string[] m_astrRomaNum = { "", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "ⅩC", "C", "D", "M" };
	/// <summary> 국제 단위계 </summary>
	string[] m_astrNumUnit = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };
	string[] m_astrDataSize = { "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
	/** 1~10까지만 사용할것 그이상은 무시함
	 * 필요할경우 만들어야함
	 * Ⅰ, Ⅱ, Ⅲ, Ⅳ, Ⅴ, Ⅵ, Ⅶ, Ⅷ, Ⅸ, Ⅹ
	 * 40 -> ⅩL, 50 -> L, 60 -> LⅩ, 70 -> LⅩⅩ, 80 -> LⅩⅩⅩ, 90 -> ⅩC, 100 -> C, 500 -> D, 1000 -> M*/
	public string Get_RomaNum(int nNum)
	{
		if (nNum > 10) return "";
		return m_astrRomaNum[nNum];
	}

	/** 1000이상 국제단위계(SI) */
	public string Get_NumUnit(long lNum)
	{
		// 1000000000000000000000000
		/* yotta 10^24 Y
		 * zetta 10^21 Z
		 * exa 10^18 E
		 * peta 10^15 P
		 * tera 10^12 T
		 * giga 10^9 G
		 * mega 10^6 M
		 * kilo 10^3 k
		 * hecto 10^2 h
		 * deca 10^1 da
		 * deci 10^-1 d
		 * milli 10^-3 c
		 * micro 10^-6 μ
		 * nano 10^-9 n
		 * pico 10^-12 p
		 * femto 10^-15 f
		 * atto 10^-18 a
		 * zepto 10^-21 z
		 * yocto 10^-24 y */
		StringBuilder strTemp = new StringBuilder("");
		double dAbsNum = Math.Abs(lNum);
		if (lNum < 1000) strTemp.Append(lNum);
		else
		{
			if (lNum < 0) strTemp.Append("-");
			double dDive = 1;
			int nCnt = 0;
			for (; dAbsNum > 999; dDive *= 1000, dAbsNum *= 0.001f, nCnt++) { }
			double dValue = (double)lNum / dDive;
			//			// 반올림이 안되도록 수치 변경
			//			double dRound = Math.Round( dValue, 1 ) ;
			//			if(dRound - dValue > 0) dValue -= 0.05;
			strTemp.AppendFormat("{0}{1}", dValue.ToString("#,#.#"), m_astrNumUnit[nCnt]);
		}
		return strTemp.ToString();
	}
	/** 1000이상 국제단위계(SI) */
	public string Get_MemoryUnit(long lNum)
	{
		// 1000000000000000000000000
		/* yotta 10^24 Y
		 * zetta 10^21 Z
		 * exa 10^18 E
		 * peta 10^15 P
		 * tera 10^12 T
		 * giga 10^9 G
		 * mega 10^6 M
		 * kilo 10^3 k
		 * hecto 10^2 h
		 * deca 10^1 da
		 * deci 10^-1 d
		 * milli 10^-3 c
		 * micro 10^-6 μ
		 * nano 10^-9 n
		 * pico 10^-12 p
		 * femto 10^-15 f
		 * atto 10^-18 a
		 * zepto 10^-21 z
		 * yocto 10^-24 y */
		StringBuilder strTemp = new StringBuilder("");
		double dAbsNum = Math.Abs(lNum);
		if (lNum < 1000) strTemp.Append(lNum);
		else
		{
			if (lNum < 0) strTemp.Append("-");
			double dDive = 1;
			int nCnt = 0;
			for (; dAbsNum > 999; dDive *= 1000, dAbsNum *= 0.001f, nCnt++) { }
			double dValue = (double)lNum / dDive;
			//			// 반올림이 안되도록 수치 변경
			//			double dRound = Math.Round( dValue, 1 ) ;
			//			if(dRound - dValue > 0) dValue -= 0.05;
			strTemp.AppendFormat("{0}{1}", dValue.ToString("#,#.#"), m_astrNumUnit[nCnt]);
		}
		return string.Format("{0}B", strTemp);
	}

	public static string CommaValue(long lValue)
	{
		return lValue.ToString("N0");
	}
	public static string CommaValue(double dValue)
	{
		return dValue.ToString("N0");
	}

	public static string GetColorString(int value, string ZColor, bool Comma = false)
	{
		if(value < 10) return string.Format("<color={0}>0</color>{1}", ZColor, value);
		return Comma ? CommaValue(value) : value.ToString();
	}
	public static string GetColorString(string text, Color color) {

		StringBuilder builder = new StringBuilder("<color=#");
		builder.Append(ColorUtility.ToHtmlStringRGBA(color));
		builder.Append(">");
		builder.Append(text);
		builder.Append("</color>");
		return builder.ToString();
	}
	/// <summary> 컬러 코드로 색상 반환 </summary>
	public static Color GetCodeColor(string _code) { 
		Color setcolor = Color.white;
		ColorUtility.TryParseHtmlString(_code, out setcolor);
		return setcolor;
	}
	/// <summary> 색상을 컬러 코드로 반환 </summary>
	public static string GetColorCode(Color _color) {
		return string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(_color));
	}

	public static string StringFormat(String format, params object[] args)
	{
		string temp = string.Format(format, args);
		string re = "";
		Regex regex = new Regex(@"(<#이/가=)([가-힣 ]+)(>)|(<#가/이=)([가-힣 ]+)(>)|(<#으로/로=)([가-힣 ]+)(>)|(<#로/으로=)([가-힣 ]+)(>)|(<#은/는=)([가-힣 ]+)(>)|(<#는/은=)([가-힣 ]+)(>)|(<#을/를=)([가-힣 ]+)(>)|(<#를/을=)([가-힣 ]+)(>)");
		// 이, 가
		if (regex.IsMatch(temp)) {
			Match m = regex.Match(temp);
			int Offset = 0;
			while (m.Success) {
				re += temp.Substring(Offset, m.Index - Offset);
				if (m.Value.IndexOf("이/가") > -1 || m.Value.IndexOf("가/이") > -1) {
					string name = m.Value.Substring(6, m.Value.Length - 7);
					re += getComleteWordByJongsung(name, "이", "가");
				}
				else if (m.Value.IndexOf("로/으로") > -1 | m.Value.IndexOf("으로/로") > -1) {
					string name = m.Value.Substring(7, m.Value.Length - 8);
					re += getComleteWordByJongsung(name, "으로", "로");
				}
				else if (m.Value.IndexOf("을/를") > -1 | m.Value.IndexOf("를/을") > -1)
				{
					string name = m.Value.Substring(7, m.Value.Length - 8);
					re += getComleteWordByJongsung(name, "을", "를");
				}
				else {
					string name = m.Value.Substring(6, m.Value.Length - 7);
					re += getComleteWordByJongsung(name, "은", "는");
				}
				Offset = m.Index + m.Value.Length;
				m = m.NextMatch();
			}

			re += temp.Substring(Offset, temp.Length - Offset);
		}
		else re = temp;
		return re;
	}

	public static string getComleteWordByJongsung(String name, String firstValue, String secondValue)
	{
		char lastName = name[name.Length - 1];

		// 한글의 제일 처음과 끝의 범위밖일 경우는 오류
		if (lastName < 0xAC00 || lastName > 0xD7A3) return "";
		String seletedValue = (lastName - 0xAC00) % 28 > 0 ? firstValue : secondValue;
		return seletedValue;
	}

	/** 파일 사이즈 표기 */
	public string Get_FileSize(long lNum)
	{
		StringBuilder strTemp = new StringBuilder(128);
		double dNum = lNum;
		int unitpos = 0;
		while(dNum > 1023)
		{
			dNum /= 1024D;
			unitpos++;
		}
		strTemp.AppendFormat("{0}{1}", dNum.ToString("#,#.00"), m_astrDataSize[unitpos]);
		return strTemp.ToString();
	}

	public static bool IS_SpecialChar(string _str) {
		//string pattern = @"[^a-zA-Z0-9가-힣ㄱ-ㅎㅏ-ㅣぁ-ゔァ-ヴー々〆〤一-龥]";
		//string pattern = @"^[a-zA-Z0-9가-힣ぁ-ゔァ-ヴー々〆〤一-龥أ-ي]*$";
		return !Regex.IsMatch(_str, LangPattern);
	}

	public static string GetNationRankNum(LanguageCode code, long no)
	{
		switch(code)
		{
		case LanguageCode.EN:
			if(no / 10 != 1)
			{
				// 10~19를 제외한 1의 자리가 1,2,3으로 끝나면 st,nd,rd 넣어줌 나머지는 th
				var oneseat = no % 10;
				switch (oneseat)
				{
				case 1: return $"{no}st";
				case 2: return $"{no}nd";
				case 3: return $"{no}rd";
				}
			}
			return $"{no}th";
		}
		return no.ToString();
	}

	public static string Remove_Tag(string data, string tag)
	{
		return data.Replace($"<{tag}>", "").Replace($"</{tag}> ", "");
	}

	public static string Remove_RichTextTag(string data, bool Color = true, bool size = true)
	{
		data = Remove_Tag(data, "i");
		data = Remove_Tag(data, "u");
		data = Remove_Tag(data, "b");
		data = Remove_Tag(data, "s");
		data = data.Replace("</br>", " ");
		data = data.Replace("<br>", " ");

		StringBuilder strRegex = new StringBuilder();
		if (Color) strRegex.Append("<color=(.)*?>|</color>");
		if(size)
		{
			if (strRegex.Length > 0) strRegex.Append("|");
			strRegex.Append("<size=(.)*?>|</size>");
		}
		
		Regex pImage = new Regex(strRegex.ToString());
		int indexText = 0;
		int nCnt = 0;
		StringBuilder strOut = new StringBuilder();
		foreach (Match match in pImage.Matches(data))
		{
			strOut.Append(data.Substring(indexText, match.Index - indexText));
			indexText = match.Index + match.Length;
		}
		strOut.Append(data.Substring(indexText, data.Length - indexText));
		return strOut.ToString();
	}
}
