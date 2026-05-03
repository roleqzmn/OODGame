using OODGame.Items;
using OODGame.Map;
using OODGame.Entities;
using System;
using System.Collections.Generic;
using OODGame.Players;
using OODGame.Actions;
using OODGame;
namespace OODGame.Logger
{
    public sealed interface IEventLogger<TIn>
    {
        public void LogEvent(TIn message);

    }
}