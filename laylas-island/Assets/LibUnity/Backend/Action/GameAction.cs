using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bencodex.Types;
using LibUnity.Backend.State;

namespace LibUnity.Backend.Action
{
    [Serializable]
    public abstract class GameAction : BaseAction
    {
        public Guid Id { get; private set; }
        public override IValue PlainValue =>
#pragma warning disable LAA1002
            new Bencodex.Types.Dictionary(
                PlainValueInternal
                    .SetItem("id", Id.Serialize())
                    .Select(kv => new KeyValuePair<IKey, IValue>((Text) kv.Key, kv.Value))
            );
#pragma warning restore LAA1002
        protected abstract IImmutableDictionary<string, IValue> PlainValueInternal { get; }

        protected GameAction()
        {
            Id = Guid.NewGuid();
        }

        public override void LoadPlainValue(IValue plainValue)
        {
#pragma warning disable LAA1002
            var dict = ((Bencodex.Types.Dictionary) plainValue)
                .Select(kv => new KeyValuePair<string, IValue>((Text) kv.Key, kv.Value))
                .ToImmutableDictionary();
#pragma warning restore LAA1002
            Id = dict["id"].ToGuid();
            LoadPlainValueInternal(dict);
        }
        
        protected abstract void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue);
    }
}
