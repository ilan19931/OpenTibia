﻿using OpenTibia.Common.Objects;
using System.Collections.Generic;

namespace OpenTibia.Game.Common.ServerObjects
{
    public interface ICombatCollection
    {
        void AddHitToTarget(Creature attacker, Creature target, int damage);
        
        Dictionary<Creature, Hit> GetHitsByTargetAndRemove(Creature target);


        bool ContainsOffense(Player attacker, Player target);

        void AddOffense(Player attacker, Player target);

        bool ContainsDefense(Player target, Player attacker);

        void AddDefense(Player target, Player attacker);

        bool WhiteSkullContains(Player attacker);

        void WhiteSkullAdd(Player attacker);

        bool YellowSkullContains(Player target, Player attacker);

        void YellowSkullAdd(Player target, Player attacker);
    }
}