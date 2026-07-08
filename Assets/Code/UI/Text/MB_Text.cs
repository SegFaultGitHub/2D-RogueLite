using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Code.Utils;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;

namespace Code.UI.Text {
    public class MB_Text : MonoBehaviour {
        private class C_Character {
            public char Character { get; set; }
            public Dictionary<AC_Group, int> IndexInGroup { get; set; } = new();
            public bool IsTag { get; set; }
            public List<AC_Group> Groups { get; } = new();

            #region Data
            public float CurrentHeight { get; set; }
            public Color Color { get; set; }
            public string NoBreakGuid { get; set; }
            public string LineHeightGuid { get; set; }
            public float LineHeight { get; set; }
            #endregion

            public void AddGroup(AC_Group group) {
                if (this.Groups.Any(currentGroup => !group.CompatibleWith(currentGroup))) {
                    throw new Exception("Group not compatible with current group");
                }

                this.Groups.Add(group);
            }
        }

        private abstract class AC_Group {
            public int Size { get; set; }
            public int SizeWithoutSpaces { get; set; }
            public abstract int Priority { get; }

            public abstract bool CompatibleWith(AC_Group other);

            public abstract void TweenCharacter(MB_Text parent, C_Character character);
        }

        private class C_DefaultGroup : AC_Group {
            public override int Priority { get => 0; }

            public override bool CompatibleWith(AC_Group other) => true;
            public override void TweenCharacter(MB_Text parent, C_Character character) { }
        }

        private class C_VOffsetGroup : AC_Group {
            public override int Priority { get => 2; }

            private float Height { get; }
            private float Delay { get; }
            private float Offset { get; }
            private float Duration { get; }
            private bool Loop { get; }
            public float LoopDelay { get; set; }
            private bool Progressive { get; }
            private Color OriginalColor { get; set; }

            public C_VOffsetGroup(string options) {
                Match heightMatch = VOFFSET_HEIGHT_OPTION_REGEX.Match(options);
                Match delayMatch = VOFFSET_DELAY_OPTION_REGEX.Match(options);
                Match offsetMatch = VOFFSET_OFFSET_OPTION_REGEX.Match(options);
                Match durationMatch = VOFFSET_DURATION_OPTION_REGEX.Match(options);
                Match loopMatch = VOFFSET_LOOP_OPTION_REGEX.Match(options);
                Match loopDelayMatch = VOFFSET_LOOP_DELAY_OPTION_REGEX.Match(options);
                Match progressiveMatch = VOFFSET_PROGRESSIVE_OPTION_REGEX.Match(options);

                this.Height = heightMatch.Success
                    ? float.Parse(heightMatch.Groups["Value"].Value, CultureInfo.InvariantCulture)
                    : 0f;
                this.Delay = delayMatch.Success
                    ? float.Parse(delayMatch.Groups["Value"].Value, CultureInfo.InvariantCulture)
                    : 0f;
                this.Offset = offsetMatch.Success
                    ? float.Parse(offsetMatch.Groups["Value"].Value, CultureInfo.InvariantCulture)
                    : 0f;
                this.Duration = durationMatch.Success
                    ? float.Parse(durationMatch.Groups["Value"].Value, CultureInfo.InvariantCulture)
                    : 0f;
                this.Loop = loopMatch.Success && loopMatch.Groups["Value"].Value.ToLower() == "true";
                this.LoopDelay = loopDelayMatch.Success
                    ? float.Parse(loopDelayMatch.Groups["Value"].Value, CultureInfo.InvariantCulture)
                    : 0f;
                this.Progressive = progressiveMatch.Success && progressiveMatch.Groups["Value"].Value.ToLower() == "true";
            }

            public override bool CompatibleWith(AC_Group other) => other is not C_VOffsetGroup;

