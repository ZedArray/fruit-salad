using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace CompositeCurves.Editor
{
    public sealed class CompositeCurveSegmentReferenceTests
    {
        private string originalGeneratedSource;

        [SetUp]
        public void SetUp()
        {
            CompositeCurveGeneratedRegistry.Register(null);
            originalGeneratedSource = CompositeCurveTestUtility.BackupGeneratedSource();
            CompositeCurveTestUtility.CleanupTempAssets();
        }

        [TearDown]
        public void TearDown()
        {
            CompositeCurveGeneratedRegistry.Register(null);
            CompositeCurveTestUtility.CleanupTempAssets();
            CompositeCurveTestUtility.RestoreGeneratedSource(originalGeneratedSource);
        }

        [Test]
        public void CustomSegmentCanEvaluateAnotherSegmentWithItsOwnVariables()
        {
            var curve = ScriptableObject.CreateInstance<CompositeCurveDefinition>();
            var custom = new CompositeCurveSegment
            {
                DisplayName = "A",
                StartX = 0f,
                EndX = 10f,
                Mode = CompositeCurveSegmentMode.Custom,
                CustomExpression = "Ramp(x) + 1"
            };
            var ramp = new CompositeCurveSegment
            {
                DisplayName = "Ramp",
                StartX = 10f,
                EndX = 20f,
                Preset = CompositeCurvePreset.Linear
            };
            ramp.SetVariables(new[] { new CompositeCurveVariable("m", 2f), new CompositeCurveVariable("c", 1f) });

            curve.Segments.Add(custom);
            curve.Segments.Add(ramp);
            curve.RebuildRuntimeCache();

            // Mirrors what the generator emits for "Ramp(x) + 1".
            var rampId = ramp.SegmentId;
            CompositeCurveGeneratedRegistry.Register(
                delegate(string curveId, string segmentId, float x, CompositeCurveVariable[] variables, out float value)
                {
                    value = CompositeCurveGeneratedRegistry.EvaluateSegment(rampId, x) + 1f;
                    return true;
                });

            // Ramp is evaluated at x = 3 even though 3 is outside its domain.
            Assert.AreEqual(8f, curve.GetValue(3f), 0.0001f);
            Assert.AreEqual(0f, CompositeCurveGeneratedRegistry.EvaluateSegment(rampId, 3f), "no curve context outside evaluation");
        }

        [Test]
        public void RegenerateAllResolvesSegmentReferencesById()
        {
            var curve = CompositeCurveTestUtility.CreateCurveAsset("SegmentReference");
            var custom = new CompositeCurveSegment
            {
                DisplayName = "A",
                StartX = 0f,
                EndX = 1f,
                Mode = CompositeCurveSegmentMode.Custom,
                CustomExpression = "Ramp(x * 2) + sin(x)"
            };
            var ramp = new CompositeCurveSegment
            {
                DisplayName = "Ramp",
                StartX = 1f,
                EndX = 2f,
                Preset = CompositeCurvePreset.Linear
            };

            curve.Segments.Add(custom);
            curve.Segments.Add(ramp);
            EditorUtility.SetDirty(curve);
            AssetDatabase.SaveAssets();

            CompositeCurveCodeGenerator.RegenerateAll();

            StringAssert.Contains(
                "value = CompositeCurveGeneratedRegistry.EvaluateSegment(\"" + ramp.SegmentId + "\", x * 2) + Mathf.Sin(x);",
                CompositeCurveTestUtility.ReadGeneratedSource());
        }

        [Test]
        public void RegenerateAllRejectsReferenceCycles()
        {
            var curve = CompositeCurveTestUtility.CreateCurveAsset("SegmentCycle");
            var a = CreateCustom("A", "B(x)");
            var b = CreateCustom("B", "A(x) + 1");
            curve.Segments.Add(a);
            curve.Segments.Add(b);
            EditorUtility.SetDirty(curve);
            AssetDatabase.SaveAssets();

            LogAssert.Expect(LogType.Error, new Regex("'A'.*reference cycle"));
            LogAssert.Expect(LogType.Error, new Regex("'B'.*reference cycle"));
            CompositeCurveCodeGenerator.RegenerateAll();

            AssertEmitsZero(a);
            AssertEmitsZero(b);
        }

        [Test]
        public void RegenerateAllRejectsUnknownSegmentNames()
        {
            var curve = CompositeCurveTestUtility.CreateCurveAsset("SegmentUnknown");
            var a = CreateCustom("A", "Missing(x)");
            curve.Segments.Add(a);
            EditorUtility.SetDirty(curve);
            AssetDatabase.SaveAssets();

            LogAssert.Expect(LogType.Error, new Regex("unknown function or segment 'Missing'"));
            CompositeCurveCodeGenerator.RegenerateAll();

            AssertEmitsZero(a);
        }

        [Test]
        public void NestedSegmentSharesCallerRandomStream()
        {
            var curve = ScriptableObject.CreateInstance<CompositeCurveDefinition>();
            var a = CreateCustom("A", "random() + B(x) + random()");
            a.SetVariables(new[] { new CompositeCurveVariable("__seed__", 42f) });
            var b = CreateCustom("B", "random()");
            b.StartX = 10f;
            b.EndX = 20f;
            curve.Segments.Add(a);
            curve.Segments.Add(b);
            curve.RebuildRuntimeCache();

            var bId = b.SegmentId;
            CompositeCurveGeneratedRegistry.Register(
                delegate(string curveId, string segmentId, float x, CompositeCurveVariable[] variables, out float value)
                {
                    if (segmentId == bId)
                    {
                        value = CompositeCurveRandom.NextFloat();
                        return true;
                    }

                    CompositeCurveRandom.NextFloat();
                    CompositeCurveGeneratedRegistry.EvaluateSegment(bId, x);
                    value = CompositeCurveRandom.NextFloat();
                    return true;
                });

            var expected = new System.Random(42);
            expected.NextDouble();
            expected.NextDouble();
            Assert.AreEqual((float)expected.NextDouble(), curve.GetValue(0.5f), 0.000001f);
        }

        private static CompositeCurveSegment CreateCustom(string name, string expression)
        {
            return new CompositeCurveSegment
            {
                DisplayName = name,
                StartX = 0f,
                EndX = 1f,
                Mode = CompositeCurveSegmentMode.Custom,
                CustomExpression = expression
            };
        }

        private static void AssertEmitsZero(CompositeCurveSegment segment)
        {
            var pattern = "case \"" + segment.SegmentId + "\":\\s*\\{?\\s*value = 0f;";
            Assert.IsTrue(Regex.IsMatch(CompositeCurveTestUtility.ReadGeneratedSource(), pattern), $"'{segment.DisplayName}' should emit value = 0f");
        }

        [Test]
        public void SegmentNameValidation()
        {
            Assert.IsNull(CompositeCurveCodeGenerator.GetSegmentNameProblem("Ramp_2"));
            Assert.IsNotNull(CompositeCurveCodeGenerator.GetSegmentNameProblem("Custom Segment"));
            Assert.IsNotNull(CompositeCurveCodeGenerator.GetSegmentNameProblem("2Ramp"));
            Assert.IsNotNull(CompositeCurveCodeGenerator.GetSegmentNameProblem("Sin"));
            Assert.IsNotNull(CompositeCurveCodeGenerator.GetSegmentNameProblem("PI"));
        }
    }
}
