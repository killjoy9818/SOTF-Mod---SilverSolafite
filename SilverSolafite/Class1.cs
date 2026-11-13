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

namespace SilverSolafite
{
    public class SilverSolafiteMod : SonsMod
    {
        public static ConfigCategory? SettingsCategory;
        public static ConfigEntry<float>? PowerMultiplierSetting;
        public static ConfigEntry<int>? ColorPresetSetting;
        public static ConfigEntry<string>? ActivePresetDisplaySetting;
        public static ConfigEntry<bool>? UseCustomRgbSetting;
        public static ConfigEntry<float>? CustomRedSetting;
        public static ConfigEntry<float>? CustomGreenSetting;
        public static ConfigEntry<float>? CustomBlueSetting;

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
            // 1: Original Gold (Restores cached vanilla values)
            new ColorPresetData
            {
                Name = "ORIGINAL GOLD",
                Albedo = new Color(1.00f, 0.886f, 0.608f, 0.0f),
                Opaque = new Color(1.00f, 0.886f, 0.608f, 1.0f),
                Surface = new Color(0.532f, 0.403f, 0.041f, 1.0f),
                Crack = new Color(0.491f, 0.378f, 0.123f, 1.0f)
            },
            // 2: Platinum
            new ColorPresetData
            {
                Name = "PLATINUM",
                Albedo = new Color(0.93f, 0.95f, 0.98f, 0.0f),
                Opaque = new Color(0.93f, 0.95f, 0.98f, 1.0f),
                Surface = new Color(0.70f, 0.74f, 0.80f, 1.0f),
                Crack = new Color(0.85f, 0.90f, 0.96f, 1.0f)
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

        private class OriginalMaterialColors
        {
            public bool HasGoldAlbedo;
            public Color GoldAlbedo;
            public bool HasBaseColor;
            public Color BaseColor;
            public bool HasCrackColor;
            public Color CrackColor;
            public bool HasSpecularColor;
            public Color SpecularColor;
            public bool HasColor;
            public Color MainColor;
            public bool HasOutlineColor;
            public Color OutlineColor;
        }

        private readonly Dictionary<int, OriginalMaterialColors> _originalCache = new Dictionary<int, OriginalMaterialColors>();
        private float _nextScanTime = 0f;
        private string _lastAppliedSignature = "";

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
            catch (System.Exception ex)
            {
                RLog.Warning($"[SilverSolafite] Settings UI notice: {ex.Message}");
            }

            SdkEvents.OnGameActivated.Subscribe(OnGameActivated);
            GlobalEvents.OnUpdate.Subscribe(OnUpdate);

            HarmonyInstance.PatchAll();
            ApplyDynamicBuffPatches();

            RLog.Msg($"[SilverSolafite] v7 Loaded! {SilverPowerMultiplier}x Power Buffs + 8 Presets + Custom RGB active.");
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

            ColorPresetSetting = SettingsCategory.CreateEntry(
                "ColorPresetIndex",
                0,
                "Color Preset (0-8)",
                "0=Silver, 1=OriginalGold, 2=Platinum, 3=CobaltBlue, 4=Obsidian, 5=RoseGold, 6=Emerald, 7=Crimson, 8=CustomRGB"
            );
            ColorPresetSetting.SetRange(0, 8);

            ActivePresetDisplaySetting = SettingsCategory.CreateEntry(
                "ActiveColorStatus",
                "SILVER (R:0.86, G:0.89, B:0.93)",
                "Active Color Preview",
                "Displays the currently selected preset name and RGB values."
            );

            UseCustomRgbSetting = SettingsCategory.CreateEntry(
                "UseCustomRGB",
                false,
                "Enable Custom RGB Sliders",
                "Turn ON (or set Preset to 8) to use the Custom Red, Green, and Blue sliders below."
            );

            CustomRedSetting = SettingsCategory.CreateEntry(
                "CustomRed",
                0.86f,
                "Custom Red (R)",
                "Red channel intensity (0.00 to 1.00)."
            );
            CustomRedSetting.SetRange(0.0f, 1.0f);

            CustomGreenSetting = SettingsCategory.CreateEntry(
                "CustomGreen",
                0.89f,
                "Custom Green (G)",
                "Green channel intensity (0.00 to 1.00)."
            );
            CustomGreenSetting.SetRange(0.0f, 1.0f);

            CustomBlueSetting = SettingsCategory.CreateEntry(
                "CustomBlue",
                0.93f,
                "Custom Blue (B)",
                "Blue channel intensity (0.00 to 1.00)."
            );
            CustomBlueSetting.SetRange(0.0f, 1.0f);
        }

