using OODGame.Items.Decorators;
using System;

namespace OODGame.Items.Weapons
{
    public static class WeaponFactory
    {
        private static readonly Random _random = new Random();

        public static Weapon CreateRandom() =>
            _random.Next(3) switch
            {
                0 => CreateRandomSword(),
                1 => CreateRandomCrossbow(),
                _ => CreateRandomAxe()
            };

        public static Weapon CreateRandomSword()
        {
            Weapon w = new Sword
            {
                Damage = (Int16)_random.Next(8,  16),
                Range  = (Int16)_random.Next(1,  3),
                Weight = (Int16)_random.Next(1,  3),
                MinLvl = 0,
            };
            return ApplyRandomDecorators(w);
        }

        public static Weapon CreateRandomCrossbow()
        {
            Weapon w = new Crossbow
            {
                Damage = (Int16)_random.Next(6,  12),
                Range  = (Int16)_random.Next(2,  5),
                Weight = (Int16)_random.Next(1,  2),
                MinLvl = 0,
            };
            return ApplyRandomDecorators(w);
        }

        public static Weapon CreateRandomAxe()
        {
            Weapon w = new TwoHandedAxe
            {
                Damage = (Int16)_random.Next(18, 30),
                Range  = 1,
                Weight = (Int16)_random.Next(2,  4),
                MinLvl = 0,
            };
            return ApplyRandomDecorators(w);
        }

        private static Weapon ApplyRandomDecorators(Weapon w)
        {
            if (_random.Next(3) == 0) w = new StrongDecorator(w);
            if (_random.Next(3) == 0) w = new UnluckyDecorator(w);
            if (_random.Next(3) == 0) w = new ProtectiveDecorator(w);
            return w;
        }
    }
}
