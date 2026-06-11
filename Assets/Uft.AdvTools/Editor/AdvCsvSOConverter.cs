#nullable enable

using System.Collections.Generic;
using System.IO;
using Uft.AdvTools.Entities;
using Uft.AdvTools.Loader;
using Uft.UnityUtils;
using UnityEditor;
using UnityEngine;

namespace Uft.AdvTools.Editor
{
    public class AdvCsvSOConverter : EditorWindow
    {
        const string TITLE = "Csv SO Converter";

        [MenuItem("Tools/Uft.AdvTools/" + TITLE, priority = 21062000 + 1, secondaryPriority = 1)]
        static void Open()
        {
            var w = GetWindow<AdvCsvSOConverter>(TITLE);
            w.minSize = new Vector2(600, w.minSize.y);
            w.position = new Rect(w.position.x, w.position.y, Mathf.Max(w.position.width, 600), w.position.height);
        }

        static readonly DevLogWithTag DevLog = new($"[{nameof(AdvCsvSOConverter)}]");

        List<TextAsset?> _scenarioCsvs = new();
        TextAsset? _characterCsv;
        TextAsset? _textureCsv;
        TextAsset? _soundCsv;
        TextAsset? _paramCsv;
        string _foundFolder = "";
        string _inputFolder = "";
        string _outputFolder = "Assets";
        Vector2 _scenarioScrollPos;
        readonly AssetLoadProxy _proxy = EditorAssetLoadProxy.Create();

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find CSV Folder", GUILayout.ExpandWidth(false))) this.FindFolder();
            EditorGUILayout.LabelField("Search order: StreamingAssets → Resources → AssetBundle → AssetBundles", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Found Folder", this._foundFolder);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            this._inputFolder = EditorGUILayout.TextField("Input Folder", this._inputFolder);
            {
                EditorGUILayout.BeginHorizontal();
                this._outputFolder = EditorGUILayout.TextField("Output Folder", this._outputFolder);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    var selected = EditorUtility.OpenFolderPanel("Output Folder", this._outputFolder, "");
                    if (!string.IsNullOrEmpty(selected))
                        this._outputFolder = "Assets" + selected.Replace(Application.dataPath, "");
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("CSV Files", EditorStyles.boldLabel);

            // Character/Texture/Sound/Param
            {
                EditorGUILayout.BeginHorizontal();
                this._characterCsv = (TextAsset?)EditorGUILayout.ObjectField("Character CSV", this._characterCsv, typeof(TextAsset), false);
                if (GUILayout.Button("Convert", GUILayout.Width(70))) this.ConvertCharacter();
                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();
                this._paramCsv = (TextAsset?)EditorGUILayout.ObjectField("Param CSV", this._paramCsv, typeof(TextAsset), false);
                if (GUILayout.Button("Convert", GUILayout.Width(70))) this.ConvertParam();
                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();
                this._soundCsv = (TextAsset?)EditorGUILayout.ObjectField("Sound CSV", this._soundCsv, typeof(TextAsset), false);
                if (GUILayout.Button("Convert", GUILayout.Width(70))) this.ConvertSound();
                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();
                this._textureCsv = (TextAsset?)EditorGUILayout.ObjectField("Texture CSV", this._textureCsv, typeof(TextAsset), false);
                if (GUILayout.Button("Convert", GUILayout.Width(70))) this.ConvertTexture();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            // Scenarios
            {
                EditorGUILayout.LabelField($"Scenario CSVs ({this._scenarioCsvs.Count})", EditorStyles.boldLabel);
                var lineH = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var scrollViewH = Mathf.Clamp(this._scenarioCsvs.Count * lineH, lineH, lineH * 5);
                this._scenarioScrollPos = EditorGUILayout.BeginScrollView(this._scenarioScrollPos, GUILayout.Height(scrollViewH));
                for (int i = 0; i < this._scenarioCsvs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    this._scenarioCsvs[i] = (TextAsset?)EditorGUILayout.ObjectField(this._scenarioCsvs[i], typeof(TextAsset), false);
                    if (GUILayout.Button("-", GUILayout.Width(22)))
                    {
                        this._scenarioCsvs.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+ Add", GUILayout.Width(70))) this._scenarioCsvs.Add(null);
                if (GUILayout.Button("Convert", GUILayout.Width(70))) this.ConvertScenario();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Convert All")) this.ConvertAll();
        }

        /// <summary>csvの親フォルダを探す。StreamingAssets → Resources → AssetBundle → AssetBundles の順にフォールバック</summary>
        void FindFolder()
        {
            static string? FindBaseFolderIn(string searchRoot, string[] names, string? requiredAncestorFolder)
            {
                if (!AssetDatabase.IsValidFolder(searchRoot)) return null;
                foreach (var name in names)
                {
                    var guids = AssetDatabase.FindAssets($"{name} t:TextAsset", new[] { searchRoot });
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (!string.Equals(Path.GetFileNameWithoutExtension(path), name, System.StringComparison.OrdinalIgnoreCase))
                            continue;
                        if (requiredAncestorFolder != null && !path.Contains($"/{requiredAncestorFolder}/"))
                            continue;
                        return Path.GetDirectoryName(path)?.Replace('\\', '/');
                    }
                }
                return null;
            }
            static TextAsset? LoadCsvFromFolder(string folder, string name) => AssetDatabase.LoadAssetAtPath<TextAsset>($"{folder}/{name}.csv");
            static List<TextAsset?> FindScenarioCsvs(string baseFolder)
            {
                var result = new List<TextAsset?>();
                foreach (var dirName in new[] { "Scenario", "Scenarios" })
                {
                    var scenarioFsPath = Application.dataPath + $"/{baseFolder["Assets".Length..]}/{dirName}";
                    if (!Directory.Exists(scenarioFsPath)) continue;

                    foreach (var file in Directory.GetFiles(scenarioFsPath, "*.csv", SearchOption.AllDirectories))
                    {
                        var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                        if (asset != null) result.Add(asset);
                    }
                    break;
                }
                return result;
            }

            string[] names = { "Character", "Param", "Sound", "Texture" };
            var baseFolder = FindBaseFolderIn("Assets/StreamingAssets", names, null)
                ?? FindBaseFolderIn("Assets", names, "Resources")
                ?? FindBaseFolderIn("Assets", names, "AssetBundle")
                ?? FindBaseFolderIn("Assets", names, "AssetBundles");
            if (baseFolder == null)
            {
                this._foundFolder = "";
                return;
            }
            this._foundFolder = baseFolder;
            this._inputFolder = baseFolder;
            this._outputFolder = baseFolder;

            this._characterCsv = LoadCsvFromFolder(baseFolder, "Character");
            this._paramCsv = LoadCsvFromFolder(baseFolder, "Param");
            this._soundCsv = LoadCsvFromFolder(baseFolder, "Sound");
            this._textureCsv = LoadCsvFromFolder(baseFolder, "Texture");
            this._scenarioCsvs = FindScenarioCsvs(baseFolder);

            DevLog.Log($"FindFolder done. folder={baseFolder}, scenario={this._scenarioCsvs.Count}");
        }

        void ConvertCharacter()
        {
            static CharacterTableSO.DetailData ToDetailData(CharacterDetail d) =>
                new()
                {
                    pattern = d.Pattern,
                    x = d.OffsetX,
                    y = d.OffsetY,
                    z = d.OffsetZ,
                    pivot = d.Pivot,
                    scale = d.Scale,
                    sprite = d.Sprite,
                    animationState = d.AnimatorState ?? "",
                };

            if (this._characterCsv == null) return;
            var dict = new CharacterCsvLoader(this._proxy).Load(this._characterCsv.text, this._inputFolder + "/");
            var soName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this._characterCsv));
            var so = this.LoadOrCreate<CharacterTableSO>(this._outputFolder + $"/{soName}_so.asset");
            so.characters = new List<CharacterTableSO.CharacterData>(dict.Count);
            foreach (var (_, character) in dict)
            {
                var details = new List<CharacterTableSO.DetailData> { ToDetailData(character.DefaultDetail) };
                foreach (var (pattern, detail) in character.CharacterDetailDictionary)
                {
                    if (pattern == character.DefaultDetail.Pattern) continue;
                    details.Add(ToDetailData(detail));
                }
                so.characters.Add(new CharacterTableSO.CharacterData
                {
                    characterName = character.CharacterName,
                    nameText = character.NameText,
                    fileTypeValue = character.FileType.value,
                    prefab = character.Prefab,
                    details = details,
                });
            }
            EditorUtility.SetDirty(so);
            DevLog.Log($"Character converted. count={so.characters.Count}");
        }

        void ConvertParam()
        {
            if (this._paramCsv == null) return;
            var dict = new ParamCsvLoader().Load(this._paramCsv.text);
            var soName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this._paramCsv));
            var so = this.LoadOrCreate<ParamTableSO>(this._outputFolder + $"/{soName}_so.asset");
            so.entries = new List<ParamTableSO.ParamData>(dict.Count);
            foreach (var (label, param) in dict)
                so.entries.Add(new ParamTableSO.ParamData { label = label, value = param.Value });
            EditorUtility.SetDirty(so);
            DevLog.Log($"Param converted. count={so.entries.Count}");
        }

