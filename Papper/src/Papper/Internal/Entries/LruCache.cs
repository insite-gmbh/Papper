using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Papper.Internal
{
    internal class LruCache : IDisposable
    {
        private readonly Dictionary<string, LruState> _states = new();

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out LruState state) 
            => _states.TryGetValue(key, out state);

        public LruState Create(string key, Memory<byte> data, DateTime detect, int validationTime)
        {
            var state = new LruState(data, detect, validationTime);
            _states.Add(key, state);
            return state;
        }

        public LruState Create(string key, object? data, DateTime detect, int validationTime)
        {
            var state = new LruState(data, detect, validationTime);
            _states.Add(key, state);
            return state;
        }

        public static void Update(LruState state, DateTime detect) => state?.ApplyUsage(detect);

        public static void Update(LruState state, Memory<byte> data, DateTime detect) => state?.ApplyChange(data, detect);

        public static void Update(LruState state, object? data, DateTime detect) => state?.ApplyChange(data, detect);

        public void RemoveUnused(DateTime detect)
        {
            _states.Where(x => x.Value.IsOutdated(detect)).ToList()
                   .Where(state => _states.Remove(state.Key)).ToList()
                   .ForEach(state => state.Value.Dispose());
        }

        public void Dispose()
        {
            _states.ToList()
                   .Where(state => _states.Remove(state.Key)).ToList()
                   .ForEach(state => state.Value.Dispose());
        }
    }
}
