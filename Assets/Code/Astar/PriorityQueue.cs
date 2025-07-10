using System.Collections.Generic;

namespace Code.Astar
{
    public class PriorityQueue<T>
    {
        List<(T item, int priority)> elements = new();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIdx = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIdx].priority)
                {
                    bestIdx = i;
                }
            }

            T bestItem = elements[bestIdx].item;
            elements.RemoveAt(bestIdx);
            return bestItem;
        }

        public bool Contains(T item)
        {
            foreach (var element in elements)
            {
                if (EqualityComparer<T>.Default.Equals(element.item, item))
                    return true;
            }
            return false;
        }
    }
}

