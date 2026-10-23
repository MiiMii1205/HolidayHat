using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using HarmonyLib;
using HolidayHat.Behaviour;
using HolidayHat.Compatibility;
using HolidayHat.Extensions;
using HolidayHat.Utilities;
using Peak;
using Peak.Network;
using PEAKLib.Core.Extensions;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;
using Object = UnityEngine.Object;

namespace HolidayHat.Patchers;

public static class HolidayHatPatcher
{
    public static bool dummyPatched = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Transform GetHatObject(Transform baseHat)
    {
        return baseHat.Find(@"Armature/Hip/Mid/AimJoint/Torso/Head/Hat") ??
               baseHat.Find(@"Armature/Hip (RIGIDBODY)/Mid/AimJoint/Torso/Head/Hat");
    }

    public static bool CreateHatOption(Customization customization, string name, Texture2D icon)
    {
        if (Array.Exists(customization.hats, hat => hat.name == name))
        {
            HolidayHatPlugin.Log.LogWarning($"Trying to add {name} a second time.");
            return false;
        }

        var hatOption = ScriptableObject.CreateInstance<CustomizationOption>();
        hatOption.color = Color.white;
        hatOption.name = name;
        hatOption.texture = icon;
        hatOption.type = Customization.Type.Hat;
        hatOption.requiredAchievement = ACHIEVEMENTTYPE.NONE;
        customization.hats = customization.hats.AddToArray(hatOption);

        HolidayHatPlugin.Log.LogDebug($"Added Hat Option {name} in PassportManager @ {customization.hats.Length - 1}!");

        return true;
    }

    [HarmonyPatch(typeof(PassportManager), nameof(PassportManager.Awake))]
    [HarmonyPostfix]
    [HarmonyBefore(
        "radsi.hats",
        "figgies.smorehats",
        "unknown.flarebits-who_up_pea-ing_their_king-moremustommats",
        "joberggg.BigHamHats",
        "addzeey.crumphats",
        "my.pahsiv.MyMousyWig",
        "joberggg.JobergsHatEmporium",
        "BobaHats",
        "TeddyBRB.toomanyhats",
        "MoreCustomizations")]
    public static void PassportManagerAwakePostfix(PassportManager __instance)
    {
        HolidayHatPlugin.Log.LogInfo("INIT PASPORT");

        HolidayHatPlugin.Log.LogDebug($"Adding Holiday Hat option...");
        var cal = new GregorianCalendar();

        var customization = __instance.GetComponent<Customization>();

        CreateHatOption(customization, "Holiday_Hat", HolidayHatPlugin.HolidayHatIcon);
        HolidayHatPlugin.HolidayHatCosmeticIndex = customization.hats.Length - 1;
        // Add every custom hats

        CreateHatOption(customization, "PartyHat", HolidayHatPlugin.PartyHatIcon);

        HolidayHatPlugin.SetHatIndex(new DateTime(DateTime.Today.Year, 1, 1),
            customization.hats.Length - 1);

        HolidayHatPlugin.SetHatLookup("PartyHat", customization.hats.Length - 1);

        HolidayHatPlugin.MinHatIndex = customization.hats.Length - 1;

        CreateHatOption(customization, "HeartArrowHeadHat", HolidayHatPlugin.HeartArrowHeadHatIcon);

        HolidayHatPlugin.SetHatIndex(new DateTime(DateTime.Today.Year, 2, 14),
            customization.hats.Length - 1);

        HolidayHatPlugin.SetHatLookup("HeartArrowHeadHat", customization.hats.Length - 1);

        CreateHatOption(customization, "StPatrickHat", HolidayHatPlugin.StPatrickHatIcon);

        HolidayHatPlugin.SetHatIndex(new DateTime(DateTime.Today.Year, 3, 17),
            customization.hats.Length - 1);

        HolidayHatPlugin.SetHatLookup("StPatrickHat", customization.hats.Length - 1);

        CreateHatOption(customization, "FishHat", HolidayHatPlugin.FishHatIcon);

        HolidayHatPlugin.SetHatIndex(new DateTime(DateTime.Today.Year, 4, 1),
            customization.hats.Length - 1);
        HolidayHatPlugin.SetHatLookup("FishHat", customization.hats.Length - 1);

        CreateHatOption(customization, "BunnyEars", HolidayHatPlugin.BunnyEarsIcon);

        HolidayHatPlugin.SetHatIndex(HolidayHatPlugin.GetGregorianEaster(DateTime.Today.Year),
            customization.hats.Length - 1);
        HolidayHatPlugin.SetHatLookup("BunnyEars", customization.hats.Length - 1);

        CreateHatOption(customization, "HardHat", HolidayHatPlugin.HardHatIcon);

        HolidayHatPlugin.SetHatIndex(HolidayHatPlugin.GetMondayOfMonth(DateTime.Today.Year, 5, 1, cal),
            customization.hats.Length - 1);

        HolidayHatPlugin.SetHatIndex(HolidayHatPlugin.GetMondayOfMonth(DateTime.Today.Year, 9, 1, cal),
            customization.hats.Length - 1);
        HolidayHatPlugin.SetHatLookup("HardHat", customization.hats.Length - 1);

        CreateHatOption(customization, "PilgrimHat", HolidayHatPlugin.PilgrimHatIcon);

        HolidayHatPlugin.SetHatIndex(
            cal.AddDays(HolidayHatPlugin.GetMondayOfMonth(DateTime.Today.Year, 11, 4, cal), 3),
            customization.hats.Length - 1);

        HolidayHatPlugin.SetHatIndex(HolidayHatPlugin.GetMondayOfMonth(DateTime.Today.Year, 10, 2, cal),
            customization.hats.Length - 1);
        HolidayHatPlugin.SetHatLookup("PilgrimHat", customization.hats.Length - 1);

        CreateHatOption(customization, "SantaHat", HolidayHatPlugin.SantaHatIcon);

        HolidayHatPlugin.SetHatIndex(
            new DateTime(DateTime.Today.Year, 12, 25),
            customization.hats.Length - 1);
        HolidayHatPlugin.SetHatLookup("SantaHat", customization.hats.Length - 1);

        CreateHatOption(customization, "Ribbon", HolidayHatPlugin.RibbonHatIcon);

        HolidayHatPlugin.SetHatIndex(
            new DateTime(DateTime.Today.Year, 12, 26),
            customization.hats.Length - 1);

        HolidayHatPlugin.SetHatLookup("Ribbon", customization.hats.Length - 1);

        HolidayHatPlugin.MaxHatIndex = customization.hats.Length - 1;

        // Updates the hat override index for the bundled fits

        foreach (var fit in customization.fits)
        {
            if (fit.overrideHat)
            {
                // The original position should move to +nb of hats, since we'll insert it there
                // +1 for the holiday hat
                fit.overrideHatIndex += HolidayHatPlugin.customHolidayHats.Count + 1;
            }
        }
    }

