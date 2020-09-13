using System;
using System.Collections.Generic;
using Xunit;

namespace Spx.Linq.UnitTests
{
    public class LinqExtensionsTest
    {
        [Fact]
        public void LinqExtensions_FindPosition_ReturnsCorrectPosition()
        {
            // -- Arrange:
            var someList = new List<Tuple<int>>()
            {
                Tuple.Create(1),
                Tuple.Create(2),
                Tuple.Create(3),
                Tuple.Create(4),
            };

            IEnumerable<Tuple<int>> enumerable = someList;
            
            // -- Act:
            var index = enumerable.FindPosition(e => e.Item1 == 3);
            
            // -- Assert:
            Assert.Equal(2, index);
        }

        [Fact]
        public void LinqExtensions_FindPositionNotExistsItem_ReturnsNegativeValue()
        {
            // -- Arrange:
            var someList = new List<Tuple<int>>()
            {
                Tuple.Create(1),
                Tuple.Create(2),
                Tuple.Create(3),
                Tuple.Create(4),
            };

            IEnumerable<Tuple<int>> enumerable = someList;
            
            // -- Act:
            var index = enumerable.FindPosition(e => e.Item1 == 10);
            
            // -- Assert:
            Assert.Equal(-1, index); 
        }
        
        [Fact]
        public void LinqExtensions_FindPositionButEnumerableIsNull_ThrowsArgumentNull()
        {
            // -- Arrange:
            IEnumerable<Tuple<int>> enumerable = null!;
            
            // -- Act & Assert:
            Assert.Throws<ArgumentNullException>(
                () => enumerable.FindPosition(e => e.Item1 == 1));
        }

        [Fact]
        public void LinqExtensions_FindPositionButPredicateIsNull_ThrowsArgumentNull()
        {
            // -- Arrange:
            var someList = new List<Tuple<int>>();
            IEnumerable<Tuple<int>> enumerable = someList;
            
            // -- Act & Assert:
            Assert.Throws<ArgumentNullException>(
                () => enumerable.FindPosition(null));
        }
        
        [Fact]
        public void LinqExtensions_PositionOf_ReturnsCorrectPosition()
        {
            // -- Arrange:
            var someList = new List<int>()
            {
                1, 2, 3, 4
            };

            IEnumerable<int> enumerable = someList;
            
            // -- Act:
            var index = enumerable.PositionOf(3);
            
            // -- Assert:
            Assert.Equal(2, index);
        }
        
        [Fact]
        public void LinqExtensions_PositionOfButItemNotExists_ReturnsNegativePosition()
        {
            // -- Arrange:
            var someList = new List<int>()
            {
                1, 2, 3, 4
            };

            IEnumerable<int> enumerable = someList;
            
            // -- Act:
            var index = enumerable.PositionOf(10);
            
            // -- Assert:
            Assert.Equal(-1, index);
        }
        
        [Fact]
        public void LinqExtensions_PositionOfButEnumerableIsNull_ThrowsArgumentNull()
        {
            // -- Arrange:
            IEnumerable<int> enumerable = null!;
            
            // -- Act & Assert:
            Assert.Throws<ArgumentNullException>(
                () => enumerable.PositionOf(2));
        }
    }
}
