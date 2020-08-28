using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Spx.Collections.UnitTests.Utils;
using Spx.Collections.Weak;
using Xunit;

namespace Spx.Collections.UnitTests
{
    public class WeakSetTest
    {
        [Fact]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakSet_CreateSet_SetIsEmpty()
        {
            // ---- Arrange
            var ws = new WeakSet<object>();

            // ---- Assert
            Assert.Empty(ws);
        }

        [Fact]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakSet_CheckIsReadOnly_IsNotReadOnly()
        {
            // ---- Arrange
            var ws = new WeakSet<object>();

            // ---- Assert
            Assert.False(ws.IsReadOnly);
        }

        [Fact]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public void WeakSet_AddItem_ItemWasAddedToSet()
        {
            // ---- Arrange
            var item = new object();
            var ws = new WeakSet<object>();

            // ---- Act
            ws.Add(item);

            // ---- Assert
            Assert.Contains(item, ws);
        }

        [Fact]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public void WeakSet_AddItemTwice_ItemAddedOnce()
        {
            // ---- Arrange
            var item = new object();
            var ws = new WeakSet<object>();

            // ---- Act
            ws.Add(item);
            ws.Add(item);

            // ---- Assert
            Assert.Single(ws);
            Assert.Contains(item, ws);
        }

        [Fact]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public void WeakSet_AddNull_NullInSet()
        {
            // ---- Arrange
            var ws = new WeakSet<object>();

            // ---- Act
            ws.Add(null);

            // ---- Assert
            Assert.Contains(null, ws);
        }

        [Fact]
        public void WeakSet_AddItemToSetInterface_AddItemAndReturnTrue()
        {
            // ---- Arrange
            var item = new object();
            ISet<object> ws = new WeakSet<object>();

            // ---- Act
            var result = ws.Add(item);

            // ---- Assert
            Assert.True(result);
            Assert.Single(ws);
            Assert.Contains(item, ws);
        }

        [Fact]
        public void WeakSet_AddItemTwiceToSetInterface_OnlyOneItemInSet()
        {
            // ---- Arrange
            var item = new object();
            ISet<object> ws = new WeakSet<object>();

            // ---- Act
            var firstResult = ws.Add(item);
            var secondResult = ws.Add(item);

            // ---- Assert
            Assert.Single(ws);
            Assert.Contains(item, ws);
            Assert.True(firstResult);
            Assert.False(secondResult);
        }

        [Fact]
        public void WeakSet_AddWeakItemToSet_WeakItemRemoved()
        {
            // ---- Arrange
            var ws = new WeakSet<object>();

            // ---- Act
            TestUtils.RunIt(() =>
            {
                var weakItem = new object();
                ws.Add(weakItem);

                // ---- Assert
                Assert.Single(ws);
                Assert.Contains(weakItem, ws);
            });

            GC.Collect();

            // ---- Assert
            Assert.Empty(ws);
        }

        [Fact]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public void WeakSet_AddItemsToSet_OnlyWeakRefRemoved()
        {
            // ---- Arrange
            var item = new DummyEntity(0);
            var ws = new WeakSet<DummyEntity>();

            // ---- Act
            ws.Add(null);
            ws.Add(item);
            TestUtils.RunIt(() =>
            {
                var weakItem = new DummyEntity(1);
                ws.Add(weakItem);

                // ---- Assert
                Assert.Contains(weakItem, ws);
                Assert.Equal(3, ws.Count);
            });

            GC.Collect();

            // ---- Assert
            Assert.Equal(2, ws.Count);
            Assert.Contains(item, ws);
            Assert.Contains(null, ws);
        }
    }
}