    private static bool syncHats(CustomizationRefs dst, CustomizationRefs src = null)
    {
        if (src == null)
        {
            if (Character.localCharacter is { } localChar &&
                localChar.TryGetComponent<CharacterCustomization>(out var characterCustomization))
            {
                src = characterCustomization.refs;
            }
            else
            {
                HolidayHatPlugin.Log.LogError("CharacterCustomization component is missing.");
                return false;
            }
        }

        Renderer[] srcPlayerHats = src.playerHats;
        Renderer[] dstPlayerHats = dst.playerHats;

        if (srcPlayerHats.Length == dstPlayerHats.Length)
        {
            return true;
        }

        var firstDummyRend = dstPlayerHats.FirstOrDefault();

        if (!firstDummyRend)
        {
            HolidayHatPlugin.Log.LogError("Cannot find renderer in dst");
            return false;
        }

        var hatLayer = firstDummyRend.gameObject.layer;

        var srcHatsTransform = GetHatObject(src.transform);
        var dstHatsTransform = GetHatObject(dst.transform);

        var hatRendererList = new List<Renderer>();

        var srcSelectableHats = srcPlayerHats[..^2];
        var dstSelectableHats = dstPlayerHats[..^2];

        // Add what's missing
        for (int i = dstSelectableHats.Length, length = srcSelectableHats.Length; i < length; ++i)
        {
            var scrHatInst = srcSelectableHats[i].transform;

            while (scrHatInst.parent != srcHatsTransform)
            {
                scrHatInst = scrHatInst.parent;
            }

            //Clone the top gameObject of hat instance.
            var clonedHatInstance = Object.Instantiate(scrHatInst.gameObject, dstHatsTransform, false);
            clonedHatInstance.name = scrHatInst.name;

            //Get Renderer and add it to list
            var hatInstanceRender = clonedHatInstance.GetComponentInChildren<Renderer>(true);

            hatInstanceRender.gameObject.layer = hatLayer;
            hatRendererList.Add(hatInstanceRender);
        }

        var hoodies = dst.playerHats[^2..];
        var selectableHats = dst.playerHats[..^2];

        dst.playerHats = selectableHats.AddRangeToArray(hatRendererList.ToArray()).AddRangeToArray(hoodies);
        return true;
    }

    [HarmonyPatch(typeof(PeakHandler), nameof(PeakHandler.SetCosmetics))]
    [HarmonyPrefix]
    public static void PeakHandlerSetCosmeticsPrefix(PeakHandler __instance, List<Character> characters)
    {
        // the character who climbs the helicopter
        if (!syncHats(__instance.firstCutsceneScout))
            HolidayHatPlugin.Log.LogError($"Something went wrong [firstCutsceneScout]...");

        // the characters who sit down
        for (int i = 0, length = __instance.cutsceneScoutRefs.Count(); i < length; ++i)
        {
            if (!syncHats(__instance.cutsceneScoutRefs[i]))
                HolidayHatPlugin.Log.LogError(
                    $"Something went wrong [cutsceneScoutRefs-{i}]...");
        }

        // True ending characters
        for (int i = 0, length = __instance.finalCutsceneScoutRefs.Count(); i < length; ++i)
        {
            if (!syncHats(__instance.finalCutsceneScoutRefs[i]))
                HolidayHatPlugin.Log.LogError(
                    $"Something went wrong [finalCutsceneScoutRefs-{i}]...");
        }
    }
    
