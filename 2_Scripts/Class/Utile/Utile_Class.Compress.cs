using System.IO;
using System.IO.Compression;
using System.Text;


public partial class Utile_Class
{
	public const int COMPRESS_MODE_GZIP = 0;
	public const int COMPRESS_MODE_DEFLATE = 1;

	public static int COMPRESS_MODE = COMPRESS_MODE_DEFLATE;

	public enum CompressType
	{
		RFC1950 = 0 // zlib 스펙 RFC 1950
		, RFC1951   // zlib 스펙 RFC 1951
	}

	public static byte[] Compress(byte[] abyBuf, int nOffset, int nLen)
	{

		byte[] abyIn = new byte[nLen];
		System.Array.Copy(abyBuf, nOffset, abyIn, 0, nLen);
		MemoryStream ms = new MemoryStream();
		switch (COMPRESS_MODE)
		{
		case COMPRESS_MODE_GZIP:
			GZipStream gzip = new GZipStream(ms, CompressionMode.Compress);
			gzip.Write(abyIn, 0, nLen);
			gzip.Close();
			break;
		case COMPRESS_MODE_DEFLATE:
			DeflateStream deflate = new DeflateStream(ms, CompressionMode.Compress);
			deflate.Write(abyIn, 0, nLen);
			deflate.Close();
			break;
		}

		byte[] abyOut = ms.ToArray();
		ms.Close();
		return abyOut;
	}

	public static byte[] Decompress(byte[] abyBuf, int nOffset, int nLen, int outsize = 0, CompressType eComType = CompressType.RFC1951)
	{

		byte[] abyOut = new byte[nLen];
		System.Array.Copy(abyBuf, nOffset, abyOut, 0, nLen);
		byte[] output = abyOut;


		MemoryStream ms = new MemoryStream(abyOut);

		switch (COMPRESS_MODE)
		{
		case COMPRESS_MODE_GZIP:
			GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
			StreamReader reader = new StreamReader(gzip);
			output = Encoding.UTF8.GetBytes(reader.ReadToEnd());
			reader.Close();
			gzip.Close();
			break;
		case COMPRESS_MODE_DEFLATE:
			// 자바 측의 DeflaterOutputStream은 zlib 스펙을 정의한 RFC 1950을 따르는 반면
			// 닷넷 측의 DeflateStream은 액면 그대로 deflate 스펙을 정의한 RFC 1951을 따르기 때문에 처음의 2byte만을 건너뛰어서 사용해야함
			ms.Position = eComType == CompressType.RFC1950 ? 2 : 0;
			DeflateStream deflate = new DeflateStream(ms, CompressionMode.Decompress);
			output = ReadAllBytesFromStream(deflate);
			deflate.Close();
			break;
		}
		ms.Close();
		return output;// Encoding.UTF8.GetBytes(output);
	}
	public static byte[] ReadAllBytesFromStream(Stream stream)
	{
		MemoryStream ret = new MemoryStream();

		byte[] buffer = new byte[2048];
		int size = 0;

		while (true)
		{
			size = stream.Read(buffer, 0, buffer.Length);
			if (size > 0) ret.Write(buffer, 0, size);
			else break;
		}

		ret.Flush();
		ret.Close();
		return ret.ToArray();
	}
}
