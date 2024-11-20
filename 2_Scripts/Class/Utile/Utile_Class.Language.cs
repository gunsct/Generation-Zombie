using System.Collections.Generic;
using UnityEngine;


///////////////////////////////////////////////////////////////////////////////////////////////
// 언어
public enum LanguageCode
{
	// 사용 언어만 등록할것
	KO = 0,	// 한국어
	EN,		// 영어
	JA,		// 일본어
	CN,		// 중국어 간체
	TW,		// 중국어 번체
	TH,		// 태국어
	TL,		// 필리핀
	AR,		// 아랍어
	JV,		// 자와어(인도네시아)
	FR,		// 프랑스어
	ES,		// 스페인어
	PT,		// 포르투갈어
	DE,		// 독일어
	MAX,
}

public partial class Utile_Class
{
	public static bool IsUseLanguage(LanguageCode eLan)
	{
		switch(eLan)
		{
		case LanguageCode.TH:
		case LanguageCode.TL:
		case LanguageCode.AR:
		case LanguageCode.JV:
			return false;

		// 임시제거
		case LanguageCode.CN:       // 중국어 간체
		case LanguageCode.TW:       // 중국어 번체
		case LanguageCode.FR:       // 프랑스어
		case LanguageCode.ES:       // 스페인어
		case LanguageCode.PT:       // 포르투갈어
		case LanguageCode.DE:       // 독일어
			return false;
		}
		return true;
	}

	public static string Get_LanguageName(LanguageCode eLan)
	{
		string strCode = Get_LanguageCode(eLan);
		switch (strCode)
		{
		case "af": return "Afrikaans";
		case "ar": return "عربى";
		case "eu": return "Euskara";
		case "by": return "беларускі";
		case "bg": return "български";
		case "ca": return "Català";
		case "zh":
		case "zh-CN": return "简体中文";
		case "zh-TW": return "繁體中文";
		case "cs": return "Čeština";
		case "da": return "Dansk";
		case "nl": return "Deens";
		case "et": return "Eesti keel";
		case "fo": return "Føroyskt";
		case "fi": return "Suomalainen";
		case "fr": return "Français";
		case "de": return "Deutsche";
		case "el": return "Ελληνικά";
		case "iw": return "עברית";
		case "hu": return "Magyar";
		case "is": return "Íslensku";
		case "in": return "Indonesia";
		case "it": return "Italiano";
		case "ja": return "日本語";
		case "ko": return "한국어";
		case "lv": return "Latviešu valoda";
		case "lt": return "Lietuvių";
		case "no": return "Norsk";
		case "pl": return "Polski";
		case "pt": return "Português";
		case "ro": return "Română";
		case "ru": return "Русский";
		case "sh": return "Srpsko-hrvatski";
		case "sk": return "Slovenský";
		case "sl": return "Slovenščina";
		case "es": return "Español";
		case "sv": return "Svenska";
		case "th": return "ไทย";
		case "tr": return "Türkçe";
		case "uk": return "Українська";
		case "vi": return "Tiếng Việt";
		case "id": return "Bahasa Indonesia";
		case "tl": return "Tagalog";
		case "jv": return "Jawa";
		}
		return "English";
	}

	public static string Get_LanguageCode(SystemLanguage eLan)
	{
		switch (eLan)
		{
		case SystemLanguage.Afrikaans: return "af";
		case SystemLanguage.Arabic: return "ar";
		case SystemLanguage.Basque: return "eu";
		case SystemLanguage.Belarusian: return "by";
		case SystemLanguage.Bulgarian: return "bg";
		case SystemLanguage.Catalan: return "ca";
		case SystemLanguage.Chinese: return "zh";
		case SystemLanguage.ChineseSimplified: return "zh-CN";
		case SystemLanguage.ChineseTraditional: return "zh-TW";
		case SystemLanguage.Czech: return "cs";
		case SystemLanguage.Danish: return "da";
		case SystemLanguage.Dutch: return "nl";
		case SystemLanguage.Estonian: return "et";
		case SystemLanguage.Faroese: return "fo";
		case SystemLanguage.Finnish: return "fi";
		case SystemLanguage.French: return "fr";
		case SystemLanguage.German: return "de";
		case SystemLanguage.Greek: return "el";
		case SystemLanguage.Hebrew: return "iw";
		case SystemLanguage.Hungarian: return "hu";
		case SystemLanguage.Icelandic: return "is";
		case SystemLanguage.Indonesian: return "jv";
		case SystemLanguage.Italian: return "it";
		case SystemLanguage.Japanese: return "ja";
		case SystemLanguage.Korean: return "ko";
		case SystemLanguage.Latvian: return "lv";
		case SystemLanguage.Lithuanian: return "lt";
		case SystemLanguage.Norwegian: return "no";
		case SystemLanguage.Polish: return "pl";
		case SystemLanguage.Portuguese: return "pt";
		case SystemLanguage.Romanian: return "ro";
		case SystemLanguage.Russian: return "ru";
		case SystemLanguage.SerboCroatian: return "sh";
		case SystemLanguage.Slovak: return "sk";
		case SystemLanguage.Slovenian: return "sl";
		case SystemLanguage.Spanish: return "es";
		case SystemLanguage.Swedish: return "sv";
		case SystemLanguage.Thai: return "th";
		case SystemLanguage.Turkish: return "tr";
		case SystemLanguage.Ukrainian: return "uk";
		case SystemLanguage.Vietnamese: return "vi";
		}
		return "en";
	}
	public static LanguageCode Get_LanguageCode(string Code)
	{
		LanguageCode re = LanguageCode.EN;
		switch (Code)
		{
		case "ko": re = LanguageCode.KO; break;
		case "ja": re = LanguageCode.JA; break;
		case "zh-CN": re = LanguageCode.CN; break;
		case "zh-TW": re = LanguageCode.TW; break;
		//case "th":			return LanguageCode.TH;
		//case "tl":			return LanguageCode.TL;
		//case "ar":			return LanguageCode.AR;
		//case "jv":			return LanguageCode.JV;
		case "fr": re = LanguageCode.FR; break;
		case "es": re = LanguageCode.ES; break;
		case "pt": re = LanguageCode.PT; break;
		case "de": re = LanguageCode.DE; break;
		}
		if (IsUseLanguage(re)) return re;
		return LanguageCode.EN;
	}

