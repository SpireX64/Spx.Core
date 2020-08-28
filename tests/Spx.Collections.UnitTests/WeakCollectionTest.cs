using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Spx.Collections.UnitTests.Utils;
using Spx.Collections.Weak;
using Xunit;

namespace Spx.Collections.UnitTests
{
    public class WeakCollectionTest
    {
        [Fact]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakCollection_CreateCollection_WillBeEmpty()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object>();

            // ---- Assert
            Assert.Empty(weakCollection);
        }

        [Fact]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakCollection_CheckCollectionIsReadOnly_IsNotReadOnly()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object>();

            // ---- Assert
            Assert.False(weakCollection.IsReadOnly);
        }

        [Fact]
        public void WeakCollection_InitializeCollection_CollectionWithItems()
        {
            // ---- Arrange & Act
            var item1 = new DummyEntity(0);
            var item2 = new DummyEntity(1);
            var weakCollection = new WeakCollection<DummyEntity>
            {
                item1,
                item2
            };

            // ---- Assert
            Assert.Contains(item1, weakCollection);
            Assert.Contains(item2, weakCollection);
        }


        [Fact]
        public void WeakCollection_AddItem_ItemInCollection()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<DummyEntity>();
            var item = new DummyEntity(0);

            // ---- Act
            weakCollection.Add(item);

            // ---- Assert
            Assert.Single(weakCollection);
            Assert.Contains(item, weakCollection);
        }

        [Fact]
        [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
        public void WeakCollection_AddNull_NullValueInCollection()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<DummyEntity>();

            // ---- Act
            weakCollection.Add(null);

            // ---- Assert
            Assert.Single(weakCollection);
            Assert.Contains(null, weakCollection);
        }

        [Fact]
        public void WeakCollection_AddWeakItem_WeakRefRemoved()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object>();

            // ---- Act
            TestUtils.RunIt(() =>
            {
                var weakItem = new object();
                weakCollection.Add(weakItem);
                Assert.Single(weakCollection);
            });

            GC.Collect();

            // ---- Assert
            Assert.DoesNotContain(null, weakCollection);
            Assert.Empty(weakCollection);
        }

        [Fact]
        public void WeakCollection_AddItems_OnlyWeakItemRemoved()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object>();
            var item = new object();

            // ---- Act
            TestUtils.RunIt(() =>
            {
                var weakItem = new object();
                weakCollection.Add(item);
                weakCollection.Add(null);
                weakCollection.Add(weakItem);

                Assert.Equal(3, weakCollection.Count);
            });

            GC.Collect();

            // ---- Assert
            Assert.Equal(2, weakCollection.Count);
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollection_ClearCollection_CollectionEmptyAndUpdatedAfterClear()
        {
            // ---- Arrange
            var items = new[]
            {
                new DummyEntity(1),
                new DummyEntity(2),
                new DummyEntity(3),
            };
            var weakCollection = new WeakCollection<DummyEntity> {items[0], items[1], items[2]};
            var enumerator = weakCollection.GetEnumerator();
            Assert.Equal(items.Length, weakCollection.Count);

            // ---- Act
            weakCollection.Clear();

            // ---- Assert
            Assert.Empty(weakCollection);
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakCollection_ClearEmptyCollection_CollectionNotUpdated()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object>();
            var enumerator = weakCollection.GetEnumerator();

            // ---- Act
            weakCollection.Clear();

            // ---- Assert
            Assert.Empty(weakCollection);
            Assert.False(enumerator.MoveNext()); // Throws InvalidOperationException is collection was updated
        }

        [Fact]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakCollection_RemoveNotExistsItem_ReturnsFalse()
        {
            // ---- Arrange
            var item = new object();
            var weakCollection = new WeakCollection<object> {item};

            // ---- Assert
            Assert.False(weakCollection.Remove(null));
        }

        [Fact]
        public void WeakCollection_RemoveExistsItem_ItemWasRemoved()
        {
            // ---- Arrange
            var item = new object();
            var weakCollection = new WeakCollection<object> {item};

            // ---- Assert
            Assert.NotEmpty(weakCollection);
            Assert.True(weakCollection.Remove(item));
            Assert.Empty(weakCollection);
        }

        [Fact]
        public void WeakCollection_CopyToArray_ItemsWasCopiedToArray()
        {
            // ---- Arrange
            var items = new[]
            {
                new DummyEntity(0),
                new DummyEntity(1),
                new DummyEntity(2),
            };
            var specialItem = new DummyEntity(3);

            var weakCollection = new WeakCollection<DummyEntity>
            {
                items[0],
                items[1],
                items[2],
            };
            var targetArray = new[] {specialItem, null, null, null};

            // ---- Act
            weakCollection.CopyTo(targetArray, 1);

            // ---- Assert
            Assert.Equal(specialItem, targetArray[0]);
            Assert.Contains(items[0], targetArray);
            Assert.Contains(items[1], targetArray);
            Assert.Contains(items[2], targetArray);
        }

        [Fact]
        public void WeakCollection_CopyToNull_ThrowException()
        {
            // ---- Arrange
            var item = new object();
            var weakCollection = new WeakCollection<object> {item};

            // ---- Assert
            Assert.Throws<ArgumentNullException>(
                () => weakCollection.CopyTo(null!, 0));
        }

        [Fact]
        public void WeakCollection_CopyToNegativeIndex_ThrowException()
        {
            // ---- Arrange
            var item = new object();
            var weakCollection = new WeakCollection<object> {item};
            var targetArray = new object[] {null, null};

            // ---- Assert
            Assert.Throws<ArgumentOutOfRangeException>(
                () => weakCollection.CopyTo(targetArray, -1));
        }

        [Fact]
        public void WeakCollection_CopyToSmallArray_ThrowException()
        {
            // ---- Arrange
            var item = new DummyEntity(0);
            var weakCollection = new WeakCollection<DummyEntity> {item};
            var targetArray = new DummyEntity[] {null};

            // ---- Assert
            Assert.Throws<ArgumentException>(
                () => weakCollection.CopyTo(targetArray, 1));
        }

        [Fact]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        public void WeakCollection_ContainsInEmptyCollection_CollectionNotContainsItem()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<DummyEntity>();
            var someItem = new DummyEntity(1);

            // ---- Assert
            Assert.DoesNotContain(someItem, weakCollection);
            Assert.DoesNotContain(null, weakCollection);
        }

        [Fact]
        public void WeakCollection_CheckContainsNull_CollectionContainsNull()
        {
            // ---- Arrange & Act
            var weakCollection = new WeakCollection<object> {null};

            // ---- Assert
            Assert.Contains(null, weakCollection);
        }

        /**
         * Тест покрывает условие, в котором выполняется проверка наличия
         * элемента в коллекции, когда в коллекции имеются мертвые ссылки
         */
        [Fact]
        public void WeakCollection_ContainsWithDeadRef_TestPassed()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object> {null};
            var otherObject = new object();

            // ---- Act
            TestUtils.RunIt(() => { weakCollection.Add(new object()); });

            // ---- Assert
            Assert.Contains(null, weakCollection);
            Assert.DoesNotContain(otherObject, weakCollection);
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_TryGetValueBeforeIterate_WillThrowException()
        {
            // ---- Arrage
            var weakCollection = new WeakCollection<object> {null};
            var enumerator = weakCollection.GetEnumerator();

            // ---- Assert
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_IterateAfterCollectionUpdate_WillThrowException()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object> {null};
            var enumerator = weakCollection.GetEnumerator();
            var item = new object();

            // ---- Act
            weakCollection.Add(item);

            // ---- Assert
            Assert.Throws<InvalidOperationException>(() => { enumerator.MoveNext(); });
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_GetValueAfterIterationFinished_WillThrowException()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object> {null};
            var enumerator = weakCollection.GetEnumerator();

            // ---- Act
            while (enumerator.MoveNext())
            {
                /* Nothing */
            }

            // ---- Assert
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public void WeakCollectionEnumerator_IterateNonGenericEnumerator_GenericAndNonGenericValuesAreEqual()
        {
            // ---- Arrange
            var item = new DummyEntity(0);
            var weakCollection = new WeakCollection<DummyEntity> {item};
            var genericEnumerator = weakCollection.GetEnumerator();
            IEnumerator nonGenericEnumerator = genericEnumerator;

            // ---- Act
            genericEnumerator.MoveNext();

            // ---- Assert
            Assert.Equal(genericEnumerator.Current, nonGenericEnumerator.Current);
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_MoveAfterIterationFinished_AlwaysReturnsFalse()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object> {null};
            var enumerator = weakCollection.GetEnumerator();

            // ---- Act
            while (enumerator.MoveNext())
            {
                /* Nothing */
            }

            // ---- Assert
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_ResetEnumerator_CanIterateAfterReset()
        {
            // ---- Arrange
            var weakCollection = new WeakCollection<object> {null};
            var enumerator = weakCollection.GetEnumerator();

            // ---- Act
            while (enumerator.MoveNext())
            {
                /* Nothing */
            }

            enumerator.Reset();

            // ---- Assert
            Assert.True(enumerator.MoveNext());
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_ResetEnumerator_WillReturnSameElements()
        {
            // ---- Arrange
            var item1 = new DummyEntity(1);
            var item2 = new DummyEntity(2);
            var weakCollection = new WeakCollection<DummyEntity> {item1, item2, null};
            var enumerator = weakCollection.GetEnumerator();

            // ---- Act
            enumerator.MoveNext();
            var iterItem1 = enumerator.Current;
            enumerator.MoveNext();
            var iterItem2 = enumerator.Current;

            enumerator.Reset();

            // ---- Assert
            Assert.True(enumerator.MoveNext());
            Assert.Equal(iterItem1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(iterItem2, enumerator.Current);
        }

        [Fact]
        [SuppressMessage("ReSharper", "GenericEnumeratorNotDisposed")]
        public void WeakCollectionEnumerator_ResetEnumeratorAfterCollectionUpdate_ThrowsException()
        {
            // ---- Arrange
            var someItem = new object();
            var weakCollection = new WeakCollection<object> {someItem};
            var enumerator = weakCollection.GetEnumerator();

            // ---- Act
            while (enumerator.MoveNext())
            {
                /* Nothing */
            }

            weakCollection.Add(null);

            // ---- Assert
            Assert.Throws<InvalidOperationException>(() => enumerator.Reset());
        }
    }
}