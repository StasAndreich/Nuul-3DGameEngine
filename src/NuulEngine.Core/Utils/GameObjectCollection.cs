using System.Collections;
using System.Collections.Generic;

namespace NuulEngine.Core.Utils
{
    /// <summary>
    /// A collection of GameObject elements.
    /// </summary>
    public sealed class GameObjectCollection : ICollection<GameObject>
    {
        private readonly List<GameObject> _objects;

        public int Count => _objects.Count;

        public bool IsReadOnly => false;

        public void Add(GameObject item)
        {
            _objects.Add(item);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public bool Contains(GameObject item)
        {
            if (_objects.Contains(item))
            {
                return true;
            }

            return false;
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            _objects.CopyTo(array, arrayIndex);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            foreach (var root in _objects)
            {
                yield return root;
            }
        }

        public bool Remove(GameObject item)
        {
            // TODO.
        }

        public bool Remove(string tag)
        {
            // TODO.
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
