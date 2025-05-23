﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Components;

namespace OpenTibia.Game.GameObjectScripts
{
    public class NpcScript : GameObjectScript<Npc>
    {
        public override void Start(Npc npc)
        {
            if (npc.Metadata.Voices != null)
            {
                Context.Server.GameObjectComponents.AddComponent(npc, new NpcTalkBehaviour(npc.Metadata.Voices) );
            }

            if (Context.Server.Config.GameplayPrivateNpcSystem && Context.Server.Features.HasFeatureFlag(FeatureFlag.NpcsChannel) )
            {
                Context.Server.GameObjectComponents.AddComponent(npc, new MultipleQueueNpcThinkBehaviour(new NpcWalkStrategy(2) ) );
            }
            else
            {
                Context.Server.GameObjectComponents.AddComponent(npc, new SingleQueueNpcThinkBehaviour(new NpcWalkStrategy(2) ) );
            }
        }

        public override void Stop(Npc npc)
        {

        }
    }
}