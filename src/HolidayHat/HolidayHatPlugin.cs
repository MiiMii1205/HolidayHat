using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HolidayHat.Patchers;
using Peak;
using PEAKLib.Core;
using PEAKLib.Core.Extensions;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace HolidayHat;

[BepInAutoPlugin]
[BepInDependency(CorePlugin.Id)]
[BepInDependency(SoftDependencyFix.Plugin.Id)]
[BepInDependency("com.github.MiiMii1205.BebeilleHats", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("MoreCustomizations", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("TeddyBRB.toomanyhats", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.snosz.skincolorsliders", BepInDependency.DependencyFlags.SoftDependency)]
public partial class HolidayHatPlugin : BaseUnityPlugin
{
    public static Texture2D HolidayHatIcon = null!;

    public static Texture2D PartyHatIcon = null!;
    public static Texture2D StPatrickHatIcon = null!;
    public static Texture2D FishHatIcon = null!;
    public static Texture2D BunnyEarsIcon = null!;
    public static Texture2D HardHatIcon = null!;
    public static Texture2D PilgrimHatIcon = null!;
    public static Texture2D SantaHatIcon = null!;
    public static Texture2D RibbonHatIcon = null!;

    private static Dictionary<DateTime, int> holidayDictionary = new();

    public static int MaxHatIndex;
    public static int MinHatIndex;

    internal static ManualLogSource Log
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    } = null!;

    public static int HolidayHatCosmeticIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
    }

    public static int HolidayHatIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static bool UseCustomHolidayHat
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static bool IsBunnyEars
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static ConfigEntry<int> DefaultPlayerHatIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    } = null!;


    public static GameObject? YesterdayHatPrefab
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static GameObject? TodayHatPrefab
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static GameObject? TomorrowHatPrefab
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static GameObject? CurrentHolidayHatPrefab
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static int YesterdayHatIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static int TodayHatIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static int TomorrowHatIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static DateTime ClientDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static DateTime? RoomDate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    public static Material NoRenderMaterial { get; private set; } = null!;
    public static Material DefaultPilgrimHatMaterial { get; private set; } = null!;
    public static Material DefaultRibbonHatMaterial { get; private set; } = null!;
    public static Material DefaultPartyHatMaterial { get; private set; } = null!;
    public static GameObject PartyHatPrefab { get; private set; } = null!;
    public static GameObject StPatrickHatPrefab { get; private set; } = null!;
    public static GameObject FishHatPrefab { get; private set; } = null!;
    public static GameObject BunnyEarsPrefab { get; private set; } = null!;
    public static GameObject HardHatPrefab { get; private set; } = null!;
    public static GameObject PilgrimHatPrefab { get; private set; } = null!;

    public static List<GameObject> customHolidayHats;
    public static GameObject SantaHatPrefab { get; private set; } = null!;
    public static GameObject RibbonPrefab { get; private set; } = null!;
    public static Dictionary<DateTime, int>.ValueCollection HolidayHatIndexes => holidayDictionary.Values;

    private void Awake()
    {
        Log = Logger;

        customHolidayHats = [];
        new Harmony(Id).PatchAll(typeof(HolidayHatPatcher));

        DefaultPlayerHatIndex = Config.Bind("General", "Default Hat Index", 0,
            "The index hat to use when no holidays are currently happening, as it appears in the Passport. (starts at 0)");

        // Adding vanilla hats
        var cal = new GregorianCalendar();

        SetHatIndex(new DateTime(DateTime.Today.Year, 2, 14), 39);
        SetHatIndex(new DateTime(DateTime.Today.Year, 4, 22), 35);
        SetHatIndex(GetNbSundayAfter(new DateTime(DateTime.Today.Year, 5, 1, cal), 2, cal), 8);
        SetHatIndex(GetNbSundayAfter(new DateTime(DateTime.Today.Year, 6, 1, cal), 3, cal), 33);
        SetHatIndex(new DateTime(DateTime.Today.Year, 10, 31), 27);
        SetHatIndex(cal.AddDays(cal.AddDays(GetMondayOfMonth(DateTime.Today.Year, 11, 4, cal), 3), 1), 21);
        SetHatIndex(new DateTime(DateTime.Today.Year, 11, 11), 7);

        // Load peakbundles 
        this.LoadBundleAndContentsWithName("holidayhat.peakbundle", LoadHolidayHatAssets);

        Log.LogInfo($"HolidayHatPlugin {Name} is loaded!");
    }

    private static Dictionary<Photon.Realtime.Player, int> _defaultHatIndexes = new();
    private static Dictionary<string, int> _hatIndexLookup = new();
    private static Dictionary<Photon.Realtime.Player, DateTime> _playerBootDates = new();

    public static int GetHatIndex(DateTime date)
    {
        return holidayDictionary[new DateTime(2000, date.Date.Month, date.Date.Day)];
    }

    public static bool HasHatIndex(DateTime date)
    {
        return holidayDictionary.ContainsKey(new DateTime(2000, date.Date.Month, date.Date.Day));
    }

    public static void SetHatIndex(DateTime date, int index)
    {
        var d = new DateTime(2000, date.Date.Month, date.Date.Day);
        if (holidayDictionary.ContainsKey(d))
        {
            holidayDictionary[d] = index;
            Log.LogInfo($"Changed Hat idx [{index}] for {date:m}");
        }
        else
        {
            holidayDictionary.Add(d, index);
            Log.LogInfo($"Inserted Hat idx [{index}] for {date:m}");
        }
    }

    private void LoadHolidayHatAssets(PeakBundle bundle)
    {
        NoRenderMaterial = bundle.LoadAsset<Material>("M_NoRender.mat");
        DefaultPilgrimHatMaterial = bundle.LoadAsset<Material>("M_PilgrimHat.mat");
        DefaultRibbonHatMaterial = bundle.LoadAsset<Material>("M_Ribbon.mat");
        DefaultPartyHatMaterial = bundle.LoadAsset<Material>("M_PartyHat.mat");

        Log.LogInfo($"Loaded material {NoRenderMaterial.name}!");
        Log.LogInfo($"Loaded material {DefaultPilgrimHatMaterial.name}!");
        Log.LogInfo($"Loaded material {DefaultRibbonHatMaterial.name}!");
        Log.LogInfo($"Loaded material {DefaultPartyHatMaterial.name}!");

        DefaultPilgrimHatMaterial.shader = Shader.Find(DefaultPilgrimHatMaterial.shader.name);
        DefaultRibbonHatMaterial.shader = Shader.Find(DefaultRibbonHatMaterial.shader.name);
        DefaultPartyHatMaterial.shader = Shader.Find(DefaultPartyHatMaterial.shader.name);

        // Load every custom holiday hats

        PartyHatPrefab = LoadHolidayHat(bundle, "PartyHat.prefab");
        StPatrickHatPrefab = LoadHolidayHat(bundle, "StPatrickHat.prefab");
        FishHatPrefab = LoadHolidayHat(bundle, "FishHat.prefab");
        BunnyEarsPrefab = LoadHolidayHat(bundle, "BunnyEars.prefab");
        HardHatPrefab = LoadHolidayHat(bundle, "HardHat.prefab");
        PilgrimHatPrefab = LoadHolidayHat(bundle, "PilgrimHat.prefab");
        SantaHatPrefab = LoadHolidayHat(bundle, "SantaHat.prefab");
        RibbonPrefab = LoadHolidayHat(bundle, "Ribbon.prefab");

        customHolidayHats.Add(PartyHatPrefab);
        customHolidayHats.Add(StPatrickHatPrefab);
        customHolidayHats.Add(FishHatPrefab);
        customHolidayHats.Add(BunnyEarsPrefab);
        customHolidayHats.Add(HardHatPrefab);
        customHolidayHats.Add(PilgrimHatPrefab);
        customHolidayHats.Add(SantaHatPrefab);
        customHolidayHats.Add(RibbonPrefab);

        var today = DateTime.Today;

        ClientDate = today;

        // var yesterdayHats = CheckSecularHolidays(today.AddDays(-1));
        // var todayHats = CheckSecularHolidays(today);
        // var tomorrowHats = CheckSecularHolidays(today.AddDays(1));
        //
        //
        // // Load each hats
        // if (yesterdayHats != null)
        // {
        //     if (yesterdayHats.EndsWith(".prefab"))
        //     {
        //         YesterdayHatPrefab = LoadHolidayHat(bundle, yesterdayHats);
        //     }
        //     else
        //     {
        //         // It's a vanilla hat
        //         YesterdayHatIndex = int.Parse(yesterdayHats);
        //     }
        // }
        // else
        // {
        //     YesterdayHatIndex = -1;
        // }
        //
        // if (todayHats != null)
        // {
        //     if (todayHats.EndsWith(".prefab"))
        //     {
        //         TodayHatPrefab = LoadHolidayHat(bundle, todayHats);
        //     }
        //     else
        //     {
        //         // It's a vanilla hat
        //         TodayHatIndex = int.Parse(todayHats);
        //     }
        // }
        // else
        // {
        //     TodayHatIndex = -1;
        // }
        //
        // if (tomorrowHats != null)
        // {
        //     if (tomorrowHats.EndsWith(".prefab"))
        //     {
        //         TomorrowHatPrefab = LoadHolidayHat(bundle, tomorrowHats);
        //     }
        //     else
        //     {
        //         // It's a vanilla hat
        //         TomorrowHatIndex = int.Parse(tomorrowHats);
        //     }
        // }
        // else
        // {
        //     TomorrowHatIndex = -1;
        // }

        HolidayHatIcon = bundle.LoadAsset<Texture2D>("HolidayHatOption.png");

        PartyHatIcon = bundle.LoadAsset<Texture2D>("PartyHatIcon.png");
        StPatrickHatIcon = bundle.LoadAsset<Texture2D>("StPatrickHatIcon.png");
        FishHatIcon = bundle.LoadAsset<Texture2D>("FishHatIcon.png");
        BunnyEarsIcon = bundle.LoadAsset<Texture2D>("BunnyHatIcon.png");
        HardHatIcon = bundle.LoadAsset<Texture2D>("HardHatIcon.png");
        PilgrimHatIcon = bundle.LoadAsset<Texture2D>("PilgrimHatIcon.png");
        SantaHatIcon = bundle.LoadAsset<Texture2D>("SantaHatIcon.png");
        RibbonHatIcon = bundle.LoadAsset<Texture2D>("RibbonHatIcon.png");
    }

    private GameObject LoadHolidayHat(PeakBundle bundle, string hatName)
    {
        var h = bundle.LoadAsset<GameObject>(hatName);
        Log.LogInfo($"Loaded Hat {hatName}!");
        ShaderExtensions.ReplaceShaders(h);
        return h;
    }

    public static DateTime GetGregorianEaster(int year)
    {
        // Easter calculations are WEIRD! 

        int a = year % 19;
        int b = year / 100;
        int c = year % 100;

        int d = b / 4;
        int e = b % 4;

        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;

        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;

        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateTime(year, month, day);
    }

    private static DateTime GetNbSundayBefore(DateTime date, int nb, Calendar cal)
    {
        // Get fist last sunday
        var d = cal.AddDays(date, -1);
        var fSunday = cal.AddDays(d, -(int) d.DayOfWeek);

        return cal.AddDays(fSunday, -(7 * (nb - 1)));
    }

    private static DateTime GetNbSundayAfter(DateTime date, int nb, Calendar cal)
    {
        // Get fist last sunday
        var d = cal.AddDays(date, 1);

        var nSunday = cal.AddDays(d, 7 - (int) d.DayOfWeek);

        return cal.AddDays(nSunday, (7 * (nb - 1)));
    }

    private static DateTime GetNbSaturdayAfter(DateTime date, int nb, Calendar cal)
    {
        // Get fist last sunday
        var sun = GetNbSundayAfter(date, nb, cal);

        return cal.AddDays(sun, -1);
    }

    private static DateTime GetJulianEaster(int year)
    {
        var cal = new JulianCalendar();

        // Easter calculations are WEIRD! 

        int a = year % 4;
        int b = year % 7;
        int c = year % 19;

        int d = ((19 * c) + 15) % 30;
        int e = ((2 * a) + (4 * b) - d + 34) % 7;

        int month = (d + e + 114) / 31;
        int day = ((d + e + 114) % 31) + 1;

        return new DateTime(year, month, day, cal);
    }


    public static DateTime GetMondayOfMonth(int year, int month, int nbMonday, Calendar c)
    {
        var first = new DateTime(year, month, 1, c);

        var firstMonday = first.AddDays(first.DayOfWeek > DayOfWeek.Monday
            ? (7 + (DayOfWeek.Monday - first.DayOfWeek))
            : (first
                .DayOfWeek == 0
                ? 1
                : 0));

        if (nbMonday == 1)
        {
            return firstMonday;
        }

        return firstMonday.AddDays(7 * (nbMonday - 1));
    }


    public static string CleanedName(string dirtyString)
    {
        return dirtyString.Replace("(Clone)", "")
            .Replace("(Instanced)", "").Replace("(Instance)", "").Trim();
    }

    private static string? CheckSecularHolidays(DateTime td)
    {
        var cal = new GregorianCalendar();

        var newYear = new DateTime(cal.GetYear(td), 1, 1, cal);

        if (td.Date == newYear.Date)
        {
            // Party hat
            return "PartyHat.prefab";
        }

        var valen = new DateTime(cal.GetYear(td), 2, 14, cal);

        if (td.Date == valen.Date)
        {
            // Arrow
            return "39";
        }

        var stpat = new DateTime(cal.GetYear(td), 3, 17, cal);

        if (td.Date == stpat.Date)
        {
            // Melon hat with 4 leaf-clover
            return "StPatrickHat.prefab";
        }

        var aprf = new DateTime(cal.GetYear(td), 4, 1, cal);

        if (td.Date == aprf.Date)
        {
            // Fish hat
            return "FishHat.prefab";
        }

        var easter = GetGregorianEaster(cal.GetYear(td));

        Log.LogDebug($"In {cal.GetYear(td)}, Easter is {easter}");

        if (td.Date == easter.Date)
        {
            // Bunny ears
            return "BunnyEars.prefab";
        }

        var earthd = new DateTime(cal.GetYear(td), 4, 22, cal);

        if (td.Date == earthd.Date)
        {
            // Stump
            return "35";
        }


        var wday = GetMondayOfMonth(cal.GetYear(td), 5, 1, cal);

        Log.LogDebug($"In {cal.GetYear(td)}, May Day is {wday}");

        if (td.Date == wday.Date)
        {
            // Construction hat
            return "HardHat.prefab";
        }

        var mday = GetNbSundayAfter(new DateTime(cal.GetYear(td), 5, 1, cal), 2, cal);

        Log.LogDebug($"In {cal.GetYear(td)}, Mother's day is {mday}");

        if (td.Date == mday.Date)
        {
            // flower bands
            return "8";
        }


        var fday = GetNbSundayAfter(new DateTime(cal.GetYear(td), 6, 1, cal), 3, cal);

        Log.LogDebug($"In {cal.GetYear(td)}, Father's day is {fday}");

        if (td.Date == fday.Date)
        {
            // Soda hat
            return "33";
        }

        var labday = GetMondayOfMonth(cal.GetYear(td), 9, 1, cal);

        Log.LogDebug($"In {cal.GetYear(td)}, Labor Day (North America) is {labday}");

        if (td.Date == labday.Date)
        {
            // Hard hat
            return "HardHat.prefab";
        }

        var tnkg = cal.AddDays(GetMondayOfMonth(cal.GetYear(td), 11, 4, cal), 3);
        var ctnkg = GetMondayOfMonth(cal.GetYear(td), 10, 2, cal);

        Log.LogDebug($"In {cal.GetYear(td)}, Thanksgiving (US) is {tnkg}");
        Log.LogDebug($"In {cal.GetYear(td)}, Thanksgiving (CA) is {ctnkg}");

        if (td.Date == tnkg.Date || td.Date == ctnkg.Date)
        {
            // Pilgrim hat
            return "PilgrimHat.prefab";
        }

        var hallw = new DateTime(cal.GetYear(td), 10, 31, cal);

        if (td.Date == hallw.Date)
        {
            // Pumpkin hat
            return "27";
        }


        var blkfri = cal.AddDays(tnkg, 1);

        Log.LogDebug($"In {cal.GetYear(td)}, Black Friday is {blkfri}");

        if (td.Date == blkfri.Date)
        {
            // Visor
            return "21";
        }

        var rday = new DateTime(cal.GetYear(td), 11, 11, cal);

        if (td.Date == rday.Date)
        {
            // Medic hat
            return "7";
        }

        var xmass = new DateTime(cal.GetYear(td), 12, 25, cal);

        if (td.Date == xmass.Date)
        {
            // Santa hat
            return "SantaHat.prefab";
        }

        var boxd = cal.AddDays(xmass, 1);

        if (td.Date == boxd.Date)
        {
            // Boxing hat
            return "Ribbon.prefab";
        }

#if DEBUG

        // return "39";

#endif

        // No hats for today 
        return null;
    }

    private string? CheckCatholicHolidays(DateTime td)
    {
        // TODO: Fill in if needed
        return null;

        var cal = new GregorianCalendar();

        var easter = GetGregorianEaster(cal.GetYear(td));

        if (td.Date == easter.Date)
        {
            // Egg shell   
            return "EggShell.prefab";
        }

        var xmass = new DateTime(cal.GetYear(td), 12, 25, cal);

        if (td.Date == xmass.Date)
        {
            // Advent Wreath
            return "AdventWreath.prefab";
        }

        var epi = GetNbSundayAfter(xmass, 2, cal);

        if (td.Date == epi.Date)
        {
            // large crown 
            return "LargeCrown.prefab";
        }

        var bthj = GetNbSundayAfter(epi, 1, cal);

        if (td.Date == bthj.Date)
        {
            // Dove 
            return "Dove.prefab";
        }

        var candl = cal.AddDays(xmass, 40);

        if (td.Date == candl.Date)
        {
            // Candle 
            return "Candle.prefab";
        }

        var annun = new DateTime(cal.GetYear(td), 3, 25, cal);

        if (td.Date == annun.Date)
        {
            // AngelWings
            return "AngleWings.prefab";
        }

        var ftthr = cal.AddDays(easter, -52);

        if (td.Date == ftthr.Date)
        {
            // Chef hat
            return "15";
        }

        var ash = cal.AddDays(ftthr, -1);

        if (td.Date == ash.Date)
        {
            // Ash cross
            return "AshCross.prefab";
        }

        var fttu = cal.AddDays(ash, -1);

        if (td.Date == fttu.Date)
        {
            // Jester 
            return "37";
        }

        var quadrag = GetNbSundayAfter(ash, 1, cal);

        if (td.Date == quadrag.Date)
        {
            // Fire
            return "Fire.prefab";
        }

        var midf = cal.AddDays(ash, 20);

        if (td.Date == midf.Date)
        {
            // Jester 
            return "37";
        }

        var d = cal.AddDays(easter, -1);

        var palm = cal.AddDays(d, (int) d.DayOfWeek);

        if (td.Date == palm.Date)
        {
            // PalmCrown 
            return "PalmCrown.prefab";
        }

        var holthu = cal.AddDays(palm, 4);

        if (td.Date == holthu.Date)
        {
            // Bread 
            return "Bread.prefab";
        }

        var holfry = cal.AddDays(holthu, 1);

        if (td.Date == holfry.Date)
        {
            // ThornCrown 
            return "ThornCrown.prefab";
        }

        var asc = cal.AddDays(easter, 3);

        if (td.Date == asc.Date)
        {
            // AngleWing
            return "AngelWings.prefab";
        }

        var meast = cal.AddDays(easter, 1);

        if (td.Date == meast.Date)
        {
            // AngleWing
            return "AngelWings.prefab";
        }

        var pent = cal.AddDays(easter, 49);

        if (td.Date == pent.Date)
        {
            // Dove
            return "Dove.prefab";
        }

        var trasf = new DateTime(cal.GetYear(td), 8, 6, cal);

        if (td.Date == trasf.Date)
        {
            // LightBall
            return "LightBall.prefab";
        }

        var fcross = new DateTime(cal.GetYear(td), 9, 14, cal);

        if (td.Date == fcross.Date)
        {
            // Cross
            return "Cross.prefab";
        }

        var alls = new DateTime(cal.GetYear(td), 11, 1, cal);

        if (td.Date == alls.Date)
        {
            // Candle
            return "Candle.prefab";
        }

        // No hats for today 
        return null;
    }

    private string? CheckOrthodoxHolidays(DateTime td)
    {
        // TODO: Fill in if needed
        return null;

        var cal = new JulianCalendar();

        var easter = GetJulianEaster(cal.GetYear(td));

        if (td.Date == easter.Date)
        {
            // Egg shell   
            return "EggShell.prefab";
        }

        var zach = GetNbSundayBefore(easter, 11, cal);

        if (td.Date == zach.Date)
        {
        }

        var pharise = cal.AddDays(easter, -70);

        if (td.Date == pharise.Date)
        {
        }

        var prodi = cal.AddDays(easter, -63);

        if (td.Date == prodi.Date)
        {
        }

        var jugment = cal.AddDays(easter, -56);

        if (td.Date == jugment.Date)
        {
        }

        var forg = cal.AddDays(easter, -49);

        if (td.Date == forg.Date)
        {
        }

        var clnm = cal.AddDays(easter, -48);

        if (td.Date == clnm.Date)
        {
        }

        var thds = GetNbSaturdayAfter(clnm, 1, cal);

        if (td.Date == thds.Date)
        {
        }

        var truorth = cal.AddDays(thds, 1);

        if (td.Date == truorth.Date)
        {
        }

        var greg = GetNbSundayBefore(easter, 5, cal);

        if (td.Date == greg.Date)
        {
        }

        var sjl = GetNbSundayBefore(easter, 3, cal);

        if (td.Date == sjl.Date)
        {
        }


        var akath = GetNbSaturdayAfter(clnm, 5, cal);

        if (td.Date == akath.Date)
        {
        }

        var smoe = cal.AddDays(akath, 1);

        if (td.Date == smoe.Date)
        {
        }

        var lazarsat = cal.AddDays(easter, -8);

        if (td.Date == lazarsat.Date)
        {
        }

        var holm = cal.AddDays(easter, -6);

        if (td.Date == holm.Date)
        {
        }


        var holtu = cal.AddDays(easter, -5);

        if (td.Date == holtu.Date)
        {
        }

        var holw = cal.AddDays(easter, -4);

        if (td.Date == holw.Date)
        {
        }

        var holthur = cal.AddDays(easter, -3);

        if (td.Date == holthur.Date)
        {
            // Bread 
            return "Bread.prefab";
        }

        var holtfr = cal.AddDays(easter, -2);

        if (td.Date == holtfr.Date)
        {
            // ThornCrown 
            return "ThornCrown.prefab";
        }

        var stom = GetNbSundayAfter(easter, 1, cal);

        if (td.Date == stom.Date)
        {
        }


        var myrr = GetNbSundayAfter(easter, 2, cal);

        if (td.Date == myrr.Date)
        {
        }

        var paraly = GetNbSundayAfter(easter, 3, cal);

        if (td.Date == paraly.Date)
        {
        }

        var midp = cal.AddDays(easter, 24);

        if (td.Date == midp.Date)
        {
        }

        var samarw = cal.AddDays(easter, 28);

        if (td.Date == samarw.Date)
        {
        }

        var blind = cal.AddDays(easter, 35);

        if (td.Date == blind.Date)
        {
        }

        var apodosis = cal.AddDays(easter, 38);

        if (td.Date == apodosis.Date)
        {
        }

        var alls = cal.AddDays(easter, 56);

        if (td.Date == alls.Date)
        {
        }

        var nmar = new DateTime(cal.GetYear(td), 9, 8, cal);

        if (td.Date == nmar.Date)
        {
        }

        var fcross = new DateTime(cal.GetYear(td), 9, 14, cal);

        if (td.Date == fcross.Date)
        {
            // Cross
            return "OrthoCross.prefab";
        }

        var theoto = new DateTime(cal.GetYear(td), 11, 21, cal);

        if (td.Date == theoto.Date)
        {
        }

        var xmas = new DateTime(cal.GetYear(td), 12, 25, cal);

        if (td.Date == xmas.Date)
        {
            // Advent Wreath
            return "AdventWreath.prefab";
        }

        var epi = new DateTime(cal.GetYear(td), 1, 6, cal);

        if (td.Date == epi.Date)
        {
            // large crown 
            return "LargeCrown.prefab";
        }

        var penta = cal.AddDays(easter, 49);

        if (td.Date == penta.Date)
        {
        }

        var asc = cal.AddDays(easter, 39);

        if (td.Date == asc.Date)
        {
            // AngleWing
            return "AngelWings.prefab";
        }

        var d = cal.AddDays(easter, -1);

        var palm = cal.AddDays(d, (int) d.DayOfWeek);

        if (td.Date == palm.Date)
        {
        }

        var annun = new DateTime(cal.GetYear(td), 3, 25, cal);

        if (td.Date == annun.Date)
        {
        }

        var transf = new DateTime(cal.GetYear(td), 8, 6, cal);

        if (td.Date == transf.Date)
        {
        }

        var dormit = new DateTime(cal.GetYear(td), 8, 15, cal);

        if (td.Date == dormit.Date)
        {
        }


        // No hats for today 
        return null;
    }

    private string? CheckJewishHolidays(DateTime td)
    {
        // TODO: Fill in if needed
        return null;

        var cal = new HebrewCalendar();

        var isLeapYear = cal.IsLeapYear(td.Year);

        var ny = new DateTime(cal.GetYear(td), 1, 1, cal);
        var nye = cal.AddDays(ny, 1);

        if (td.Date >= ny.Date && td.Date <= nye.Date)
        {
        }

        var tubis = new DateTime(cal.GetYear(td), 5, 15, cal);

        if (td.Date == tubis.Date)
        {
        }

        var fny = new DateTime(cal.GetYear(td), isLeapYear ? 8 : 7, 1, cal);

        if (td.Date == fny.Date)
        {
        }

        var yomk = new DateTime(cal.GetYear(td), 1, 10, cal);

        if (td.Date == yomk.Date)
        {
        }

        var sukkot = new DateTime(cal.GetYear(td), 1, 15, cal);
        var sukkotEnd = cal.AddDays(sukkot, 6);

        if (td.Date >= sukkot.Date && td.Date <= sukkotEnd.Date)
        {
        }

        var hanu = new DateTime(cal.GetYear(td), 3, 25, cal);
        var hanuEnd = cal.AddDays(hanu, 7);

        if (td.Date >= hanu.Date && td.Date <= hanuEnd.Date)
        {
        }

        var tt = new DateTime(cal.GetYear(td), 4, 10, cal);

        if (td.Date == tt.Date)
        {
        }

        if (isLeapYear)
        {
            var purim = new DateTime(cal.GetYear(td), 7, 14, cal);
            var purimEnd = new DateTime(cal.GetYear(td), 7, 15, cal);

            if (td.Date >= purim.Date && td.Date <= purimEnd.Date)
            {
            }


            var purimkat = new DateTime(cal.GetYear(td), 6, 14, cal);
            var purimkatEnd = new DateTime(cal.GetYear(td), 6, 15, cal);

            if (td.Date >= purimkat.Date && td.Date <= purimkatEnd.Date)
            {
            }

            var festher = new DateTime(cal.GetYear(td), 7, 13, cal);

            if (td.Date == festher.Date)
            {
            }
        }
        else
        {
            var purim = new DateTime(cal.GetYear(td), 6, 14, cal);
            var purimEnd = new DateTime(cal.GetYear(td), 6, 15, cal);

            if (td.Date >= purim.Date && td.Date <= purimEnd.Date)
            {
            }

            var festher = new DateTime(cal.GetYear(td), 6, 13, cal);

            if (td.Date == festher.Date)
            {
            }
        }

        var pesach = new DateTime(cal.GetYear(td), isLeapYear ? 8 : 7, 15, cal);
        var pesachS = new DateTime(cal.GetYear(td), isLeapYear ? 8 : 7, 16, cal);

        if (td.Date >= pesach.Date && td.Date <= pesachS.Date)
        {
        }

        var pesachEnd = new DateTime(cal.GetYear(td), isLeapYear ? 8 : 7, 21, cal);
        var pesachSEdd = new DateTime(cal.GetYear(td), isLeapYear ? 8 : 7, 22, cal);


        if (td.Date >= pesachEnd.Date && td.Date <= pesachSEdd.Date)
        {
        }

        var pesachSh = new DateTime(cal.GetYear(td), isLeapYear ? 9 : 8, 14, cal);

        if (td.Date == pesachSh.Date)
        {
        }

        var lagbao = new DateTime(cal.GetYear(td), isLeapYear ? 9 : 8, 18, cal);

        if (td.Date == lagbao.Date)
        {
        }

        var shavuo = new DateTime(cal.GetYear(td), isLeapYear ? 10 : 9, 6, cal);
        var shavuoEnd = new DateTime(cal.GetYear(td), isLeapYear ? 10 : 9, 7, cal);

        if (td.Date >= shavuo.Date && td.Date <= shavuoEnd.Date)
        {
        }

        var shivaasar = new DateTime(cal.GetYear(td), isLeapYear ? 11 : 10, 17, cal);

        if (td.Date == shivaasar.Date)
        {
        }

        var tishabav = new DateTime(cal.GetYear(td), isLeapYear ? 12 : 11, 9, cal);

        if (td.Date == tishabav.Date)
        {
        }

        var tubav = new DateTime(cal.GetYear(td), isLeapYear ? 12 : 11, 15, cal);

        if (td.Date == tubav.Date)
        {
        }
    }

    private string? CheckChineseHolidays(DateTime td)
    {
        // TODO: Fill in if needed
        return null;

        var cal = new ChineseLunisolarCalendar();

        var p = new PersianCalendar();
        var equi = new DateTime(p.GetYear(td), 1, 1, p);

        var chny = new DateTime(cal.GetYear(td), 1, 1, cal);
        var chnyEnd = chny.AddDays(3);

        if (td.Date >= chny.Date && td.Date <= chnyEnd.Date)
        {
        }

        var qingm = equi.AddDays(14);

        if (td.Date == qingm.Date)
        {
        }

        var lday = new DateTime(cal.GetYear(td), 5, 1);

        var ldayEnd = lday.AddDays(1);

        if (td.Date >= lday.Date && td.Date <= ldayEnd.Date)
        {
        }

        var dbf = new DateTime(cal.GetYear(td), 5, 5, cal);

        if (td.Date == dbf.Date)
        {
        }

        var maf = new DateTime(cal.GetYear(td), 8, 15, cal);

        if (td.Date == maf.Date)
        {
        }

        var qixi = new DateTime(cal.GetYear(td), 7, 7, cal);

        if (td.Date == qixi.Date)
        {
        }
    }

    private string? CheckJapaneseHolidays(DateTime td)
    {
        // TODO: Fill in if needed
        return null;

        var j = new GregorianCalendar();

        var seijin = GetMondayOfMonth(j.GetYear(td), 1, 2, j);

        if (td.Date == seijin.Date)
        {
        }

        var green = new DateTime(j.GetYear(td), 5, 4, j);

        if (td.Date == green.Date)
        {
            // Stump
        }

        var kids = new DateTime(j.GetYear(td), 5, 5, j);

        if (td.Date == kids.Date)
        {
        }

        var age = GetMondayOfMonth(j.GetYear(td), 9, 3, j);

        if (td.Date == age.Date)
        {
        }

        var mday = GetMondayOfMonth(j.GetYear(td), 7, 3, j);

        if (td.Date == mday.Date)
        {
        }

        var ltday = new DateTime(j.GetYear(td), 11, 23, j);

        if (ltday.DayOfWeek == DayOfWeek.Sunday)
        {
            ltday = ltday.AddDays(1);
        }

        if (td.Date == ltday.Date)
        {
        }
    }

    private string? CheckIslamicHolidays(DateTime td)
    {
        // TODO: Fill in if needed
        return null;

        var c = new HijriCalendar();

        var eidf = new DateTime(c.GetYear(td), 10, 1, c);

        var eidfEnd = eidf.AddDays(4);

        if (td.Date >= eidf.Date && td.Date <= eidfEnd.Date)
        {
        }

        var eida = new DateTime(c.GetYear(td), 12, 10, c);

        var eidaEnd = eida.AddDays(4);

        if (td.Date >= eida.Date && td.Date <= eidaEnd.Date)
        {
        }

        var inew = new DateTime(c.GetYear(td), 1, 1, c);

        if (td.Date == inew.Date)
        {
        }

        var mwl = new DateTime(c.GetYear(td), 3, 12, c);

        if (td.Date == mwl.Date)
        {
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<string?> FindPossibleHolidayHats(DateTime dayToUse)
    {
        // TODO: Maybe check for a priority system 
        return
        [
            CheckSecularHolidays(dayToUse),
            CheckCatholicHolidays(dayToUse),
            CheckOrthodoxHolidays(dayToUse),
            CheckJewishHolidays(dayToUse),
            CheckIslamicHolidays(dayToUse),
            CheckJapaneseHolidays(dayToUse),
            CheckChineseHolidays(dayToUse)
        ];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShouldUseDefaultHat(Photon.Realtime.Player player)
    {
        // TODO: Should we support special calendars, find a way to send player's preferences 
        return HolidayHatIndex < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetDefaultPlayerHat(Photon.Realtime.Player player)
    {
        // Get the default index for the current player
        var defHat = _defaultHatIndexes[player];

        return (defHat == HolidayHatCosmeticIndex || (defHat < 0 ||
                                                      defHat > Singleton<Customization>.Instance.hats.Length))
            ?
            // Default to 0 then
            0
            : defHat;
    }

    public static void ReceiveTimestamp(long timestamp)
    {
        // TODO: Check for date edge cases. Maybe loading EVERY hat in memory is the answer here but might make it incompatible with multi-calendar support

        RoomDate = new DateTime(timestamp).Date;

        if (HasHatIndex(new DateTime(timestamp).Date))
        {
            HolidayHatIndex = GetHatIndex((DateTime) RoomDate);
        }
        else
        {
            // Not in use right now
            HolidayHatIndex = -1;
        }
    }

    public static void UpdatePlayerDummies(Photon.Realtime.Player player)
    {
        var playerCharacter = PlayerHandler.GetPlayerCharacter(player);

        var playerData = GameHandler.GetService<PersistentPlayerDataService>()
            .GetPlayerData(player);

        // Update zombies
        foreach (var bot in Character.AllBotCharacters)
        {
            if (bot.refs.customization.overridePhotonPlayer != null &&
                bot.refs.customization.overridePhotonPlayer.UserId == player.UserId)
            {
                HolidayHatPatcher.UpdateHolidayHat(bot, playerData, bot.refs.customization.overridePhotonPlayer);
                Log.LogDebug($"Updated hat for {bot} (copying {playerCharacter})");
            }
        }

        // Update petrified scout remains

        foreach (var ps in FindObjectsByType<PetrifiedScout>(FindObjectsSortMode.None))
        {
            if (ps.spawnedFromCharacter == playerCharacter)
            {
                ps.customization.UpdateDummy(player);
                Log.LogDebug($"Updated hat for {ps} (statue of {playerCharacter})");
            }
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UpdatePlayerDefaultHatIndex(Photon.Realtime.Player player, int defaultHatIndex)
    {
        if (_defaultHatIndexes.ContainsKey(player))
        {
            _defaultHatIndexes[player] = defaultHatIndex;
            Log.LogDebug($"Updated default hat idx for {player} to {defaultHatIndex}");
        }
        else if (_defaultHatIndexes.TryAdd(player, defaultHatIndex))
        {
            Log.LogDebug($"Assigned default hat idx for {player} to {defaultHatIndex}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UpdatePlayerBootDate(Photon.Realtime.Player player, DateTime playerDate)
    {
        if (_playerBootDates.ContainsKey(player))
        {
            _playerBootDates[player] = playerDate;
            Log.LogDebug($"Updated player boot date of {player} to {playerDate}");
        }
        else if (_playerBootDates.TryAdd(player, playerDate))
        {
            Log.LogDebug($"Assigned player boot date for {player} to {playerDate}");
        }
    }

    public static int GetHatOrDefault(Photon.Realtime.Player player)
    {
        if (_playerBootDates.TryGetValue(player, out var d))
        {
            if (d.Year == ClientDate.Year)
            {
                // Use the normal index collection

                return ShouldUseDefaultHat(
                    player)
                    ? GetDefaultPlayerHat(player)
                    : GetHatIndex(d);
            }
            else
            {
                // We'll have to recalculate the date manually
                Log.LogInfo(
                    $"{player.NickName}'s year is different from this client. Falling back to manual calculations...");

                var calcHatIndex = CheckSecularHolidays(d);

                if (calcHatIndex == null)
                {
                    return GetDefaultPlayerHat(player);
                }

                if (int.TryParse(calcHatIndex, out var h))
                {
                    return h;
                }

                return _hatIndexLookup[calcHatIndex.Replace(".prefab", "")];
            }
        }
        else
        {
            Log.LogDebug($"Falling back to hat @ {HolidayHatIndex}");
            return HolidayHatIndex;
        }
    }

    public static void SetHatLookup(string name, int index)
    {
        if (_hatIndexLookup.TryAdd(name, index))
        {
            Log.LogInfo($"Inserted Hat idx [{index}] for {name}");
        }
        else
        {
            _hatIndexLookup[name] = index;
            Log.LogInfo($"Changed Hat idx [{index}] for {name}");
        }
    }
}