    [HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.PulseStatus))]
    [HarmonyPostfix]
    public static void PulseStatusPostfix(CharacterCustomization __instance, Color c, float intensity = 1f)
    {

        if (__instance._character)
        {
            var player = __instance.overridePhotonPlayer ?? __instance._character.view.Owner;

            var d = CharacterCustomization.GetCustomizationData(player);
            var fitIndex = d.currentOutfit;

            var currentHat = d.currentHat;
            
            if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
            {
                currentHat = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
            }

            if (currentHat == HolidayHatPlugin.HolidayHatCosmeticIndex)
            {
                currentHat = HolidayHatPlugin.GetHatOrDefault(
                    player);
            }

            if (IsWearingCustomHolidayHat(currentHat))
            {
                if (__instance.refs.playerHats[currentHat].name.StartsWith("Bunny") ||
                    __instance.refs.playerHats[currentHat].name.StartsWith("Fish"))
                {
                    // Only bunnies and fishes get the status glow
                    foreach (var r in __instance.refs.playerHats[currentHat].gameObject.GetComponentsInChildren<Renderer>(true))
                    {
                        for (int i = 0, length = r. materials.Length; i < length; ++i)
                        {
                            r.materials[i]
                                .SetColor(CharacterCustomization.StatusColor, c);
                            r.materials[i]
                                .SetFloat(CharacterCustomization.StatusGlow, intensity);
                            r.materials[i]
                                .DOFloat(0.0f, CharacterCustomization.StatusGlow, 0.5f);
                        }
                    }
                }
            }

        }
        
    }

    [HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.OnPlayerDataChange))]
    [HarmonyPostfix]
    [HarmonyAfter(
        "radsi.hats",
        "figgies.smorehats",
        "unknown.flarebits-who_up_pea-ing_their_king-moremustommats",
        "joberggg.BigHamHats",
        "addzeey.crumphats",
        "my.pahsiv.MyMousyWig",
        "joberggg.JobergsHatEmporium",
        "BobaHats",
        "TeddyBRB.toomanyhats",
        "MoreCustomizations")]
    public static void CharacterCustomizationPlayerDataChangedPostfix(CharacterCustomization __instance,
        PersistentPlayerData playerData)
    {
        if (__instance.refs.PlayerRenderers[0] != null)
        {
            if (__instance._character.isBot && __instance.overridePhotonPlayer != null)
            {
                HolidayHatPlugin.Log.LogDebug(
                    $"Received character changes for Bot copy of {__instance.overridePhotonPlayer.NickName}");

                // Bot that mimics a player. Let's use their data instead
                UpdateHolidayHat(__instance, playerData, __instance.overridePhotonPlayer);
            }
            else if (!__instance._character.data.isScoutmaster &&
                     !__instance.ignorePlayerCosmetics)
            {
                HolidayHatPlugin.Log.LogDebug(
                    $"Received character changes for {__instance.view.Owner.NickName}");

                UpdateHolidayHat(__instance, playerData, __instance.view.Owner);
            }
            else
            {
                if (__instance._character.data.isScoutmaster)
                {
                    HolidayHatPlugin.Log.LogDebug(
                        $"Bot is SCOUTMASTER. Skipping...");
                }
                else if (__instance.ignorePlayerCosmetics)
                {
                    HolidayHatPlugin.Log.LogDebug(
                        $"Ignoring player cosmetics. Skipping...");
                }
            }
        }
        else
        {
            HolidayHatPlugin.Log.LogWarning(
                $"Can't receive changes for {(__instance._character.isBot ? (__instance.overridePhotonPlayer != null ? ("Copy of " + __instance.overridePhotonPlayer.NickName) : __instance.view.Owner.NickName) + "(BOT)" : __instance.view.Owner.NickName)}. No renderers are present.");
        }
    }

    [HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.Awake))]
    [HarmonyPostfix]
    [HarmonyBefore(
        "radsi.hats",
        "figgies.smorehats",
        "unknown.flarebits-who_up_pea-ing_their_king-moremustommats",
        "joberggg.BigHamHats",
        "addzeey.crumphats",
        "my.pahsiv.MyMousyWig",
        "joberggg.JobergsHatEmporium",
        "BobaHats",
        "TeddyBRB.toomanyhats",
        "MoreCustomizations")]
    public static void CharacterCustomizationAwakePostfix(CharacterCustomization __instance)
    {
        var hatsObject = GetHatObject(__instance.refs.transform);

        var playerName = __instance.overridePhotonPlayer != null
            ? $"{__instance._character.characterName} (copy of {__instance.overridePhotonPlayer.NickName})"
            : __instance._character.characterName;

        // Inserting the Holiday Hat placeholder right now
        var hoodies = __instance.refs.playerHats[^2..];
        var selectableHats = __instance.refs.playerHats[..^2];

        // Just to make sure HolidayHat plays nice with other custom hat mods
        var cloned = Object.Instantiate(selectableHats[0].gameObject, selectableHats[0].transform.parent, false);

        cloned.name = "HolidayHatPlaceholder";

        cloned.transform.SetLocalPositionAndRotation(selectableHats[0].transform.localPosition,
            selectableHats[0].transform.localRotation);

        __instance.refs.playerHats =
            selectableHats.AddToArray(cloned.GetComponent<Renderer>()).AddRangeToArray(hoodies);

        HolidayHatPlugin.Log.LogInfo(
            $"Inserted Hat '{cloned.name}' @ [{selectableHats.Length + 1}] for {playerName}!");

        for (int i = 0, length = HolidayHatPlugin.customHolidayHats.Count; i < length; ++i)
        {
            var hat = HolidayHatPlugin.customHolidayHats[i];

            var newHat = Object.Instantiate(hat, hatsObject, true);

            SetLayerRecursively(newHat.transform, hatsObject.gameObject.layer);
            newHat.name = HolidayHatPlugin.CleanedName(newHat.name);

            var meshRenderers = newHat.GetComponentsInChildren<Renderer>(true);

            foreach (var meshRenderer in meshRenderers)
            {
                var mats = meshRenderer.materials;

                for (int mti = 0, mtl = mats.Length; mti < mtl; ++mti)
                {
                    mats[mti].shader = Shader.Find(mats[mti].shader.name);
                    // Materials shouldn't interact with each others
                    mats[mti] = Object.Instantiate(mats[mti]);
                }

                meshRenderer.SetMaterials([..mats]);
            }

            HolidayHatPlugin.Log.LogDebug($"Adding Hat '{newHat.name}' for {playerName}...");

            var hatTransform = hat.transform;

            newHat.transform.localPosition = hatTransform.position;
            newHat.transform.localRotation = hatTransform.rotation;
            newHat.transform.localScale = hatTransform.localScale;

            newHat.SetActive(false);

            hoodies = __instance.refs.playerHats[^2..];
            selectableHats = __instance.refs.playerHats[..^2];

            // When there are multiple renderers (mainly for special skel models) use the FIRST renderer as the main one, since we're gonna take it into account while updating player hats
            __instance.refs.playerHats = selectableHats.AddToArray(meshRenderers[0]).AddRangeToArray(hoodies);

            HolidayHatPlugin.Log.LogInfo(
                $"Inserted Hat '{newHat.name}' @ [{selectableHats.Length + 1}] for {playerName}!");
        }
    }

    private static void SetHatMaterial(Material[] ms, PersistentPlayerData playerData, GameObject hatP,
        bool isSkel = false, bool isLocal = false)
    {
        if (hatP.name.StartsWith("Bunny"))
        {
            var accI = CharacterCustomization.GetAccessoryIndex(playerData);

            if (accI == 17 || isSkel)
            {
                // Hide the nose since the player is wearing the Groucho mask
                HolidayHatPlugin.Log.LogDebug("Hiding bunny nose!");
                ms[0] = HolidayHatPlugin.NoRenderMaterial;
            }
            else
            {
                // Show the nose
                HolidayHatPlugin.Log.LogDebug("Showing bunny nose!");
                ms[0] = ms[2];
            }

            if (isLocal)
            {
                // Hide those pesky cosmetics
                ms[0].SetFloat("_VertexGhost", 100f);
                ms[1].SetFloat("_VertexGhost", 100f);
                ms[2].SetFloat("_VertexGhost", 100f);
            }
        }

        else if (hatP.name.StartsWith("Pilgrim"))
        {
            // Change the pilgrim material based on the current fit.

            var shoe = Singleton<Customization>.Instance.fits[playerData.customizationData.currentOutfit]
                .fitMaterialShoes;

            var hat = Singleton<Customization>.Instance.fits[playerData.customizationData.currentOutfit]
                .fitHatMaterial;

            if (!shoe || shoe.name.EndsWith("Castaway"))
            {
                // Can't use the shoe material for Castaways since it's there's no shoes.
                shoe = HolidayHatPlugin.DefaultPilgrimHatMaterial;
            }

            if (shoe)
            {
                ms[1].SetTexture(CharacterCustomization.MainTex,
                    shoe.GetTexture(CharacterCustomization.MainTex));

                var tintMult = 0.25f;

                if (shoe.name is "M_AviatorFur" or "M_Tarnished Gold Starboy Fit" or "M_Willow_Swamp_MassChara")
                {
                    // Not typical shoes material. We need lighter hats
                    tintMult = 1f;
                }

                ms[1].CopyPropsListFromMaterial(shoe, [
                    "_BaseColor",
                    "_Color1",
                    "_Color2",
                    "_Color3",
                    "_Color4",
                    "_Remap1",
                    "_Remap2",
                    "_Remap3",
                    "_Remap4",
                    "_MainTexMovement",
                    "_Tex1Movement",
                    "_Tex2Movement",
                    "_Tex3Movement",
                    "_Tex4Movement",
                    "_Tint",
                    "_HueShifts"
                ], [
                    "_Height1",
                    "_Height2",
                    "_Height3",
                    "_Height4",
                    "_HueStr",
                    "_Smooth1",
                    "_Smooth2",
                    "_Smooth3",
                    "_Smooth4",
                    "_Triplanar1On",
                    "_Triplanar2On",
                    "_Triplanar3On",
                    "_Triplanar4On",
                    "_UV1",
                    "_UV2",
                    "_UV3",
                    "_UV4",
                    "_UseHeightMask1",
                    "_UseHeightMask2",
                    "_UseHeightMask3",
                    "_UseHeightMask4",
                    "_SpecularPower",
                    "_AddSpecular",
                    "_NoShadowSpec",
                    "_Stepalphabyvertexcolor"
                ], [
                    "_MainTex",
                    "_Texture1",
                    "_Texture2",
                    "_Texture3",
                    "_Texture4",
                ]);

                ms[1].SetColor("_Tint", shoe.GetColor("_Tint") * tintMult);

                ms[1].SetTextureScale(CharacterCustomization.MainTex,
                    shoe.name == "M_Tarnished Gold Starboy Fit" ? new Vector2(0.75f, 1) : Vector2.one);
            }

            if (hat)
            {
                // ms[0] = Object.Instantiate(hat);

                if (hat.name is "M_Scout_Starboy" or "M_Scout_Tropical")
                {
                    // They use more complicated paterns 
                    ms[0].CopyPropsListFromMaterial(hat, [
                        "_BaseColor",
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4",
                        "_MainTexMovement",
                        "_Tex1Movement",
                        "_Tex2Movement",
                        "_Tex3Movement",
                        "_Tex4Movement",
                        "_Tint",
                        "_HueShifts"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_HueStr",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4",
                        "_SpecularPower",
                        "_AddSpecular",
                        "_NoShadowSpec",
                        "_Stepalphabyvertexcolor"
                    ], [
                        "_MainTex",
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);
                }
                else
                {
                    ms[0].CopyPropsListFromMaterial(hat, [
                        "_BaseColor",
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4",
                        "_MainTexMovement",
                        "_Tex1Movement",
                        "_Tex2Movement",
                        "_Tex3Movement",
                        "_Tex4Movement",
                        "_Tint",
                        "_HueShifts"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_HueStr",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4",
                        "_SpecularPower",
                        "_AddSpecular",
                        "_NoShadowSpec",
                        "_Stepalphabyvertexcolor"
                    ], [
                        "_MainTex",
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);

                    ms[0].CopyPropsListFromMaterial(HolidayHatPlugin.DefaultPilgrimHatMaterial, [
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4"
                    ], [
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);
                }

                ms[0].SetTextureOffset(CharacterCustomization.MainTex, new Vector2(0,
                    hat.name.EndsWith("Castaway") ? 0.09f : 0));
            }

            // Set the base buckle textrue to the hat one
            ms[2].SetTexture(CharacterCustomization.MainTex,
                ms[0].GetTexture(CharacterCustomization.MainTex));

            ms[2].SetFloat("_SpecularPower", ms[0].GetFloat("_SpecularPower"));
            ms[2].SetFloat("_AddSpecular", ms[0].GetFloat("_AddSpecular") + 10);
            ms[2].SetFloat("_NoShadowSpec", ms[0].GetFloat("_NoShadowSpec"));
        }

        else if (hatP.name.StartsWith("Party"))
        {
            var hat = Singleton<Customization>.Instance.fits[playerData.customizationData.currentOutfit]
                .fitHatMaterial;

            if (hat)
            {
                if (hat.name is "M_Scout_Starboy" or "M_Scout_Tropical")
                {
                    // Using fancy smanchy patterns
                    ms[0].CopyPropsListFromMaterial(hat, [
                        "_BaseColor",
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4",
                        "_MainTexMovement",
                        "_Tex1Movement",
                        "_Tex2Movement",
                        "_Tex3Movement",
                        "_Tex4Movement",
                        "_Tint",
                        "_HueShifts"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_HueStr",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4",
                        "_SpecularPower",
                        "_AddSpecular",
                        "_NoShadowSpec",
                        "_Stepalphabyvertexcolor"
                    ], [
                        "_MainTex",
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);
                }
                else
                {
                    ms[0].CopyPropsListFromMaterial(hat, [
                        "_BaseColor",
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4",
                        "_MainTexMovement",
                        "_Tex1Movement",
                        "_Tex2Movement",
                        "_Tex3Movement",
                        "_Tex4Movement",
                        "_Tint",
                        "_HueShifts"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_HueStr",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4",
                        "_SpecularPower",
                        "_AddSpecular",
                        "_NoShadowSpec",
                        "_Stepalphabyvertexcolor"
                    ], [
                        "_MainTex",
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);

                    ms[0].CopyPropsListFromMaterial(HolidayHatPlugin.DefaultPartyHatMaterial, [
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4"
                    ], [
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);
                }
            }
        }

        else if (hatP.name.StartsWith("Ribbon"))
        {
            // Change the pilgrim material based on the current fit.

            var hat = Singleton<Customization>.Instance.fits[playerData.customizationData.currentOutfit]
                .fitHatMaterial;

            if (hat)
            {
                if (hat.name is "M_Scout_Starboy" or "M_Scout_Tropical")
                {
                    // Using fancy smanchy patterns
                    ms[0].CopyPropsListFromMaterial(hat, [
                        "_BaseColor",
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4",
                        "_MainTexMovement",
                        "_Tex1Movement",
                        "_Tex2Movement",
                        "_Tex3Movement",
                        "_Tex4Movement",
                        "_Tint",
                        "_HueShifts"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_HueStr",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4",
                        "_SpecularPower",
                        "_AddSpecular",
                        "_NoShadowSpec",
                        "_Stepalphabyvertexcolor"
                    ], [
                        "_MainTex",
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);
                }
                else
                {
                    ms[0].CopyPropsListFromMaterial(hat, [
                        "_BaseColor",
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4",
                        "_MainTexMovement",
                        "_Tex1Movement",
                        "_Tex2Movement",
                        "_Tex3Movement",
                        "_Tex4Movement",
                        "_Tint",
                        "_HueShifts"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_HueStr",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4",
                        "_SpecularPower",
                        "_AddSpecular",
                        "_NoShadowSpec",
                        "_Stepalphabyvertexcolor"
                    ], [
                        "_MainTex",
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);

                    ms[0].CopyPropsListFromMaterial(HolidayHatPlugin.DefaultRibbonHatMaterial, [
                        "_Color1",
                        "_Color2",
                        "_Color3",
                        "_Color4",
                        "_Remap1",
                        "_Remap2",
                        "_Remap3",
                        "_Remap4"
                    ], [
                        "_Height1",
                        "_Height2",
                        "_Height3",
                        "_Height4",
                        "_Smooth1",
                        "_Smooth2",
                        "_Smooth3",
                        "_Smooth4",
                        "_Triplanar1On",
                        "_Triplanar2On",
                        "_Triplanar3On",
                        "_Triplanar4On",
                        "_UV1",
                        "_UV2",
                        "_UV3",
                        "_UV4",
                        "_UseHeightMask1",
                        "_UseHeightMask2",
                        "_UseHeightMask3",
                        "_UseHeightMask4"
                    ], [
                        "_Texture1",
                        "_Texture2",
                        "_Texture3",
                        "_Texture4",
                    ]);
                }

                var fit = Singleton<Customization>.Instance.fits[playerData.customizationData.currentOutfit];

                var offset = new Vector2(0, 0);
                var scale = new Vector2(1, 1);

                if (fit.fitMaterialOverrideHat == null ||
                    fit.fitMaterialOverrideHat.name.EndsWith("CookieHat"))
                {
                    if (hat.name.EndsWith("Toga"))
                    {
                        offset.x = 0.6821f;
                        offset.y = -0.4f;
                        scale.x = -scale.x;
                        scale.y = -scale.y;
                    }
                    else
                    {
                        // Uses the same texture as the fit
                        offset.x = -0.64f;
                        offset.y = 0.1355f;
                    }
                }
                else if (
                    fit.fitMaterialOverrideHat.name.EndsWith("ScoutHat") ||
                    fit.fitMaterialOverrideHat.name.EndsWith("HatOverride") ||
                    fit.fitMaterialOverrideHat.name.EndsWith("AviatorHat") ||
                    fit.fitMaterialOverrideHat.name.EndsWith("CargoOverride")
                )
                {
                    // uses a "scout hat" texture, which is mainly a hat with maybe a logo if we're lucky
                    offset = Vector2.zero;

                    if (fit.fitMaterialOverrideHat.name is "M_Scout_JesterHatOverride" or "M_Scout_SleepyGuyHatOverride"
                        or "M_Scout_GreenTeeCargoOverride" or "M_Scout_AviatorHat")
                    {
                        scale.y = scale.x = 1.003f;
                    }
                }
                else if (fit.fitMaterialOverrideHat.name is "M_BigGallonHat")
                {
                    // Cowboys play differently 
                    offset.x = -0.3828f;
                    offset.y = -0.4141f;
                }

                ms[0].SetTextureOffset(CharacterCustomization.MainTex, offset);
                ms[0].SetTextureScale(CharacterCustomization.MainTex, scale);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWearingCustomHolidayHat(int currentHatIndex)
    {
        return currentHatIndex >= HolidayHatPlugin.MinHatIndex && currentHatIndex <= HolidayHatPlugin.MaxHatIndex;
    }

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.UpdateDummy))]
    [HarmonyPostfix]
    [HarmonyBefore(
        "radsi.hats",
        "figgies.smorehats",
        "unknown.flarebits-who_up_pea-ing_their_king-moremustommats",
        "joberggg.BigHamHats",
        "addzeey.crumphats",
        "my.pahsiv.MyMousyWig",
        "joberggg.JobergsHatEmporium",
        "BobaHats",
        "TeddyBRB.toomanyhats",
        "MoreCustomizations")]
    public static void PlayerCustomizationDummyPrefix(PlayerCustomizationDummy __instance,
        Photon.Realtime.Player player)
    {
        var pToUse = player ?? PhotonNetwork.LocalPlayer;

        var playerData = GameHandler.GetService<PersistentPlayerDataService>()
            .GetPlayerData(pToUse);

        var fitIndex = CharacterCustomization.GetFitIndex(playerData);

        var currentHat = playerData.customizationData.currentHat;
        if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
        {
            currentHat = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
        }

        if (currentHat == HolidayHatPlugin.HolidayHatCosmeticIndex)
        {
            currentHat = HolidayHatPlugin.GetHatOrDefault(
                pToUse);

            HolidayHatPlugin.Log.LogDebug(
                $"Showing Hat '{__instance.refs.playerHats[currentHat].gameObject.name}' on Dummy of {pToUse.NickName}...");

            __instance.SetPlayerHat(currentHat);
        }

        if (IsWearingCustomHolidayHat(currentHat))
        {
            // If it's from a petrified dummy, we need to override skin-dependent materials

            if (__instance.TryGetComponent<PetrifiedScout>(out var ps))
            {
                var petrifiedMaterial = ps.transform.GetChild(0).GetChild(4).GetComponent<Renderer>().materials[0];

                var pms = __instance.refs.playerHats[currentHat].materials;

                for (int i = 0, length = pms.Length; i < length; ++i)
                {
                    if (pms[i].GetFloat(UseSkinTone) >= 1.0f)
                    {
                        pms[i] = petrifiedMaterial;
                    }
                }

                __instance.refs.playerHats[currentHat].SetMaterials([..pms]);
            }

            var customHatIndex = currentHat - HolidayHatPlugin.MinHatIndex;

            var ms = __instance.refs.playerHats[currentHat].materials;

            for (int i = 0, length = ms.Length; i < length; ++i)
            {
                ms[i] = Object.Instantiate(ms[i]);
            }

            SetHatMaterial(ms, playerData, HolidayHatPlugin.customHolidayHats[customHatIndex], false, false);

            if (HolidayHatPlugin.customHolidayHats[customHatIndex].name.StartsWith("Fish"))
            {
                // Dummies are NEVER skeleton!
                __instance.refs.playerHats[currentHat].enabled = true;
                __instance.refs.playerHats[currentHat].transform.GetChild(0).gameObject.SetActive(false);

                HolidayHatPlugin.Log.LogDebug(
                    $"Hiding Fish Bones on Dummy of {pToUse.NickName}...");
            }

            __instance.refs.playerHats[currentHat].SetMaterials([..ms]);
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.SetPlayerColor))]
    [HarmonyPostfix]
    [HarmonyAfter(
        "radsi.hats",
        "figgies.smorehats",
        "unknown.flarebits-who_up_pea-ing_their_king-moremustommats",
        "joberggg.BigHamHats",
        "addzeey.crumphats",
        "my.pahsiv.MyMousyWig",
        "joberggg.JobergsHatEmporium",
        "BobaHats",
        "TeddyBRB.toomanyhats",
        "MoreCustomizations")]
    public static void PlayerCustomizationDummyPColorPrefix(PlayerCustomizationDummy __instance,
        int index)
    {
        if (SkinColorSlidersCompatibility.enabled)
        {
            SkinColorSlidersCompatibility.ApplySkinColorToHolidayHats(__instance, index);
        }
        else
        {
            // For vanilla colors (or modded skin colors that have options in the passport)
            var col = Singleton<Customization>.Instance.skins[index].color;

            for (int i = HolidayHatPlugin.MinHatIndex, length = __instance.refs.playerHats.Length;
                 i < HolidayHatPlugin.MaxHatIndex && i < length;
                 ++i)
            {
                var ms = __instance.refs.playerHats[i].materials;

                foreach (var mat in ms)
                {
                    // if there's a hue shift, we'll adjust the skin color's saturation and lightness to keep the perceptual difference of hus shifting the same
                    mat.SetColor(CharacterCustomization.SkinColor, AdaptSkinColor(col, mat));
                }

                __instance.refs.playerHats[i].SetMaterials([..ms]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetPlayerSkinColor(PersistentPlayerData data, Photon.Realtime.Player player)
    {
        if (SkinColorSlidersCompatibility.enabled)
        {
            return SkinColorSlidersCompatibility.GetPlayerSkinColor(data, player);
        }
        else
        {
            HolidayHatPlugin.Log.LogWarning(
                $"Using {player.NickName}'s skin color ([{data.customizationData.currentSkin}])!");
            return Singleton<Customization>.Instance
                .skins[data.customizationData.currentSkin].color;
        }
    }

    [HarmonyPatch(typeof(GameUtils), nameof(GameUtils.Awake))]
    [HarmonyPostfix]
    public static void GameUtilsAwakePost(GameHandler __instance)
    {
        __instance.gameObject.GetOrAddComponent<HolidayHatHandler>();
    }

    [HarmonyPatch(typeof(CustomizationRefs), nameof(CustomizationRefs.SetSkeleton))]
    [HarmonyPostfix]
    public static void SetSkelPost(CustomizationRefs __instance, bool active, bool isLocal)
    {
        if ((isLocal && PhotonNetwork.LocalPlayer is var pToUse) ||
            (PhotonNetwork.TryGetPlayer(__instance.customization._character.player.GetActorNumber(), out pToUse)))
        {
            var playerData = GameHandler.GetService<PersistentPlayerDataService>()
                .GetPlayerData(pToUse);

            HolidayHatPlugin.Log.LogInfo(
                $"Player {pToUse} turned {(active ? "into a skeleton" : "human")}. Updating their hat.");

            // No need to update dummies since they're NEVER skeletons
            UpdateHolidayHat(__instance.customization, playerData, pToUse);
        }
    }

    public static Color AdaptSkinColor(Color skinColor, Material m)
    {
        var skinC = new Color(skinColor.r, skinColor.g, skinColor.b, skinColor.a);
        var shiftVector = m.GetVector(HueShifts);

        var shiftVal = 0f;

        // Still not sure how hue shifts works
        if (!Mathf.Approximately(shiftVector.x, 0.0f))
        {
            shiftVal = shiftVector.x;
        }
        else if (!Mathf.Approximately(shiftVector.y, 0.0f))
        {
            shiftVal = shiftVector.y;
        }

        if (
            m.GetFloat(HueStr) > 0.0f
            &&
            !Mathf.Approximately(shiftVal, 0.0f)
        )
        {
            var hueDeg = shiftVal * 360;

            var lch = skinC.ToOklch();

            if (!float.IsNaN(lch.h))
            {
                lch.h += hueDeg;

                var shifted = lch.ToColor();

                Color.RGBToHSV(shifted, out var h, out var s, out var v);
                Color.RGBToHSV(skinC, out var skH, out var skS, out var skV);

                // Add the differance in sat AND value

                var skDiffS = (s - skS);
                var skDiffV = (v - skV);

                skinC = Color.HSVToRGB(skH, skS + skDiffS, skV + skDiffV);
            }
            else
            {
                // When the color is monochromatic (hue is NaN), use a lightly tinted version of the shifted color

                var redC = Singleton<Customization>.Instance.skins[0].color;

                var rlch = redC.ToOklch();
                rlch.h += hueDeg;

                var col = Oklab.Lerp(rlch.ToOklab(), lch.ToOklab(), 0.75f);

                Color.RGBToHSV(col.ToColor(), out var h, out var s, out var v);
                Color.RGBToHSV(skinC, out var skH, out var skS, out var skV);

                // Add the differance in sat AND value

                var skDiffS = (s - skS);
                var skDiffV = (v - skV);

                skinC = Color.HSVToRGB(h, skS + skDiffS, skV + skDiffV);
            }
        }

        return skinC;
    }

    public static void UpdateHolidayHat(CharacterCustomization c, PersistentPlayerData data,
        Photon.Realtime.Player? player = null)
    {
        player ??= (c.view ?? c._character.photonView).Owner;

        var fitIndex = CharacterCustomization.GetFitIndex(data);

        var currentHat = data.customizationData.currentHat;

        if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
        {
            currentHat = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
        }

        MeshFilter? mshf = null;

        if (currentHat == HolidayHatPlugin.HolidayHatCosmeticIndex)
        {
            currentHat = HolidayHatPlugin.GetHatOrDefault(
                player);

            for (int i = 0, length = c.refs.playerHats.Length; i < length; ++i)
            {
                c.refs.playerHats[i].gameObject
                    .SetActive(currentHat == i);

                if (currentHat == i)
                {
                    mshf = c.refs.playerHats[i].GetComponent<MeshFilter>();

                    HolidayHatPlugin.Log.LogDebug(
                        $"Showing Hat '{c.refs.playerHats[i].name}' for {player.NickName}...");
                }
            }

            if (!IsWearingCustomHolidayHat(currentHat))
            {
                UpdateHatShadow(mshf ?? c.refs.playerHats[currentHat].GetComponent<MeshFilter>(), c);
                RefreshHolidayHatHide(c.refs.playerHats[currentHat].GetComponent<Renderer>(), c._character);
            }
        }

        if (IsWearingCustomHolidayHat(currentHat))
        {
            var isSkel = false;

            if (c._character)
            {
                isSkel = c._character.data.isSkeleton;
            }

            var ms = c.refs.playerHats[currentHat]
                .materials;

            // Player color is bugged. It only gets the color of the OWNER. This means that mimicing/zombies of other players uses the HOST color instead!
            var colorToUse = GetPlayerSkinColor(data, player);

            var skelMaterial = c.refs.skeletonRenderer.material;
            var skelCleanNamed = HolidayHatPlugin.CleanedName(skelMaterial.name);

            var customHatIndex = currentHat - HolidayHatPlugin.MinHatIndex;

            var fishSkel = false;

            if (HolidayHatPlugin.customHolidayHats[customHatIndex].name.StartsWith("Fish"))
            {
                var bones = c.refs.playerHats[currentHat]
                    .transform.GetChild(0).gameObject;

                if (isSkel)
                {
                    // Deactivate the fish hat and activate the fish bone
                    c.refs.playerHats[currentHat].enabled = false;

                    bones.gameObject.SetActive(true);

                    ms = bones.GetComponent<Renderer>().materials;

                    fishSkel = true;

                    HolidayHatPlugin.Log.LogDebug(
                        $"Showing Fish Bones on {player.NickName}...");
                }
                else
                {
                    c.refs.playerHats[currentHat].enabled = true;
                    bones.gameObject.SetActive(false);

                    HolidayHatPlugin.Log.LogDebug(
                        $"Hiding Fish Bones on {player.NickName}...");
                }
            }

            SetHatMaterial(ms, data, HolidayHatPlugin.customHolidayHats[customHatIndex], isSkel, player.IsLocal);

            for (var i = 0; i < ms.Length; i++)
            {
                var cleanName = HolidayHatPlugin.CleanedName(ms[i].name);

                if (cleanName == skelCleanNamed)
                {
                    if (isSkel)
                    {
                        // Old skeleton material
                        if (IsHueShifting(ms[i]) && !ms[i].GetColor(BaseColor).Equals(skelMaterial.GetColor(BaseColor)))
                        {
                            // It's been changed, so let's recalculate the colors
                            var boneTint = skelMaterial.GetColor(BaseColor).ToOklch();

                            var oklchToUse = colorToUse.ToOklch();

                            boneTint.C = Mathf.Lerp(boneTint.C, oklchToUse.C, 0.125f);

                            if (!float.IsNaN(oklchToUse.h))
                            {
                                boneTint.h = oklchToUse.h;
                            }

                            ms[i].SetColor(BaseColor, boneTint.ToColor());
                        }
                    }
                    else
                    {
                        // Try getting the original mesh's material
                        var ogM = Object.Instantiate(
                            (fishSkel
                                ? HolidayHatPlugin.customHolidayHats[customHatIndex].transform.GetChild(0).gameObject
                                : HolidayHatPlugin.customHolidayHats[customHatIndex])
                            .GetComponent<Renderer>()
                            .materials[i]
                        );

                        // copy only important parameters

                        ogM.CopyPropsListFromMaterial(ms[i], ["_HeightMaskSmoothness"],
                            ["_VertexGhost", "_FlipHeightMask"], []);

                        ms[i] = ogM;

                        // if there's a hue shift, we'll adjust the skin color's saturation and lightness to keep the perceptual difference of hus shifting the same
                        ms[i].SetColor(CharacterCustomization.SkinColor, AdaptSkinColor(colorToUse, ms[i]));
                    }
                }
                else if (ms[i].GetFloat(UseSkinTone) >= 1.0f)
                {
                    // Brand new skeleton colors
                    if (isSkel)
                    {
                        // Any skin-toned materials gets replaced with skeletal bones.
                        var s = Object.Instantiate(skelMaterial);

                        if (
                            IsHueShifting(ms[i]) &&
                            HolidayHatPlugin.CleanedName(ms[i].name) is not ("M_Fish" or "M_BunnyEars"))
                        {
                            var boneTint = s.GetColor(BaseColor).ToOklch();

                            var oklchToUse = colorToUse.ToOklch();

                            boneTint.C = Mathf.Lerp(boneTint.C, oklchToUse.C, 0.125f);

                            if (!float.IsNaN(oklchToUse.h))
                            {
                                boneTint.h = oklchToUse.h;
                            }

                            s.SetColor(BaseColor, boneTint.ToColor());
                        }

                        var ogM = (fishSkel
                                ? HolidayHatPlugin.customHolidayHats[customHatIndex].transform.GetChild(0).gameObject
                                : HolidayHatPlugin.customHolidayHats[customHatIndex]).GetComponent<Renderer>()
                            .materials[i];
                        //
                        // ogM.CopyPropsListFromMaterial(ms[i], ["_HeightMaskSmoothness"],
                        //     ["_VertexGhost", "_FlipHeightMask"], []);
                        //
                        s.CopyPropsListFromMaterial(ogM, [
                                "_HueShifts"
                            ],
                            [
                                "_VertexGhost",
                                "_HueStr"
                            ], []);

                        ms[i] = s;
                    }
                    else
                    {
                        // Try getting the original mesh's material
                        var ogM = Object.Instantiate(
                            (fishSkel
                                ? HolidayHatPlugin.customHolidayHats[customHatIndex].transform.GetChild(0).gameObject
                                : HolidayHatPlugin.customHolidayHats[customHatIndex])
                            .GetComponent<Renderer>()
                            .materials[i]);

                        // copy only important parameters

                        ogM.CopyPropsListFromMaterial(ms[i], ["_HeightMaskSmoothness"],
                            ["_VertexGhost", "_FlipHeightMask"], []);

                        ms[i] = ogM;

                        // if there's a hue shift, we'll adjust the skin color's saturation and lightness to keep the perceptual difference of hus shifting the same
                        ms[i].SetColor(CharacterCustomization.SkinColor, AdaptSkinColor(colorToUse, ms[i]));
                    }
                }
            }

            if (fishSkel)
            {
                c.refs.playerHats[currentHat].transform.GetChild(0).GetComponent<Renderer>()
                    .SetMaterials([..ms]);

                RefreshHolidayHatHide(c.refs.playerHats[currentHat].transform.GetChild(0)
                    .GetComponent<Renderer>(), c._character, 0f, 100f);

                UpdateHatShadow(c.refs.playerHats[currentHat].transform.GetChild(0)
                        .GetComponent<MeshFilter>(),
                    c,
                    c.refs.playerHats[currentHat].transform);
            }
            else
            {
                c.refs.playerHats[currentHat]
                    .SetMaterials([..ms]);

                RefreshHolidayHatHide(c.refs.playerHats[currentHat], c._character, 0f,
                    (HolidayHatPlugin.customHolidayHats[customHatIndex].name.StartsWith("Bunny") ||
                     HolidayHatPlugin.customHolidayHats[customHatIndex].name.StartsWith("Fish"))
                        ? 100f
                        : 1f);

                UpdateHatShadow(c.refs.playerHats[currentHat].GetComponent<MeshFilter>(),
                    c);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsHueShifting(Material material)
    {
        var shiftVector = material.GetVector(HueShifts);

        var shiftVal = 0f;

        if (!Mathf.Approximately(shiftVector.x, 0.0f))
        {
            shiftVal = shiftVector.x;
        }
        else if (!Mathf.Approximately(shiftVector.y, 0.0f))
        {
            shiftVal = shiftVector.y;
        }

        return material.GetFloat(HueStr) > 0.0f && !Mathf.Approximately(shiftVal, 0.0f);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RefreshHolidayHatHide(Renderer r, Character? character, float showOn = 0f, float showOff = 1f)
    {
        if (character && character.refs.hideTheBody)
        {
            character.refs.hideTheBody.SetShowing(r, character.refs.hideTheBody.isShowing ? showOn : showOff);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateHatShadow(MeshFilter filter, CharacterCustomization? c, Transform? tToUse = null)
    {
        if (c && c.refs.hatShadowMeshFilter)
        {
            var toUse = (tToUse ?? filter.transform);

            c.refs.hatShadowMeshFilter.sharedMesh = filter.sharedMesh;
            c.refs.hatShadowMeshFilter.transform.SetPositionAndRotation(toUse.position,
                toUse.rotation);
            c.refs.hatShadowMeshFilter.transform.localScale = toUse.localScale;

            // Adding bogus materials to match the sub-mesh count.
            var r = c.refs.hatShadowMeshFilter.GetComponent<Renderer>();
            var m = new List<Material>();

            for (int i = 0, length = filter.sharedMesh.subMeshCount; i < length; ++i)
            {
                m.Add(r.material);
            }

            r.SetMaterials(m);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetLayerRecursively(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            SetLayerRecursively(child, layer);
        }
    }

    private static PlayerCustomizationDummy lastModifiedDummy;
    private static readonly int UseSkinTone = Shader.PropertyToID("_UseSkinTone");
    private static readonly int HueStr = Shader.PropertyToID("_HueStr");
    private static readonly int HueShifts = Shader.PropertyToID("_HueShifts");
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.UpdateDummy))]
    [HarmonyPrefix]
    [HarmonyBefore(
        "radsi.hats",
        "figgies.smorehats",
        "unknown.flarebits-who_up_pea-ing_their_king-moremustommats",
        "joberggg.BigHamHats",
        "addzeey.crumphats",
        "my.pahsiv.MyMousyWig",
        "joberggg.JobergsHatEmporium",
        "BobaHats",
        "TeddyBRB.toomanyhats",
        "MoreCustomizations")]
    public static void PlayerCustomizationDummyUpdateDummyPrefix(PlayerCustomizationDummy __instance,
        Photon.Realtime.Player player)
    {
        if (__instance != lastModifiedDummy)
        {
            lastModifiedDummy = __instance;
            var pToUse = player ?? PhotonNetwork.LocalPlayer;

            var hatsObject = GetHatObject(__instance.refs.transform);

            // Inserting the Holiday Hat placeholder right now
            var hoodies = __instance.refs.playerHats[^2..];
            var selectableHats = __instance.refs.playerHats[..^2];

            // Cloning the scout cap to make HolidayHat play nicely with other hat mods.
            var cloned = Object.Instantiate(selectableHats[0].gameObject, selectableHats[0].transform.parent, false);

            cloned.transform.SetLocalPositionAndRotation(selectableHats[0].transform.localPosition,
                selectableHats[0].transform.localRotation);

            cloned.name = "HolidayHatPlaceholder";

            __instance.refs.playerHats =
                selectableHats.AddToArray(cloned.GetComponent<Renderer>()).AddRangeToArray(hoodies);

            HolidayHatPlugin.Log.LogInfo(
                $"Inserted Hat '{cloned.name}' @ [{selectableHats.Length + 1}] for Dummy of {pToUse.NickName}!");

            for (int i = 0, length = HolidayHatPlugin.customHolidayHats.Count; i < length; ++i)
            {
                var hat = HolidayHatPlugin.customHolidayHats[i];

                var newHat = Object.Instantiate(hat, hatsObject, true);

                newHat.name = HolidayHatPlugin.CleanedName(newHat.name);

                SetLayerRecursively(newHat.transform, hatsObject.gameObject.layer);

                var meshRenderers = newHat.GetComponentsInChildren<MeshRenderer>(true);

                foreach (var meshRenderer in meshRenderers)
                {
                    var mats = meshRenderer.materials;

                    for (int mti = 0, mtl = mats.Length; mti < mtl; ++mti)
                    {
                        mats[mti].shader = Shader.Find(mats[mti].shader.name);
                        // Materials shouldn't interact with each others
                        mats[mti] = Object.Instantiate(mats[mti]);
                    }

                    meshRenderer.SetMaterials([..mats]);
                }

                HolidayHatPlugin.Log.LogDebug($"Adding Hat '{newHat.name}' for Dummy of {pToUse.NickName}...");

                var hatTransform = hat.transform;

                newHat.transform.localPosition = hatTransform.position;
                newHat.transform.localRotation = hatTransform.rotation;
                newHat.transform.localScale = hatTransform.localScale;

                newHat.SetActive(false);

                hoodies = __instance.refs.playerHats[^2..];
                selectableHats = __instance.refs.playerHats[..^2];

                // When there are multiple renderers (mainly for special skel models) use the FIRST renderer as the main one, since we're gonna take it into account while updating player hats
                __instance.refs.playerHats = selectableHats.AddToArray(meshRenderers[0]).AddRangeToArray(hoodies);

                HolidayHatPlugin.Log.LogInfo(
                    $"Inserted Hat '{newHat.name}' @ [{selectableHats.Length + 1}] for Dummy of {pToUse.NickName}!");
            }
        }
    }
}