﻿using AzureSearchToolkit.IntegrationTest.Models;
using AzureSearchToolkit.IntegrationTest.Utilities;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AzureSearchToolkit.IntegrationTest.Tests
{
    [Collection("QueryTestCollection 1")]
    public class SpatialTests
    {
        static readonly GeographyPoint filterPoint = GeographyPoint.Create(-21.18442142, -128.12241032);


        [Fact]
        public void SpatialOrderByDistance()
        {
            DataAssert.SameSequence(
                DataAssert.Data.SearchQuery<Listing>()
                    .Where(q => q.Place != null)
                    .OrderBy(w => AzureSearchMethods.Distance(w.Place, filterPoint)).Take(10).ToList(),
                DataAssert.Data.Memory<Listing>()
                    .Where(q => q.Place != null)
                    .OrderBy(w => SpatialHelper.GetDistance(w.Place, filterPoint, DistanceUnit.Kilometers))
                    .Take(10)
                    .ToList()
            );
        }

        [Fact]
        public void SpatialOrderByDescendingDistance()
        {
            var expect = DataAssert.Data.SearchQuery<Listing>()
                .Where(q => q.Place != null)
                .OrderByDescending(w => AzureSearchMethods.Distance(w.Place, filterPoint)).Take(10).ToList();
            var actual = DataAssert.Data.Memory<Listing>()
                    .Where(q => q.Place != null)
                    .OrderByDescending(w => SpatialHelper.GetDistance(w.Place, filterPoint, double.MaxValue, DistanceUnit.Kilometers))
                    .Take(10)
                    .ToList();

            DataAssert.SameSequence(
                expect,
                actual
            );
        }

        [Fact]
        public void SpatialFilterByDistance()
        {
            DataAssert.SameSequence(
                DataAssert.Data.SearchQuery<Listing>()
                    .Where(w => AzureSearchMethods.Distance(w.Place, filterPoint) < 10000).OrderBy(q => q.CreatedAt).ToList(),
                DataAssert.Data.Memory<Listing>()
                    .Where(w => w.Place != null && SpatialHelper.GetDistance(w.Place, filterPoint, DistanceUnit.Kilometers) < 10000)
                    .OrderBy(q => q.CreatedAt)
                    .ToList()
            );
        }
    }
}
