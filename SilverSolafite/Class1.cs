using HarmonyLib;
using RedLoader;
using Sons.Gameplay;
using Sons.Weapon;
using SonsSdk;
using SUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SilverSolafite
{
    public enum PlatingCategory
    {
        Weapons = 0,
        Armor = 1,
        LogWalls = 2,
        DefenseWalls = 3,
        StoneWalls = 4,
        Floors = 5,
        Stairs = 6,
        Doors = 7,
        Furniture = 8,
        Other = 9,
        SilverBones = 10,
        SolafiteOre = 11
    }

    public class SilverSolafiteMod : SonsMod
    {
        public static ConfigCategory? SettingsCategory;

        public static ConfigEntry<float>? PowerMultiplierSetting;

        public static ConfigEntry<int>? WeaponsPresetSetting;
        public static ConfigEntry<int>? ArmorPresetSetting;
        public static ConfigEntry<int>? LogWallsPresetSetting;
        public static ConfigEntry<int>? DefenseWallsPresetSetting;
        public static ConfigEntry<int>? StoneWallsPresetSetting;
        public static ConfigEntry<int>? FloorsPresetSetting;
        public static ConfigEntry<int>? StairsPresetSetting;
        public static ConfigEntry<int>? DoorsPresetSetting;
        public static ConfigEntry<int>? FurniturePresetSetting;
        public static ConfigEntry<int>? OtherPresetSetting;
        public static ConfigEntry<int>? SolafiteOrePresetSetting;

        public static ConfigEntry<float>? MetallicIntensitySetting;
        public static ConfigEntry<float>? SmoothnessSheenSetting;
        public static ConfigEntry<float>? CrystalCrackGlowSetting;

        public static ConfigEntry<string>? CycleCategoryHotkeySetting;
        public static ConfigEntry<string>? CyclePresetHotkeySetting;

        public static ConfigEntry<bool>? OverrideAllWithCustomRgbSetting;
        public static ConfigEntry<float>? CustomRedSetting;
        public static ConfigEntry<float>? CustomGreenSetting;
        public static ConfigEntry<float>? CustomBlueSetting;

        public static ConfigEntry<bool>? RescanStructuresNowSetting;
        public static ConfigEntry<bool>? DebugLogMaterialsSetting;

        public static float SilverPowerMultiplier => PowerMultiplierSetting != null ? PowerMultiplierSetting.Value : 5.0f;

        private struct ColorPresetData
        {
            public string Name;
            public Color Albedo;
            public Color Opaque;
            public Color Surface;
            public Color Crack;
        }
        private static readonly ColorPresetData[] Presets = new ColorPresetData[]
        {
            // 0: Silver (Default)
            new ColorPresetData
            {
                Name = "SILVER",
                Albedo = new Color(0.86f, 0.89f, 0.93f, 0.0f),
                Opaque = new Color(0.86f, 0.89f, 0.93f, 1.0f),
                Surface = new Color(0.55f, 0.60f, 0.68f, 1.0f),
                Crack = new Color(0.72f, 0.78f, 0.86f, 1.0f)
            },
            // 1: Original Gold (Vanilla)
            new ColorPresetData
            {
                Name = "ORIGINAL GOLD",
                Albedo = new Color(1.00f, 0.886f, 0.608f, 0.0f),
                Opaque = new Color(1.00f, 0.886f, 0.608f, 1.0f),
                Surface = new Color(0.532f, 0.403f, 0.041f, 1.0f),
                Crack = new Color(0.491f, 0.378f, 0.123f, 1.0f)
            },
            // 2: Metallic Violet
            new ColorPresetData
            {
                Name = "METALLIC VIOLET",
                Albedo = new Color(0.58f, 0.24f, 0.88f, 0.0f),
                Opaque = new Color(0.58f, 0.24f, 0.88f, 1.0f),
                Surface = new Color(0.34f, 0.12f, 0.56f, 1.0f),
                Crack = new Color(0.76f, 0.44f, 1.00f, 1.0f)
            },
            // 3: Cobalt Blue
            new ColorPresetData
            {
                Name = "COBALT BLUE",
                Albedo = new Color(0.18f, 0.48f, 0.96f, 0.0f),
                Opaque = new Color(0.18f, 0.48f, 0.96f, 1.0f),
                Surface = new Color(0.08f, 0.25f, 0.62f, 1.0f),
                Crack = new Color(0.30f, 0.65f, 1.00f, 1.0f)
            },
            // 4: Obsidian
            new ColorPresetData
            {
                Name = "OBSIDIAN",
                Albedo = new Color(0.16f, 0.17f, 0.20f, 0.0f),
                Opaque = new Color(0.16f, 0.17f, 0.20f, 1.0f),
                Surface = new Color(0.08f, 0.09f, 0.11f, 1.0f),
                Crack = new Color(0.32f, 0.35f, 0.42f, 1.0f)
            },
            // 5: Rose Gold
            new ColorPresetData
            {
                Name = "ROSE GOLD",
                Albedo = new Color(0.95f, 0.66f, 0.60f, 0.0f),
                Opaque = new Color(0.95f, 0.66f, 0.60f, 1.0f),
                Surface = new Color(0.62f, 0.34f, 0.30f, 1.0f),
                Crack = new Color(0.88f, 0.56f, 0.50f, 1.0f)
            },
            // 6: Emerald
            new ColorPresetData
            {
                Name = "EMERALD",
                Albedo = new Color(0.16f, 0.82f, 0.46f, 0.0f),
                Opaque = new Color(0.16f, 0.82f, 0.46f, 1.0f),
                Surface = new Color(0.06f, 0.44f, 0.22f, 1.0f),
                Crack = new Color(0.28f, 0.92f, 0.56f, 1.0f)
            },
            // 7: Crimson
            new ColorPresetData
            {
                Name = "CRIMSON",
                Albedo = new Color(0.88f, 0.16f, 0.22f, 0.0f),
                Opaque = new Color(0.88f, 0.16f, 0.22f, 1.0f),
                Surface = new Color(0.48f, 0.06f, 0.10f, 1.0f),
                Crack = new Color(0.95f, 0.28f, 0.34f, 1.0f)
            }
        };

        private class MaterialMeta
        {
            public PlatingCategory Category;
            public bool IsPlatingSurface;
            public bool IsExplicitSpecialOther;
            public bool HasGoldAlbedo;
            public float GoldAlbedoAlpha;
            public bool HasBaseColor;
            public float BaseColorAlpha;
            public bool HasCrackColor;
            public bool HasSpecularColor;
            public bool HasColor;
            public float MainColorAlpha;
            public bool HasOutlineColor;
            public float OutlineColorAlpha;
            public bool HasMetallic;
            public bool HasGoldMetallic;
            public bool HasSmoothness;
            public bool HasGlossiness;
            public bool HasGoldSmoothness;
            public bool HasEmissionColor;
            public Color OriginalEmissionColor;
        }

        private readonly Dictionary<int, MaterialMeta> _materialCache = new Dictionary<int, MaterialMeta>();
        private readonly Dictionary<string, Material> _categoryClones = new Dictionary<string, Material>();
        private readonly HashSet<int> _routedRenderers = new HashSet<int>();
        private readonly HashSet<string> _loggedSamples = new HashSet<string>();

        private float _nextScanTime = 0f;
        private string _lastAppliedSignature = "";
        private int _scheduledPassStage = 0;
        private float _nextScheduledRouteTime = 0f;
        private int _activeHotkeyTargetIndex = 0;
        private static MethodInfo? _showMessageMethod;
        private static bool _searchedShowMessage;
        protected override void OnInitializeMod()
        {
            SetupConfig();
        }

        protected override void OnSdkInitialized()
        {
            SetupConfig();

            try
            {
                SettingsRegistry.CreateSettings(this, null, typeof(SilverSolafiteMod));
            }
            catch (Exception ex)
            {
                RLog.Warning($"[SilverSolafite] Settings UI notice: {ex.Message}");
            }

            SdkEvents.OnGameActivated.Subscribe(OnGameActivated);
            GlobalEvents.OnUpdate.Subscribe(OnUpdate);

            HarmonyInstance.PatchAll();
            ApplyDynamicBuffPatches();

            RLog.Msg($"[SilverSolafite] v35 Loaded! {SilverPowerMultiplier}x Power Buffs + 11 Independent Category Presets + Sheen & Hotkeys active.");
        }
        private static void SetupConfig()
        {
            if (SettingsCategory != null) return;

            SettingsCategory = ConfigSystem.CreateFileCategory("SilverSolafite", "Silver Solafite", "SilverSolafite.cfg");

            PowerMultiplierSetting = SettingsCategory.CreateEntry(
                "PlatingPowerMultiplier",
                5.0f,
                "Plating Power Multiplier",
                "Damage, armor, and structure toughness multiplier for all plated gear (1x to 50x)."
            );
            PowerMultiplierSetting.SetRange(1.0f, 50.0f);

            const string presetDesc = "0=Silver, 1=OriginalGold, 2=MetallicViolet, 3=CobaltBlue, 4=Obsidian, 5=RoseGold, 6=Emerald, 7=Crimson, 8=CustomRGB";

            WeaponsPresetSetting = SettingsCategory.CreateEntry("WeaponsPresetIndex", 0, "1. Weapons & Tools Preset (0-8)", presetDesc);
            WeaponsPresetSetting.SetRange(0, 8);

            ArmorPresetSetting = SettingsCategory.CreateEntry("ArmorPresetIndex", 0, "2. Armor Preset (0-8)", presetDesc);
            ArmorPresetSetting.SetRange(0, 8);

            LogWallsPresetSetting = SettingsCategory.CreateEntry("LogWallsPresetIndex", 0, "3. Log Walls Preset (0-8)", presetDesc);
            LogWallsPresetSetting.SetRange(0, 8);

            DefenseWallsPresetSetting = SettingsCategory.CreateEntry("DefenseWallsPresetIndex", 0, "4. Defense Walls & Gates Preset (0-8)", presetDesc);
            DefenseWallsPresetSetting.SetRange(0, 8);

            StoneWallsPresetSetting = SettingsCategory.CreateEntry("StoneWallsPresetIndex", 0, "5. Stone Walls Preset (0-8)", presetDesc);
            StoneWallsPresetSetting.SetRange(0, 8);

            FloorsPresetSetting = SettingsCategory.CreateEntry("FloorsPresetIndex", 0, "6. Floors & Roofs Preset (0-8)", presetDesc);
            FloorsPresetSetting.SetRange(0, 8);

            StairsPresetSetting = SettingsCategory.CreateEntry("StairsPresetIndex", 0, "7. Stairs & Ramps Preset (0-8)", presetDesc);
            StairsPresetSetting.SetRange(0, 8);

            DoorsPresetSetting = SettingsCategory.CreateEntry("DoorsPresetIndex", 0, "8. Doors Preset (0-8)", presetDesc);
            DoorsPresetSetting.SetRange(0, 8);

            FurniturePresetSetting = SettingsCategory.CreateEntry("FurniturePresetIndex", 0, "9. Furniture & Racks Preset (0-8)", presetDesc);
            FurniturePresetSetting.SetRange(0, 8);

            OtherPresetSetting = SettingsCategory.CreateEntry("OtherPresetIndex", 0, "10. Other Plated Items Preset (0-8)", presetDesc);
            OtherPresetSetting.SetRange(0, 8);

            SolafiteOrePresetSetting = SettingsCategory.CreateEntry("SolafiteOrePresetIndex", 0, "11. Solafite Ore Preset (0-8)", presetDesc);
            SolafiteOrePresetSetting.SetRange(0, 8);

            MetallicIntensitySetting = SettingsCategory.CreateEntry(
                "MetallicIntensity",
                0.85f,
                "Metallic Intensity",
                "Controls metallic reflectance on plated surfaces (0.00 to 1.00)."
            );
            MetallicIntensitySetting.SetRange(0.0f, 1.0f);

            SmoothnessSheenSetting = SettingsCategory.CreateEntry(
                "SmoothnessSheen",
                0.75f,
                "Smoothness / Gloss Sheen",
                "Controls mirror-like glossiness on plated surfaces (0.00 to 1.00)."
            );
            SmoothnessSheenSetting.SetRange(0.0f, 1.0f);

            CrystalCrackGlowSetting = SettingsCategory.CreateEntry(
                "CrystalCrackGlow",
                1.0f,
                "Crystal Crack Glow",
                "Boosts crystal crack brightness and emission glow on plated surfaces (0.00 to 3.00)."
            );
            CrystalCrackGlowSetting.SetRange(0.0f, 3.0f);

            CycleCategoryHotkeySetting = SettingsCategory.CreateEntry(
                "CycleCategoryHotkey",
                "NumpadMinus",
                "Hotkey: Cycle Target Category",
                "Unity Key name to cycle active category (e.g. NumpadMinus, F7, PageDown, or None to disable)."
            );

            CyclePresetHotkeySetting = SettingsCategory.CreateEntry(
                "CyclePresetHotkey",
                "NumpadPlus",
                "Hotkey: Cycle Color Preset",
                "Unity Key name to cycle color preset for selected category (e.g. NumpadPlus, F8, PageUp, or None to disable)."
            );

            OverrideAllWithCustomRgbSetting = SettingsCategory.CreateEntry(
                "OverrideAllWithCustomRGB",
                false,
                "Override All Categories with Custom RGB",
                "Turn ON to apply Custom RGB sliders across every category at once."
            );

            CustomRedSetting = SettingsCategory.CreateEntry("CustomRed", 0.86f, "Custom Red (R)", "Red channel (0.00 to 1.00).");
            CustomRedSetting.SetRange(0.0f, 1.0f);

            CustomGreenSetting = SettingsCategory.CreateEntry("CustomGreen", 0.89f, "Custom Green (G)", "Green channel (0.00 to 1.00).");
            CustomGreenSetting.SetRange(0.0f, 1.0f);

            CustomBlueSetting = SettingsCategory.CreateEntry("CustomBlue", 0.93f, "Custom Blue (B)", "Blue channel (0.00 to 1.00).");
            CustomBlueSetting.SetRange(0.0f, 1.0f);

            RescanStructuresNowSetting = SettingsCategory.CreateEntry(
                "RescanStructuresNow",
                false,
                "Re-Scan Placed Structures Now",
                "Toggle ON after building/plating new walls, floors, doors, or racks to route them immediately."
            );

            DebugLogMaterialsSetting = SettingsCategory.CreateEntry(
                "DebugLogMaterials",
                false,
                "Log Matched Items & Materials (Debug)",
                "Logs matched hierarchy paths and materials to the RedLoader console."
            );
        }

        private string BuildCurrentSignature()
        {
            int pWep = WeaponsPresetSetting != null ? Mathf.Clamp(WeaponsPresetSetting.Value, 0, 8) : 0;
            int pArm = ArmorPresetSetting != null ? Mathf.Clamp(ArmorPresetSetting.Value, 0, 8) : 0;
            int pLog = LogWallsPresetSetting != null ? Mathf.Clamp(LogWallsPresetSetting.Value, 0, 8) : 0;
            int pDef = DefenseWallsPresetSetting != null ? Mathf.Clamp(DefenseWallsPresetSetting.Value, 0, 8) : 0;
            int pStn = StoneWallsPresetSetting != null ? Mathf.Clamp(StoneWallsPresetSetting.Value, 0, 8) : 0;
            int pFlr = FloorsPresetSetting != null ? Mathf.Clamp(FloorsPresetSetting.Value, 0, 8) : 0;
            int pStr = StairsPresetSetting != null ? Mathf.Clamp(StairsPresetSetting.Value, 0, 8) : 0;
            int pDor = DoorsPresetSetting != null ? Mathf.Clamp(DoorsPresetSetting.Value, 0, 8) : 0;
            int pFrn = FurniturePresetSetting != null ? Mathf.Clamp(FurniturePresetSetting.Value, 0, 8) : 0;
            int pOth = OtherPresetSetting != null ? Mathf.Clamp(OtherPresetSetting.Value, 0, 8) : 0;
            int pOre = SolafiteOrePresetSetting != null ? Mathf.Clamp(SolafiteOrePresetSetting.Value, 0, 8) : 0;
            float met = MetallicIntensitySetting != null ? (float)Math.Round(MetallicIntensitySetting.Value, 2) : 0.85f;
            float smt = SmoothnessSheenSetting != null ? (float)Math.Round(SmoothnessSheenSetting.Value, 2) : 0.75f;
            float glw = CrystalCrackGlowSetting != null ? (float)Math.Round(CrystalCrackGlowSetting.Value, 2) : 1.0f;
            bool ovr = OverrideAllWithCustomRgbSetting != null && OverrideAllWithCustomRgbSetting.Value;
            float r = CustomRedSetting != null ? (float)Math.Round(CustomRedSetting.Value, 2) : 0.86f;
            float g = CustomGreenSetting != null ? (float)Math.Round(CustomGreenSetting.Value, 2) : 0.89f;
            float b = CustomBlueSetting != null ? (float)Math.Round(CustomBlueSetting.Value, 2) : 0.93f;
            return $"{pWep}_{pArm}_{pLog}_{pDef}_{pStn}_{pFlr}_{pStr}_{pDor}_{pFrn}_{pOth}_{pOre}_{met}_{smt}_{glw}_{ovr}_{r}_{g}_{b}";
        }

        private ColorPresetData GetColorsForCategory(PlatingCategory category)
        {
            if (category == PlatingCategory.SilverBones)
            {
                return Presets[0];
            }

            bool overrideAll = OverrideAllWithCustomRgbSetting != null && OverrideAllWithCustomRgbSetting.Value;
            int idx = 0;

            switch (category)
            {
                case PlatingCategory.Weapons:
                    idx = WeaponsPresetSetting != null ? Mathf.Clamp(WeaponsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.Armor:
                    idx = ArmorPresetSetting != null ? Mathf.Clamp(ArmorPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.LogWalls:
                    idx = LogWallsPresetSetting != null ? Mathf.Clamp(LogWallsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.DefenseWalls:
                    idx = DefenseWallsPresetSetting != null ? Mathf.Clamp(DefenseWallsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.StoneWalls:
                    idx = StoneWallsPresetSetting != null ? Mathf.Clamp(StoneWallsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.Floors:
                    idx = FloorsPresetSetting != null ? Mathf.Clamp(FloorsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.Stairs:
                    idx = StairsPresetSetting != null ? Mathf.Clamp(StairsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.Doors:
                    idx = DoorsPresetSetting != null ? Mathf.Clamp(DoorsPresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.Furniture:
                    idx = FurniturePresetSetting != null ? Mathf.Clamp(FurniturePresetSetting.Value, 0, 8) : 0;
                    break;
                case PlatingCategory.SolafiteOre:
                    idx = SolafiteOrePresetSetting != null ? Mathf.Clamp(SolafiteOrePresetSetting.Value, 0, 8) : 0;
                    break;
                default:
                    idx = OtherPresetSetting != null ? Mathf.Clamp(OtherPresetSetting.Value, 0, 8) : 0;
                    break;
            }
            if (overrideAll || idx == 8)
            {
                float r = CustomRedSetting != null ? CustomRedSetting.Value : 0.86f;
                float g = CustomGreenSetting != null ? CustomGreenSetting.Value : 0.89f;
                float b = CustomBlueSetting != null ? CustomBlueSetting.Value : 0.93f;
                return new ColorPresetData
                {
                    Name = "CUSTOM RGB",
                    Albedo = new Color(r, g, b, 0.0f),
                    Opaque = new Color(r, g, b, 1.0f),
                    Surface = new Color(r * 0.65f, g * 0.65f, b * 0.65f, 1.0f),
                    Crack = new Color(Mathf.Clamp01(r * 1.15f), Mathf.Clamp01(g * 1.15f), Mathf.Clamp01(b * 1.15f), 1.0f)
                };
            }
            if (idx >= 0 && idx < Presets.Length)
            {
                return Presets[idx];
            }
            return Presets[0];
        }
        private void ApplyDynamicBuffPatches()
        {
            try
            {
                Type? rangedType = AccessTools.TypeByName("Sons.Weapon.RangedWeapon");
                if (rangedType != null)
                {
                    foreach (var m in rangedType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (m.Name.Contains("Damage") && m.ReturnType == typeof(float) && m.GetParameters().Length == 0)
                        {
                            HarmonyInstance.Patch(m, postfix: new HarmonyMethod(typeof(SilverBuffHelpers), nameof(SilverBuffHelpers.BoostRangedDamagePostfix)));
                        }
                    }
                }

                Type? armorType = AccessTools.TypeByName("Sons.Armor.PlayerArmorSystem");
                if (armorType != null)
                {
                    foreach (var m in armorType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (m.Name.Contains("Damage") && m.GetParameters().Length > 0 && m.GetParameters()[0].ParameterType == typeof(float))

                        {
                            HarmonyInstance.Patch(m, prefix: new HarmonyMethod(typeof(SilverBuffHelpers), nameof(SilverBuffHelpers.ReduceDamagePrefix)));
                            break;
                        }
                    }
                }

                Type? structType = AccessTools.TypeByName("Sons.Construction.StructureDestruction")
                                ?? AccessTools.TypeByName("Sons.Construction.ScrewStructure");
                if (structType != null)
                {
                    foreach (var m in structType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (m.Name.Contains("Damage") || m.Name.Contains("Hit"))
                        {
                            var parms = m.GetParameters();
                            for (int i = 0; i < parms.Length; i++)
                            {
                                if (parms[i].ParameterType == typeof(float))
                                {
                                    HarmonyInstance.Patch(m, prefix: new HarmonyMethod(typeof(SilverBuffHelpers), nameof(SilverBuffHelpers.ReduceStructureDamagePrefix)));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RLog.Warning($"[SilverSolafite] Dynamic patch notice: {ex.Message}");
            }
        }

        private void OnGameActivated()
        {
            _materialCache.Clear();
            _categoryClones.Clear();
            _routedRenderers.Clear();
            _loggedSamples.Clear();
            SilverBuffHelpers.ClearCache();
            _lastAppliedSignature = "";

            _scheduledPassStage = 1;
            _nextScheduledRouteTime = Time.realtimeSinceStartup + 55.0f;

            ApplyAllCategoryColors(forceReapply: true);
        }

        private void OnUpdate()
        {
            HandleHotkeyInput();

            if (RescanStructuresNowSetting != null && RescanStructuresNowSetting.Value)
            {
                RescanStructuresNowSetting.Value = false;
                _routedRenderers.Clear();
                RoutePlacedStructuresAcrossAllScenes(logSummary: true);
                ApplyAllCategoryColors(forceReapply: true);
                return;
            }

            if (_scheduledPassStage > 0 && Time.realtimeSinceStartup >= _nextScheduledRouteTime)
            {
                RoutePlacedStructuresAcrossAllScenes(logSummary: true);
                ApplyAllCategoryColors(forceReapply: true);

                if (_scheduledPassStage == 1)
                {
                    _scheduledPassStage = 2;
                    _nextScheduledRouteTime = Time.realtimeSinceStartup + 20.0f;
                }
                else if (_scheduledPassStage == 2)
                {
                    _scheduledPassStage = 3;
                    _nextScheduledRouteTime = Time.realtimeSinceStartup + 20.0f;
                }
                else
                {
                    _scheduledPassStage = 0;
                }
            }

            string currentSig = BuildCurrentSignature();
            bool settingsChanged = currentSig != _lastAppliedSignature;

            if (settingsChanged)
            {
                _lastAppliedSignature = currentSig;
                ApplyAllCategoryColors(forceReapply: true);
                return;
            }

            if (Time.realtimeSinceStartup < _nextScanTime) return;
            _nextScanTime = Time.realtimeSinceStartup + 6.0f;

            if (Time.timeSinceLevelLoad < 3.0f) return;
            ApplyAllCategoryColors(forceReapply: false);
        }

        private void HandleHotkeyInput()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (WasConfiguredKeyPressed(kb, CycleCategoryHotkeySetting?.Value))
            {
                _activeHotkeyTargetIndex = (_activeHotkeyTargetIndex + 1) % 11;
                ShowHotkeyBanner();
            }

            if (WasConfiguredKeyPressed(kb, CyclePresetHotkeySetting?.Value))
            {
                var entry = GetHotkeyTargetEntry(_activeHotkeyTargetIndex);
                if (entry != null)
                {
                    entry.Value = (entry.Value + 1) % 9;
                    ShowHotkeyBanner();
                }
            }
        }

        private void ShowHotkeyBanner()
        {
            string catLabel = GetHotkeyTargetLabel(_activeHotkeyTargetIndex);
            var entry = GetHotkeyTargetEntry(_activeHotkeyTargetIndex);
            int presetIdx = entry != null ? Mathf.Clamp(entry.Value, 0, 8) : 0;

            string presetName;
            Color swatchColor;

            if (OverrideAllWithCustomRgbSetting != null && OverrideAllWithCustomRgbSetting.Value)
            {
                float r = CustomRedSetting != null ? CustomRedSetting.Value : 0.86f;
                float g = CustomGreenSetting != null ? CustomGreenSetting.Value : 0.89f;
                float b = CustomBlueSetting != null ? CustomBlueSetting.Value : 0.93f;
                presetName = "CUSTOM RGB (GLOBAL OVERRIDE)";
                swatchColor = new Color(r, g, b, 1f);
            }
            else if (presetIdx < Presets.Length)
            {
                presetName = Presets[presetIdx].Name;
                swatchColor = Presets[presetIdx].Opaque;
            }
            else
            {
                float r = CustomRedSetting != null ? CustomRedSetting.Value : 0.86f;
                float g = CustomGreenSetting != null ? CustomGreenSetting.Value : 0.89f;
                float b = CustomBlueSetting != null ? CustomBlueSetting.Value : 0.93f;
                presetName = "CUSTOM RGB";
                swatchColor = new Color(r, g, b, 1f);
            }

            RLog.Msg($"[SilverSolafite] {catLabel} -> {presetName} ({presetIdx})");

            string hex = ColorUtility.ToHtmlStringRGB(swatchColor);
            string hudText = "<b>" + catLabel + "</b>\n<color=#" + hex + ">Preset [" + presetIdx + "]: " + presetName + "</color>";
            DisplayOnScreenNotification(hudText, 3.0f);
        }

        private static void DisplayOnScreenNotification(string message, float duration)
        {
            try
            {
                if (!_searchedShowMessage)
                {
                    _searchedShowMessage = true;
                    System.Type? toolsType = AccessTools.TypeByName("SonsSdk.SonsTools");
                    if (toolsType != null)
                    {
                        foreach (var m in toolsType.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            if (m.Name == "ShowMessage")
                            {
                                var p = m.GetParameters();
                                if (p.Length >= 1 && p[0].ParameterType == typeof(string))
                                {
                                    _showMessageMethod = m;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (_showMessageMethod != null)
                {
                    var p = _showMessageMethod.GetParameters();
                    if (p.Length == 1)
                    {
                        _showMessageMethod.Invoke(null, new object[] { message });
                    }
                    else if (p.Length == 2 && p[1].ParameterType == typeof(float))
                    {
                        _showMessageMethod.Invoke(null, new object[] { message, duration });
                    }
                    else if (p.Length == 3 && p[1].ParameterType == typeof(float))
                    {
                        _showMessageMethod.Invoke(null, new object?[] { message, duration, null });
                    }
                }
            }
            catch (System.Exception ex)
            {
                RLog.Warning($"[SilverSolafite] HUD message notice: {ex.Message}");
            }
        }

        private static bool WasConfiguredKeyPressed(Keyboard kb, string? keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) return false;
            string trimmed = keyName.Trim();
            if (trimmed.Equals("None", StringComparison.OrdinalIgnoreCase)) return false;

            if (Enum.TryParse<Key>(trimmed, true, out Key parsedKey) && parsedKey != Key.None)
            {
                var control = kb[parsedKey];
                return control != null && control.wasPressedThisFrame;
            }

            return false;
        }

        private static string GetHotkeyTargetLabel(int idx)
        {
            return idx switch
            {
                0 => "1. Weapons & Tools",
                1 => "2. Armor",
                2 => "3. Log Walls",
                3 => "4. Defense Walls & Gates",
                4 => "5. Stone Walls",
                5 => "6. Floors & Roofs",
                6 => "7. Stairs & Ramps",
                7 => "8. Doors",
                8 => "9. Furniture & Racks",
                9 => "10. Other Plated Items",
                10 => "11. Solafite Ore",
                _ => "1. Weapons & Tools"
            };
        }

        private static ConfigEntry<int>? GetHotkeyTargetEntry(int idx)
        {
            return idx switch
            {
                0 => WeaponsPresetSetting,
                1 => ArmorPresetSetting,
                2 => LogWallsPresetSetting,
                3 => DefenseWallsPresetSetting,
                4 => StoneWallsPresetSetting,
                5 => FloorsPresetSetting,
                6 => StairsPresetSetting,
                7 => DoorsPresetSetting,
                8 => FurniturePresetSetting,
                9 => OtherPresetSetting,
                10 => SolafiteOrePresetSetting,
                _ => WeaponsPresetSetting
            };
        }

        private static bool IsUnplatedStickOrExcludedProp(string text)
        {
            if (text.Contains("gold") || text.Contains("solafite") || text.Contains("plated") || text.Contains("_ss_"))
            {
                return false;
            }

            bool isArmor = text.Contains("hidearmor") || text.Contains("hidearmour") ||
                           text.Contains("leafarmor") || text.Contains("leafarmour") ||
                           text.Contains("bonearmor") || text.Contains("bonearmour");
            if (isArmor) return false;

            return text.Contains("stick") || text.Contains("twig") || text.Contains("bark") ||
                   text.Contains("branch") || text.Contains("pole") || text.Contains("kindling") ||
                   text.Contains("firewood") || text.Contains("rod") || text.Contains("wicker") ||
                   text.Contains("clock") || text.Contains("dial") || text.Contains("spearstand") ||
                   text.Contains("stickholder") || text.Contains("stickrack") || text.Contains("rope") ||
                   text.Contains("cloth") || text.Contains("tarp") || text.Contains("leaf") ||
                   text.Contains("leaves") || text.Contains("feather") || text.Contains("paper") ||
                   text.Contains("hide") || text.Contains("skin");
        }

        private static bool IsSolafiteOreItem(string fullPath, string matName)
        {
            if (fullPath.Contains("solafiteupgrader") || fullPath.Contains("solafite_upgrader"))
            {
                if (!fullPath.Contains("ore") && !fullPath.Contains("chunk") &&
                    !fullPath.Contains("crystal") && !fullPath.Contains("rock") &&
                    !fullPath.Contains("mineral") && !fullPath.Contains("nugget") &&
!fullPath.Contains("ingot") && !fullPath.Contains("shard") &&
!fullPath.Contains("pickup") && !fullPath.Contains("item"))
                {
                    return false;
                }
            }
            if (fullPath.Contains("solafiteore") || fullPath.Contains("solafite_ore") ||
            fullPath.Contains("solafitechunk") || fullPath.Contains("solafite_chunk") ||
            fullPath.Contains("solafitecrystal") || fullPath.Contains("solafite_crystal") ||
            fullPath.Contains("solafiterock") || fullPath.Contains("solafite_rock") ||
            fullPath.Contains("solafitepickup") || fullPath.Contains("solafite_pickup") ||
            fullPath.Contains("solafiteitem") || fullPath.Contains("solafite_item") ||
            matName.Contains("solafiteore") || matName.Contains("solafite_ore") ||
            matName.Contains("solafitechunk") || matName.Contains("solafitecrystal"))
            {
                return true;
            }
            bool hasSolafite = fullPath.Contains("solafite") || matName.Contains("solafite");
            if (hasSolafite && (fullPath.Contains("ore") || fullPath.Contains("chunk") ||
            fullPath.Contains("crystal") || fullPath.Contains("mineral") ||
            fullPath.Contains("nugget") || fullPath.Contains("shard") ||
            fullPath.Contains("vein") || fullPath.Contains("node") ||
            matName.Contains("ore") || matName.Contains("chunk") ||
            matName.Contains("crystal") || matName.Contains("vein")))
            {
                return true;
            }
            return false;
        }

        private static bool IsExplicitSpecialOtherKeyword(string text)
        {
            return text.Contains("shrine") || text.Contains("attract") || text.Contains("repel") ||
                   text.Contains("teleport") || text.Contains("portal") || text.Contains("monolith") ||
                   text.Contains("obelisk") || text.Contains("upgrader") || text.Contains("plater") ||
                   text.Contains("solafiteupgrader") || text.Contains("armorplater") ||
                   text.Contains("weaponupgrader") || text.Contains("artifact");
        }

        private static bool IsPlatingMaterial(Material mat)
        {
            if (mat == null || mat.shader == null) return false;

            string mName = mat.name.ToLowerInvariant();
            if (IsUnplatedStickOrExcludedProp(mName)) return false;

            if (mat.HasProperty("_GoldAlbedo")) return true;

            string sName = mat.shader.name.ToLowerInvariant();
            if (sName.Contains("plating") || sName.Contains("solafite") || sName.Contains("gold")) return true;

            if (mName.Contains("gold") || mName.Contains("solafite") || mName.Contains("plated") ||
                mName.Contains("plating") || mName.Contains("shrine") || mName.Contains("teleport")) return true;

            return false;
        }

        private static bool IsBookOrPreviewHierarchy(string path)
        {
            return path.Contains("book") || path.Contains("blueprint") || path.Contains("ghost") ||
                   path.Contains("preview") || path.Contains("recipe") || path.Contains("handbook") ||
                   path.Contains("guide") || path.Contains("buildbook") || path.Contains("ui_") ||
                   path.Contains("canvas");
        }

        private static bool IsSilverBoneOrUpgraderIngredient(Transform t, string fullPath, string matName)
        {
            bool inUpgrader = fullPath.Contains("upgrader") || fullPath.Contains("plater") ||
                              fullPath.Contains("solafiteupgrader") || fullPath.Contains("armorplater") ||
                              fullPath.Contains("weaponupgrader");

            if (fullPath.Contains("bonearmor") || fullPath.Contains("bonearmour") ||
                matName.Contains("bonearmor") || matName.Contains("bonearmour"))
            {
                return false;
            }

            if (fullPath.Contains("skull") || fullPath.Contains("bone") || fullPath.Contains("rib") ||
                fullPath.Contains("spine") || fullPath.Contains("vertebra") || fullPath.Contains("trophy") ||
                fullPath.Contains("effigy") || matName.Contains("skull") || matName.Contains("bone"))
            {
                return true;
            }

            if (inUpgrader && (fullPath.Contains("pile") || fullPath.Contains("basket") ||
                               fullPath.Contains("ingredient") || fullPath.Contains("offer") ||
                               fullPath.Contains("gib") || fullPath.Contains("limb") ||
                               fullPath.Contains("part")))
            {
                return true;
            }

            return false;
        }

        private static bool HasWeaponComponentOrId(Transform t, string fullPath)
        {
            if (fullPath.Contains("guitar") || fullPath.Contains("340") || fullPath.Contains("acoustic") ||
                fullPath.Contains("instrument") || fullPath.Contains("katana") || fullPath.Contains("machete") ||
                fullPath.Contains("putter") || fullPath.Contains("chainsaw") || fullPath.Contains("crossbow") ||
                fullPath.Contains("shotgun") || fullPath.Contains("pistol") || fullPath.Contains("revolver") ||
                fullPath.Contains("rifle") || fullPath.Contains("slingshot") || fullPath.Contains("canopener") ||
                fullPath.Contains("repairtool") || fullPath.Contains("shovel") || fullPath.Contains("pickaxe") ||
                fullPath.Contains("taser") || fullPath.Contains("baton") || fullPath.Contains("stun") ||
                fullPath.Contains("spear") || fullPath.Contains("club") || fullPath.Contains("axe"))
            {
                return true;
            }

            Transform? cur = t;
            int depth = 0;
            while (cur != null && depth < 6)
            {
                var comps = cur.GetComponents<Component>();
                for (int i = 0; i < comps.Length; i++)
                {
                    var c = comps[i];
                    if (c == null) continue;
                    string typeName = c.GetType().Name;
                    if (typeName.Contains("MeleeWeapon") || typeName.Contains("RangedWeapon") ||
                        typeName.Contains("HeldController") || typeName.Contains("Equippable") ||
                        typeName.Contains("WeaponController") || typeName.Contains("ItemInstance"))
                    {
                        return true;
                    }
                }
                cur = cur.parent;
                depth++;
            }

            return false;
        }

        private static bool IsPartOfDoorAssembly(Transform t)
        {
            Transform? cur = t;
            int depth = 0;
            while (cur != null && depth < 10)
            {
                string n = cur.name.ToLowerInvariant();

                if (n.Contains("doorframe") || n.Contains("frame") || n.Contains("header") ||
                    n.Contains("lintel") || n.Contains("jamb") || n.Contains("arch") ||
                    n.Contains("opening") || n.Contains("logwall") || n.Contains("stonewall") ||
                    n.Contains("defensive") || n.Contains("palisade"))
                {
                    return false;
                }

                if (n.Contains("door") || n.Contains("hinge") || n.Contains("shutter"))
                {
                    return true;
                }

                var comps = cur.GetComponents<Component>();
                for (int i = 0; i < comps.Length; i++)
                {
                    var c = comps[i];
                    if (c == null) continue;
                    string typeName = c.GetType().Name;
                    if (typeName == "Door" || typeName.Contains("DoorController") ||
                        typeName.Contains("Openable") || typeName.Contains("Hinge"))
                    {
                        return true;
                    }
                }

                cur = cur.parent;
                depth++;
            }

            return false;
        }

        private static bool IsRampHierarchy(string fullPath)
        {
            if (fullPath.Contains("doorframe") || fullPath.Contains("frame") ||
                fullPath.Contains("header") || fullPath.Contains("lintel") ||
                fullPath.Contains("jamb") || fullPath.Contains("opening") ||
                fullPath.Contains("wall"))
            {
                return false;
            }

            return fullPath.Contains("ramp") || fullPath.Contains("incline") ||
                   fullPath.Contains("inclined") || fullPath.Contains("slope") ||
                   fullPath.Contains("sloped") || fullPath.Contains("lean") ||
                   fullPath.Contains("leaning") || fullPath.Contains("diagonal") ||
                   fullPath.Contains("bridge");
        }

        private void RoutePlacedStructuresAcrossAllScenes(bool logSummary)
        {
            try
            {
                int cLogWalls = 0, cDefWalls = 0, cStoneWalls = 0, cFloors = 0, cStairs = 0, cDoors = 0, cFurn = 0, cWep = 0, cArm = 0, cOther = 0, cBones = 0, cOre = 0;

                List<GameObject> rootObjects = new List<GameObject>();

                int sceneCount = SceneManager.sceneCount;
                for (int s = 0; s < sceneCount; s++)
                {
                    Scene sc = SceneManager.GetSceneAt(s);
                    if (!sc.IsValid() || !sc.isLoaded) continue;
                    var roots = sc.GetRootGameObjects();
                    if (roots != null) rootObjects.AddRange(roots);
                }

                GameObject? ddolProbe = null;
                try
                {
                    ddolProbe = new GameObject("_SS_DDOL_Probe");
                    UnityEngine.Object.DontDestroyOnLoad(ddolProbe);
                    var ddolRoots = ddolProbe.scene.GetRootGameObjects();
                    if (ddolRoots != null) rootObjects.AddRange(ddolRoots);
                }
                finally
                {
                    if (ddolProbe != null) UnityEngine.Object.Destroy(ddolProbe);
                }

                for (int i = 0; i < rootObjects.Count; i++)
                {
                    GameObject root = rootObjects[i];
                    if (root == null) continue;

                    string rName = root.name.ToLowerInvariant();
                    if (rName.Contains("terrain") || rName.Contains("water") || rName.Contains("ocean") ||
                        rName.Contains("audio") || rName.Contains("weather") || rName.Contains("sky") ||
                        rName.Contains("foliage") || rName.Contains("trees") || rName.Contains("bushes"))
                    {
                        continue;
                    }

                    var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
                    if (renderers == null || renderers.Length == 0) continue;

                    for (int r = 0; r < renderers.Length; r++)
                    {
                        MeshRenderer mr = renderers[r];
                        if (mr == null) continue;

                        int rId = mr.GetInstanceID();
                        if (_routedRenderers.Contains(rId)) continue;

                        var shared = mr.sharedMaterials;
                        if (shared == null || shared.Length == 0) continue;

                        bool hasPlating = false;
                        string firstPlatingMatName = "";
                        for (int m = 0; m < shared.Length; m++)
                        {
                            if (shared[m] != null && IsPlatingMaterial(shared[m]))
                            {
                                hasPlating = true;
                                firstPlatingMatName = shared[m].name.ToLowerInvariant();
                                break;
                            }
                        }
                        if (!hasPlating) continue;
                        string path = BuildHierarchyPath(mr.transform);
                        if (IsBookOrPreviewHierarchy(path) || IsUnplatedStickOrExcludedProp(path))
                        {
                            _routedRenderers.Add(rId);
                            continue;
                        }
                        PlatingCategory cat = DetectCategoryFromHierarchy(mr.transform, path, firstPlatingMatName);
                        _routedRenderers.Add(rId);
                        if (_loggedSamples.Count < 30 && _loggedSamples.Add($"{cat}:{path}"))
                        {
                            if (DebugLogMaterialsSetting != null && DebugLogMaterialsSetting.Value)
                            {
                                RLog.Msg($"[SilverSolafite] Routed -> [{cat}] Path: '{path}'");
                            }
                        }
                        bool changed = false;
                        bool isSpecialOther = cat == PlatingCategory.Other && IsExplicitSpecialOtherKeyword(path);
                        for (int m = 0; m < shared.Length; m++)
                        {
                            Material origMat = shared[m];
                            if (origMat == null || !IsPlatingMaterial(origMat)) continue;
                            Material cloneMat = GetOrCreateCategoryClone(origMat, cat, isSpecialOther);
                            if (cloneMat != null && cloneMat != origMat)
                            {
                                shared[m] = cloneMat;
                                changed = true;
                            }
                        }
                        if (changed)
                        {
                            mr.sharedMaterials = shared;
                        }
                        switch (cat)
                        {
                            case PlatingCategory.LogWalls: cLogWalls++; break;
                            case PlatingCategory.DefenseWalls: cDefWalls++; break;
                            case PlatingCategory.StoneWalls: cStoneWalls++; break;
                            case PlatingCategory.Floors: cFloors++; break;
                            case PlatingCategory.Stairs: cStairs++; break;
                            case PlatingCategory.Doors: cDoors++; break;
                            case PlatingCategory.Furniture: cFurn++; break;
                            case PlatingCategory.Weapons: cWep++; break;
                            case PlatingCategory.Armor: cArm++; break;
                            case PlatingCategory.SilverBones: cBones++; break;
                            case PlatingCategory.SolafiteOre: cOre++; break;
                            default: cOther++; break;
                        }
                    }
                }
                if (logSummary)
                {
                    RLog.Msg($"[SilverSolafite] Routed -> LogWalls:{cLogWalls}, DefWalls:{cDefWalls}, StoneWalls:{cStoneWalls}, Floors:{cFloors}, Stairs:{cStairs}, Doors:{cDoors}, Furn:{cFurn}, Wep:{cWep}, Arm:{cArm}, Ore:{cOre}, Other:{cOther}, SilverBones:{cBones}");
                }
            }
            catch (Exception ex)
            {
                RLog.Warning($"[SilverSolafite] Scene routing notice: {ex.Message}");
            }
        }
        private static string BuildHierarchyPath(Transform t)
        {
            string path = t.name;
            Transform? cur = t.parent;
            int depth = 0;
            while (cur != null && depth < 10)
            {
                path = cur.name + "/" + path;
                cur = cur.parent;
                depth++;
            }
            return path.ToLowerInvariant();
        }
        private static PlatingCategory DetectCategoryFromHierarchy(Transform t, string path, string matName)
        {
            if (IsSilverBoneOrUpgraderIngredient(t, path, matName))
            {
                return PlatingCategory.SilverBones;
            }
            if (HasWeaponComponentOrId(t, path))
            {
                return PlatingCategory.Weapons;
            }
            if ((path.Contains("armor") || path.Contains("armour") || path.Contains("techarmor") ||
            path.Contains("bonearmor") || path.Contains("creepyarmor") || path.Contains("hidearmor") ||
            path.Contains("leafarmor") || path.Contains("goldenarmor")) &&
            !path.Contains("armorrack") && !path.Contains("armor_rack") &&
            !path.Contains("armorplater") && !path.Contains("armor_plater"))
            {
                return PlatingCategory.Armor;
            }
            if (IsSolafiteOreItem(path, matName))
            {
                return PlatingCategory.SolafiteOre;
            }
            if (IsExplicitSpecialOtherKeyword(path))
            {
                return PlatingCategory.Other;
            }
            if (path.Contains("weaponrack") || path.Contains("wallweaponrack") || path.Contains("wallrack") ||
            path.Contains("gunrack") || path.Contains("bowrack") || path.Contains("armorrack") ||
            path.Contains("armor_rack") || path.Contains("mannequin") || path.Contains("rack") ||
            path.Contains("holder") || path.Contains("shelf") || path.Contains("shelves") ||
            path.Contains("table") || path.Contains("chair") || path.Contains("bed") ||
            path.Contains("bench") || path.Contains("planter") || path.Contains("furniture") ||
            path.Contains("storage") || path.Contains("stand") || path.Contains("drying") ||
            path.Contains("bonfire") || path.Contains("torch"))
            {
                return PlatingCategory.Furniture;
            }
            if (path.Contains("defensive") || path.Contains("defense") || path.Contains("palisade") ||
            path.Contains("spike") || path.Contains("sharpened"))
            {
                return PlatingCategory.DefenseWalls;
            }
            if (IsPartOfDoorAssembly(t))
            {
                return PlatingCategory.Doors;
            }
            if (path.Contains("stair") || path.Contains("ladder") || path.Contains("step") ||
            IsRampHierarchy(path))
            {
                return PlatingCategory.Stairs;
            }
            if (path.Contains("floor") || path.Contains("roof") || path.Contains("ceiling") ||
            path.Contains("deck") || path.Contains("foundation") || path.Contains("platform") ||
            path.Contains("dock") || path.Contains("plank"))
            {
                return PlatingCategory.Floors;
            }
            if (path.Contains("stone") || path.Contains("rock") || path.Contains("boulder") ||
            path.Contains("masonry") || path.Contains("brick") || path.Contains("cobble"))
            {
                return PlatingCategory.StoneWalls;
            }
            if (path.Contains("log") || path.Contains("freeform") || path.Contains("strut") ||
            path.Contains("beam") || path.Contains("pillar") || path.Contains("column") ||
            path.Contains("wall") || path.Contains("doorframe") || path.Contains("frame"))
            {
                return PlatingCategory.LogWalls;
            }
            return PlatingCategory.Other;
        }
        private Material GetOrCreateCategoryClone(Material sourceMat, PlatingCategory category, bool isExplicitSpecialOther)
        {
            string tag = isExplicitSpecialOther ? "OtherSpecial" : category.ToString();
            string key = $"{sourceMat.GetInstanceID()}_{tag}";
            if (_categoryClones.TryGetValue(key, out Material? existing) && existing != null)
            {
                return existing;
            }

            Material clone = new Material(sourceMat);
            clone.name = $"{sourceMat.name}_SS_{tag}";
            _categoryClones[key] = clone;

            MaterialMeta meta = InspectMaterial(clone, category, isExplicitSpecialOther);
            _materialCache[clone.GetInstanceID()] = meta;
            ApplyPresetToMaterial(clone, meta, GetColorsForCategory(category));

            return clone;
        }

        private PlatingCategory ClassifyGlobalMaterial(Material mat, out bool isExplicitSpecialOther)
        {
            isExplicitSpecialOther = false;
            string mName = mat.name.ToLowerInvariant();
            string sName = mat.shader != null ? mat.shader.name.ToLowerInvariant() : "";

            if (mName.Contains("_ss_otherspecial"))
            {
                isExplicitSpecialOther = true;
                return PlatingCategory.Other;
            }
            if (mName.Contains("_ss_logwalls")) return PlatingCategory.LogWalls;
            if (mName.Contains("_ss_defensewalls")) return PlatingCategory.DefenseWalls;
            if (mName.Contains("_ss_stonewalls")) return PlatingCategory.StoneWalls;
            if (mName.Contains("_ss_floors")) return PlatingCategory.Floors;
            if (mName.Contains("_ss_stairs")) return PlatingCategory.Stairs;
            if (mName.Contains("_ss_doors")) return PlatingCategory.Doors;
            if (mName.Contains("_ss_furniture")) return PlatingCategory.Furniture;
            if (mName.Contains("_ss_weapons")) return PlatingCategory.Weapons;
            if (mName.Contains("_ss_armor")) return PlatingCategory.Armor;
            if (mName.Contains("_ss_silverbones")) return PlatingCategory.SilverBones;
            if (mName.Contains("_ss_solafiteore")) return PlatingCategory.SolafiteOre;
            if (mName.Contains("_ss_other")) return PlatingCategory.Other;

            if ((mName.Contains("skull") || mName.Contains("bone")) && !mName.Contains("armor"))
            {
                return PlatingCategory.SilverBones;
            }

            if ((mName.Contains("armor") || mName.Contains("armour") || mName.Contains("goldenarmor") ||
                 mName.Contains("techarmor") || mName.Contains("bonearmor") || mName.Contains("creepyarmor") ||
                 mName.Contains("hidearmor") || mName.Contains("leafarmor")) &&
                !mName.Contains("rack") && !mName.Contains("plater"))
            {
                return PlatingCategory.Armor;
            }

            if (IsSolafiteOreItem(mName, mName))
            {
                return PlatingCategory.SolafiteOre;
            }

            if (IsExplicitSpecialOtherKeyword(mName))
            {
                isExplicitSpecialOther = true;
                return PlatingCategory.Other;
            }

            if (mName.Contains("axe") || mName.Contains("katana") || mName.Contains("pistol") ||
                mName.Contains("shotgun") || mName.Contains("crossbow") || mName.Contains("putter") ||
                mName.Contains("guitar") || mName.Contains("340") || mName.Contains("acoustic") ||
                mName.Contains("instrument") || mName.Contains("machete") || mName.Contains("spear") ||
                mName.Contains("club") || mName.Contains("bow") || mName.Contains("rifle") ||
                mName.Contains("revolver") || mName.Contains("weapon") || mName.Contains("chainsaw") ||
                mName.Contains("stun") || mName.Contains("knife") || mName.Contains("slingshot") ||
                mName.Contains("shovel") || mName.Contains("pickaxe") || mName.Contains("repair") ||
                mName.Contains("canopener") || mName.Contains("flashlight") || mName.Contains("taser") ||
                mName.Contains("baton"))
            {
                return PlatingCategory.Weapons;
            }

            if (mName.Contains("rack") || mName.Contains("holder") || mName.Contains("shelf") ||
                mName.Contains("table") || mName.Contains("chair") || mName.Contains("bed") ||
                mName.Contains("bench") || mName.Contains("planter") || mName.Contains("furniture") ||
                mName.Contains("stand"))
            {
                return PlatingCategory.Furniture;
            }

            if (mName.Contains("defensive") || mName.Contains("defense") || mName.Contains("palisade") ||
                mName.Contains("spike") || mName.Contains("sharpened"))
            {
                return PlatingCategory.DefenseWalls;
            }

            if (mName.Contains("door") || mName.Contains("shutter"))
            {
                return PlatingCategory.Doors;
            }

            if (mName.Contains("stair") || mName.Contains("ramp") || mName.Contains("ladder"))
            {
                return PlatingCategory.Stairs;
            }

            if (mName.Contains("floor") || mName.Contains("roof") || mName.Contains("deck") ||
                mName.Contains("foundation") || mName.Contains("platform") || mName.Contains("plank"))
            {
                return PlatingCategory.Floors;
            }

            if (mName.Contains("stone") || mName.Contains("rock") || mName.Contains("boulder") ||
                mName.Contains("masonry"))
            {
                return PlatingCategory.StoneWalls;
            }

            if (mName.Contains("wall") || mName.Contains("log") || mName.Contains("beam") ||
                mName.Contains("pillar"))
            {
                return PlatingCategory.LogWalls;
            }

            if (sName.Contains("platingsurface") || mName.Contains("solafitesurface"))
            {
                return PlatingCategory.Other;
            }

            return PlatingCategory.Other;
        }

        private MaterialMeta InspectMaterial(Material mat, PlatingCategory category, bool isExplicitSpecialOther)
        {
            string mName = mat.name.ToLowerInvariant();
            string sName = mat.shader != null ? mat.shader.name.ToLowerInvariant() : "";

            bool hasGoldAlbedo = mat.HasProperty("_GoldAlbedo");
            bool isPlatingSurface = sName.Contains("platingsurface") || mName.Contains("solafitesurface") ||
                                    sName.Contains("solafite") || mName.Contains("solafite");

            MaterialMeta meta = new MaterialMeta
            {
                Category = category,
                IsPlatingSurface = isPlatingSurface,
                IsExplicitSpecialOther = isExplicitSpecialOther,
                HasGoldAlbedo = hasGoldAlbedo
            };

            if (hasGoldAlbedo)
            {
                meta.GoldAlbedoAlpha = mat.GetColor("_GoldAlbedo").a;
            }
            if (mat.HasProperty("_BaseColor"))
            {
                meta.HasBaseColor = true;
                meta.BaseColorAlpha = mat.GetColor("_BaseColor").a;
            }
            if (mat.HasProperty("_CrackColor"))
            {
                meta.HasCrackColor = true;
            }
            if (mat.HasProperty("_SpecularColor"))
            {
                meta.HasSpecularColor = true;
            }
            if (mat.HasProperty("_Color"))
            {
                meta.HasColor = true;
                meta.MainColorAlpha = mat.GetColor("_Color").a;
            }
            if (mat.HasProperty("_OutlineColor"))
            {
                meta.HasOutlineColor = true;
                meta.OutlineColorAlpha = mat.GetColor("_OutlineColor").a;
            }
            if (mat.HasProperty("_Metallic"))
            {
                meta.HasMetallic = true;
            }
            if (mat.HasProperty("_GoldMetallic"))
            {
                meta.HasGoldMetallic = true;
            }
            if (mat.HasProperty("_Smoothness"))
            {
                meta.HasSmoothness = true;
            }
            if (mat.HasProperty("_Glossiness"))
            {
                meta.HasGlossiness = true;
            }
            if (mat.HasProperty("_GoldSmoothness"))
            {
                meta.HasGoldSmoothness = true;
            }
            if (mat.HasProperty("_EmissionColor"))
            {
                meta.HasEmissionColor = true;
                meta.OriginalEmissionColor = mat.GetColor("_EmissionColor");
            }

            return meta;
        }

        private void ApplyPresetToMaterial(Material mat, MaterialMeta meta, ColorPresetData active)
        {
            float metallic = MetallicIntensitySetting != null ? Mathf.Clamp01(MetallicIntensitySetting.Value) : 0.85f;
            float smoothness = SmoothnessSheenSetting != null ? Mathf.Clamp01(SmoothnessSheenSetting.Value) : 0.75f;
            float crackGlow = CrystalCrackGlowSetting != null ? Mathf.Clamp(CrystalCrackGlowSetting.Value, 0.0f, 3.0f) : 1.0f;

            if (meta.HasGoldAlbedo)
            {
                mat.SetColor("_GoldAlbedo", new Color(active.Albedo.r, active.Albedo.g, active.Albedo.b, meta.GoldAlbedoAlpha));
            }

            if (meta.HasGoldMetallic)
            {
                mat.SetFloat("_GoldMetallic", metallic);
            }
            if (meta.HasGoldSmoothness)
            {
                mat.SetFloat("_GoldSmoothness", smoothness);
            }

            if (meta.HasCrackColor)
            {
                if (meta.HasBaseColor)
                {
                    mat.SetColor("_BaseColor", active.Surface);
                }
                Color boostedCrack = new Color(
                active.Crack.r * crackGlow,
                active.Crack.g * crackGlow,
                active.Crack.b * crackGlow,
                active.Crack.a
                );
                mat.SetColor("_CrackColor", boostedCrack);
            }
            // Only Armor, SilverBones, SolafiteOre, and explicitly matched Special Other items
            // may update _BaseColor/_Color/_SpecularColor/standard PBR sheen so unplated stick props & wood never get tinted!
            bool allowBaseColorOverride = meta.Category == PlatingCategory.Armor ||
            meta.Category == PlatingCategory.SilverBones ||
            meta.Category == PlatingCategory.SolafiteOre ||
            (meta.Category == PlatingCategory.Other && meta.IsExplicitSpecialOther);
            if (!allowBaseColorOverride)
            {
                if (meta.IsPlatingSurface && !meta.HasCrackColor)
                {
                    if (meta.HasBaseColor)
                        mat.SetColor("_BaseColor", new Color(active.Opaque.r, active.Opaque.g, active.Opaque.b, meta.BaseColorAlpha));
                    if (meta.HasSpecularColor)
                        mat.SetColor("_SpecularColor", active.Opaque);
                    if (meta.HasMetallic)
                        mat.SetFloat("_Metallic", metallic);
                    if (meta.HasSmoothness)
                        mat.SetFloat("_Smoothness", smoothness);
                    if (meta.HasGlossiness)
                        mat.SetFloat("_Glossiness", smoothness);
                }
                return;
            }
            if (meta.HasMetallic)
            {
                mat.SetFloat("_Metallic", metallic);
            }
            if (meta.HasSmoothness)
            {
                mat.SetFloat("_Smoothness", smoothness);
            }
            if (meta.HasGlossiness)
            {
                mat.SetFloat("_Glossiness", smoothness);
            }
            if (meta.HasSpecularColor)
            {
                mat.SetColor("_SpecularColor", active.Opaque);
            }
            if (meta.HasBaseColor)
            {
                mat.SetColor("_BaseColor", new Color(active.Opaque.r, active.Opaque.g, active.Opaque.b, meta.BaseColorAlpha));
            }
            if (meta.HasColor)
            {
                mat.SetColor("_Color", new Color(active.Opaque.r, active.Opaque.g, active.Opaque.b, meta.MainColorAlpha));
            }
            if (meta.HasOutlineColor)
            {
                mat.SetColor("_OutlineColor", new Color(active.Opaque.r * 1.6f, active.Opaque.g * 1.6f, active.Opaque.b * 1.6f, meta.OutlineColorAlpha));
            }
            if (meta.HasEmissionColor && (meta.Category == PlatingCategory.SolafiteOre || meta.IsExplicitSpecialOther))
            {
                Color glowColor = new Color(
                active.Crack.r * crackGlow * 0.55f,
                active.Crack.g * crackGlow * 0.55f,
                active.Crack.b * crackGlow * 0.55f,
                meta.OriginalEmissionColor.a
                );
                mat.SetColor("_EmissionColor", glowColor);
            }
        }
        private void ApplyAllCategoryColors(bool forceReapply)
        {
            try
            {
                var materials = Resources.FindObjectsOfTypeAll<Material>();
                if (materials == null) return;
                bool debugLog = DebugLogMaterialsSetting != null && DebugLogMaterialsSetting.Value;
                for (int i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    if (mat == null) continue;
                    int id = mat.GetInstanceID();
                    bool isCached = _materialCache.TryGetValue(id, out MaterialMeta? meta);
                    if (isCached && !forceReapply) continue;
                    if (!isCached)
                    {
                        if (!IsPlatingMaterial(mat)) continue;
                        PlatingCategory cat = ClassifyGlobalMaterial(mat, out bool isSpecialOther);
                        meta = InspectMaterial(mat, cat, isSpecialOther);
                        _materialCache[id] = meta;
                        if (debugLog)
                        {
                            RLog.Msg($"[SilverSolafite] Material '{mat.name}' -> {cat} (SpecialOther={isSpecialOther})");
                        }
                    }
                    if (meta == null) continue;
                    ColorPresetData active = GetColorsForCategory(meta.Category);
                    ApplyPresetToMaterial(mat, meta, active);
                }
            }
            catch (Exception ex)
            {
                RLog.Error($"[SilverSolafite] Material error: {ex.Message}");
            }
        }
    }
    public static class SilverBuffHelpers
    {
        private static readonly Dictionary<int, bool> _platingCache = new Dictionary<int, bool>();
        public static void ClearCache()
        {
            _platingCache.Clear();
        }
        public static void BoostRangedDamagePostfix(MonoBehaviour __instance, ref float __result)
        {
            if (__instance == null) return;
            if (HasSilverPlating(__instance.gameObject))
            {
                __result *= SilverSolafiteMod.SilverPowerMultiplier;
            }
        }
        public static void ReduceDamagePrefix(ref float __0)
        {
            float mult = Mathf.Max(0.1f, SilverSolafiteMod.SilverPowerMultiplier);
            __0 /= mult;
        }
        public static void ReduceStructureDamagePrefix(MonoBehaviour __instance, ref float __0)
        {
            if (__instance == null) return;
            if (HasSilverPlating(__instance.gameObject))
            {
                float mult = Mathf.Max(0.1f, SilverSolafiteMod.SilverPowerMultiplier);
                __0 /= mult;
            }
        }
        public static bool HasSilverPlating(GameObject obj)
        {
            if (obj == null) return false;
            int id = obj.GetInstanceID();
            if (_platingCache.TryGetValue(id, out bool cached))
            {
                return cached;
            }
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (r == null) continue;
                var mats = r.sharedMaterials;
                for (int j = 0; j < mats.Length; j++)
                {
                    var mat = mats[j];
                    if (mat == null) continue;
                    string n = mat.name.ToLowerInvariant();
                    if (n.Contains("gold") || n.Contains("solafite") || n.Contains("plated") || n.Contains("_ss_"))
                    {
                        _platingCache[id] = true;
                        return true;
                    }
                }
            }
            _platingCache[id] = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.GetDamage))]
    public static class SilverMeleeBuffPatch
    {
        public static void Postfix(MeleeWeapon __instance, ref float __result)
        {
            if (__instance == null) return;
            if (SilverBuffHelpers.HasSilverPlating(__instance.gameObject))
            {
                __result *= SilverSolafiteMod.SilverPowerMultiplier;
            }
        }
    }
}