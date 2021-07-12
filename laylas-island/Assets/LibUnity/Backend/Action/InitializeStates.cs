using System.Linq;
using System;
using System.Collections.Immutable;
using Libplanet.Action;
using LibUnity.Backend.State;

namespace LibUnity.Backend.Action
{
    [Serializable]
    [ActionType("initialize_states")]
    public class InitializeStates : GameAction
    {
        public Bencodex.Types.Dictionary GoldCurrency { get; set; }

        public Bencodex.Types.List GoldDistributions { get; set; }

        public InitializeStates()
        {
        }

        public InitializeStates(
            GoldCurrencyState goldCurrencyState,
            GoldDistribution[] goldDistributions)
        {
            GoldCurrency = (Bencodex.Types.Dictionary) goldCurrencyState.Serialize();
            GoldDistributions = new Bencodex.Types.List(
                goldDistributions.Select(d => d.Serialize()).Cast<Bencodex.Types.IValue>()
            );
        }

        public override IAccountStateDelta Execute(IActionContext context)
        {
            IActionContext ctx = context;
            var states = ctx.PreviousStates;

            if (ctx.Rehearsal)
            {
                states = states.SetState(Addresses.GoldCurrency, MarkChanged);
                states = states.SetState(Addresses.GoldDistribution, MarkChanged);
                return states;
            }

            if (ctx.BlockIndex != 0)
            {
                return states;
            }

            states = states
                .SetState(Addresses.GoldCurrency, GoldCurrency)
                .SetState(Addresses.GoldDistribution, GoldDistributions);

            var currency = new GoldCurrencyState(GoldCurrency).Currency;
            states = states.MintAsset(Addresses.GoldCurrency, currency * 1000000000);
            return states;
        }

        protected override IImmutableDictionary<string, Bencodex.Types.IValue> PlainValueInternal
        {
            get
            {
                var rv = ImmutableDictionary<string, Bencodex.Types.IValue>.Empty
                    .Add("gold_currency_state", GoldCurrency)
                    .Add("gold_distributions", GoldDistributions);

                return rv;
            }
        }

        protected override void LoadPlainValueInternal(IImmutableDictionary<string, Bencodex.Types.IValue> plainValue)
        {
            GoldCurrency = (Bencodex.Types.Dictionary) plainValue["gold_currency_state"];
            GoldDistributions = (Bencodex.Types.List) plainValue["gold_distributions"];
        }
    }
}
