using System;
using UnityEngine;

namespace HolidayHat.Utilities;

public static class Oklab
{
    public struct Lab
    {
        public float L;
        public float a;
        public float b;
    };

    public struct LCH
    {
        public float L;
        public float C;
        public float h;
    };

    public static Lab ToOklab(this Color color)
    {
        var c = color;

        var l = 0.4122214708f * c.r + 0.5363325363f * c.g + 0.0514459929f * c.b;
        var m = 0.2119034982f * c.r + 0.6806995451f * c.g + 0.1073969566f * c.b;
        var s = 0.0883024619f * c.r + 0.2817188376f * c.g + 0.6299787005f * c.b;

        var l_ = (float) Math.Cbrt(l);
        var m_ = (float) Math.Cbrt(m);
        var s_ = (float) Math.Cbrt(s);

        return new Lab
        {
            L = 0.2104542553f * l_ + 0.7936177850f * m_ - 0.0040720468f * s_,
            a = 1.9779984951f * l_ - 2.4285922050f * m_ + 0.4505937099f * s_,
            b = 0.0259040371f * l_ + 0.7827717662f * m_ - 0.8086757660f * s_
        };
    }
    public static LCH ToOklch(this Color color)
    {
        var ok = color.ToOklab();

        var hue = float.NaN;

        var c = Mathf.Sqrt(ok.a * ok.a + ok.b * ok.b);

        if ((c * 100) > 0)
        {
            hue = Mathf.Rad2Deg * Mathf.Atan2(ok.b, ok.a);
        }

        return new LCH
        {
            L = ok.L,
            C = c,
            h = hue
        };
    }

    public static Lab ToOklab(this LCH oklch)
    {
        var hasChroma = (oklch.C * 100 > 0);

        return new Lab
        {
            L = oklch.L,
            a = hasChroma ? oklch.C * Mathf.Cos(Mathf.Deg2Rad * oklch.h) : 0,
            b = hasChroma ? oklch.C * Mathf.Sin(Mathf.Deg2Rad * oklch.h) : 0,
        };
    }

    public static Color ToColor(this Lab c)
    {
        var l_ = c.L + 0.3963377774f * c.a + 0.2158037573f * c.b;
        var m_ = c.L - 0.1055613458f * c.a - 0.0638541728f * c.b;
        var s_ = c.L - 0.0894841775f * c.a - 1.2914855480f * c.b;

        var l = l_ * l_ * l_;
        var m = m_ * m_ * m_;
        var s = s_ * s_ * s_;

        return new Color(
            +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
            -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
            -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s
        );
    }
    public static Color ToColor(this LCH c)
    {
        return c.ToOklab().ToColor();
    }

    public static Lab Lerp(Lab a, Lab b, float f)
    {
        return new Lab
        {
            L = Mathf.Lerp(a.L, b.L, f),
            a = Mathf.Lerp(a.a, b.a, f),
            b = Mathf.Lerp(a.b, b.b, f)
        };
    }
    public static Vector2 ToVec(this Lab a)
    {
        return new Vector2
        {
            x = a.a,
            y = a.b
        };
    }

    public static Color OklabLerp(Color a, Color b, float f)
    {
        return Lerp(a.ToOklab(), b.ToOklab(), f).ToColor();
    }
    
    public static Color OklchLerp(Color a, Color b, float f)
    {
        var aLch = a.ToOklch();
        var bLch = b.ToOklch();
        
        var aLab= b.ToOklab();
        var bLab = b.ToOklab();
        
        float hue;

        var dot = Mathf.Abs( Vector2.Dot(aLab.ToVec(), bLab.ToVec()));
        var blnd = OklabLerp(a, b, f);

        var blndLch = blnd.ToOklch();

        blndLch.C = Mathf.Lerp(aLch.C, bLch.C, f);

        blndLch = Color.Lerp(blndLch.ToColor(), Color.Lerp(a, b, f), 0.5f).ToOklch();
        
        if (!float.IsNaN(aLch.h) || !float.IsNaN(bLch.h))
        {

            if (float.IsNaN(aLch.h))
            {
                hue = bLch.h;
            } else if (float.IsNaN(bLch.h))
            {
                hue = aLch.h;
            }
            else
            {
                var hueAx = Mathf.Cos(Mathf.Deg2Rad * aLch.h);
                var hueAy = Mathf.Sin(Mathf.Deg2Rad * aLch.h);
                
                var hueBx = Mathf.Cos(Mathf.Deg2Rad * bLch.h);
                var hueBy = Mathf.Sin(Mathf.Deg2Rad * bLch.h);

                var dHueX = Mathf.Lerp(hueAx, hueBx, f);
                var dHueY = Mathf.Lerp(hueAy, hueBy, f);

                hue = Mathf.Rad2Deg * Mathf.Atan2(dHueY, dHueX);

            }
            
        }
        else
        {
            hue = float.NaN;
        }

        var mix = Color.Lerp(a, b, f);

        var r = Lerp(
            new LCH
            {
                L = Mathf.Lerp(aLch.L, aLch.L, f),
                C = Mathf.Lerp(aLch.C, aLch.C, f),
                h = NormalizeHue(hue)
            }.ToOklab(),
            blndLch.ToOklab(), float.IsNaN(dot) ? 0.5f : dot);

        if (mix != null)
        {
            r = Lerp(
                mix.ToOklab(),
                r,
                0.125f
            );
        }
        
        return r.ToColor();
    }

    public static float NormalizeHue(float hue)
    {
        if (float.IsNaN(hue))
        {
            return hue;
        }

        if ((hue %= 360) < 0)
        {
            return hue + 360;
        }
        else
        {
            return hue;
        }
    }

    public static Color EvaluateOklab(this Gradient g, float time)
    {
        var it = 0;
        while (it < g.colorKeys.Length - 1 && time >= g.colorKeys[it + 1].time)
        {
            it++;
        }

        Color c;
        if (it >= g.colorKeys.Length - 1)
        {
            c = g.colorKeys[^1].color;
        }
        else
        {
            var lerpAmount = (time - g.colorKeys[it].time) / (g.colorKeys[it + 1].time - g.colorKeys[it].time);
            c = OklabLerp(g.colorKeys[it].color, g.colorKeys[it + 1].color, lerpAmount);
        }

        // alpha
        it = 0;
        while (it < g.alphaKeys.Length - 1 && time >= g.alphaKeys[it + 1].time)
        {
            it++;
        }

        if (it >= g.alphaKeys.Length - 1)
        {
            c.a = g.alphaKeys[^1].alpha;
        }
        else
        {
            var lerpAmount = (time - g.alphaKeys[it].time) / (g.alphaKeys[it + 1].time - g.alphaKeys[it].time);
            c.a = Mathf.Lerp(g.alphaKeys[it].alpha, g.alphaKeys[it + 1].alpha, lerpAmount);
        }

        return c;
    }

}