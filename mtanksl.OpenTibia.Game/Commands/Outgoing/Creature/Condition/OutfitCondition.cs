﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Components;
using System;

namespace OpenTibia.Game.Commands
{
    public class OutfitCondition : Condition
    {
        private DelayBehaviour delayBehaviour;

        public OutfitCondition(Outfit outfit, TimeSpan duration) : base(ConditionSpecialCondition.Outfit)
        {
            Outfit = outfit;

            Duration = duration;
        }

        public Outfit Outfit { get; set; }

        public TimeSpan Duration { get; set; }

        public override Promise Start(Creature target)
        {
            return Context.Current.AddCommand(new CreatureUpdateOutfitCommand(target, target.BaseOutfit, Outfit) ).Then( () =>
            {
                delayBehaviour = Context.Current.Server.GameObjectComponents.AddComponent(target, new DelayBehaviour(Duration), false);

                return delayBehaviour.Promise;

            } ).Then( () =>
            {
                return Context.Current.AddCommand(new CreatureUpdateOutfitCommand(target, target.BaseOutfit, target.BaseOutfit) );
            } );
        }

        public override void Stop()
        {
            if (delayBehaviour != null)
            {
                delayBehaviour.Stop();
            }
        }
    }
}