        void ConvertSound()
        {
            if (this._soundCsv == null) return;
            var resourcesFolderPathPart = this._inputFolder + "/";
            var bgmRoot = resourcesFolderPathPart + "Sound/BGM/";
            var seRoot = resourcesFolderPathPart + "Sound/SE/";
            var voiceRoot = resourcesFolderPathPart + "Sound/Voice/";
            var sounds = new SoundCsvLoader(this._proxy).Load(this._soundCsv.text, resourcesFolderPathPart);
            var soName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this._soundCsv));
            var so = this.LoadOrCreate<SoundTableSO>(this._outputFolder + $"/{soName}_so.asset");
            so.entries = new List<SoundTableSO.SoundData>(sounds._bgmPathDict.Count + sounds._sePathDict.Count + sounds._voicePathDict.Count);
            foreach (var (label, path) in sounds._bgmPathDict)
                so.entries.Add(new SoundTableSO.SoundData { label = label, type = SoundCsvLoader.Bgm, fileName = RelativeFileName(path, bgmRoot) });
            foreach (var (label, path) in sounds._sePathDict)
                so.entries.Add(new SoundTableSO.SoundData { label = label, type = SoundCsvLoader.Se, fileName = RelativeFileName(path, seRoot) });
            foreach (var (label, path) in sounds._voicePathDict)
                so.entries.Add(new SoundTableSO.SoundData { label = label, type = SoundCsvLoader.Voice, fileName = RelativeFileName(path, voiceRoot) });
            EditorUtility.SetDirty(so);
            DevLog.Log($"Sound converted. count={so.entries.Count}");
        }

        void ConvertTexture()
        {
            if (this._textureCsv == null) return;
            var resourcesFolderPathPart = this._inputFolder + "/";
            var bgRoot = resourcesFolderPathPart + "Texture/BG/";
            var spriteRoot = resourcesFolderPathPart + "Texture/Sprite/";

            TextureTableSO.TextureData ToTextureData(TextureRow r)
            {
                var typeRoot = r.Type.ToLower() == TextureCsvLoader.Bg ? bgRoot : spriteRoot;
                var relFileName = RelativeFileName(r.AssetPath, typeRoot);
                return new()
                {
                    label = r.Label,
                    type = r.Type,
                    x = r.OffsetX,
                    y = r.OffsetY,
                    pivot = r.Pivot,
                    scale = r.Scale,
                    fileName = relFileName,
                    fileType = r.FileType.ToString(),
                };
            }

            var dicts = new TextureCsvLoader(this._proxy).Load(this._textureCsv.text, resourcesFolderPathPart);
            var soName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this._textureCsv));
            var so = this.LoadOrCreate<TextureTableSO>(this._outputFolder + $"/{soName}_so.asset");
            so.entries = new List<TextureTableSO.TextureData>(dicts._bgDict.Count + dicts._spriteDict.Count);
            foreach (var (_, row) in dicts._bgDict)
                so.entries.Add(ToTextureData(row));
            foreach (var (_, row) in dicts._spriteDict)
                so.entries.Add(ToTextureData(row));
            EditorUtility.SetDirty(so);
            DevLog.Log($"Texture converted. count={so.entries.Count}");
        }

        void ConvertScenario()
        {
            static void EnsureFolder(string folderPath)
            {
                if (AssetDatabase.IsValidFolder(folderPath)) return;
                var parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/') ?? "Assets";
                EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, Path.GetFileName(folderPath));
            }

            var valid = this._scenarioCsvs.FindAll(c => c != null);
            if (valid.Count == 0) return;
            foreach (var csv in valid)
            {
                var dtoList = ScenarioCsvDto.Load(csv!.text);
                var csvPath = AssetDatabase.GetAssetPath(csv);
                var soName = Path.GetFileNameWithoutExtension(csvPath);
                var csvDir = Path.GetDirectoryName(csvPath)?.Replace('\\', '/') ?? "";
                var relDir = csvDir.StartsWith(this._inputFolder) ? csvDir[this._inputFolder.Length..].TrimStart('/') : "";
                var outDir = string.IsNullOrEmpty(relDir) ? this._outputFolder : this._outputFolder + "/" + relDir;
                EnsureFolder(outDir);
                var so = this.LoadOrCreate<ScenarioTableSO>(outDir + $"/{soName}_so.asset");
                so.rows = new List<ScenarioTableSO.RowData>(dtoList.Count);
                foreach (var dto in dtoList)
                {
                    if (dto.IsAllNullOrWhiteSpace()) continue;
                    so.rows.Add(new ScenarioTableSO.RowData
                    {
                        command = dto.Command ?? "",
                        arg1 = dto.Arg1 ?? "",
                        arg2 = dto.Arg2 ?? "",
                        arg3 = dto.Arg3 ?? "",
                        arg4 = dto.Arg4 ?? "",
                        arg5 = dto.Arg5 ?? "",
                        arg6 = dto.Arg6 ?? "",
                        waitType = dto.WaitType ?? "",
                        text = dto.Text ?? "",
                        pageCtrl = dto.PageCtrl ?? "",
                        voice = dto.Voice ?? "",
                        windowType = dto.WindowType ?? "",
                    });
                }
                EditorUtility.SetDirty(so);
                DevLog.Log($"Scenario converted. name={soName}, rows={so.rows.Count}");
            }
        }

        void ConvertAll()
        {
            this.ConvertCharacter();
            this.ConvertParam();
            this.ConvertSound();
            this.ConvertTexture();
            this.ConvertScenario();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DevLog.Log("Convert All done.");
        }

        static string RelativeFileName(string fullPath, string root) =>
            fullPath.StartsWith(root, System.StringComparison.Ordinal) ? fullPath[root.Length..] : Path.GetFileName(fullPath);

        T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            var so = CreateInstance<T>();
            AssetDatabase.CreateAsset(so, path);
            return so;
        }
    }
}
