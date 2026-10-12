using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CompositeCurves.Editor
{
    internal sealed class CompositeCurveCompareWindow : EditorWindow
    {
        private const float GraphPadding = 8f;
        private static readonly Color CurveAColor = new Color(0.2f, 0.7f, 1f, 1f);
        private static readonly Color CurveBColor = new Color(1f, 0.6f, 0.2f, 1f);
        private static readonly Color DifferenceColor = new Color(1f, 0.25f, 0.25f, 0.8f);

        [SerializeField] private CompositeCurveDefinition curveA;
        [SerializeField] private CompositeCurveDefinition curveB;
        [SerializeField] private float minX;
        [SerializeField] private float maxX = 1f;
        [SerializeField] private int samples = 512;
        [SerializeField] private float tolerance = 0.0001f;

        [MenuItem("Tools/Composite Curves/Compare Curves")]
        private static void Open()
        {
            GetWindow<CompositeCurveCompareWindow>("Compare Curves");
        }

        // Keeps the comparison live while either curve is edited in the inspector.
        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            curveA = (CompositeCurveDefinition)EditorGUILayout.ObjectField("Curve A", curveA, typeof(CompositeCurveDefinition), false);
            curveB = (CompositeCurveDefinition)EditorGUILayout.ObjectField("Curve B", curveB, typeof(CompositeCurveDefinition), false);
            if (EditorGUI.EndChangeCheck())
            {
                FitRange();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                minX = EditorGUILayout.FloatField("Min X", minX);
                maxX = EditorGUILayout.FloatField("Max X", maxX);
                if (GUILayout.Button("Fit", GUILayout.Width(50f)))
                {
                    FitRange();
                }
            }

            samples = Mathf.Max(2, EditorGUILayout.IntField("Samples", samples));
            tolerance = Mathf.Max(0f, EditorGUILayout.FloatField("Tolerance", tolerance));

            if (curveA == null || curveB == null)
            {
                EditorGUILayout.HelpBox("Pick two curves to compare.", MessageType.Info);
                return;
            }

            if (maxX <= minX)
            {
                EditorGUILayout.HelpBox("Max X must be greater than Min X.", MessageType.Warning);
                return;
            }

            var result = Compare(curveA, curveB, minX, maxX, samples, tolerance);
            if (result.Same)
            {
                EditorGUILayout.HelpBox(
                    $"Same: the curves match within {tolerance} at all {result.X.Length} sampled points in [{minX}, {maxX}].",
                    MessageType.Info);
            }
            else
            {
                var i = result.MaxDifferenceIndex;
                EditorGUILayout.HelpBox(
                    $"Different: largest difference is {result.MaxDifference:0.#####} at x = {result.X[i]:0.#####} (A = {result.A[i]:0.#####}, B = {result.B[i]:0.#####}).",
                    MessageType.Warning);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawLegend(CurveAColor, curveA.name);
                DrawLegend(CurveBColor, curveB.name);
            }

            DrawGraph(GUILayoutUtility.GetRect(10f, 240f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)), result);
        }

        internal static ComparisonResult Compare(
            CompositeCurveDefinition a,
            CompositeCurveDefinition b,
            float minX,
            float maxX,
            int samples,
            float tolerance)
        {
            a.RebuildRuntimeCache();
            b.RebuildRuntimeCache();

            var xs = new List<float>(samples + 16);
            for (var i = 0; i < samples; i++)
            {
                xs.Add(Mathf.Lerp(minX, maxX, i / (samples - 1f)));
            }

            // Segment boundaries catch steps and inclusive/exclusive differences that uniform samples can miss.
            AddSegmentBoundaries(a, minX, maxX, xs);
            AddSegmentBoundaries(b, minX, maxX, xs);
            xs.Sort();

            var result = new ComparisonResult
            {
                X = xs.ToArray(),
                A = new float[xs.Count],
                B = new float[xs.Count]
            };

            // ponytail: sampled comparison, differences narrower than the sample spacing between boundaries can slip through.
            for (var i = 0; i < xs.Count; i++)
            {
                // Preview evaluation seeds random() per x, so curves using random() compare deterministically.
                result.A[i] = a.GetValueForPreview(xs[i]);
                result.B[i] = b.GetValueForPreview(xs[i]);

                var difference = Difference(result.A[i], result.B[i]);
                if (difference > result.MaxDifference)
                {
                    result.MaxDifference = difference;
                    result.MaxDifferenceIndex = i;
                }
            }

            result.Same = result.MaxDifference <= tolerance;
            return result;
        }

        private static void AddSegmentBoundaries(CompositeCurveDefinition curve, float minX, float maxX, List<float> xs)
        {
            if (curve.Segments == null)
            {
                return;
            }

            for (var i = 0; i < curve.Segments.Count; i++)
            {
                var segment = curve.Segments[i];
                if (segment == null || !segment.Enabled)
                {
                    continue;
                }

                if (segment.StartX >= minX && segment.StartX <= maxX)
                {
                    xs.Add(segment.StartX);
                }

                if (segment.EndX >= minX && segment.EndX <= maxX)
                {
                    xs.Add(segment.EndX);
                }
            }
        }

        private static float Difference(float a, float b)
        {
            if (a == b || (float.IsNaN(a) && float.IsNaN(b)))
            {
                return 0f;
            }

            var difference = Mathf.Abs(a - b);
            return float.IsNaN(difference) ? float.PositiveInfinity : difference;
        }

        private void FitRange()
        {
            var min = float.PositiveInfinity;
            var max = float.NegativeInfinity;
            IncludeDomain(curveA, ref min, ref max);
            IncludeDomain(curveB, ref min, ref max);

            if (float.IsInfinity(min) || float.IsInfinity(max))
            {
                min = 0f;
                max = 1f;
            }

            minX = min;
            maxX = max > min ? max : min + 1f;
        }

        private static void IncludeDomain(CompositeCurveDefinition curve, ref float min, ref float max)
        {
            if (curve == null || curve.Segments == null)
            {
                return;
            }

            for (var i = 0; i < curve.Segments.Count; i++)
            {
                var segment = curve.Segments[i];
                if (segment != null && segment.Enabled)
                {
                    min = Mathf.Min(min, segment.StartX);
                    max = Mathf.Max(max, segment.EndX);
                }
            }
        }

        private static void DrawLegend(Color color, string label)
        {
            var swatch = GUILayoutUtility.GetRect(12f, 12f, GUILayout.Width(12f), GUILayout.Height(12f));
            swatch.y += 3f;
            EditorGUI.DrawRect(swatch, color);
            EditorGUILayout.LabelField(label, GUILayout.ExpandWidth(false), GUILayout.MinWidth(60f));
        }

        private static void DrawGraph(Rect rect, ComparisonResult result)
        {
            EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.12f, 1f));

            var minY = float.PositiveInfinity;
            var maxY = float.NegativeInfinity;
            IncludeFinite(result.A, ref minY, ref maxY);
            IncludeFinite(result.B, ref minY, ref maxY);
            if (float.IsInfinity(minY))
            {
                EditorGUI.LabelField(rect, "No finite values to plot.", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            if (Mathf.Approximately(minY, maxY))
            {
                minY -= 1f;
                maxY += 1f;
            }

            var yPadding = (maxY - minY) * 0.1f;
            var view = new Rect(result.X[0], minY - yPadding, result.X[result.X.Length - 1] - result.X[0], (maxY - minY) + (2f * yPadding));

            Handles.BeginGUI();

            Handles.color = new Color(0.45f, 0.45f, 0.45f, 1f);
            if (view.yMin <= 0f && view.yMax >= 0f)
            {
                Handles.DrawLine(ToRect(view.xMin, 0f, rect, view), ToRect(view.xMax, 0f, rect, view));
            }

            if (view.xMin <= 0f && view.xMax >= 0f)
            {
                Handles.DrawLine(ToRect(0f, view.yMin, rect, view), ToRect(0f, view.yMax, rect, view));
            }

            if (!result.Same)
            {
                var x = result.X[result.MaxDifferenceIndex];
                Handles.color = DifferenceColor;
                Handles.DrawLine(ToRect(x, view.yMin, rect, view), ToRect(x, view.yMax, rect, view));
            }

            DrawSeries(result.X, result.A, CurveAColor, 3f, rect, view);
            DrawSeries(result.X, result.B, CurveBColor, 2f, rect, view);
            Handles.EndGUI();

            EditorGUI.LabelField(
                new Rect(rect.x + GraphPadding, rect.y + GraphPadding, rect.width - (GraphPadding * 2f), 18f),
                $"X [{view.xMin:0.###}, {view.xMax:0.###}]  Y [{minY:0.###}, {maxY:0.###}]",
                EditorStyles.whiteMiniLabel);
        }

        private static void DrawSeries(float[] xs, float[] ys, Color color, float width, Rect rect, Rect view)
        {
            Handles.color = color;
            for (var i = 1; i < xs.Length; i++)
            {
                if (IsFinite(ys[i - 1]) && IsFinite(ys[i]))
                {
                    Handles.DrawAAPolyLine(width, ToRect(xs[i - 1], ys[i - 1], rect, view), ToRect(xs[i], ys[i], rect, view));
                }
            }
        }

        private static Vector3 ToRect(float x, float y, Rect rect, Rect view)
        {
            return new Vector3(
                Mathf.Lerp(rect.xMin + GraphPadding, rect.xMax - GraphPadding, Mathf.InverseLerp(view.xMin, view.xMax, x)),
                Mathf.Lerp(rect.yMax - GraphPadding, rect.yMin + GraphPadding, Mathf.InverseLerp(view.yMin, view.yMax, y)),
                0f);
        }

        private static void IncludeFinite(float[] values, ref float min, ref float max)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (IsFinite(values[i]))
                {
                    min = Mathf.Min(min, values[i]);
                    max = Mathf.Max(max, values[i]);
                }
            }
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        internal sealed class ComparisonResult
        {
            public float[] X;
            public float[] A;
            public float[] B;
            public bool Same;
            public float MaxDifference;
            public int MaxDifferenceIndex;
        }
    }
}
