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
                case "c4e52f5a3a7e4a82b6728e3d8e505910":
                {
                    var max_shared = variables != null && variables.Length > 0 ? variables[0].Value : 4f;
                    var min_shared = variables != null && variables.Length > 1 ? variables[1].Value : 0.4f;
                    var a_shared = variables != null && variables.Length > 2 ? variables[2].Value : 0.1f;
                    var b_shared = variables != null && variables.Length > 3 ? variables[3].Value : 0.5f;
                    var c_shared = variables != null && variables.Length > 4 ? variables[4].Value : 15f;
                    var t1_shared = variables != null && variables.Length > 5 ? variables[5].Value : 7.50000048f;
                    var t2_shared = variables != null && variables.Length > 6 ? variables[6].Value : 23f;
                    var t3_shared = variables != null && variables.Length > 7 ? variables[7].Value : 40f;
                    var d_shared = variables != null && variables.Length > 8 ? variables[8].Value : 0.210000038f;
                    switch (segmentId)
                    {
                        case "3f7a1540228241798abccceef4c494d8":
                        {
                            value = max_shared - Mathf.Pow(x,a_shared);
                            return true;
                        }
                        case "3278a0051fc14dad90ed391ec3969ae7":
                        {
                            value = (max_shared - Mathf.Pow(t1_shared,a_shared)) + Mathf.Log(t1_shared, c_shared) - Mathf.Log(x,c_shared);
                            return true;
                        }
                        case "4fa12ec7197844a7a442bc5e85c122ad":
                        {
                            value = ((max_shared - Mathf.Pow(t1_shared,a_shared)) + Mathf.Log(t1_shared, c_shared) - Mathf.Log(t2_shared,c_shared))+Mathf.Pow(t2_shared,b_shared)-Mathf.Pow(x,b_shared);
                            return true;
                        }
                        case "1ee36ad054c9468b8c369c7084d93877":
                        {
                            value = Mathf.Max(min_shared, (((max_shared - Mathf.Pow(t1_shared,a_shared)) + Mathf.Log(t1_shared, c_shared) - Mathf.Log(t2_shared,c_shared))+Mathf.Pow(t2_shared,b_shared)-Mathf.Pow(t3_shared,b_shared)) + Mathf.Pow(t3_shared, d_shared)-Mathf.Pow(x,d_shared));
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
