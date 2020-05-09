﻿using System.Linq;
using SlideDotNet.Enums;
using SlideDotNet.Models;
using Xunit;
using A = DocumentFormat.OpenXml.Drawing;

// ReSharper disable TooManyDeclarations
// ReSharper disable InconsistentNaming
// ReSharper disable TooManyChainedReferences

namespace SlideDotNet.Tests
{
    public class TestFile_025
    {
        [Fact]
        public void Chart_Test()
        {
            // Arrange
            var pre = new PresentationEx(Properties.Resources._025);
            var sld1 = pre.Slides[0];
            var sld2 = pre.Slides[1];
            var chart8 = sld1.Shapes.First(x => x.Id == 8).Chart;
            var chart4 = sld1.Shapes.First(x => x.Id == 4).Chart;
            var chart5 = sld1.Shapes.First(x => x.Id == 5).Chart;
            var chart11 = sld2.Shapes.First(x => x.Id == 11).Chart;
            var chart4ChildCat = chart4.Categories[0];
            var chart5SeriesCollection = chart5.SeriesCollection;

            // Act
            var chart8HasXValues = chart8.HasXValues;
            var chart11HasXValues = chart11.HasXValues;
            var chart4ChildCatVal = chart4ChildCat.Name;
            var chart4ParentCatVal = chart4ChildCat.Parent.Name;
            var serName1 = chart5SeriesCollection[0].Name;
            var serName3 = chart5SeriesCollection[2].Name;

            // Assert
            Assert.False(chart8HasXValues);
            Assert.False(chart11HasXValues);
            Assert.Equal("Dresses", chart4ChildCatVal);
            Assert.Equal("Clothing", chart4ParentCatVal);
            Assert.Equal("Ряд1", serName1);
            Assert.Equal("Ряд3", serName3);
        }
    }
}