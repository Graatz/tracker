using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracker.Helpers;
using Tracker.Models;
using System.IO;
using System.Collections.Generic;

namespace Tracker.UnitTests
{
    [TestClass]
    public class GeoMathTest
    {
        GpxParser parser;
        List<TrackPoint> data;

        public GeoMathTest()
        {
            parser = new GpxParser();
            data = parser.Parse(File.OpenRead(System.IO.Path.GetFullPath(@"..\..\") + @"\gpx\test.gpx"));
        }

        [TestMethod]
        public void CalculateTrackDistanceTest()
        {
            var result = GeoMath.CalculateTrackDistance(data);

            Assert.AreEqual(12.7, Math.Round(result, 1));
        }

        [TestMethod]
        public void CalculateAverageTrackSpeedTest()
        {
            var result = GeoMath.CalculateAvarageTrackSpeed(data);

            Assert.AreEqual(3.58, Math.Round(result, 2));
        }

        [TestMethod]
        public void CalculateTrackTimeTest()
        {
            var result = GeoMath.CalculateTrackTime(data);

            Assert.AreEqual(213, Math.Round(result));
        }

        [TestMethod]
        public void GetPointsCloseToTrackTest()
        {
            var expected = data.Count;
            var distance = 0.000001;
            var timeSpan = new TimeSpan(1, 0, 0);
            var result = GeoMath.GetPointsCloseToTrack(new List<TrackPoint>(data), new List<TrackPoint>(data), distance, timeSpan).Count;

            Assert.AreEqual(expected, result);
        }
    }
}
