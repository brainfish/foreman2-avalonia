using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Foreman.Properties {
    internal sealed partial class Settings {
        private static readonly string StorePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Foreman", "user-settings.json");

        private readonly Dictionary<string, JsonNode?> _values = new(StringComparer.Ordinal);
        private static readonly HashSet<string> BoolKeys = new(StringComparer.Ordinal) {
            "AltGridlines","ShowHidden","IgnoreAssemblerStatus","DynamicLineWidth","RecipeNameOnlyFilter",
            "ShowRecipeToolTip","ShowUnavailable","LockedRecipeEditorPosition","UseRecipeBWfilters",
            "ShowWarningArrows","ShowErrorArrows","AbbreviateSciPacks","RoundAssemblerCount",
            "EnableExtraProductivityForNonMiners","ShowDisconnectedArrows","FlagOUSuppliedNodes",
            "ShowOUSuppliedArrows","IconsOnlyView","SimplePassthroughNodes","ArrowsOnLinks",
            "SmartNodeDirection","FlagDarkMode","UpgradeRequired"
        };
        private static readonly Settings defaultInstance = new();
        public static Settings Default => defaultInstance;

        private Settings() {
            LoadDefaults();
            Load();
        }

        public object? this[string key] {
            get => Get(key);
            set { Set(key, value); }
        }

        private object? Get(string key) {
            if (!_values.TryGetValue(key, out JsonNode? n) || n is null)
                return DefaultFor(key);
            return n switch {
                JsonValue v when v.TryGetValue(out bool b) => b,
                JsonValue v when BoolKeys.Contains(key) && v.TryGetValue(out int i) => i != 0,
                JsonValue v when v.TryGetValue(out int i) => i,
                JsonValue v when v.TryGetValue(out string? s) => s ?? "",
                _ => n.ToString()
            };
        }

        private void Set(string key, object? value) {
            _values[key] = value switch {
                bool b => JsonValue.Create(b),
                int i => JsonValue.Create(i),
                string s => JsonValue.Create(s),
                null => JsonValue.Create(""),
                _ => JsonValue.Create(Convert.ToString(value, CultureInfo.InvariantCulture))
            };
        }

        public void Save() {
            Directory.CreateDirectory(Path.GetDirectoryName(StorePath)!);
            var obj = new JsonObject();
            foreach (var kv in _values)
                obj[kv.Key] = kv.Value?.DeepClone();
            File.WriteAllText(StorePath, obj.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }

        public void Upgrade() { }

        private void Load() {
            try {
                if (!File.Exists(StorePath)) return;
                if (JsonNode.Parse(File.ReadAllText(StorePath)) is not JsonObject obj) return;
                foreach (var kv in obj)
                    _values[kv.Key] = kv.Value?.DeepClone();
            } catch {
                // keep defaults
            }
        }

        private static object DefaultFor(string key) {
            if (BoolKeys.Contains(key)) {
                return key is "ShowRecipeToolTip" or "UseRecipeBWfilters" or "ShowWarningArrows" or "ShowErrorArrows"
                    or "AbbreviateSciPacks" or "SmartNodeDirection" or "UpgradeRequired";
            }
            return key switch {
            "NodeCountForSimpleView" => 300,
            "IconsSize" => 24,
            "AnnotTextFontFamily" => "Segoe UI",
            "AnnotTextFontSize" => "14",
            "AnnotTextFontStyle" => 1,
            "AnnotTextColorARGB" => -16777216,
            "AnnotTextAlign" => 1,
            "AnnotShapeFillColorARGB" => 5278975,
            "AnnotShapeBorderColorARGB" => -600016676,
            "AnnotShapeBorderWidth" => 2,
            _ => key.Contains("ARGB", StringComparison.Ordinal) ? 0 : key.EndsWith("Name", StringComparison.Ordinal) ? "" : (object)0
            };
        }

        private void LoadDefaults() {
            string[] keys = [
                "CurrentPresetName","DefaultModuleOption","MinorGridlines","MajorGridlines","AltGridlines","ShowHidden",
                "IgnoreAssemblerStatus","DynamicLineWidth","RecipeNameOnlyFilter","LevelOfDetail","DefaultAssemblerOption",
                "DefaultRateUnit","LastSaveFileLocation","ShowRecipeToolTip","ShowUnavailable","LockedRecipeEditorPosition",
                "NodeCountForSimpleView","UseRecipeBWfilters","ShowWarningArrows","ShowErrorArrows","AbbreviateSciPacks",
                "RoundAssemblerCount","EnableExtraProductivityForNonMiners","ShowDisconnectedArrows","DefaultNodeDirection",
                "FlagOUSuppliedNodes","ShowOUSuppliedArrows","IconsOnlyView","IconsSize","SimplePassthroughNodes",
                "ArrowsOnLinks","SmartNodeDirection","FlagDarkMode","UpgradeRequired","AnnotTextFontFamily","AnnotTextFontSize",
                "AnnotTextFontStyle","AnnotTextColorARGB","AnnotTextBackColorARGB","AnnotTextAlign","AnnotShapeType",
                "AnnotShapeFillColorARGB","AnnotShapeBorderColorARGB","AnnotShapeBorderWidth"
            ];
            foreach (string k in keys)
                Set(k, DefaultFor(k));
        }
    }
}