            private void CreateCharacterTween(MB_Text parent, C_Character character) {
                Sequence sequence = DOTween.Sequence();
                sequence.SetUpdate(parent.RealTime)
                    .SetDelay(this.Offset * character.IndexInGroup[this])
                    .AppendCallback(() => character.Color = this.OriginalColor)
                    .Append(
                        DOTween.To( //
                                () => character.CurrentHeight,
                                x => character.CurrentHeight = x,
                                this.Height,
                                this.Duration / 2
                            )
                            .SetEase(Ease.InOutSine)
                    )
                    .Append(
                        DOTween.To( //
                                () => character.CurrentHeight,
                                x => character.CurrentHeight = x,
                                0,
                                this.Duration / 2
                            )
                            .SetEase(Ease.InOutSine)
                    )
                    .OnComplete(() => {
                            character.CurrentHeight = 0;
                        }
                    );
                parent.Sequences.Add(sequence);
            }

            private void _CreateCoroutine(MB_Text parent, C_Character character) {
                this.CreateCharacterTween(parent, character);

                if (this.Loop) {
                    if (parent.RealTime) parent.InRealSeconds(this.LoopDelay, () => this._CreateCoroutine(parent, character));
                    else parent.InSeconds(this.LoopDelay, () => this._CreateCoroutine(parent, character));
                }
            }

            public override void TweenCharacter(MB_Text parent, C_Character character) {
                this.OriginalColor = character.Color;
                if (this.Progressive) character.Color = new Color(0, 0, 0, 0);
                if (parent.RealTime) parent.InRealSeconds(this.Delay, () => this._CreateCoroutine(parent, character));
                else parent.InSeconds(this.Delay, () => this._CreateCoroutine(parent, character));
            }
        }

        private class C_ColorGroup : AC_Group {
            public override int Priority { get => 1; }

            private Color Color { get; }

            public C_ColorGroup(string options) {
                Match colorMatch = COLOR_VALUE_OPTION_REGEX.Match(options);

                if (!colorMatch.Success) throw new Exception($"Invalid color options: {options}");

                string value = colorMatch.Groups["Value"].Value;

                if (ColorUtility.TryParseHtmlString(value, out Color myColor)) {
                    this.Color = myColor;
                } else {
                    throw new Exception($"Invalid color value: {value}");
                }
            }

            public override bool CompatibleWith(AC_Group other) => true;

            public override void TweenCharacter(MB_Text parent, C_Character character) {
                character.Color = this.Color;
            }
        }

        private class C_NoBreakGroup : AC_Group {
            public override int Priority { get => 1; }

            private string Guid { get; } = System.Guid.NewGuid().ToString();

            public override bool CompatibleWith(AC_Group other) => other is not C_NoBreakGroup;

            public override void TweenCharacter(MB_Text parent, C_Character character) {
                character.NoBreakGuid = this.Guid;
            }
        }

        private class C_LineHeightGroup : AC_Group {
            public override int Priority { get => 1; }

            private string Guid { get; } = System.Guid.NewGuid().ToString();
            private float Height { get; }

            public C_LineHeightGroup(string options) {
                Match heightMatch = LINE_HEIGHT_OPTION_REGEX.Match(options);

                if (!heightMatch.Success) throw new Exception($"Invalid line height options: {options}");

                this.Height = heightMatch.Success
                    ? float.Parse(heightMatch.Groups["Value"].Value, CultureInfo.InvariantCulture)
                    : 0f;
            }

            public override bool CompatibleWith(AC_Group other) => other is not C_LineHeightGroup;

            public override void TweenCharacter(MB_Text parent, C_Character character) {
                character.LineHeightGuid = this.Guid;
                character.LineHeight = this.Height;
            }
        }

        #region Members
        [Foldout("MB_Text", true)]
        [SerializeField] private protected bool m_RealTime;
        [SerializeField] private protected bool m_IsStatic;
        [SerializeField][ConditionalField(nameof(m_IsStatic))][TextArea] private protected string m_StaticText;


        [Separator("Read only")]
        [ReadOnly][SerializeField] private protected TMP_Text m_Text;
        #endregion

        #region Getters / Setters
        private bool RealTime { get => this.m_RealTime; }
        private bool IsStatic { get => this.m_IsStatic; }
        private string StaticText { get => this.m_StaticText; }

