using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////////////////////
// 암호화 및 복호화
public partial class Utile_Class
{
	const int ENCODE_KEY = 78372;
	const int ENCODE_C1 = 57351;
	const int ENCODE_C2 = 26397;
	//바이트 배열 암호화
	public static bool Encode(byte[] abySrc, int nLen, int nStartOffset)
	{
		int i, key = ENCODE_KEY;

		if (abySrc == null || nLen <= 0) return false;

		for (i = nStartOffset; i < nLen; i++)
		{
			abySrc[i] = (byte)((abySrc[i] & 0xFF) ^ key >> 8);
			key = ((abySrc[i] & 0xFF) + key) * ENCODE_C1 + ENCODE_C2;
		}
		return true;
	}

	//바이트 배열 복호화
	public static bool Decode(byte[] abySrc, int nLen, int nStartOffset)
	{
		int i, key = ENCODE_KEY;
		byte pre;

		if (abySrc == null || nLen < 1) return false;

		for (i = nStartOffset; i < nLen; i++)
		{
			pre = abySrc[i];
			abySrc[i] = (byte)((abySrc[i] & 0xFF) ^ key >> 8);
			key = ((pre & 0xFF) + key) * ENCODE_C1 + ENCODE_C2;
		}
		return true;
	}

	public static string Get_FileName_Encode(string strName)
	{
		// 단순하게 뒤에서 부터 바이트 16진수로 표현한다.
		string strFileName = Path.GetFileName(strName);
		byte[] aubyName = Encoding.UTF8.GetBytes(strFileName);
		StringBuilder strTemp = new StringBuilder();
		for (int i = aubyName.Length - 1; i > -1; i--) strTemp.AppendFormat("{0:x}", aubyName[i]);
		return strTemp.ToString();
	}

	public static string Get_FileName_Decode(string strName)
	{
		// 단순하게 뒤에서 부터 바이트 16진수로 표현한다.
		string strFileName = Path.GetFileName(strName);
		byte[] aubyName = Encoding.UTF8.GetBytes(strFileName);
		StringBuilder strTemp = new StringBuilder();
		for (int i = strName.Length - 2; i > -1; i -= 2)
		{
			var hex = strName.Substring(i, 2);
			int value = Convert.ToInt32(hex, 16);
			string stringValue = Char.ConvertFromUtf32(value);
			strTemp.AppendFormat(stringValue);
		}
		return strTemp.ToString();
	}

	public static string UserNoEncrypt(long UserNo)
	{
		// 202203150000000001
		// AHZHzqJ4y/0XiU9VQlxRZQ1l
		var abyData = Encoding.UTF8.GetBytes(UserNo.ToString());
		//// ARTCnFw/gqN7TKVLhg==
		//abyData = Compress(abyData, 0, abyData.Length);
		// Encrypt
		Encode(abyData, abyData.Length, 0);

		// Base64
		return Convert.ToBase64String(abyData);
	}
	public static long UserNoDecrypt(string data)
	{
		if (string.IsNullOrEmpty(data)) return 0;
		try
		{
			data = Check_Base64Code(data);
			var abyData = Convert.FromBase64String(data);

			// Decrypt
			Decode(abyData, abyData.Length, 0);

			// Decompress
			//abyData = Decompress(abyData, 0, abyData.Length, CompressType.RFC1951);
			long userno = 0;
			return long.TryParse(Encoding.UTF8.GetString(abyData), out userno) ? userno : 0;
		}
#pragma warning disable 0168
		catch (Exception e)
		{
			return 0;
		}
#pragma warning restore 0168
	}

	public static string GetToken_Decode(string token)
	{
		var parts = token.Split('.');
		if (parts.Length > 2)
		{
			var decode = Check_Base64Code(parts[1]);
			var bytes = Convert.FromBase64String(decode);
			return Encoding.UTF8.GetString(bytes);
		}
		return "";
	}

	public static string Check_Base64Code(string data)
	{
		var padLength = 4 - data.Length % 4;
		if (padLength < 4)
		{
			data += new string('=', padLength);
		}
		return data;
	}

	public static string Base64UrlEncode(byte[] input)
	{
		var output = Convert.ToBase64String(input);
		output = output.Split('=')[0]; // Remove any trailing '='s
		output = output.Replace('+', '-'); // 62nd char of encoding
		output = output.Replace('/', '_'); // 63rd char of encoding
		return output;
	}

	public static string Base64UrlDecode(string data)
	{
		var output = data;
		output = output.Replace('-', '+'); // 62nd char of encoding
		output = output.Replace('_', '/'); // 63rd char of encoding
		switch (output.Length % 4) // Pad with trailing '='s
		{
		case 0: break; // No pad chars in this case
		case 2: output += "=="; break; // Two pad chars
		case 3: output += "="; break;  // One pad char
		default: throw new Exception("Illegal base64url string!");
		}
		return output;
	}

	public static string GetJWT_Decoding(string token)
	{
		var parts = token.Split('.');
		if (parts.Length > 2)
		{
			var decode = Base64UrlDecode(parts[1]);
			var bytes = Convert.FromBase64String(decode);
			return Encoding.UTF8.GetString(bytes);
		}
		return "";
	}


	public static string Get_PayLoad_Decoding(string payload)
	{
		// Base64
		var abyData = Convert.FromBase64String(payload);

		// Decrypt
		Decode(abyData, abyData.Length, 0);

		return Encoding.UTF8.GetString(abyData);
	}
}
