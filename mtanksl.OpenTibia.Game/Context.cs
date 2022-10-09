﻿using OpenTibia.Common.Events;
using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using OpenTibia.Network.Packets.Outgoing;
using OpenTibia.Threading;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game
{
    public class Context : IDisposable
    {
        public static Context Current
        {
            get
            {
                var scope = Scope<Context>.Current;

                if (scope == null)
                {
                    return null;
                }

                return scope.Value;
            }
        }

        public Context(Server server)
        {
            this.server = server;
        }

        ~Context()
        {
            Dispose(false);
        }

        private Server server;

        public Server Server
        {
            get
            {
                return server;
            }
        }

        private DatabaseContext databaseContext;

        public DatabaseContext DatabaseContext
        {
            get
            {
                return databaseContext ?? (databaseContext = new DatabaseContext() );
            }
        }

        private Dictionary<string, object> data;

        public Dictionary<string, object> Data
        {
            get
            {
                return data ?? (data = new Dictionary<string, object>() );
            }
        }

        public Promise AddCommand(Command command)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            var commandHandlers = server.CommandHandlers.Get(command).GetEnumerator();

            Promise Next(Context context)
            {
                if (commandHandlers.MoveNext() )
                {
                    return commandHandlers.Current.Handle(context, Next, command);
                }

                return command.Execute(context);
            }

            return Next(this);
        }

        public PromiseResult<TResult> AddCommand<TResult>(CommandResult<TResult> command)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            var commandHandlers = server.CommandHandlers.Get(command).GetEnumerator();

            PromiseResult<TResult> Next(Context context)
            {
                if (commandHandlers.MoveNext() )
                {
                    return commandHandlers.Current.Handle(context, Next, command);
                }

                return command.Execute(context);
            }

            return Next(this);
        }

        private Dictionary<IConnection, Message> messages = null;

        public void AddPacket(IConnection connection, IOutgoingPacket packet)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            if (messages == null)
            {
                messages = new Dictionary<IConnection, Message>();
            }

            Message message;

            if ( !messages.TryGetValue(connection, out message) )
            {
                message = new Message();

                messages.Add(connection, message);
            }

            message.Add(packet);
        }

        public void AddPacket(IConnection connection, params IOutgoingPacket[] packet)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            if (messages == null)
            {
                messages = new Dictionary<IConnection, Message>();
            }

            Message message;

            if ( !messages.TryGetValue(connection, out message) )
            {
                message = new Message();

                messages.Add(connection, message);
            }

            message.Add(packet);
        }

        private HashSet<IConnection> connections = null;

        public void Disconnect(IConnection connection)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            if (connections == null)
            {
                connections = new HashSet<IConnection>();
            }

            connections.Add(connection);
        }

        private List<GameEventArgs> events;

        public void AddEvent(GameEventArgs e)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            if (events == null)
            {
                events = new List<GameEventArgs>();
            }

            events.Add(e);
        }

        public void Flush()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Context) );
            }

            if (events != null)
            {
                foreach (var e in events)
                {
                    server.EventHandlers.Publish(this, e);
                }

                events.Clear();
            }

            if (messages != null)
            {
                foreach (var pair in messages)
                {
                    IConnection connection = pair.Key;

                    Message message = pair.Value;

                    connection.Send( message.GetBytes(connection.Keys) );
                }

                messages.Clear();
            }

            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    connection.Disconnect();
                }

                connections.Clear();
            }            
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !disposed )
            {
                disposed = true;

                if (disposing)
                {
                    if (databaseContext != null)
                    {
                        databaseContext.Dispose();
                    }
                }
            }
        }        
    }
}