using NUnit.Framework;
using UnityEngine;

namespace CompositeCurves.Editor
{
    public sealed class CompositeCurveCompareTests
    {
        [Test]
        public void IdenticalCurvesAreSame()
        {
            var result = CompositeCurveCompareWindow.Compare(CreateLinear(2f), CreateLinear(2f), 0f, 1f, 64, 0.0001f);

            Assert.IsTrue(result.Same);
        }

        [Test]
        public void DifferentCurvesReportLargestDifference()
        {
            var result = CompositeCurveCompareWindow.Compare(CreateLinear(2f), CreateLinear(3f), 0f, 1f, 64, 0.0001f);

            Assert.IsFalse(result.Same);
            Assert.AreEqual(1f, result.MaxDifference, 0.0001f);
            Assert.AreEqual(1f, result.X[result.MaxDifferenceIndex], 0.0001f);
        }

        [Test]
        public void BoundaryInclusionDifferenceIsDetectedBetweenUniformSamples()
        {
            // Both curves step from 0 to 1 at x = 1, but A's step includes x = 1 and B's doesn't.
            var a = CreateStep(CompositeCurveBoundaryInclusion.Exclusive, CompositeCurveBoundaryInclusion.Inclusive);
            var b = CreateStep(CompositeCurveBoundaryInclusion.Inclusive, CompositeCurveBoundaryInclusion.Exclusive);

            // 4 uniform samples (0, 0.67, 1.33, 2) never land on x = 1.
            var result = CompositeCurveCompareWindow.Compare(a, b, 0f, 2f, 4, 0.0001f);

            Assert.IsFalse(result.Same);
            Assert.AreEqual(1f, result.X[result.MaxDifferenceIndex], 0.0001f);
        }

        private static CompositeCurveDefinition CreateLinear(float slope)
        {
            var curve = ScriptableObject.CreateInstance<CompositeCurveDefinition>();
            var segment = new CompositeCurveSegment { StartX = 0f, EndX = 1f, Preset = CompositeCurvePreset.Linear };
            segment.SetVariables(new[] { new CompositeCurveVariable("m", slope), new CompositeCurveVariable("c", 0f) });
            curve.Segments.Add(segment);
            return curve;
        }

        private static CompositeCurveDefinition CreateStep(CompositeCurveBoundaryInclusion lowEnd, CompositeCurveBoundaryInclusion highStart)
        {
            var curve = ScriptableObject.CreateInstance<CompositeCurveDefinition>();
            var low = new CompositeCurveSegment { StartX = 0f, EndX = 1f, EndInclusion = lowEnd, Preset = CompositeCurvePreset.Constant };
            low.SetVariables(new[] { new CompositeCurveVariable("y", 0f) });
            var high = new CompositeCurveSegment { StartX = 1f, EndX = 2f, StartInclusion = highStart, EndInclusion = CompositeCurveBoundaryInclusion.Inclusive, Preset = CompositeCurvePreset.Constant };
            high.SetVariables(new[] { new CompositeCurveVariable("y", 1f) });
            curve.Segments.Add(low);
            curve.Segments.Add(high);
            return curve;
        }
    }
}
