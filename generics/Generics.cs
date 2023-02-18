using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace CDreyer.Generics
{
    #region Pools

    public abstract class GenericPool<T> where T : Component
    {
        public int amount { get; private set; }
        public T original { get; private set; }
        protected Transform parent = null;

        public GenericPool(T original, int amount, Transform parent = null)
        {
            this.original = original;
            this.amount = amount;
            this.parent = parent;

            CreateObjects(original, amount, parent);
        }

        protected abstract void CreateObjects(T original, int amount, Transform parent = null);
        public abstract T Get(Vector3 position, Quaternion rotation);
        public abstract T Get(Transform parent);
        public abstract void Release(T obj);
    }

    #region Queue Pool
    public class QueuePool<T> : GenericPool<T> where T : Component
    {
        Queue<T> q = new();

        public QueuePool(T original, int amount, Transform parent = null) : base(original, amount, parent)
        {

        }

        protected override void CreateObjects(T original, int amount, Transform parent = null)
        {
            for (int i = 0; i < amount; i++)
            {
                T obj = Object.Instantiate(original, parent);
                obj.gameObject.SetActive(false);
                q.Enqueue(obj);
            }
        }

        public override T Get(Vector3 position, Quaternion rotation)
        {
            if (q.Count == 0)
            {
                CreateObjects(original, 1, parent);
            }

            T t = q.Dequeue();

            t.transform.SetParent(null);

            t.transform.position = position;
            t.transform.rotation = rotation;

            t.gameObject.SetActive(true);

            return t;
        }

        public override T Get(Transform parent)
        {
            if (q.Count == 0)
            {
                CreateObjects(original, 1, parent);
            }

            T t = q.Dequeue();

            t.transform.SetParent(parent);

            t.transform.localPosition = Vector3.zero;
            t.transform.localEulerAngles = Vector3.zero;

            t.gameObject.SetActive(true);

            return t;
        }

        public override void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(parent);

            q.Enqueue(obj);
        }
    }

    #endregion

    #region Stack Pool
    public class StackPool<T> : GenericPool<T> where T : Component
    {
        Stack<T> s = new();

        public StackPool(T original, int amount, Transform parent = null) : base(original, amount, parent)
        {

        }

        protected override void CreateObjects(T original, int amount, Transform parent = null)
        {
            for (int i = 0; i < amount; i++)
            {
                T obj = Object.Instantiate(original, parent);
                obj.gameObject.SetActive(false);
                s.Push(obj);
            }
        }

        public override T Get(Vector3 position, Quaternion rotation)
        {
            if (s.Count == 0)
            {
                CreateObjects(original, 1, parent);
            }

            T t = s.Pop();

            t.transform.SetParent(null);

            t.transform.position = position;
            t.transform.rotation = rotation;

            t.gameObject.SetActive(true);

            return t;
        }

        public override T Get(Transform parent)
        {
            if (s.Count == 0)
            {
                CreateObjects(original, 1, parent);
            }

            T t = s.Pop();

            t.transform.SetParent(parent);

            t.transform.localPosition = Vector3.zero;
            t.transform.localEulerAngles = Vector3.zero;

            t.gameObject.SetActive(true);

            return t;
        }

        public override void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(parent);

            s.Push(obj);
        }
    }
    #endregion

    #endregion
}