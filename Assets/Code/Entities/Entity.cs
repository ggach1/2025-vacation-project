using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Entities
{
    public interface IEntityComponent
    {
        public void Initialize(Entity entity);
    }

    public interface IAfterInitialize
    {
        public void AfterInitialize();
    }

    public abstract class Entity : MonoBehaviour
    {
        protected Dictionary<Type, IEntityComponent> _components;

        private void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponents();
        }

        protected virtual void Start()
        {

        }

        protected virtual void AddComponents()
        {
            GetComponentsInChildren<IEntityComponent>().ToList().ForEach(compo => _components.Add(compo.GetType(), compo));
        }

        protected virtual void InitializeComponents()
        {
            _components.Values.ToList().ForEach(compo => compo.Initialize(this));
        }

        protected virtual void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitialize>().ToList().ForEach(compo => compo.AfterInitialize());
        }

        public T GetCompo<T>() where T : IEntityComponent => (T)_components.GetValueOrDefault(typeof(T));
        public IEntityComponent GetCompo(Type type) => _components.GetValueOrDefault(type);

        public void DestroyEntity()
        {
            Destroy(gameObject);
        }
    }
}