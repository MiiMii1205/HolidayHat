using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HolidayHat.Behaviour;
using HolidayHat.Compatibility;
using HolidayHat.Extensions;
using HolidayHat.Utilities;
using Peak;
using Peak.Network;
using PEAKLib.Core.Extensions;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Object = UnityEngine.Object;

namespace HolidayHat.Patchers;

public static class HolidayHatPatcher
{
    public static bool dummyPatched = false;

    private static readonly Color SkeletonColor = new(1f, 0.9597551f, 0.8459119f, 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Transform GetHatObject(Transform baseHat)
    {
        var hatsObject = baseHat.Find(@"Armature/Hip/Mid/AimJoint/Torso/Head/Hat");

        if (hatsObject == null)
        {
            // For petrified scouts
            hatsObject = baseHat.Find(@"Armature/Hip (RIGIDBODY)/Mid/AimJoint/Torso/Head/Hat");
        }
        
        return hatsObject;
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
    [HarmonyBefore("MoreCustomizations")]
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

        for (var i = dstSelectableHats.Length; i < srcSelectableHats.Length; ++i)
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
        for (int i = 0; i < __instance.cutsceneScoutRefs.Count(); i++)
        {
            if (!syncHats(__instance.cutsceneScoutRefs[i]))
                HolidayHatPlugin.Log.LogError(
                    $"Something went wrong [cutsceneScoutRefs-{i}]...");
        }

        // True ending characters
        for (int i = 0; i < __instance.finalCutsceneScoutRefs.Count(); i++)
        {
            if (!syncHats(__instance.finalCutsceneScoutRefs[i]))
                HolidayHatPlugin.Log.LogError(
                    $"Something went wrong [finalCutsceneScoutRefs-{i}]...");
        }
    }

    [HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.OnPlayerDataChange))]
    [HarmonyPostfix]
    public static void CharacterCustomizationPlayerDataChangedPostfix(CharacterCustomization __instance,
        PersistentPlayerData playerData)
    {

        if (__instance._character.isBot && __instance.overridePhotonPlayer != null)
        {
            // Bot that mimics a player. Let's use their data instead
            UpdateHolidayHat(__instance._character, playerData, __instance.overridePhotonPlayer);
        }
        else
        {
            UpdateHolidayHat(__instance._character, playerData);
        }
        
    }

    [HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.Awake))]
    [HarmonyPostfix]
    [HarmonyBefore("MoreCustomizations")]
    public static void CharacterCustomizationAwakePostfix(CharacterCustomization __instance)
    {
        var hatsObject = GetHatObject(__instance.refs.transform);

        // Inserting the Holiday Hat placeholder right now
        var hoodies = __instance.refs.playerHats[^2..];
        var selectableHats = __instance.refs.playerHats[..^2];

        __instance.refs.playerHats = selectableHats.AddToArray(selectableHats[0]).AddRangeToArray(hoodies);

        HolidayHatPlugin.Log.LogInfo($"Inserted Placeholder Hat @ {selectableHats.Length + 1}!");

        for (var i = 0; i < HolidayHatPlugin.customHolidayHats.Count; i++)
        {
            var newHat = Object.Instantiate(HolidayHatPlugin.customHolidayHats[i], hatsObject, true);
            SetLayerRecursively(newHat.transform, hatsObject.gameObject.layer);

            var meshRenderer = newHat.GetComponent<MeshRenderer>();

            var mats = meshRenderer.materials;

            for (int index = 0, length = mats.Length; index < length; ++index)
            {
                mats[index].shader = Shader.Find(mats[index].shader.name);
                // Materials shouldn't interact with each others
                mats[index] = Object.Instantiate(mats[index]);
            }

            meshRenderer.SetMaterials([..mats]);

            HolidayHatPlugin.Log.LogDebug($"Adding Hat {newHat.name}...");

            newHat.transform.localPosition = HolidayHatPlugin.customHolidayHats[i].transform.position;
            newHat.transform.localRotation = HolidayHatPlugin.customHolidayHats[i].transform.rotation;
            newHat.transform.localScale = HolidayHatPlugin.customHolidayHats[i].transform.localScale;

            newHat.SetActive(false);

            hoodies = __instance.refs.playerHats[^2..];
            selectableHats = __instance.refs.playerHats[..^2];

            __instance.refs.playerHats = selectableHats.AddToArray(meshRenderer).AddRangeToArray(hoodies);

            HolidayHatPlugin.Log.LogInfo($"Inserted Holiday Hat {newHat.name} @ {selectableHats.Length + 1}!");
        }

        // if (HolidayHatPlugin.CurrentHolidayHatPrefab is not { } hatPrefab)
        // {
        //     /*
        //      * If there's no holiday hat today, we STILL need to fill in a placeholder hat to keep cosmetics offset in check
        //      * But that's ok because we're going to override it later anyway
        //      */
        //     
        //     var hoodies = __instance.refs.playerHats[^2..];
        //     var selectableHats = __instance.refs.playerHats[..^2];
        //     
        //     __instance.refs.playerHats = selectableHats.AddToArray(selectableHats[0]).AddRangeToArray(hoodies);
        //     
        //     HolidayHatPlugin.Log.LogInfo($"Inserted Placeholder Hat @ {selectableHats.Length + 1}!");
        // }
        // else
        // {
        //     // There's only one custom hat right now and it's the holiday hat option
        //     var newHat = Object.Instantiate(hatPrefab, hatsObject, true);
        //     SetLayerRecursively(newHat.transform, hatsObject.gameObject.layer);
        //     
        //     var meshRenderer = newHat.GetComponent<MeshRenderer>();
        //     
        //     // meshRenderer.material.shader = meshRenderer.sharedMaterial.shader = Shader.Find("W/Character");
        //     
        //     var mats = meshRenderer.materials;
        //     
        //     for (int index = 0, length = mats.Length; index < length; ++index)
        //     {
        //         mats[index].shader = Shader.Find(mats[index].shader.name);
        //         // Materials shouldn't interact with each others
        //         mats[index] = Object.Instantiate(mats[index]);
        //     }
        //     
        //     meshRenderer.SetMaterials([..mats]);
        //     
        //     HolidayHatPlugin.Log.LogDebug($"Adding Holiday Hat {newHat.name}...");
        //     
        //     newHat.transform.localPosition = hatPrefab.transform.position;
        //     newHat.transform.localRotation = hatPrefab.transform.rotation;
        //     newHat.transform.localScale = hatPrefab.transform.localScale;
        //     
        //     newHat.SetActive(false);
        //     
        //     var hoodies = __instance.refs.playerHats[^2..];
        //     var selectableHats = __instance.refs.playerHats[..^2];
        //     
        //     __instance.refs.playerHats = selectableHats.AddToArray(meshRenderer).AddRangeToArray(hoodies);
        //     
        //     HolidayHatPlugin.Log.LogInfo($"Inserted Holiday Hat {newHat.name} @ {selectableHats.Length + 1}!");
        // }
    }

    private static void SetHatMaterial(Material[] ms, PersistentPlayerData playerData, GameObject hatP,
        bool isSkel = false)
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
                shoe = HolidayHatPlugin.DefaultPilgrimHatMaterial;
            }

            if (shoe)
            {
                ms[1].SetTexture(CharacterCustomization.MainTex,
                    shoe.GetTexture(CharacterCustomization.MainTex));

                var tintMult = 0.25f;

                if (shoe.name is "M_AviatorFur" or "M_Tarnished Gold Starboy Fit" or "M_Willow_Swamp_MassChara")
                {
                    // Maet it contrasting enough
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
            // Change the pilgrim material based on the current fit.

            var hat = Singleton<Customization>.Instance.fits[playerData.customizationData.currentOutfit]
                .fitHatMaterial;

            if (hat)
            {
                if (hat.name is "M_Scout_Starboy" or "M_Scout_Tropical")
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
                        // Is a "default" scout costume.
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
                    // uses a scout hat texture
                    offset = Vector2.zero;

                    if (fit.fitMaterialOverrideHat.name is "M_Scout_JesterHatOverride" or "M_Scout_SleepyGuyHatOverride"
                        or "M_Scout_GreenTeeCargoOverride" or "M_Scout_AviatorHat")
                    {
                        scale.y = scale.x = 1.003f;
                    }
                }
                else if (fit.fitMaterialOverrideHat.name is "M_BigGallonHat")
                {
                    offset.x = -0.3828f;
                    offset.y = -0.4141f;
                }

                ms[0].SetTextureOffset(CharacterCustomization.MainTex, offset);
                ms[0].SetTextureScale(CharacterCustomization.MainTex, scale);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.UpdateDummy))]
    [HarmonyPostfix]
    [HarmonyBefore("MoreCustomizations")]
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

        if (currentHat == 0)
        {
            // Because playerHats has a dupe of the cap, it got disabled by not wearing the holiday hat... Let's re-enable it manually then
            __instance.refs.playerHats[0].gameObject
                .SetActive(true);
        }
        else if (currentHat == HolidayHatPlugin.HolidayHatCosmeticIndex)
        {
            currentHat = HolidayHatPlugin.GetHatOrDefault(
                pToUse);
            
            HolidayHatPlugin.Log.LogDebug(
                $"Showing hat @ {currentHat} on dummy...");

            __instance.SetPlayerHat(currentHat);
        }

        if (currentHat >= HolidayHatPlugin.MinHatIndex && currentHat <= HolidayHatPlugin.MaxHatIndex)
        {
            // If it's from a petrified dummy, we need to override the materials for skin-dependent materials
            if (__instance.TryGetComponent<PetrifiedScout>(out var ps))
            {
                var petrifiedMaterial = ps.transform.GetChild(0).GetChild(4).GetComponent<Renderer>().materials[0];

                var pms = __instance.refs.playerHats[currentHat].materials;

                for (var i = 0; i < pms.Length; i++)
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

            for (var i = 0; i < ms.Length; i++)
            {
                ms[i] = Object.Instantiate(ms[i]);
            }
            
            SetHatMaterial(ms, playerData, HolidayHatPlugin.customHolidayHats[customHatIndex]);
            
            __instance.refs.playerHats[currentHat].SetMaterials([..ms]);
            
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.SetPlayerColor))]
    [HarmonyPostfix]
    public static void PlayerCustomizationDummyPColorPrefix(PlayerCustomizationDummy __instance,
        int index)
    {
        if (SkinColorSlidersCompatibility.enabled)
        {
            SkinColorSlidersCompatibility.ApplySkinColorToHolidayHats(__instance, index);
        }
        else
        {
            // For vanilla colors.
            var col = Singleton<Customization>.Instance.skins[index].color;

            for (var i = HolidayHatPlugin.MinHatIndex;
                 i < HolidayHatPlugin.MaxHatIndex && i < __instance.refs.playerHats.Length;
                 i++)
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

    [HarmonyPatch(typeof(GameUtils), nameof(GameUtils.Awake))]
    [HarmonyPostfix]
    public static void GameUtilsAwakePost(GameHandler __instance)
    {
        __instance.gameObject.AddComponent<HolidayHatHandler>();
    }

    [HarmonyPatch(typeof(PersistentPlayerDataService), nameof(PersistentPlayerDataService.OnCharacterRegistered))]
    [HarmonyPostfix]
    public static void PersistentPlayerDataServiceConstructorPostfix(PersistentPlayerDataService __instance,
        Character newCharacter)
    {
        // //Register holiday hat character update
        // __instance.SubscribeToPlayerDataChange(newCharacter.photonView.Owner,
        //     (obj) => { UpdateHolidayHat(newCharacter, obj); });
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
                $"Player {pToUse} turned {(active ? "into a skeleton" : "human")}. Updating holiday hat.");
            UpdateHolidayHat(__instance.customization._character, playerData);
        }
    }

    public static Color AdaptSkinColor(Color skinColor, Material m)
    {
        var skinC = new Color(skinColor.r, skinColor.g, skinColor.b, skinColor.a);
        var shiftVector = m.GetVector(HueShifts);

        var shiftVal = 0f;

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
        }

        return skinC;
    }

    public static void UpdateHolidayHat(Character newCharacter, PersistentPlayerData data,
        Photon.Realtime.Player? player = null)
    {
        player ??= (newCharacter.view ?? newCharacter.photonView).Owner;
        
        var fitIndex = CharacterCustomization.GetFitIndex(data);

        var currentHat = data.customizationData.currentHat;
        if (Singleton<Customization>.Instance.fits[fitIndex].overrideHat)
        {
            currentHat = Singleton<Customization>.Instance.fits[fitIndex].overrideHatIndex;
        }

        if (currentHat == 0)
        {
            // Because playerHats has a dupe of the cap, it got disabled by not wearing the holiday hat... Let's re-enable it manually then
            newCharacter.refs.customization.refs.playerHats[0].gameObject
                .SetActive(true);
        }
        else if (currentHat == HolidayHatPlugin.HolidayHatCosmeticIndex)
        {
            currentHat = HolidayHatPlugin.GetHatOrDefault(
                player);

            for (int index1 = 0; index1 < newCharacter.refs.customization.refs.playerHats.Length; ++index1)
            {
                newCharacter.refs.customization.refs.playerHats[index1].gameObject
                    .SetActive(currentHat == index1);

                if (currentHat == index1)
                {
                    HolidayHatPlugin.Log.LogDebug(
                        $"Showing hat {newCharacter.refs.customization.refs.playerHats[index1].name}...");
                }
            }
        }

        if (currentHat >= HolidayHatPlugin.MinHatIndex && currentHat <= HolidayHatPlugin.MaxHatIndex)
        {
            var isSkel = newCharacter.data.isSkeleton;

            // Only custom hats have possible skin colors

            var ms = newCharacter.refs.customization.refs.playerHats[currentHat]
                .materials;

            var colorToUse = newCharacter
                .refs.customization.PlayerColor;

            var skelMaterial = newCharacter.refs.customization.refs.skeletonRenderer.material;
            var skelCleanNamed = HolidayHatPlugin.CleanedName(skelMaterial.name);

            var customHatIndex = currentHat - HolidayHatPlugin.MinHatIndex;
            
            SetHatMaterial(ms, data, HolidayHatPlugin.customHolidayHats[customHatIndex], isSkel);

            for (var i = 0; i < ms.Length; i++)
            {
                var cleanName= HolidayHatPlugin.CleanedName(ms[i].name);

                if (cleanName == skelCleanNamed)
                {
                    if (isSkel)
                    {
                        // Old skeleton material
                        if (
                            ms[i].GetFloat(HueStr) > 0.0f
                            &&
                            !ms[i].GetColor("_BaseColor").Equals(skelMaterial.GetColor("_BaseColor"))
                        )
                        {
                            // It's been changed, so let's recalculate the colors
                            var boneTint = skelMaterial.GetColor("_BaseColor").ToOklch();

                            var oklchToUse = colorToUse.ToOklch();

                            boneTint.C = Mathf.Lerp(boneTint.C, oklchToUse.C, 0.125f);

                            if (!float.IsNaN(oklchToUse.h))
                            {
                                boneTint.h = oklchToUse.h;
                            }

                            ms[i].SetColor("_BaseColor", boneTint.ToColor());
                            
                            ms[i].SetFloat("_VertexGhost", 100f);
                        }
                    }
                    else
                    {
                        // Try getting the original mesh's material
                        var ogM = Object.Instantiate(HolidayHatPlugin.customHolidayHats[customHatIndex].GetComponent<Renderer>()
                            .materials[i]);

                        // copy only important parameters

                        ogM.CopyPropsListFromMaterial(ms[i], ["_HeightMaskSmoothness"],
                            ["_VertexGhost", "_FlipHeightMask"], []);

                        ms[i] = ogM;

                        // if there's a hue shift, we'll adjust the skin color's saturation and lightness to keep the perceptual difference of hus shifting the same
                        ms[i].SetColor(CharacterCustomization.SkinColor, AdaptSkinColor(colorToUse, ms[i]));
                    }
                
                } else if (ms[i].GetFloat(UseSkinTone) >= 1.0f)
                {
                    // Brand new skeleton colros
                    if (isSkel)
                    {
                        // Any skin-toned materials gets replaced with skeletal bones.
                        var s = Object.Instantiate(skelMaterial);
                        
                        if (
                            ms[i].GetFloat(HueStr) > 0.0f &&
                            HolidayHatPlugin.CleanedName(ms[i].name) is not ("M_Fish" or "M_BunnyEars"))
                        {
                            var boneTint = s.GetColor("_BaseColor").ToOklch();

                            var oklchToUse = colorToUse.ToOklch();
                            
                            boneTint.C = Mathf.Lerp(boneTint.C, oklchToUse.C, 0.125f);
                            
                            if (!float.IsNaN(oklchToUse.h))
                            {
                                boneTint.h = oklchToUse.h;
                            }
                            
                            s.SetColor("_BaseColor", boneTint.ToColor());
                        }
                        
                        var ogM = HolidayHatPlugin.customHolidayHats[customHatIndex].GetComponent<Renderer>()
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
                        
                        s.SetFloat("_VertexGhost", 100f);
                        
                        ms[i] = s;
                    }
                    else
                    {
                        // Try getting the original mesh's material
                        var ogM = Object.Instantiate(HolidayHatPlugin.customHolidayHats[customHatIndex].GetComponent<Renderer>()
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

            newCharacter.refs.customization.refs.playerHats[currentHat]
                .SetMaterials([..ms]);
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

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.UpdateDummy))]
    [HarmonyPrefix]
    [HarmonyBefore("MoreCustomizations")]
    public static void PlayerCustomizationDummyUpdateDummyPrefix(PlayerCustomizationDummy __instance)
    {
        if (__instance == lastModifiedDummy)
        {
            return;
        }

        lastModifiedDummy = __instance;

        var hatsObject = GetHatObject(__instance.refs.transform);

        // Inserting the Holiday Hat placeholder right now
        var hoodies = __instance.refs.playerHats[^2..];
        var selectableHats = __instance.refs.playerHats[..^2];

        __instance.refs.playerHats = selectableHats.AddToArray(selectableHats[0]).AddRangeToArray(hoodies);

        HolidayHatPlugin.Log.LogInfo($"Inserted Placeholder Hat @ {selectableHats.Length + 1}!");

        for (var i = 0; i < HolidayHatPlugin.customHolidayHats.Count; i++)
        {
            var newHat = Object.Instantiate(HolidayHatPlugin.customHolidayHats[i], hatsObject, true);
            SetLayerRecursively(newHat.transform, hatsObject.gameObject.layer);

            var meshRenderer = newHat.GetComponent<MeshRenderer>();

            var mats = meshRenderer.materials;

            for (int index = 0, length = mats.Length; index < length; ++index)
            {
                mats[index].shader = Shader.Find(mats[index].shader.name);
                // Materials shouldn't interact with each others
                mats[index] = Object.Instantiate(mats[index]);
            }

            meshRenderer.SetMaterials([..mats]);

            HolidayHatPlugin.Log.LogDebug($"Adding Hat {newHat.name}...");

            newHat.transform.localPosition = HolidayHatPlugin.customHolidayHats[i].transform.position;
            newHat.transform.localRotation = HolidayHatPlugin.customHolidayHats[i].transform.rotation;
            newHat.transform.localScale = HolidayHatPlugin.customHolidayHats[i].transform.localScale;

            newHat.SetActive(false);

             hoodies = __instance.refs.playerHats[^2..];
             selectableHats = __instance.refs.playerHats[..^2];

            __instance.refs.playerHats = selectableHats.AddToArray(meshRenderer).AddRangeToArray(hoodies);

            HolidayHatPlugin.Log.LogInfo($"Inserted Hat {newHat.name} @ {selectableHats.Length + 1}!");
        }
    }
}