        protected TMP_Text Text { get => this.m_Text; private set => this.m_Text = value; }

        private List<C_Character> Characters { get; set; } = new();
        private List<Sequence> Sequences { get; } = new();
        #endregion

        #region Static / Readonly / Const
        private static Regex Tag(string tag) => new($"(?<TagOpen>{{(?<Tag>{tag})\\s?(?<Options>.*?)}})(?<Text>.*?)(?<TagClose>{{/{tag}}})");

        private static readonly Regex INT_REGEX = new("-?[0-9]+");
        private static readonly Regex FLOAT_REGEX = new("-?[0-9]+(?:\\.[0-9]+)?");
        private static readonly Regex POSITIVE_FLOAT_REGEX = new("[0-9]+(?:\\.[0-9]+)?");
        private static readonly Regex BOOL_REGEX = new("(?:true|false)");
        private static readonly Regex COLOR_REGEX = new("#[0-9a-fA-F]{8}");

        private static readonly Regex DEFAULT_TAG_REGEX = Tag("default");

        private static readonly Regex VOFFSET_TAG_REGEX = Tag("voffset");
        private static readonly Regex VOFFSET_HEIGHT_OPTION_REGEX = new($"height=(?<Value>{POSITIVE_FLOAT_REGEX})");
        private static readonly Regex VOFFSET_DELAY_OPTION_REGEX = new($"delay=(?<Value>{FLOAT_REGEX})");
        private static readonly Regex VOFFSET_OFFSET_OPTION_REGEX = new($"offset=(?<Value>{FLOAT_REGEX})");
        private static readonly Regex VOFFSET_DURATION_OPTION_REGEX = new($"duration=(?<Value>{FLOAT_REGEX})");
        private static readonly Regex VOFFSET_LOOP_OPTION_REGEX = new($"loop=(?<Value>{BOOL_REGEX})");
        private static readonly Regex VOFFSET_LOOP_DELAY_OPTION_REGEX = new($"loopDelay=(?<Value>{FLOAT_REGEX})");
        private static readonly Regex VOFFSET_PROGRESSIVE_OPTION_REGEX = new($"progressive=(?<Value>{BOOL_REGEX})");

        private static readonly Regex COLOR_TAG_REGEX = Tag("color");
        private static readonly Regex COLOR_VALUE_OPTION_REGEX = new($"value=(?<Value>{COLOR_REGEX})");

        private static readonly Regex NOBREAK_TAG_REGEX = Tag("nobreak");

        private static readonly Regex LINE_HEIGHT_TAG_REGEX = Tag("lineHeight");
        private static readonly Regex LINE_HEIGHT_OPTION_REGEX = new($"height=(?<Value>{FLOAT_REGEX})");
        #endregion

        #region Unity methods
        private void Awake() {
            this.Text = this.GetComponent<TMP_Text>();
        }

        private void OnEnable() {
            if (this.IsStatic) this.SetText(this.StaticText);

            this.AlwaysUpdate();
        }
        #endregion

        private void AlwaysUpdate() {
            if (this.RealTime) {
                this.InRealSeconds(
                    0,
                    () => {
                        this.Text.SetText(this.GetText());
                        this.AlwaysUpdate();
                    }
                );
            } else {
                this.InSeconds(
                    0,
                    () => {
                        this.Text.SetText(this.GetText());
                        this.AlwaysUpdate();
                    }
                );
            }
        }

