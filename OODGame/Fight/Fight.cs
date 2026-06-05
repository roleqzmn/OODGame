using System;
using OODGame.Entities;
using OODGame.Fight.Actions;
using OODGame.Input;
using OODGame.Players;
using System;
using System.Collections.Generic;

namespace OODGame.Fight
{
    public class FightRunner
    {
        private readonly FightContext _ctx;
        private readonly IInputSource _inputSource;
        private readonly List<IFightAction> _actions = new List<IFightAction>
        {
            new NormalFightAction(),
            new StealthFightAction(),
            new MagicalFightAction()
        };
        private int _selectedIdx = 0;

        public FightRunner(Player player, Enemy enemy, IInputSource? inputSource = null)
        {
            _ctx = new FightContext(player, enemy);
            _inputSource = inputSource ?? new ConsoleInputSource();
        }

        public bool Run()
        {
            FightScreen.DrawInitial();
            FightScreen.Draw(_ctx, _actions, _selectedIdx);

            while (!_ctx.IsOver)
            {
                HandleInput();
                CheckFightOver();
                if (!_ctx.IsOver)
                    FightScreen.Draw(_ctx, _actions, _selectedIdx);
            }

            FightScreen.DrawResult(_ctx);
            return _ctx.PlayerWon;
        }

        private void HandleInput()
        {
            var key = _inputSource.ReadKey();
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (_selectedIdx > 0) _selectedIdx--;
                    FightScreen.Draw(_ctx, _actions, _selectedIdx);
                    break;
                case ConsoleKey.DownArrow:
                    if (_selectedIdx < _actions.Count - 1) _selectedIdx++;
                    FightScreen.Draw(_ctx, _actions, _selectedIdx);
                    break;
                case ConsoleKey.E:
                    _actions[_selectedIdx].Execute(_ctx);
                    break;
            }
        }

        private void CheckFightOver()
        {
            if (_ctx.Enemy.Health <= 0)
            {
                _ctx.IsOver = true;
                _ctx.PlayerWon = true;
            }
            else if (_ctx.Player.Stats.Health <= 0)
            {
                _ctx.IsOver = true;
                _ctx.PlayerWon = false;
            }
        }
    }
}




