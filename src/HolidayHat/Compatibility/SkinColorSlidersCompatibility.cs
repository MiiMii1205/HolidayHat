using System.Runtime.CompilerServices;
using HolidayHat.Patchers;
using PEAKLib.Core;
using SkinColorSliders;

namespace HolidayHat.Compatibility;

public static class SkinColorSlidersCompatibility
{
    private static bool? _enabled;

    public static bool enabled
    {
        get
        {
            if (_enabled == null)
            {
                _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(
                    "com.snosz.skincolorsliders");
                HolidayHatPlugin.Log.LogInfo($"SkinColorSliders support is {((bool) _enabled ? "enabled" : "disabled")}");
            }

            return (bool) _enabled;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void ApplySkinColorToHolidayHats(PlayerCustomizationDummy pcd, int skinIndex)
    {
        var dummyColors = SkinColorManager.dummyColors;

        var face = dummyColors.colors[SkinColorManager.SkinPart.Face];

        for (var i = HolidayHatPlugin.MinHatIndex; i < HolidayHatPlugin.MaxHatIndex && i < pcd.refs.playerHats.Length; i++)
        {
            var ms = pcd.refs.playerHats[i].materials;

            foreach (var mat in ms)
            {
                // if there's a hue shift, we'll adjust the skin color's saturation and lightness to keep the perceptual difference of hus shifting the same
                mat.SetColor(CharacterCustomization.SkinColor, HolidayHatPatcher.AdaptSkinColor(face, mat));
            }

            pcd.refs.playerHats[i].SetMaterials([..ms]);
        }
    }
}