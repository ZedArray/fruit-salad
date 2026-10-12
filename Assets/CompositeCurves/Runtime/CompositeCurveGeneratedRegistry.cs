using System;

namespace CompositeCurves
{
    public static class CompositeCurveGeneratedRegistry
    {
        private const string SharedSeedName = "__seed__shared";

        public delegate bool GeneratedCurveEvaluator(
            string curveId,
            string segmentId,
            float x,
            CompositeCurveVariable[] variables,
            out float value);

        private static GeneratedCurveEvaluator s_evaluator = FallbackEvaluator;

        [ThreadStatic] private static CompositeCurveDefinition s_currentCurve;
        [ThreadStatic] private static int s_depth;

        internal static CompositeCurveDefinition CurrentCurve { get => s_currentCurve; set => s_currentCurve = value; }

        public static void Register(GeneratedCurveEvaluator evaluator)
        {
            s_evaluator = evaluator ?? FallbackEvaluator;
        }

        public static bool TryEvaluate(
            string curveId,
            string segmentId,
            float x,
            CompositeCurveVariable[] variables,
            out float value)
        {
            return TryEvaluate(curveId, segmentId, x, variables, false, 0, out value);
        }

        public static bool TryEvaluate(
            string curveId,
            string segmentId,
            float x,
            CompositeCurveVariable[] variables,
            bool useFixedSeed,
            int fixedSeed,
            out float value)
        {
            // Nested segment references share the outer segment's random stream.
            if (s_depth == 0)
            {
                InitializeRandom(variables, useFixedSeed, fixedSeed);
            }

            s_depth++;
            try
            {
                return s_evaluator(curveId, segmentId, x, variables, out value);
            }
            finally
            {
                s_depth--;
            }
        }

        // Called from generated code when a custom expression references another segment of the same curve.
        public static float EvaluateSegment(string segmentId, float x)
        {
            return s_currentCurve != null ? s_currentCurve.EvaluateSegmentById(segmentId, x) : 0f;
        }

        private static void InitializeRandom(CompositeCurveVariable[] variables, bool useFixedSeed, int fixedSeed)
        {
            if (useFixedSeed)
            {
                CompositeCurveRandom.ResetToFixedSeed(fixedSeed);
                return;
            }

            var seed = ExtractSeed(variables);
            if (seed.HasValue)
            {
                if (seed.Value >= 0)
                {
                    CompositeCurveRandom.ResetToFixedSeed(seed.Value);
                }
                else
                {
                    CompositeCurveRandom.ResetToSessionSeed();
                }
            }
            else
            {
                CompositeCurveRandom.ResetToSessionSeed();
            }
        }

        internal static int? ExtractSeed(CompositeCurveVariable[] variables)
        {
            if (variables == null)
            {
                return null;
            }

            int? segmentSeed = null;
            int? sharedSeed = null;

            for (var i = 0; i < variables.Length; i++)
            {
                if (string.Equals(variables[i].Name, "__seed__", StringComparison.Ordinal))
                {
                    segmentSeed = (int)variables[i].Value;
                }
                else if (string.Equals(variables[i].Name, SharedSeedName, StringComparison.Ordinal))
                {
                    sharedSeed = (int)variables[i].Value;
                }
            }

            if (segmentSeed.HasValue)
            {
                return segmentSeed.Value;
            }

            if (sharedSeed.HasValue)
            {
                return sharedSeed.Value;
            }

            return null;
        }

        private static bool FallbackEvaluator(
            string curveId,
            string segmentId,
            float x,
            CompositeCurveVariable[] variables,
            out float value)
        {
            value = 0f;
            return false;
        }
    }
}
