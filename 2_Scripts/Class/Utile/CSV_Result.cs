//#define CSV_ERROR_CHECK
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

#pragma warning disable 0168
public class CSV_Result {
	string[][]	m_pastrData;
	int			m_nLineOffset;
	CultureInfo m_Culture = new CultureInfo("en-US");
	public CSV_Result(string strData)
	{
		Load(strData, 0);
	}

	public CSV_Result(string strData, int nStartLine)
	{
		Load(strData, nStartLine);
	}

	void Load(string strData, int nStartLine)
	{
		string[] sprit = {"\r\n"};
		string[] astrLine = strData.Trim().Split(sprit, System.StringSplitOptions.RemoveEmptyEntries);
		CsvRow pOut = new CsvRow();
		int i, iMax = astrLine.Length;
		int j, jMax;
		m_pastrData = new string[iMax - nStartLine][];
		int nOffset = 0;
		for(i = nStartLine; i < iMax; i++, nOffset++)
		{
			Read(astrLine[i], pOut);
			m_pastrData[nOffset] = new string[pOut.Count];
			for(j = 0, jMax = pOut.Count; j < jMax; j++)	m_pastrData[nOffset][j] = pOut[j].Replace("\\n", "\r\n").Replace("\'", "'");
		}
	}
	
		
	/// <summary>
	/// Reads a row of data from a CSV file
	/// </summary>
	/// <param name="row"></param>
	/// <returns></returns>
	public bool Read(string strData, CsvRow pOut)
	{
		pOut.LineText = strData;
		pOut.Clear();
		if (string.IsNullOrEmpty(pOut.LineText))	return false;

		int pos = 0;
		int rows = 0;

		while (pos < pOut.LineText.Length)
		{
			string value;

			// Special handling for quoted field
			if (pOut.LineText[pos] == '"')
			{
				// Skip initial quote
				pos++;

				// Parse quoted value
				int start = pos;
				while (pos < pOut.LineText.Length)
				{
					// Test for quote character
					if (pOut.LineText[pos] == '"')
					{
						// Found one
						pos++;

						// If two quotes together, keep one
						// Otherwise, indicates end of value
						if (pos >= pOut.LineText.Length || pOut.LineText[pos] == ',')
						{
							pos--;
							break;
						}
					}
					pos++;
				}
				value = pOut.LineText.Substring(start, pos - start);
				value = value.Replace("\"\"", "\"");
			}
			else
			{
				// Parse unquoted value
				int start = pos;
				while (pos < pOut.LineText.Length && pOut.LineText[pos] != ',')	pos++;

				value = pOut.LineText.Substring(start, pos - start);
			}

			// Add field to list
			if (rows < pOut.Count)	pOut[rows] = value;
			else					pOut.Add(value);
			rows++;

			// Eat up to and including next comma
			while (pos < pOut.LineText.Length && pOut.LineText[pos] != ',')	pos++;

			if (pos < pOut.LineText.Length)	pos++;
		}
		// Delete any unused items
		while (pOut.Count > rows)	pOut.RemoveAt(rows);

		// Return true if any columns read
		return (pOut.Count > 0);
	}
		
	public int Get_LineSize()
	{
		return m_pastrData.Length;
	}
	
	public bool IS_Nextdata()
	{
		return m_pastrData.Length > m_nLineOffset;
	}

	public bool next()
	{
		m_nLineOffset++;
		m_nLineReadPos = 0;
		return (m_pastrData.Length < m_nLineOffset);
	}

	int m_nLineReadPos = 0;
	public void NextReadPos()
	{
		m_nLineReadPos++;
	}
	public int GetPos()
	{
		return m_nLineReadPos;
	}
	
		
	public int Get_ColumnSize()
	{
		return m_pastrData[m_nLineOffset].Length;
	}

	public object Get_Enum(Type type)
	{
		object objRe = Get_Enum(type, m_nLineReadPos);
		m_nLineReadPos++;
		return objRe;
	}

	public T Get_Enum<T>() where T : System.Enum
	{
		T objRe = Get_Enum<T>(m_nLineReadPos);
		m_nLineReadPos++;
		return objRe;
	}

	public bool Get_Boolean()
	{
		bool bRe = Get_Boolean(m_nLineReadPos);
		m_nLineReadPos++;
		return bRe;
	}

	public sbyte Get_Int8()
	{
		sbyte byRe = Get_Int8(m_nLineReadPos);
		m_nLineReadPos++;
		return byRe;
	}

	public byte Get_UInt8()
	{
		byte ubyRe = Get_UInt8(m_nLineReadPos);
		m_nLineReadPos++;
		return ubyRe;
	}

	public int Get_Int16()
	{
		int nRe = Get_Int16(m_nLineReadPos);
		m_nLineReadPos++;
		return nRe;
	}

	public int Get_UInt16()
	{
		int nRe = Get_UInt16(m_nLineReadPos);
		m_nLineReadPos++;
		return nRe;
	}
	
	public float Get_Float()
	{
		float fRe = Get_Float(m_nLineReadPos);
		m_nLineReadPos++;
		return fRe;
	}
	
	public int Get_Int32()
	{
		int nRe = Get_Int32(m_nLineReadPos);
		m_nLineReadPos++;
		return nRe;
	}
	