	public static string Get_LanguageCode(LanguageCode Code)
	{
		if (!IsUseLanguage(Code)) return "en";
		switch (Code)
		{
		case LanguageCode.KO: return "ko";
		case LanguageCode.JA: return "ja";
		case LanguageCode.CN: return "zh-CN";		// 중국어 간체
		case LanguageCode.TW: return "zh-TW";		// 중국어 번체
		//case LanguageCode.TH: return "th";			// 태국어
		//case LanguageCode.TL: return "tl";			// 필리핀
		//case LanguageCode.AR: return "ar";			// 아랍어
		//case LanguageCode.JV: return "jv";			// 자와어(인도네시아)
		case LanguageCode.FR: return "fr";          // 스페인어
		case LanguageCode.ES: return "es";			// 스페인어
		case LanguageCode.PT: return "pt";          // 포르투갈어
		case LanguageCode.DE: return "de";          // 포르투갈어
		}
		return "en";
	}

	private static readonly Dictionary<SystemLanguage, string> COUTRY_CODES = new Dictionary<SystemLanguage, string>()
	{
		{ SystemLanguage.Afrikaans,             "ZA" },
		{ SystemLanguage.Arabic,                "SA" },
		{ SystemLanguage.Basque,                "US" },
		{ SystemLanguage.Belarusian,            "BY" },
		{ SystemLanguage.Bulgarian,             "BJ" },
		{ SystemLanguage.Catalan,               "ES" },
		{ SystemLanguage.Chinese,               "CN" },
		{ SystemLanguage.ChineseSimplified,     "CN" },
		{ SystemLanguage.ChineseTraditional,    "CN" },
		{ SystemLanguage.Czech,                 "CZ" },
		{ SystemLanguage.Danish,                "DK" },
		{ SystemLanguage.Dutch,                 "BE" },
		{ SystemLanguage.English,               "US" },
		{ SystemLanguage.Estonian,              "EE" },
		{ SystemLanguage.Faroese,               "FU" },
		{ SystemLanguage.Finnish,               "FI" },
		{ SystemLanguage.French,                "FR" },
		{ SystemLanguage.German,                "DE" },
		{ SystemLanguage.Greek,                 "JR" },
		{ SystemLanguage.Hebrew,                "IL" },
		{ SystemLanguage.Hungarian,             "HU" },
		{ SystemLanguage.Icelandic,             "IS" },
		{ SystemLanguage.Indonesian,            "ID" },
		{ SystemLanguage.Italian,               "IT" },
		{ SystemLanguage.Japanese,              "JP" },
		{ SystemLanguage.Korean,                "KR" },
		{ SystemLanguage.Latvian,               "LV" },
		{ SystemLanguage.Lithuanian,            "LT" },
		{ SystemLanguage.Norwegian,             "NO" },
		{ SystemLanguage.Polish,                "PL" },
		{ SystemLanguage.Portuguese,            "PT" },
		{ SystemLanguage.Romanian,              "RO" },
		{ SystemLanguage.Russian,               "RU" },
		{ SystemLanguage.SerboCroatian,         "SP" },
		{ SystemLanguage.Slovak,                "SK" },
		{ SystemLanguage.Slovenian,             "SI" },
		{ SystemLanguage.Spanish,               "ES" },
		{ SystemLanguage.Swedish,               "SE" },
		{ SystemLanguage.Thai,                  "TH" },
		{ SystemLanguage.Turkish,               "TR" },
		{ SystemLanguage.Ukrainian,             "UA" },
		{ SystemLanguage.Vietnamese,            "VN" },
		{ SystemLanguage.Unknown,               "US" },
	};

	/// <summary> 국가코드 </summary>
	public static string Get_CountryCode()
	{
		SystemLanguage language = Application.systemLanguage;
		string result;
		if (COUTRY_CODES.TryGetValue(language, out result))
		{
			return result;
		}
		else
		{
			return COUTRY_CODES[SystemLanguage.Unknown];
		}
	}
}
