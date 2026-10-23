using System.Runtime.CompilerServices;
using HolidayHat.Patchers;
using PEAKLib.Core;
using SkinColorSliders;
using UnityEngine;
using Zorro.Core;

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

        for (int i = HolidayHatPlugin.MinHatIndex, length= pcd.refs.playerHats.Length; i < HolidayHatPlugin.MaxHatIndex && i <
             length; ++i)
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

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static Color GetPlayerSkinColor(PersistentPlayerData data, Photon.Realtime.Player player)
    {
        HolidayHatPlugin.Log.LogDebug($"Fetching {player.NickName}'s face color..");
        var color = SkinColorManager.GetColor(player.ActorNumber,
            SkinColorManager.SkinPart.Face);

        if (color.HasValue)
        {
            HolidayHatPlugin.Log.LogDebug($"Found face color {color.Value} for {player.NickName}.");
            return color.Value;
        }
        else
        {
            HolidayHatPlugin.Log.LogWarning($"No skin colors found for {player.NickName}. Using skin color [{data.customizationData.currentSkin}]!");
            return Singleton<Customization>.Instance
                .skins[data.customizationData.currentSkin].color;
        }
    }
}