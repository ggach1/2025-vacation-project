using UnityEngine;

namespace Code.System.EventBus
{
    public interface IEvent { }

    public static class Bus<T> where T : class
    {
        public delegate void Event(T evt);

        public static Event onEvent;
        public static void Raise(T evt) => onEvent?.Invoke(evt);
    }
}