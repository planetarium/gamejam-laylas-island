﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Libplanet.Tx;

namespace LibUnity.Frontend.BlockChain
{
    public class TransactionMap
    {
        public readonly int Size;

        private readonly ConcurrentQueue<KeyValuePair<Guid, TxId>> _queue =
            new ConcurrentQueue<KeyValuePair<Guid, TxId>>();

        public TransactionMap(int size)
        {
            Size = size;
        }

        public bool TryGetValue(Guid key, out TxId value)
        {
            if (!_queue.Any(kv => kv.Key.Equals(key)))
            {
                return false;
            }

            value = _queue.FirstOrDefault(kv => kv.Key.Equals(key)).Value;
            return true;
        }

        // FIXME: Should prevent duplicated item?
        public void TryAdd(Guid key, TxId value)
        {
            _queue.Enqueue(new KeyValuePair<Guid, TxId>(key, value));
            if (_queue.Count > Size)
            {
                _queue.TryDequeue(out _);
            }
        }

        public bool ContainsKey(Guid key)
        {
            return _queue.Any(kv => kv.Key.Equals(key));
        }
    }
}
