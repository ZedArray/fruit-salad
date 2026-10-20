using UnityEngine;

namespace CompositeCurves
{
    public static class CompositeCurveGeneratedBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterRuntime()
        {
            CompositeCurveGeneratedRegistry.Register(Evaluate);
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void RegisterEditor()
        {
            CompositeCurveGeneratedRegistry.Register(Evaluate);
        }
#endif

        private static bool Evaluate(string curveId, string segmentId, float x, CompositeCurveVariable[] variables, out float value)
        {
            switch (curveId)
            {
                case "c20c2250e6484a7aaf9a1a8520a5ec30":
                {
                    var jitter_shared = variables != null && variables.Length > 0 ? variables[0].Value : 0.1f;
                    var end_shared = variables != null && variables.Length > 1 ? variables[1].Value : 4f;
                    switch (segmentId)
                    {
                        case "a4e85c0c1e6843c2a8b6eefb47d60630":
                        {
                            var start = variables != null && variables.Length > 2 ? variables[2].Value : 13f;
                            var length = variables != null && variables.Length > 3 ? variables[3].Value : 165f;
                            value = (start - (start - end_shared) * x / length) * (1 - jitter_shared  + 2 * jitter_shared * CompositeCurveRandom.NextFloat());
                            return true;
                        }
                        case "701401c18b3f4632827d6cac2e0031e6":
                        {
                            value = end_shared * (1 - jitter_shared  + 2 * jitter_shared * CompositeCurveRandom.NextFloat());
                            return true;
                        }
                        default:
                            break;
                    }
                    break;
                }
                case "e6a2c40d45ee4f2fbd48e4b52a2f2552":
                {
                    switch (segmentId)
                    {
                        case "3f7a1540228241798abccceef4c494d8":
                        {
                            var maxValue = variables != null && variables.Length > 0 ? variables[0].Value : 4f;
                            var steepness = variables != null && variables.Length > 1 ? variables[1].Value : 0.2f;
                            value = maxValue - Mathf.Pow(x,steepness);
                            return true;
                        }
                        case "3278a0051fc14dad90ed391ec3969ae7":
                        {
                            var start = variables != null && variables.Length > 0 ? variables[0].Value : 7.8f;
                            var steepness = variables != null && variables.Length > 1 ? variables[1].Value : 13f;
                            value = CompositeCurveGeneratedRegistry.EvaluateSegment("3f7a1540228241798abccceef4c494d8", start) + Mathf.Log(start, steepness) - Mathf.Log(x,steepness);
                            return true;
                        }
                        case "4fa12ec7197844a7a442bc5e85c122ad":
                        {
                            var start = variables != null && variables.Length > 0 ? variables[0].Value : 23f;
                            var steepness = variables != null && variables.Length > 1 ? variables[1].Value : 0.38f;
                            value = CompositeCurveGeneratedRegistry.EvaluateSegment("3278a0051fc14dad90ed391ec3969ae7", start) + Mathf.Pow(start,steepness) - Mathf.Pow(x,steepness);
                            return true;
                        }
                        case "1ee36ad054c9468b8c369c7084d93877":
                        {
                            var start = variables != null && variables.Length > 0 ? variables[0].Value : 40f;
                            var steepness = variables != null && variables.Length > 1 ? variables[1].Value : 0.230000019f;
                            var minimum = variables != null && variables.Length > 2 ? variables[2].Value : 0.4f;
                            value = Mathf.Max(minimum, CompositeCurveGeneratedRegistry.EvaluateSegment("4fa12ec7197844a7a442bc5e85c122ad", start) + Mathf.Pow(start, steepness) - Mathf.Pow(x,steepness));
                            return true;
                        }
                        default:
                            break;
                    }
                    break;
                }
                default:
                    break;
            }

            value = 0f;
            return false;
        }
    }
}
