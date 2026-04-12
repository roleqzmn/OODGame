using OODGame.Entities;
using OODGame.Fight.Visitors;
using OODGame.Items;
using OODGame.Players;

namespace OODGame.Fight
{
    public class FightContext
    {
        public Player Player { get; }
        public Enemy Enemy { get; }
        public bool IsOver { get; set; }
        public bool PlayerWon { get; set; }
        public string LastLog { get; set; } = string.Empty;

        public FightContext(Player player, Enemy enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        public int GetDamage(IAttackVisitor visitor)
        {
            var weapon = Player.EItems.RightHand ?? Player.EItems.LeftHand;
            return (weapon as IWeaponCategory)?.AcceptAttack(visitor, Player.Stats)
                ?? visitor.VisitNoWeapon(Player.Stats);
        }

        public int GetDefense(IDefenseVisitor visitor)
        {
            var weapon = Player.EItems.RightHand ?? Player.EItems.LeftHand;
            return (weapon as IWeaponCategory)?.AcceptDefense(visitor, Player.Stats)
                ?? visitor.VisitNoWeapon(Player.Stats);
        }
    }
}

