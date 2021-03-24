﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NuulEngine.Core.Utils
{
    /// <summary>
    /// A collection of GameObject elements.
    /// </summary>
    public sealed class GameObjectCollection : ICollection<GameObject>
    {
        private readonly GameObject _owner;
        private readonly List<GameObject> _objects;
        private readonly List<GameObject> _objectsToRemove;

        public GameObjectCollection(GameObject owner)
        {
            _owner = owner;
        }

        public GameObject this[string tag]
        {
            get
            {
                GameObject result = Find(tag);
                if (result != null)
                {
                    return result;
                }

                throw new ArgumentOutOfRangeException(nameof(tag), "There is no object with that 'tag'");
            }
        }

        public int Count => _objects.Count + _objects.Sum(o => o.ChildObjects.Count);

        public bool IsReadOnly => false;

        public void Add(GameObject item)
        {
            _objects.Add(item);
            item.Parent = _owner;
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

            foreach (var @object in _objects)
            {
                if (@object.ChildObjects.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        internal void CommitRemove()
        {
            _objects.RemoveAll(o => _objectsToRemove.Contains(o));
            _objectsToRemove.Clear();

            foreach (var @object in _objects)
            {
                @object.ChildObjects.CommitRemove();
            }
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            _objects.CopyTo(array, arrayIndex);
        }

        public GameObject Find(string tag)
        {
            var resultObj = _objects.FirstOrDefault(o => o.Tag == tag);
            if (resultObj != null)
            {
                return resultObj;
            }

            foreach (var @object in _objects)
            {
                var obj = @object.ChildObjects.Find(tag);
                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            foreach (var root in _objects)
            {
                yield return root;

                foreach (var @object in _objects)
                {
                    yield return @object;
                }
            }
        }

        public bool Remove(GameObject item)
        {
            if (_objects.Contains(item))
            {
                _objectsToRemove.Add(item);
                return true;
            }

            foreach (var @object in _objects)
            {
                if (@object.ChildObjects.Remove(item))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Remove(string tag)
        {
            var objectToRemove = _objects.FirstOrDefault(o => o.Tag == tag);

            if (objectToRemove != null)
            {
                _objectsToRemove.Add(objectToRemove);
                return true;
            }

            foreach (var @object in _objects)
            {
                if (@object.ChildObjects.Remove(tag))
                {
                    return true;
                }
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}