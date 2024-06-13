using FlashPlanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Models
{
    [TestClass]
    public class FixedMaxPriorityQueueTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void Can_SetSize(int size)
        {
            // ARRANGE
            // ACT
            var queue = new FixedMaxPriorityQueue<int>(size);

            // ASSERT
            Assert.AreEqual(size, queue.Size);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Size must be larger than 0!")]
        [DataRow(0)]
        [DataRow(-10)]
        public void Cant_SetSize_IfBelow1(int size)
        {
            // ARRANGE
            // ACT
            var queue = new FixedMaxPriorityQueue<int>(size);
        }

        [TestMethod]
        // Size 5
        [DataRow(5, 1u, 2u, 3u, 4u)]
        [DataRow(5, 50u, 2u, 16u)]
        [DataRow(5, 50u, 2u, 16u, 10u, 4u, 100u)]
        // Size 1
        [DataRow(1, 1u, 2u, 3u, 4u)]
        [DataRow(1, 50u, 2u, 16u)]
        [DataRow(1, 50u, 2u, 16u, 10u, 4u, 100u)]
        // Size 50
        [DataRow(50, 1u, 2u, 3u, 4u)]
        [DataRow(50, 50u, 2u, 16u)]
        [DataRow(50, 50u, 2u, 16u, 10u, 4u, 100u)]
        public void Can_EnqueueWithPriority(int size, params uint[] elements)
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<uint>(size);

            // ACT
            for (int i = 0; i < elements.Length; i++)
                queue.Enqueue(elements[i], elements[i]);

            // ASSERT
            uint prev = uint.MaxValue;
            while (queue.Count > 0)
            {
                var element = queue.Dequeue();
                Assert.IsTrue(element < prev);
                prev = element;
            }
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 1)]
        [DataRow(10, 5)]
        public void Can_EnqueueWithinSize(int size, int addElements)
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(size);

            // ACT
            for (int i = 0; i < addElements; i++)
                queue.Enqueue(i, 0);

            // ASSERT
            Assert.AreEqual(addElements, queue.Count);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(-50)]
        [DataRow(500000)]
        public void Can_Dequeue(int value)
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(1);
            queue.Enqueue(value, 0);

            // ACT
            var dequed = queue.Dequeue();

            // ASSERT
            Assert.AreEqual(value, dequed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The queue is already empty!")]
        public void Cant_Dequeue_IfQueueEmpty()
        {
            // ARRANGE
            var queue = new FixedMaxPriorityQueue<int>(1);

            // ACT
            queue.Dequeue();
        }
    }
}
