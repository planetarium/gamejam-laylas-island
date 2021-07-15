using System;
using LaylasIsland.Backend.Extensions;
using Libplanet;
using UnityEngine;

namespace LaylasIsland.Frontend.Game
{
    using UniRx;

    [Serializable]
    public class Player : IDisposable
    {
        public readonly ReactiveProperty<Address> address
            = new ReactiveProperty<Address>();

        public readonly ReactiveProperty<string> nickname
            = new ReactiveProperty<string>(string.Empty);

        public readonly IReadOnlyReactiveProperty<string> nicknameWithHex;

        public readonly ReactiveProperty<string> portrait
            = new ReactiveProperty<string>(string.Empty);
        
        public Player(Address address)
        {
            // Subscribe
            this.address.Subscribe(_ => UpdateNickname());

            nicknameWithHex = nickname.AsObservable()
                .Select(value =>
                {
                    var hex = this.address.Value.ToSimpleHex();
                    return $"{value} {hex}";
                })
                .ToReactiveProperty(string.Empty);
            // ~Subscribe
            
            this.address.Value = address;
        }

        public Player(string nicknameWithHex, string portrait)
        {
            this.nicknameWithHex = new ReactiveProperty<string>()
                .ToReadOnlyReactiveProperty(nicknameWithHex);
            this.portrait.Value = portrait;
        }

        ~Player()
        {
            Dispose();
        }

        private static string GetNicknameKey(Address address) =>
            $"{address.ToSimpleHex()}-nickname";

        

        private void UpdateNickname()
        {
            var key = GetNicknameKey(address.Value);
            nickname.Value = PlayerPrefs.HasKey(key)
                ? PlayerPrefs.GetString(key)
                : "Player";
        }

        public void Dispose()
        {
            address?.Dispose();
            nickname?.Dispose();
            portrait?.Dispose();

            if (nicknameWithHex is IDisposable nicknameWithHexDisposable)
            {
                nicknameWithHexDisposable.Dispose();
            }
        }
    }
}
