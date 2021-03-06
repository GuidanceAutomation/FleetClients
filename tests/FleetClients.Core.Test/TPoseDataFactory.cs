﻿using FleetClients.Core;
using GAAPICommon.Core.Dtos;
using NUnit.Framework;
using System.Collections.Generic;

namespace FleetClients.Core.Test
{
    [TestFixture]
    [Category("PoseDataFactory")]
    public class TPoseDataFactory
    {
        [Test]
        [TestCase(0, 1, 2)]
        [TestCase(0, -1, -2)]
        [TestCase(1, 1.38, 1.57)]
        [TestCase(1, -1.38, -1.57)]
        public void ParseString(double x, double y, double heading)
        {
            foreach (string subString in FormattedStrings(x, y, heading))
            {
                PoseDto pose = PoseDtoFactory.ParseString(subString);

                Assert.AreEqual(x, pose.X);
                Assert.AreEqual(y, pose.Y);
                Assert.AreEqual(heading, pose.Heading);
            }
        }

        private IEnumerable<string> FormattedStrings(double x, double y, double heading)
        {
            List<string> strings = new List<string>();

            strings.Add(string.Format("x{0},y{1},heading{2}", x, y, heading));
            strings.Add(string.Format("{0},{1},{2}", x, y, heading));
            strings.Add(string.Format("x:{0},y:{1},heading:{2}", x, y, heading));
            strings.Add(string.Format("x {0},y {1},heading {2}", x, y, heading));

            return strings;
        }
    }
}