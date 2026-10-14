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
