﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Events;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ContainerRefreshItemCommand : Command
    {
        public ContainerRefreshItemCommand(Container container, Item item)
        {
            Container = container;

            Item = item;
        }

        public Container Container { get; set; }

        public Item Item { get; set; }
        
        public override Promise Execute()
        {
            byte index = (byte)Container.GetIndex(Item);

            foreach (var observer in Container.GetPlayers() )
            {
                foreach (var pair in observer.Client.Containers.GetIndexedContainers() )
                {
                    if (pair.Value == Container)
                    {
                        Context.AddPacket(observer.Client.Connection, new ContainerUpdateOutgoingPacket(pair.Key, index, Item) );
                    }
                }

                Context.AddEvent(observer, new ContainerRefreshItemEventArgs(Container, Item, index) );
            }

            Context.AddEvent(new ContainerRefreshItemEventArgs(Container, Item, index) );

            return Promise.Completed;
        }
    }
}