	public long Get_UInt32()
	{
		long lRe = Get_UInt32(m_nLineReadPos);
		m_nLineReadPos++;
		return lRe;
	}
	
	public long Get_Int64()
	{
		long lRe = Get_Int64(m_nLineReadPos);
		m_nLineReadPos++;
		return lRe;
	}

	public ulong Get_UInt64()
	{
		ulong lRe = Get_UInt64(m_nLineReadPos);
		m_nLineReadPos++;
		return lRe;
	}

	public double Get_Double()
	{
		double dRe = Get_Double(m_nLineReadPos);
		m_nLineReadPos++;
		return dRe;
	}
	
	public string Get_String()
	{
		string strRe = Get_String(m_nLineReadPos);
		m_nLineReadPos++;
		return strRe;
	}

	public object Get_Enum(Type type, int nPos)
	{
		return Get_Enum(type, m_nLineOffset, nPos);
	}

	public T Get_Enum<T>(int nPos) where T : System.Enum
	{
		return Get_Enum<T>(m_nLineOffset, nPos);
	}

	public bool Get_Boolean(int nPos)
	{
		return Get_Boolean(m_nLineOffset, nPos);
	}

	public sbyte Get_Int8(int nPos)
	{
		return Get_Int8(m_nLineOffset, nPos);
	}

	public byte Get_UInt8(int nPos)
	{
		return Get_UInt8(m_nLineOffset, nPos);
	}

	public short Get_Int16(int nPos)
	{
		return Get_Int16(m_nLineOffset, nPos);
	}

	public ushort Get_UInt16(int nPos)
	{
		return Get_UInt16(m_nLineOffset, nPos);
	}
	
	public float Get_Float(int nPos)
	{
		return Get_Float(m_nLineOffset, nPos);
	}
	
	public int Get_Int32(int nPos)
	{
		return Get_Int32(m_nLineOffset, nPos);
	}
	
	public uint Get_UInt32(int nPos)
	{
		return Get_UInt32(m_nLineOffset, nPos);
	}
	
	public long Get_Int64(int nPos)
	{
		return Get_Int64(m_nLineOffset, nPos);
	}

	public ulong Get_UInt64(int nPos)
	{
		return Get_UInt64(m_nLineOffset, nPos);
	}

	public double Get_Double(int nPos)
	{
		return Get_Double(m_nLineOffset, nPos);
	}
	
	public string Get_String(int nPos)
	{
		return Get_String(m_nLineOffset, nPos);
	}

	public bool Get_Boolean(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return false;
		string data = m_pastrData[nLine][nPos];
		sbyte temp = 0;
		if (sbyte.TryParse(data, NumberStyles.Integer, m_Culture, out temp)) return temp != 0;
		bool re = false;
		return bool.TryParse(data, out re) ? re : false;
	}

	public object Get_Enum(Type type, int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		string value = m_pastrData[nLine][nPos];
		if (value == null || value.Length < 1) return 0;
		try
		{
			return Enum.Parse(type, value, true);
		}
		catch (Exception e)
		{
			Debug.LogWarning(e);
			return 0;
		}
	}

	public T Get_Enum<T>(int nLine, int nPos) where T : System.Enum
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return (T)(object)0;
		string value = m_pastrData[nLine][nPos];
		if (value == null || value.Length < 1) return (T)(object)0;
		try
		{
			return (T)(object)Enum.Parse(typeof(T), value, true);
		}
		catch (Exception e)
		{
			Debug.LogWarning(e);
			return (T)(object)0;
		}

	}

	public sbyte Get_Int8(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		sbyte temp = 0;
		if (sbyte.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format Int8 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public byte Get_UInt8(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		byte temp = 0;
		if (byte.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format UInt8 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public short Get_Int16(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		short temp = 0;
		if (short.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format Int16 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public ushort Get_UInt16(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		ushort temp = 0;
		if (ushort.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format UInt16 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public float Get_Float(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		float temp = 0;
		if (float.TryParse(m_pastrData[nLine][nPos], NumberStyles.AllowThousands | NumberStyles.Float, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format float : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public int Get_Int32(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		int temp = 0;
		if (int.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format Int32 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public uint Get_UInt32(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		uint temp = 0;
		if (uint.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format UInt32 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public long Get_Int64(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		long temp = 0;
		if (long.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format Int64 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public ulong Get_UInt64(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		ulong temp = 0;
		if (ulong.TryParse(m_pastrData[nLine][nPos], NumberStyles.Integer, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format UInt64 : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public double Get_Double(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return 0;
		double temp = 0;
		if (double.TryParse(m_pastrData[nLine][nPos], NumberStyles.AllowThousands | NumberStyles.Float, m_Culture, out temp)) return temp;
#if CSV_ERROR_CHECK
		else Debug.LogError("Format Double : " + m_pastrData[nLine][nPos]);
#endif
		return 0;
	}

	public string Get_String(int nLine, int nPos)
	{
		if (m_pastrData.Length <= nLine || m_pastrData[nLine].Length <= nPos) return "";
		string temp = m_pastrData[nLine][nPos];
		return !string.IsNullOrEmpty(temp) ? temp : "";
	}

	/// <summary>
	/// Class to store one CSV row
	/// </summary>
	public class CsvRow : List<string>
	{
		public string LineText { get; set; }
	}
}
#pragma warning restore 0168
