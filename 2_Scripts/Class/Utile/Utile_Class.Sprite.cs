using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class Utile_Class
{
	public enum ImageFilterMode : int
	{
		Nearest = 0,
		Biliner = 1,
		Average = 2
	}

    public IEnumerator GetCaptureSprite(Action<Sprite> _CB)
	{
		yield return new WaitForEndOfFrame();

		float m_SafeW = Screen.width, m_SafeH = Screen.height;
		Texture2D texture = new Texture2D((int)m_SafeW, (int)m_SafeH, TextureFormat.RGB24, false);
		texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
		texture.Apply();
		Rect rect = new Rect(0, 0, texture.width, texture.height);
        _CB?.Invoke(Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f)));
    }
    public IEnumerator GetCaptureSprite(Image[] _imgs)
    {
        yield return new WaitForEndOfFrame();

        float m_SafeW = Screen.width, m_SafeH = Screen.height;
        Texture2D texture = new Texture2D((int)m_SafeW, (int)m_SafeH, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        for (int i = 0; i < _imgs.Length; i++) _imgs[i].sprite = sprite;
    }

    public IEnumerator GetCaptureResizeSprite(Action<Sprite> _CB, float Scale = 0.5f)
    {
        yield return new WaitForEndOfFrame();

        float m_SafeW = Screen.width, m_SafeH = Screen.height;
        Texture2D texture = new Texture2D((int)m_SafeW, (int)m_SafeH, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        texture = ResizeTexture(texture, ImageFilterMode.Average, Scale);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        _CB?.Invoke(Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f)));
    }


    public IEnumerator GetCaptureBlurSprite(Action<Sprite> _CB, int _Dist = 1)
    {
        yield return new WaitForEndOfFrame();
        float m_SafeW = Screen.width, m_SafeH = Screen.height;
        Texture2D texture = new Texture2D((int)m_SafeW, (int)m_SafeH, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        texture = TextureBlur(texture, _Dist);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        _CB?.Invoke(Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f)));
    }

    Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, float pScale)
    {

        //*** Variables
        int i;

        //*** Get All the source pixels
        Color[] aSourceColor = pSource.GetPixels(0);
        Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

        //*** Calculate New Size
        float xWidth = Mathf.RoundToInt((float)pSource.width * pScale);
        float xHeight = Mathf.RoundToInt((float)pSource.height * pScale);

        //*** Make New
        Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = (int)xWidth * (int)xHeight;
        Color[] aColor = new Color[xLength];

        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

        //*** Loop through destination pixels and process
        Vector2 vCenter = new Vector2();
        for (i = 0; i < xLength; i++)
        {

            //*** Figure out x&y
            float xX = (float)i % xWidth;
            float xY = Mathf.Floor((float)i / xWidth);

            //*** Calculate Center
            vCenter.x = (xX / xWidth) * vSourceSize.x;
            vCenter.y = (xY / xHeight) * vSourceSize.y;

            //*** Do Based on mode
            //*** Nearest neighbour (testing)
            if (pFilterMode == ImageFilterMode.Nearest)
            {

                //*** Nearest neighbour (testing)
                vCenter.x = Mathf.Round(vCenter.x);
                vCenter.y = Mathf.Round(vCenter.y);

                //*** Calculate source index
                int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

                //*** Copy Pixel
                aColor[i] = aSourceColor[xSourceIndex];
            }

            //*** Bilinear
            else if (pFilterMode == ImageFilterMode.Biliner)
            {

                //*** Get Ratios
                float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
                float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

                //*** Get Pixel index's
                int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
                int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

                //*** Calculate Color
                aColor[i] = Color.Lerp(
                    Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
                    Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
                    xRatioY
                );
            }

            //*** Average
            else if (pFilterMode == ImageFilterMode.Average)
            {

                //*** Calculate grid around point
                int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
                int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
                int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
                int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

                //*** Loop and accumulate
                Color oColorTemp = new Color();
                float xGridCount = 0;
                for (int iy = xYFrom; iy < xYTo; iy++)
                {
                    for (int ix = xXFrom; ix < xXTo; ix++)
                    {

                        //*** Get Color
                        oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                        //*** Sum
                        xGridCount++;
                    }
                }

                //*** Average Color
                aColor[i] = oColorTemp / (float)xGridCount;
            }
        }

        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();

        //*** Return
        return oNewTex;
    }

    //Gaussian 5*5
    public Texture2D TextureBlur(Texture2D pSource, int dist = 1)
    {
        //*** Variables
        int i;
        //*** Get All the source pixels
        Color[] aSourceColor = pSource.GetPixels(0);

        //*** Calculate New Size
        int xWidth = pSource.width;
        int xHeight = pSource.height;

        //*** Make New
        Texture2D oNewTex = new Texture2D(xWidth, xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = xWidth * xHeight;
        Color[] aColor = new Color[xLength];


        //*** Loop through destination pixels and process
        for (i = 0; i < xLength; i++)
        {
            //*** Figure out x&y
            int xX = i % xWidth;
            int xY = i / xWidth;

            int[] indexs = new int[25];

            int tc2 = Mathf.Clamp(i - xWidth * dist * 2, 0, xLength - 1); indexs[0] = tc2;
            int tl2 = Mathf.Clamp(tc2 - dist, 0, xLength - 1); indexs[1] = tl2;
            int tll2 = Mathf.Clamp(tc2 - dist * 2, 0, xLength - 1); indexs[2] = tll2;
            int tr2 = Mathf.Clamp(tc2 + dist, 0, xLength - 1); indexs[3] = tr2;
            int trr2 = Mathf.Clamp(tc2 + dist * 2, 0, xLength - 1); indexs[4] = trr2;

            int tc = Mathf.Clamp(i - xWidth * dist, 0, xLength - 1); indexs[5] = tc;
            int tl = Mathf.Clamp(tc - dist, 0, xLength - 1); indexs[6] = tl;
            int tll = Mathf.Clamp(tc - dist * 2, 0, xLength - 1); indexs[7] = tll;
            int tr = Mathf.Clamp(tc + dist, 0, xLength - 1); indexs[8] = tr;
            int trr = Mathf.Clamp(tc + dist * 2, 0, xLength - 1); indexs[9] = trr;

            int mc = i; indexs[10] = mc;
            int ml = Mathf.Clamp(i - dist, 0, xLength - 1); indexs[11] = ml;
            int mll = Mathf.Clamp(i - dist * 2, 0, xLength - 1); indexs[12] = mll;
            int mr = Mathf.Clamp(i + dist, 0, xLength - 1); indexs[13] = mr;
            int mrr = Mathf.Clamp(i + dist * 2, 0, xLength - 1); indexs[14] = mrr;

            int dc = Mathf.Clamp(i + xWidth * dist, 0, xLength - 1); indexs[15] = dc;
            int dl = Mathf.Clamp(dc - dist, 0, xLength - 1); indexs[16] = dl;
            int dll = Mathf.Clamp(dc - dist * 2, 0, xLength - 1); indexs[17] = dll;
            int dr = Mathf.Clamp(dc + dist, 0, xLength - 1); indexs[18] = dr;
            int drr = Mathf.Clamp(dc + dist * 2, 0, xLength - 1); indexs[19] = drr;


            int dc2 = Mathf.Clamp(i + xWidth * dist * 2, 0, xLength - 1); indexs[20] = dc2;
            int dl2 = Mathf.Clamp(dc2 - dist, 0, xLength - 1); indexs[21] = dl2;
            int dll2 = Mathf.Clamp(dc2 - dist * 2, 0, xLength - 1); indexs[22] = dll2;
            int dr2 = Mathf.Clamp(dc2 + dist, 0, xLength - 1); indexs[23] = dr2;
            int drr2 = Mathf.Clamp(dc2 + dist * 2, 0, xLength - 1); indexs[24] = drr2;

            float r = 0, g = 0, b = 0;
            for (int j = 0; j < indexs.Length; j++)
            {
                r += aSourceColor[indexs[j]].r;
                g += aSourceColor[indexs[j]].g;
                b += aSourceColor[indexs[j]].b;
            }
            r /= indexs.Length;
            g /= indexs.Length;
            b /= indexs.Length;
            aColor[i] = new Color(r, g, b, 1);
        }
        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();
        return oNewTex;
    }
    public Texture2D GetTextureFromSprite(Sprite sprite) {
        if (sprite.rect.width != sprite.texture.width) {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}
