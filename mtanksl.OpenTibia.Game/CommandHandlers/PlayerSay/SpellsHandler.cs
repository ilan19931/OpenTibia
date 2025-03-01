﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Game.Components;
using OpenTibia.Game.Plugins;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Linq;

namespace OpenTibia.Game.CommandHandlers
{
    public class SpellsHandler : CommandHandler<PlayerSayCommand>
    {
        public override async Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            Creature target = null;

            SpellPlugin plugin = null;

            int index = command.Message.IndexOf(" \"");

            if (index == -1)
            {
                PlayerThinkBehaviour playerThinkBehaviour = Context.Server.GameObjectComponents.GetComponent<PlayerThinkBehaviour>(command.Player);

                if (playerThinkBehaviour != null)
                {
                    target = playerThinkBehaviour.Target;
                }

                plugin = Context.Server.Plugins.GetSpellPlugin(false, command.Message);
            }
            else
            {
                string name = command.Message.Substring(index + 2).TrimEnd('\"');

                Creature observer = Context.Server.GameObjects.GetPlayerByName(name);

                if (observer != null)
                {
                    target = observer;

                    plugin = Context.Server.Plugins.GetSpellPlugin(true, command.Message.Substring(0, index) );
                }
            }

            if (plugin != null)
            {
                if (command.Player.Rank != Rank.Gamemaster)
                {
                    if (plugin.Spell.Group == "Attack" && command.Player.Combat.GetSkullIcon(null) == SkullIcon.Black)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.SorryNotPossible) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }

                    if (plugin.Spell.Vocations != null && !plugin.Spell.Vocations.Contains(command.Player.Vocation) )
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YourVocationCannotUseThisSpell) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }

                    if (Context.Server.Config.GameplayLearnSpellFirst)
                    {
                        if ( !command.Player.Spells.HasSpell(plugin.Spell.Name) )
                        {
                            Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouNeedToLearnThisSpellFirst) );

                            await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                            await Promise.Break;
                        }
                    }

                    if (plugin.Spell.Premium && !command.Player.Premium)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouNeedAPremiumAccount) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }

                    if (command.Player.Level < plugin.Spell.Level)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouDoNotHaveEnoughLevel) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }
                 
                    if (command.Player.Mana < plugin.Spell.Mana)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouDoNotHaveEnoughMana) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }

                    if (command.Player.Soul < plugin.Spell.Soul)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouDoNotHaveEnoughSoul) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }
                }

                if (plugin.Spell.Group == "Attack")
                {
                    if (command.Player.Tile.ProtectionZone)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.ThisActionIsNotPermittedInAProtectionZone) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }

                    if (target is Npc || (target is Player player && (player.Rank == Rank.Gamemaster || player.Rank == Rank.AccountManager || Context.Server.Config.GameplayWorldType == WorldType.NonPvp || player.Level <= Context.Server.Config.GameplayProtectionLevel || command.Player.Level <= Context.Server.Config.GameplayProtectionLevel) ) )
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouMayNotAttackThisCreature) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }

                    if (target is Player player2 && command.Player.Client.SafeMode == SafeMode.YouCannotAttackUnmarkedCharacter && player2.Combat.GetSkullIcon(null) == SkullIcon.None)
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.TurnSecureModeOffIfYouReallyWantToAttackUnmarkedPlayers) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff));

                        await Promise.Break;
                    }
                }

                PlayerCooldownBehaviour playerCooldownBehaviour = Context.Server.GameObjectComponents.GetComponent<PlayerCooldownBehaviour>(command.Player);

                if (playerCooldownBehaviour != null)
                {
                    if (playerCooldownBehaviour.HasCooldown(plugin.Spell.Name) || playerCooldownBehaviour.HasCooldown(plugin.Spell.Group) )
                    {
                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouAreExhausted) );

                        await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                        await Promise.Break;
                    }
                }

                if ( !await plugin.OnCasting(command.Player, target, command.Message) )
                {
                    Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.SorryNotPossible) );

                    await Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );

                    await Promise.Break;
                }

                playerCooldownBehaviour.AddCooldown(plugin.Spell.Name, plugin.Spell.Cooldown);

                playerCooldownBehaviour.AddCooldown(plugin.Spell.Group, plugin.Spell.GroupCooldown);

                if (plugin.Spell.Mana > 0)
                {
                    await Context.AddCommand(new PlayerAddSkillPointsCommand(command.Player, Skill.MagicLevel, (ulong)plugin.Spell.Mana) );

                    await Context.AddCommand(new PlayerUpdateManaCommand(command.Player, command.Player.Mana - plugin.Spell.Mana) );
                }

                if (plugin.Spell.Soul > 0)
                {
                    await Context.AddCommand(new PlayerUpdateSoulCommand(command.Player, command.Player.Soul - plugin.Spell.Soul) );
                }

                await plugin.OnCast(command.Player, target, command.Message);
            }

            await next();
        }
    }
}