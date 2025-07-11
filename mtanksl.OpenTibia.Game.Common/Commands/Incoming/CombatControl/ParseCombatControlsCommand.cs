﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Game.Components;

namespace OpenTibia.Game.Commands
{
    public class ParseCombatControlsCommand : IncomingCommand
    {
        public ParseCombatControlsCommand(Player player, FightMode fightMode, ChaseMode chaseMode, SafeMode safeMode, PVPMode pvpMode)
        {
            Player = player;

            FightMode = fightMode;

            ChaseMode = chaseMode;

            SafeMode = safeMode;

            PVPMode = pvpMode;
        }

        public Player Player { get; set; }

        public FightMode FightMode { get; set; }

        public ChaseMode ChaseMode { get; set; }

        public SafeMode SafeMode { get; set; }

        public PVPMode PVPMode { get; set; }

        public override Promise Execute()
        {
            if (FightMode != Player.Client.FightMode)
            {
                Player.Client.FightMode = FightMode;
            }

            if (ChaseMode != Player.Client.ChaseMode)
            {
                Player.Client.ChaseMode = ChaseMode;

                PlayerThinkBehaviour playerThinkBehaviour = Context.Server.GameObjectComponents.GetComponent<PlayerThinkBehaviour>(Player);

                if (Player.Client.ChaseMode == ChaseMode.StandWhileFighting)
                {
                    if (playerThinkBehaviour != null)
                    {
                        playerThinkBehaviour.StopFollow();                        
                    }
                }
                else
                {
                    if (playerThinkBehaviour != null)
                    {
                        playerThinkBehaviour.StartFollow();
                    }
                }
            }

            if (SafeMode != Player.Client.SafeMode)
            {
                Player.Client.SafeMode = SafeMode;
            }

            if (PVPMode != Player.Client.PVPMode)
            {
                Player.Client.PVPMode = PVPMode;
            }

            return Promise.Completed;
        }
    }
}