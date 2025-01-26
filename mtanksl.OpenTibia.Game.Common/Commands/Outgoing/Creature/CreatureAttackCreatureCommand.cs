﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.Commands
{
    public class CreatureAttackCreatureCommand : Command
    {
        public CreatureAttackCreatureCommand(Creature attacker, Creature target, Attack attack)

            : this(attacker, target, attack, null)
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
                        ICombatCollection combats = Context.Server.Combats;

                        if (combats.ContainsDefense(attacker1.Id, target1.Id) )
                        {
                            await Context.Current.AddCommand(new CreatureAddConditionCommand(attacker1, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                        }
                        else
                        {
                            if ( !combats.ContainsOffense(attacker1.Id, target1.Id) )
                            {
                                combats.AddOffense(attacker1.Id, target1.Id);

                                combats.AddDefense(target1.Id, attacker1.Id);

                                if ( !combats.SkullContains(attacker1.Id, out _) )
                                {
                                    if ( !combats.SkullContains(target1.Id, out _) )
                                    {
                                        combats.SkullAdd(attacker1.Id, SkullIcon.White);

                                        foreach (var observer in Context.Server.Map.GetObserversOfTypePlayer(attacker1.Tile.Position) )
                                        {
                                            byte clientIndex;

                                            if (observer.Client.TryGetIndex(attacker1, out clientIndex) )
                                            {
                                                Context.AddPacket(observer, new SetSkullIconOutgoingPacket(attacker1.Id, SkullIcon.White) );
                                            }
                                        }
                                    }
                                    else
                                    {
                                        combats.YellowSkullAddOffense(attacker1.Id, target1.Id);

                                        combats.YellowSkullAddDefense(target1.Id, attacker1.Id);

                                        Context.AddPacket(target1, new SetSkullIconOutgoingPacket(attacker1.Id, SkullIcon.Yellow) );
                                    }                                
                                }
                            }

                            await Context.Current.AddCommand(new CreatureAddConditionCommand(attacker1, new ProtectionZoneBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                        }

                        await Context.Current.AddCommand(new CreatureAddConditionCommand(target1, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                    }
                }
                else if (Attacker is Player attacker2)
                {
                    await Context.Current.AddCommand(new CreatureAddConditionCommand(attacker2, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                }
                else if (Target is Player target2)
                {                    
                    await Context.Current.AddCommand(new CreatureAddConditionCommand(target2, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                }
            }

            int damage = Attack.Calculate(Attacker, Target);

            if (damage == 0)
            {
                await Attack.Missed(Attacker, Target);
            }
            else
            {
                await Attack.Hit(Attacker, Target, damage);

                if (Condition != null)
                {
                    await Context.AddCommand(new CreatureAddConditionCommand(Target, Condition) );
                }
            }            
        }
    }
}