using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CompositeCurves.Editor
{
    internal static class CompositeCurveCodeGenerator
    {
        private const string OutputAssetPath = "Assets/CompositeCurves/Generated/CompositeCurveGenerated.Evaluator.g.cs";
        private static readonly HashSet<string> CSharpKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
            "void", "volatile", "while"
        };
        private static readonly string[,] FunctionAliases =
        {
            { "sin", "Mathf.Sin" }, { "cos", "Mathf.Cos" }, { "tan", "Mathf.Tan" },
            { "asin", "Mathf.Asin" }, { "acos", "Mathf.Acos" }, { "atan", "Mathf.Atan" },
            { "atan2", "Mathf.Atan2" }, { "sqrt", "Mathf.Sqrt" }, { "pow", "Mathf.Pow" },
            { "abs", "Mathf.Abs" }, { "min", "Mathf.Min" }, { "max", "Mathf.Max" },
            { "clamp", "Mathf.Clamp" }, { "clamp01", "Mathf.Clamp01" }, { "random", "CompositeCurveRandom.NextFloat" },
            { "exp", "Mathf.Exp" }, { "log", "Mathf.Log" }, { "log10", "Mathf.Log10" },
            { "floor", "Mathf.Floor" }, { "ceil", "Mathf.Ceil" }, { "round", "Mathf.Round" },
            { "sign", "Mathf.Sign" }
        };
        private static readonly string[] ConstantAliases = { "pi", "deg2rad", "rad2deg" };
        private static readonly Regex IdentifierPattern = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
        private static readonly Regex FunctionCallPattern = new Regex(@"(?<![\w.])([A-Za-z_][A-Za-z0-9_]*)\s*\(");
        private static bool s_isGenerating;

        [MenuItem("Tools/Composite Curves/Regenerate Generated Curves")]
        public static void RegenerateAll()
        {
            if (s_isGenerating)
            {
                return;
            }

            s_isGenerating = true;

            try
            {
                var customSegments = CollectCustomSegments();
                var output = BuildSource(customSegments);
                WriteGeneratedFile(output);
            }
            finally
            {
                s_isGenerating = false;
            }
        }

        private static List<CustomSegmentInfo> CollectCustomSegments()
        {
            var results = new List<CustomSegmentInfo>();
            var assetGuids = AssetDatabase.FindAssets("t:CompositeCurveDefinition");

            for (var i = 0; i < assetGuids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                var curve = AssetDatabase.LoadAssetAtPath<CompositeCurveDefinition>(path);
                if (curve == null)
                {
                    continue;
                }

                var beforeCurveId = curve.CurveId;
                curve.EnsureIdentifiers();
                var dirty = beforeCurveId != curve.CurveId;

                var segments = curve.Segments;
                if (segments == null)
                {
                    continue;
                }

                var segmentsByName = BuildSegmentNameMap(segments);
                var curveResults = new List<CustomSegmentInfo>();

                for (var segmentIndex = 0; segmentIndex < segments.Count; segmentIndex++)
                {
                    var segment = segments[segmentIndex];
                    if (segment == null || !segment.Enabled || segment.Mode != CompositeCurveSegmentMode.Custom)
                    {
                        continue;
                    }

                    var beforeSegmentId = segment.SegmentId;
                    segment.EnsureIdentifier(CreateId);

                    var normalizedExpression = NormalizeExpression(segment.CustomExpression);
                    if (string.IsNullOrWhiteSpace(normalizedExpression))
                    {
                        Debug.LogError($"CompositeCurves: custom segment '{segment.DisplayName}' on '{curve.name}' has an empty expression.");
                        normalizedExpression = "0f";
                    }

                    var references = new List<string>();
                    normalizedExpression = ResolveSegmentReferences(normalizedExpression, curve, segment, segmentsByName, references);

                    var variableBindings = BuildVariableBindings(curve, segment, ref normalizedExpression);

                    curveResults.Add(new CustomSegmentInfo
                    {
                        CurveId = curve.CurveId,
                        CurveName = curve.name,
                        SegmentId = segment.SegmentId,
                        SegmentName = segment.DisplayName,
                        Expression = normalizedExpression,
                        Variables = variableBindings,
                        References = references
                    });

                    dirty |= beforeSegmentId != segment.SegmentId;
                }

                RejectReferenceCycles(curveResults);
                results.AddRange(curveResults);

                curve.RebuildRuntimeCache();

                if (dirty)
                {
                    EditorUtility.SetDirty(curve);
                }
            }

            return results;
        }

        // Returns null when the name can be referenced from custom expressions, otherwise the reason it can't.
        internal static string GetSegmentNameProblem(string name)
        {
            if (string.IsNullOrEmpty(name) || !IdentifierPattern.IsMatch(name))
            {
                return "it must start with a letter or underscore and contain only letters, digits, and underscores";
            }

            for (var i = 0; i < FunctionAliases.GetLength(0); i++)
            {
                if (string.Equals(name, FunctionAliases[i, 0], StringComparison.OrdinalIgnoreCase))
                {
                    return $"'{FunctionAliases[i, 0]}' is a built-in function";
                }
            }

            for (var i = 0; i < ConstantAliases.Length; i++)
            {
                if (string.Equals(name, ConstantAliases[i], StringComparison.OrdinalIgnoreCase))
                {
                    return $"'{ConstantAliases[i]}' is a built-in constant";
                }
            }

            return null;
        }

        // Maps referenceable names to segments. Duplicated names map to null so references to them can be rejected.
        private static Dictionary<string, CompositeCurveSegment> BuildSegmentNameMap(List<CompositeCurveSegment> segments)
        {
            var map = new Dictionary<string, CompositeCurveSegment>(StringComparer.Ordinal);
            for (var i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];
                if (segment == null || GetSegmentNameProblem(segment.DisplayName) != null)
                {
                    continue;
                }

                map[segment.DisplayName] = map.ContainsKey(segment.DisplayName) ? null : segment;
            }

            return map;
        }

        // Rewrites Name(arg) calls to other segments into registry calls by segment id.
        private static string ResolveSegmentReferences(
            string expression,
            CompositeCurveDefinition curve,
            CompositeCurveSegment segment,
            Dictionary<string, CompositeCurveSegment> segmentsByName,
            List<string> references)
        {
            string error = null;
            var resolved = FunctionCallPattern.Replace(expression, match =>
            {
                var name = match.Groups[1].Value;
                // Built-in aliases are already rewritten to dotted calls, so any bare call left must be a segment.
                if (!segmentsByName.TryGetValue(name, out var target))
                {
                    error = error ?? $"calls unknown function or segment '{name}'";
                }
                else if (target == null)
                {
                    error = error ?? $"references '{name}', but more than one segment has that name";
                }
                else if (!target.Enabled)
                {
                    error = error ?? $"references disabled segment '{name}'";
                }
                else if (ReferenceEquals(target, segment))
                {
                    error = error ?? "references itself";
                }
                else
                {
                    references.Add(target.SegmentId);
                    return $"CompositeCurveGeneratedRegistry.EvaluateSegment(\"{EscapeForString(target.SegmentId)}\", ";
                }

                return match.Value;
            });

            if (error == null)
            {
                return resolved;
            }

            Debug.LogError($"CompositeCurves: custom segment '{segment.DisplayName}' on '{curve.name}' {error}. It will evaluate to 0.");
            references.Clear();
            return "0f";
        }

        private static void RejectReferenceCycles(List<CustomSegmentInfo> curveSegments)
        {
            var byId = new Dictionary<string, CustomSegmentInfo>(StringComparer.Ordinal);
            for (var i = 0; i < curveSegments.Count; i++)
            {
                byId[curveSegments[i].SegmentId] = curveSegments[i];
            }

            // ponytail: one DFS per segment (O(n^2)), fine for hand-authored curves.
            var inCycle = new List<CustomSegmentInfo>();
            for (var i = 0; i < curveSegments.Count; i++)
            {
                if (CanReach(curveSegments[i], curveSegments[i].SegmentId, byId, new HashSet<string>(StringComparer.Ordinal)))
                {
                    inCycle.Add(curveSegments[i]);
                }
            }

            for (var i = 0; i < inCycle.Count; i++)
            {
                Debug.LogError($"CompositeCurves: custom segment '{inCycle[i].SegmentName}' on '{inCycle[i].CurveName}' is part of a segment reference cycle. It will evaluate to 0.");
                inCycle[i].Expression = "0f";
            }
        }

        private static bool CanReach(CustomSegmentInfo from, string targetId, Dictionary<string, CustomSegmentInfo> byId, HashSet<string> visited)
        {
            for (var i = 0; i < from.References.Count; i++)
            {
                var next = from.References[i];
                if (string.Equals(next, targetId, StringComparison.Ordinal))
                {
                    return true;
                }

                if (visited.Add(next) && byId.TryGetValue(next, out var nextInfo) && CanReach(nextInfo, targetId, byId, visited))
                {
                    return true;
                }
            }

            return false;
        }

        private static List<VariableBinding> BuildVariableBindings(
            CompositeCurveDefinition curve,
            CompositeCurveSegment segment,
            ref string expression)
        {
            var result = new List<VariableBinding>();
            var mergedVariables = curve.GetMergedVariables(segment.Variables);

            for (var i = 0; i < mergedVariables.Length; i++)
            {
                var sourceName = string.IsNullOrWhiteSpace(mergedVariables[i].Name) ? $"var{i}" : mergedVariables[i].Name.Trim();
                var localName = MakeSafeIdentifier(sourceName, i);
                if (!string.Equals(sourceName, localName, StringComparison.Ordinal))
                {
                    Debug.LogWarning(
                        $"CompositeCurves: variable '{sourceName}' on segment '{segment.DisplayName}' in '{curve.name}' is not a valid C# identifier. Using '{localName}' in generated code.");
                    expression = ReplaceIdentifier(expression, sourceName, localName);
                }

                result.Add(new VariableBinding
                {
                    SourceName = sourceName,
                    LocalName = localName,
                    DefaultValue = mergedVariables[i].Value,
                    Index = i,
                    IsShared = IsSharedVariable(sourceName)
                });
            }

            return result;
        }

        private static string BuildSource(List<CustomSegmentInfo> customSegments)
        {
            var builder = new StringBuilder(4096);
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine();
            builder.AppendLine("namespace CompositeCurves");
            builder.AppendLine("{");
            builder.AppendLine("    public static class CompositeCurveGeneratedBootstrap");
            builder.AppendLine("    {");
            builder.AppendLine("        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]");
            builder.AppendLine("        private static void RegisterRuntime()");
            builder.AppendLine("        {");
            builder.AppendLine("            CompositeCurveGeneratedRegistry.Register(Evaluate);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("#if UNITY_EDITOR");
            builder.AppendLine("        [UnityEditor.InitializeOnLoadMethod]");
            builder.AppendLine("        private static void RegisterEditor()");
            builder.AppendLine("        {");
            builder.AppendLine("            CompositeCurveGeneratedRegistry.Register(Evaluate);");
            builder.AppendLine("        }");
            builder.AppendLine("#endif");
            builder.AppendLine();
            builder.AppendLine("        private static bool Evaluate(string curveId, string segmentId, float x, CompositeCurveVariable[] variables, out float value)");
            builder.AppendLine("        {");

            if (customSegments.Count == 0)
            {
                builder.AppendLine("            value = 0f;");
                builder.AppendLine("            return false;");
            }
            else
            {
                builder.AppendLine("            switch (curveId)");
                builder.AppendLine("            {");

                var emittedCurveIds = new HashSet<string>(StringComparer.Ordinal);
                for (var i = 0; i < customSegments.Count; i++)
                {
                    var segment = customSegments[i];
                    if (!emittedCurveIds.Add(segment.CurveId))
                    {
                        continue;
                    }

                    builder.AppendLine($"                case \"{EscapeForString(segment.CurveId)}\":");
                    builder.AppendLine("                {");

                    var sharedVariables = CollectSharedVariables(customSegments, segment.CurveId);
                    for (var variableIndex = 0; variableIndex < sharedVariables.Count; variableIndex++)
                    {
                        var variable = sharedVariables[variableIndex];
                        builder.AppendLine(
                            $"                    var {variable.LocalName} = variables != null && variables.Length > {variable.Index} ? variables[{variable.Index}].Value : {FormatFloat(variable.DefaultValue)};");
                    }

                    builder.AppendLine("                    switch (segmentId)");
                    builder.AppendLine("                    {");

                    for (var j = 0; j < customSegments.Count; j++)
                    {
                        var candidate = customSegments[j];
                        if (!string.Equals(candidate.CurveId, segment.CurveId, StringComparison.Ordinal))
                        {
                            continue;
                        }

                        builder.AppendLine($"                        case \"{EscapeForString(candidate.SegmentId)}\":");
                        builder.AppendLine("                        {");

                        for (var variableIndex = 0; variableIndex < candidate.Variables.Count; variableIndex++)
                        {
                            var variable = candidate.Variables[variableIndex];
                            if (variable.IsShared)
                            {
                                continue;
                            }

                            builder.AppendLine(
                                $"                            var {variable.LocalName} = variables != null && variables.Length > {variable.Index} ? variables[{variable.Index}].Value : {FormatFloat(variable.DefaultValue)};");
                        }

                        builder.AppendLine($"                            value = {candidate.Expression};");
                        builder.AppendLine("                            return true;");
                        builder.AppendLine("                        }");
                    }

                    builder.AppendLine("                        default:");
                    builder.AppendLine("                            break;");
                    builder.AppendLine("                    }");
                    builder.AppendLine("                    break;");
                    builder.AppendLine("                }");
                }

                builder.AppendLine("                default:");
                builder.AppendLine("                    break;");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine("            value = 0f;");
                builder.AppendLine("            return false;");
            }

            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }


        private static List<VariableBinding> CollectSharedVariables(List<CustomSegmentInfo> customSegments, string curveId)
        {
            var result = new List<VariableBinding>();
            var emittedNames = new HashSet<string>(StringComparer.Ordinal);

            for (var segmentIndex = 0; segmentIndex < customSegments.Count; segmentIndex++)
            {
                var segment = customSegments[segmentIndex];
                if (!string.Equals(segment.CurveId, curveId, StringComparison.Ordinal))
                {
                    continue;
                }

                for (var variableIndex = 0; variableIndex < segment.Variables.Count; variableIndex++)
                {
                    var variable = segment.Variables[variableIndex];
                    if (!variable.IsShared || !emittedNames.Add(variable.LocalName))
                    {
                        continue;
                    }

                    result.Add(variable);
                }
            }

            return result;
        }

        private static void WriteGeneratedFile(string content)
        {
            var projectRoot = Directory.GetCurrentDirectory();
            var absolutePath = Path.Combine(projectRoot, OutputAssetPath);
            var directory = Path.GetDirectoryName(absolutePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(absolutePath, content, Encoding.UTF8);
            AssetDatabase.ImportAsset(OutputAssetPath, ImportAssetOptions.ForceUpdate);
        }

        private static string NormalizeExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return string.Empty;
            }

            var normalized = expression.Replace("\r", " ").Replace("\n", " ").Trim();
            normalized = normalized.Replace("Mathf.PI", "Mathf.PI");

            if (normalized.IndexOf(';') >= 0 || normalized.IndexOf('{') >= 0 || normalized.IndexOf('}') >= 0)
            {
                return "0f";
            }

            for (var i = 0; i < FunctionAliases.GetLength(0); i++)
            {
                normalized = ReplaceFunctionCall(normalized, FunctionAliases[i, 0], FunctionAliases[i, 1]);
            }

            normalized = Regex.Replace(normalized, @"(?<![\w.])pi(?![\w.])", "Mathf.PI", RegexOptions.IgnoreCase);
            normalized = Regex.Replace(normalized, @"(?<![\w.])deg2rad(?![\w.])", "Mathf.Deg2Rad", RegexOptions.IgnoreCase);
            normalized = Regex.Replace(normalized, @"(?<![\w.])rad2deg(?![\w.])", "Mathf.Rad2Deg", RegexOptions.IgnoreCase);
            return normalized;
        }

        private static bool IsSharedVariable(string sourceName)
        {
            return !string.IsNullOrEmpty(sourceName) && sourceName.EndsWith("_shared", StringComparison.Ordinal);
        }

        private static string ReplaceFunctionCall(string input, string alias, string replacement)
        {
            return Regex.Replace(
                input,
                $@"(?<![\w.]){alias}\s*\(",
                $"{replacement}(",
                RegexOptions.IgnoreCase);
        }

        private static string ReplaceIdentifier(string input, string oldIdentifier, string newIdentifier)
        {
            return Regex.Replace(
                input,
                $@"(?<![\w.]){Regex.Escape(oldIdentifier)}(?![\w.])",
                newIdentifier);
        }

        private static string MakeSafeIdentifier(string sourceName, int index)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return $"var{index}";
            }

            var builder = new StringBuilder(sourceName.Length + 4);

            for (var i = 0; i < sourceName.Length; i++)
            {
                var character = sourceName[i];
                if (i == 0)
                {
                    if (char.IsLetter(character) || character == '_')
                    {
                        builder.Append(character);
                    }
                    else if (char.IsDigit(character))
                    {
                        builder.Append('_');
                        builder.Append(character);
                    }
                    else
                    {
                        builder.Append('_');
                    }
                }
                else
                {
                    builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
                }
            }

            if (builder.Length == 0)
            {
                builder.Append($"var{index}");
            }

            var candidate = builder.ToString();
            if (CSharpKeywords.Contains(candidate))
            {
                candidate = "_" + candidate;
            }

            return candidate;
        }

        private static string EscapeForString(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string FormatFloat(float value)
        {
            return value.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "f";
        }

        private static string CreateId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private sealed class CustomSegmentInfo
        {
            public string CurveId;
            public string CurveName;
            public string SegmentId;
            public string SegmentName;
            public string Expression;
            public List<VariableBinding> Variables;
            public List<string> References;
        }

        private sealed class VariableBinding
        {
            public string SourceName;
            public string LocalName;
            public float DefaultValue;
            public int Index;
            public bool IsShared;
        }
    }
}
