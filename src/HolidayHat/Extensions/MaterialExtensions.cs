using System.Collections.Generic;
using UnityEngine;

namespace HolidayHat.Extensions;

public static class MaterialExtensions
{
    public static void CopyPropsListFromMaterial(this Material to, Material from, List<string> colorsProps,
        List<string> floatProps, List<string> textureProps)
    {
        foreach (var p in colorsProps)
        {
            if (int.TryParse(p,  out var i))
            {
                to.SetColor(i, from.GetColor(i));
            }
            else
            {
                to.SetColor(p, from.GetColor(p));
            }
            
        }
        
        foreach (var p in floatProps)
        {
            if (int.TryParse(p, out var i))
            {
                to.SetFloat(i, from.GetFloat(i));
            }
            else
            {
                to.SetFloat(p, from.GetFloat(p));
            }
            
        }
        
        foreach (var p in textureProps)
        {

            if (int.TryParse(p, out var i))
            {
                to.SetTexture(i, from.GetTexture(i));
                to.SetTextureOffset(i, from.GetTextureOffset(i));
                to.SetTextureScale(i, from.GetTextureScale(i));
            }
            else
            {
                to.SetTexture(p, from.GetTexture(p));
                to.SetTextureOffset(p, from.GetTextureOffset(p));
                to.SetTextureScale(p, from.GetTextureScale(p));
            }
            
        }
    }
}