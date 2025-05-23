﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.Commands
{
    public class CreatureAttackCreatureCommand : Command
    {
        public CreatureAttackCreatureCommand(Creature attacker, Creature target, Attack attack) : this(attacker, target, attack, null)
        {
           
        }

        public CreatureAttackCreatureCommand(Creature attacker, Creature target, Attack attack, Condition condition)
        {
            Attacker = attacker;

            Target = target;

            Attack = attack;

            Condition = condition;
        }

        public Creature Attacker { get; set; }

        public Creature Target { get; set; }

        public Attack Attack { get; set; }

        public Condition Condition { get; set; }

        public override async Promise Execute()
        {
            if (Attack is DamageAttack)
            {
                if (Attacker is Player attacker1 && Target is Player target1)
                {
                    if (attacker1 != target1)
                    {
                        if (target1.Combat.Attacked(attacker1) )
                        {
                            await Context.AddCommand(new CreatureAddConditionCommand(attacker1, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                        }
                        else
                        {
                            if (attacker1.Combat.CanAttack(target1) )
                            {
                                foreach (var observer in Context.Server.Map.GetObserversOfTypePlayer(attacker1.Tile.Position) )
                                {
                                    byte clientIndex;

                                    if (observer.Client.TryGetIndex(attacker1, out clientIndex) )
                                    {
                                        Context.AddPacket(observer, new SetSkullIconOutgoingPacket(attacker1.Id, observer.Client.GetClientSkullIcon(attacker1) ) );
                                    }
                                }
                            }

                            await Context.AddCommand(new CreatureAddConditionCommand(attacker1, new ProtectionZoneBlockCondition(TimeSpan.FromSeconds(Context.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                        }

                        await Context.AddCommand(new CreatureAddConditionCommand(target1, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                    }
                }
                else if (Attacker is Player attacker2)
                {
                    await Context.AddCommand(new CreatureAddConditionCommand(attacker2, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                }
                else if (Target is Player target2)
                {                    
                    await Context.AddCommand(new CreatureAddConditionCommand(target2, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                }
            }

            (int damage, BlockType blockType, HashSet<Item> removeCharges) = Attack.Calculate(Attacker, Target);

            if (Context.Server.Config.GameplayRemoveArmorCharges)
            {
                if (removeCharges != null)
                {
                    foreach (var item in removeCharges)
                    {
                        item.Charges--;

                        if (item.Charges == 0)
                        {
                            await Context.AddCommand(new ItemDestroyCommand(item) );
                        }
                    }
                }
            }

            if (damage == 0)
            {
                await Attack.NoDamage(Attacker, Target, blockType);
            }
            else
            {
                await Attack.Damage(Attacker, Target, damage);

                if (Condition != null)
                {
                    await Context.AddCommand(new CreatureAddConditionCommand(Target, Condition) );
                }
            }            
        }
    }
}