        public void SetText(string text) {
            foreach (Sequence sequence in this.Sequences) DOTween.Kill(sequence);
            this.Sequences.Clear();

            text = $"{{default}}{text}{{/default}}";

            /***
             * Extract all tags
             */
            List<Match> matches = new();
            matches.AddRange(DEFAULT_TAG_REGEX.Matches(text).ToList());
            matches.AddRange(VOFFSET_TAG_REGEX.Matches(text).ToList());
            matches.AddRange(COLOR_TAG_REGEX.Matches(text).ToList());
            matches.AddRange(NOBREAK_TAG_REGEX.Matches(text).ToList());
            matches.AddRange(LINE_HEIGHT_TAG_REGEX.Matches(text).ToList());

            matches.Sort((match1, match2) => match1.Index.CompareTo(match2.Index));

            Dictionary<int, C_Character> characters = new();
            List<AC_Group> groups = new();

            foreach (Match match in matches) {
                AC_Group group = match.Groups["Tag"].Value switch {
                    "voffset" => new C_VOffsetGroup(match.Groups["Options"].Value),
                    "color" => new C_ColorGroup(match.Groups["Options"].Value),
                    "nobreak" => new C_NoBreakGroup(),
                    "lineHeight" => new C_LineHeightGroup(match.Groups["Options"].Value),
                    "default" => new C_DefaultGroup(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                groups.Add(group);

                int start = match.Groups["Text"].Index;
                int end = start + match.Groups["Text"].Length;
                for (int ii = start; ii < end; ii++) {
                    if (!characters.ContainsKey(ii)) {
                        characters[ii] = new C_Character {
                            Character = text[ii],
                            Color = this.Text.color,
                        };
                    }

                    characters[ii].AddGroup(group);
                }

                start = match.Groups["TagOpen"].Index;
                end = start + match.Groups["TagOpen"].Length;
                for (int ii = start; ii < end; ii++) {
                    if (characters.TryGetValue(ii, out C_Character character)) {
                        character.IsTag = true;
                    }
                }

                start = match.Groups["TagClose"].Index;
                end = start + match.Groups["TagClose"].Length;
                for (int ii = start; ii < end; ii++) {
                    if (characters.TryGetValue(ii, out C_Character character)) {
                        character.IsTag = true;
                    }
                }
            }

            List<int> keys = characters.Keys.ToList();
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (int key in keys) {
                if (characters[key].IsTag) characters.Remove(key);
            }

            /*
             * Set index in tag
             */
            foreach (AC_Group group in groups) {
                int index = 0;
                foreach ((_, C_Character character) in characters) {
                    if (character.Groups.Contains(group)) {
                        character.IndexInGroup[group] = index;
                        if (character.Character != ' ') {
                            index++;
                            group.SizeWithoutSpaces++;
                        }

                        group.Size++;
                    }
                }
            }

            /*
             * Set VOffset groups loopDelay
             */
            foreach (AC_Group group in groups) {
                if (group is C_VOffsetGroup vOffsetGroup) {
                    vOffsetGroup.LoopDelay = Mathf.Max(
                        -1000, // (group.SizeWithoutSpaces - 1) * vOffsetGroup.Offset + vOffsetGroup.Duration,
                        vOffsetGroup.LoopDelay
                    );
                }
            }

            this.Characters = characters.Values.ToList();

            foreach (C_Character character in this.Characters) {
                character.Groups.Sort((group1, group2) => group1.Priority.CompareTo(group2.Priority));
                character.Groups.ForEach(group => group.TweenCharacter(this, character));
            }

            this.Text.SetText(this.GetText());
            this.Text.ForceMeshUpdate();
        }

        public int GetLineCount() => this.Text.textInfo.lineCount - 1;

        private string GetText() {
            // Tweak with line-height to prevent jitter during animation
            StringBuilder stringBuilder = new("<line-height=0%>\n<line-height=100%>");

            if (this.Characters.Count != 0) {
                Color previousColor = this.Characters[0].Color;
                stringBuilder.Append(OpenColorTag(previousColor));

                string previousNoBreakGuid = this.Characters[0].NoBreakGuid;
                bool noBreakTagOpened = false;
                if (previousNoBreakGuid != null) {
                    stringBuilder.Append(OpenNoBrTag());
                    noBreakTagOpened = true;
                }

                string previousLineHeightGuid = this.Characters[0].LineHeightGuid;
                bool lineHeightTagOpened = false;
                if (previousLineHeightGuid != null) {
                    stringBuilder.Append(OpenLineHeightTag(this.Characters[0].LineHeight));
                    lineHeightTagOpened = true;
                }

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (C_Character character in this.Characters) {
                    if (character.Color != previousColor) {
                        stringBuilder.Append(CloseColorTag());
                        stringBuilder.Append(OpenColorTag(character.Color));
                    }

                    if (noBreakTagOpened && previousNoBreakGuid != character.NoBreakGuid) {
                        stringBuilder.Append(CloseNoBrTag());
                        noBreakTagOpened = false;
                    }

                    if (character.NoBreakGuid != null && character.NoBreakGuid != previousNoBreakGuid) {
                        stringBuilder.Append(OpenNoBrTag());
                        noBreakTagOpened = true;
                    }

                    if (lineHeightTagOpened && previousLineHeightGuid != character.LineHeightGuid) {
                        stringBuilder.Append(CloseLineHeightTag());
                        lineHeightTagOpened = false;
                    }

                    if (character.LineHeightGuid != null && character.LineHeightGuid != previousLineHeightGuid) {
                        stringBuilder.Append(OpenLineHeightTag(character.LineHeight));
                        lineHeightTagOpened = true;
                    }

                    string text = WithVOffset(character.Character, character.CurrentHeight);
                    stringBuilder.Append(text);

                    previousColor = character.Color;
                    previousNoBreakGuid = character.NoBreakGuid;
                    previousLineHeightGuid = character.LineHeightGuid;
                }

                if (noBreakTagOpened) stringBuilder.Append(CloseNoBrTag());
                if (lineHeightTagOpened) stringBuilder.Append(CloseLineHeightTag());
                stringBuilder.Append(CloseColorTag());
            }

            return stringBuilder.ToString();
        }

        private static string WithVOffset(string s, float offset) =>
            offset == 0
                ? s
                : $"<voffset={offset}>{s}</voffset>";

        private static string WithVOffset(char c, float offset) =>
            offset == 0
                ? c.ToString()
                : $"<voffset={offset}>{c}</voffset>";

        private static string OpenColorTag(Color color) => $"<color={color.ToHex()}>";
        private static string CloseColorTag() => $"</color>";
        private static string OpenNoBrTag() => $"<nobr>";
        private static string CloseNoBrTag() => $"</nobr>";
        private static string OpenLineHeightTag(float height) => $"<line-height={height}>";
        private static string CloseLineHeightTag() => $"</line-height>";
    }

    public static class SC_MB_TextExtensions {
        public static string VOffset(
            this string s,
            float height = 0,
            float delay = 0,
            float offset = 0,
            float duration = 0,
            bool loop = false,
            float loopDelay = 0,
            bool progressive = false
        ) {
            string heightStr = $"height={height.ToString(CultureInfo.InvariantCulture)}";
            string delayStr = $"delay={delay.ToString(CultureInfo.InvariantCulture)}";
            string offsetStr = $"offset={offset.ToString(CultureInfo.InvariantCulture)}";
            string durationStr = $"duration={duration.ToString(CultureInfo.InvariantCulture)}";
            string loopStr = $"loop={loop.ToString(CultureInfo.InvariantCulture).ToLower()}";
            string loopDelayStr = $"loopDelay={loopDelay.ToString(CultureInfo.InvariantCulture)}";
            string progressiveStr = $"progressive={progressive.ToString(CultureInfo.InvariantCulture).ToLower()}";

            return $"{{voffset {heightStr} {delayStr} {offsetStr} {durationStr} {loopStr} {loopDelayStr} {progressiveStr}}}{s}{{/voffset}}";
        }

        public static string Color(this string s, string color) => $"{{color value={color}}}{s}{{/color}}";
        public static string Color(this string s, Color color) => $"{{color value={color.ToHex()}}}{s}{{/color}}";

        public static string Green(this string s) => s.Color(new Color(126, 148, 50));
        public static string Red(this string s) => s.Color(new Color(194, 55, 83));

        public static string NoBreak(this string s) => $"{{nobreak}}{s}{{/nobreak}}";
    }
}