        private string BuildCurrentSignature()
        {
            int preset = ColorPresetSetting != null ? Mathf.Clamp(ColorPresetSetting.Value, 0, 8) : 0;
            bool custom = (UseCustomRgbSetting != null && UseCustomRgbSetting.Value) || preset == 8;
            float r = CustomRedSetting != null ? (float)Math.Round(CustomRedSetting.Value, 2) : 0.86f;
            float g = CustomGreenSetting != null ? (float)Math.Round(CustomGreenSetting.Value, 2) : 0.89f;
            float b = CustomBlueSetting != null ? (float)Math.Round(CustomBlueSetting.Value, 2) : 0.93f;
            return $"{preset}_{custom}_{r}_{g}_{b}";
        }

        private ColorPresetData GetActiveColors()
        {
            int idx = ColorPresetSetting != null ? Mathf.Clamp(ColorPresetSetting.Value, 0, 8) : 0;
            bool useCustom = (UseCustomRgbSetting != null && UseCustomRgbSetting.Value) || idx == 8;

            if (useCustom)
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

        private void UpdateStatusDisplay(ColorPresetData active)
        {
            if (ActivePresetDisplaySetting == null) return;
            string formatted = $"{active.Name} (R:{active.Opaque.r:F2}, G:{active.Opaque.g:F2}, B:{active.Opaque.b:F2})";
            if (ActivePresetDisplaySetting.Value != formatted)
            {
                ActivePresetDisplaySetting.Value = formatted;
            }
        }

        private void ApplyDynamicBuffPatches()
        {
            try
            {
                System.Type? rangedType = AccessTools.TypeByName("Sons.Weapon.RangedWeapon");
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

                System.Type? armorType = AccessTools.TypeByName("Sons.Armor.PlayerArmorSystem");
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

                System.Type? structType = AccessTools.TypeByName("Sons.Construction.StructureDestruction")
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
            catch (System.Exception ex)
            {
                RLog.Warning($"[SilverSolafite] Dynamic patch notice: {ex.Message}");
            }
        }

        private void OnGameActivated()
        {
            _originalCache.Clear();
            _lastAppliedSignature = "";
            ApplySelectedPresetToAllMaterials(forceReapply: true);
        }

        private void OnUpdate()
        {
            string currentSig = BuildCurrentSignature();
            bool settingsChanged = currentSig != _lastAppliedSignature;

            if (settingsChanged)
            {
                _lastAppliedSignature = currentSig;
                UpdateStatusDisplay(GetActiveColors());
                ApplySelectedPresetToAllMaterials(forceReapply: true);
                return;
            }

            if (Time.realtimeSinceStartup < _nextScanTime) return;
            _nextScanTime = Time.realtimeSinceStartup + 3.0f;

            if (Time.timeSinceLevelLoad < 2.0f) return;
            ApplySelectedPresetToAllMaterials(forceReapply: false);
        }

        private void ApplySelectedPresetToAllMaterials(bool forceReapply)
        {
            try
            {
                var materials = Resources.FindObjectsOfTypeAll<Material>();
                if (materials == null) return;

                int presetIdx = ColorPresetSetting != null ? Mathf.Clamp(ColorPresetSetting.Value, 0, 8) : 0;
                bool useCustom = (UseCustomRgbSetting != null && UseCustomRgbSetting.Value) || presetIdx == 8;
                bool restoreOriginal = !useCustom && presetIdx == 1;
                ColorPresetData active = GetActiveColors();

                foreach (var mat in materials)
                {
                    if (mat == null || mat.shader == null) continue;

                    int id = mat.GetInstanceID();
                    bool isCached = _originalCache.TryGetValue(id, out OriginalMaterialColors? orig);

                    if (!isCached)
                    {
                        string mName = mat.name.ToLower();
                        string sName = mat.shader.name.ToLower();

                        bool hasGoldAlbedo = mat.HasProperty("_GoldAlbedo");
                        bool isPlatingSurface = sName.Contains("platingsurface") || mName.Contains("solafitesurface");
                        bool isGoldNamed = mName.Contains("gold") || mName.Contains("solafite") ||
                                           mName.Contains("plated") || mName.Contains("plating") ||
                                           mName.Contains("artifact") || mName.Contains("brass");

                        if (!hasGoldAlbedo && !isPlatingSurface && !isGoldNamed) continue;

                        orig = new OriginalMaterialColors();
                        if (hasGoldAlbedo)
                        {
                            orig.HasGoldAlbedo = true;
                            orig.GoldAlbedo = mat.GetColor("_GoldAlbedo");
                        }
                        if (isPlatingSurface)
                        {
                            if (mat.HasProperty("_BaseColor"))
                            {
                                orig.HasBaseColor = true;
                                orig.BaseColor = mat.GetColor("_BaseColor");
                            }
                            if (mat.HasProperty("_CrackColor"))
                            {
                                orig.HasCrackColor = true;
                                orig.CrackColor = mat.GetColor("_CrackColor");
                            }
                        }
                        if (isGoldNamed)
                        {
                            if (mat.HasProperty("_SpecularColor"))
                            {
                                orig.HasSpecularColor = true;
                                orig.SpecularColor = mat.GetColor("_SpecularColor");
                            }
                            if (mat.HasProperty("_BaseColor"))
                            {
                                orig.HasBaseColor = true;
                                orig.BaseColor = mat.GetColor("_BaseColor");
                            }
                            if (mat.HasProperty("_Color"))
                            {
                                orig.HasColor = true;
                                orig.MainColor = mat.GetColor("_Color");
                            }
                            if (mat.HasProperty("_OutlineColor"))
                            {
                                orig.HasOutlineColor = true;
                                orig.OutlineColor = mat.GetColor("_OutlineColor");
                            }
                        }

                        _originalCache[id] = orig;
                    }
                    else if (!forceReapply)
                    {
                        continue;
                    }

                    if (orig == null) continue;

                    if (restoreOriginal)
                    {
                        if (orig.HasGoldAlbedo) mat.SetColor("_GoldAlbedo", orig.GoldAlbedo);
                        if (orig.HasBaseColor) mat.SetColor("_BaseColor", orig.BaseColor);
                        if (orig.HasCrackColor) mat.SetColor("_CrackColor", orig.CrackColor);
                        if (orig.HasSpecularColor) mat.SetColor("_SpecularColor", orig.SpecularColor);
                        if (orig.HasColor) mat.SetColor("_Color", orig.MainColor);
                        if (orig.HasOutlineColor) mat.SetColor("_OutlineColor", orig.OutlineColor);
                    }
                    else
                    {
                        if (orig.HasGoldAlbedo)
                            mat.SetColor("_GoldAlbedo", new Color(active.Albedo.r, active.Albedo.g, active.Albedo.b, orig.GoldAlbedo.a));

                        if (orig.HasCrackColor)
                        {
                            if (orig.HasBaseColor) mat.SetColor("_BaseColor", active.Surface);
                            mat.SetColor("_CrackColor", active.Crack);
                        }
                        else
                        {
                            if (orig.HasSpecularColor && orig.SpecularColor.r > orig.SpecularColor.b + 0.15f)
                                mat.SetColor("_SpecularColor", active.Opaque);

                            if (orig.HasBaseColor && orig.BaseColor.r > orig.BaseColor.b + 0.12f)
                                mat.SetColor("_BaseColor", new Color(active.Opaque.r, active.Opaque.g, active.Opaque.b, orig.BaseColor.a));

                            if (orig.HasColor && orig.MainColor.r > orig.MainColor.b + 0.12f)
                                mat.SetColor("_Color", new Color(active.Opaque.r, active.Opaque.g, orig.MainColor.b, orig.MainColor.a));

                            if (orig.HasOutlineColor && orig.OutlineColor.r > orig.OutlineColor.b + 0.15f)
                                mat.SetColor("_OutlineColor", new Color(active.Opaque.r * 1.6f, active.Opaque.g * 1.6f, active.Opaque.b * 1.6f, orig.OutlineColor.a));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                RLog.Error($"[SilverSolafite] Material error: {ex.Message}");
            }
        }
    }

    public static class SilverBuffHelpers
    {
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
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                if (r == null) continue;
                foreach (var mat in r.sharedMaterials)
                {
                    if (mat == null) continue;
                    string n = mat.name.ToLower();
                    if (n.Contains("gold") || n.Contains("solafite") || n.Contains("plated"))
                        return true;
                }
            }
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
