﻿using NLua;
using OpenTibia.Common.Objects;
using OpenTibia.Game.GameObjectScripts;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.Common.ServerObjects
{
    public class GameObjectScriptCollection : IGameObjectScriptCollection
    {
        private IServer server;

        public GameObjectScriptCollection(IServer server)
        {
            this.server = server;
        }

        ~GameObjectScriptCollection()
        {
            Dispose(false);
        }
        private ILuaScope script;

        public void Start()
        {
            script = server.LuaScripts.LoadScript(
                server.PathResolver.GetFullPath("data/gameobjectscripts/config.lua"),
                server.PathResolver.GetFullPath("data/gameobjectscripts/lib.lua"),
                server.PathResolver.GetFullPath("data/lib.lua") );

            foreach (LuaTable plugin in ( (LuaTable)script["gameobjectscripts.items"] ).Values)
            {
                ushort openTibiaId = LuaScope.GetUInt16(plugin["opentibiaid"] );

                string fileName = LuaScope.GetString(plugin["filename"] );

                items.Add(openTibiaId, (GameObjectScript<Item>)Activator.CreateInstance(server.PluginLoader.GetType(fileName) ) );
            }

            foreach (LuaTable plugin in ( (LuaTable)script["gameobjectscripts.players"] ).Values)
            {
                string name = LuaScope.GetString(plugin["name"] );

                string fileName = LuaScope.GetString(plugin["filename"] );

                players.Add(name, (GameObjectScript<Player>)Activator.CreateInstance(server.PluginLoader.GetType(fileName) ) );               
            }

            foreach (LuaTable plugin in ( (LuaTable)script["gameobjectscripts.monsters"] ).Values)
            {
                string name = LuaScope.GetString(plugin["name"] );

                string fileName = LuaScope.GetString(plugin["filename"] );

                monsters.Add(name, (GameObjectScript<Monster>)Activator.CreateInstance(server.PluginLoader.GetType(fileName) ) );
            }

            foreach (LuaTable plugin in ( (LuaTable)script["gameobjectscripts.npcs"] ).Values)
            {
                string name = LuaScope.GetString(plugin["name"] );

                string fileName = LuaScope.GetString(plugin["filename"] );

                npcs.Add(name, (GameObjectScript<Npc>)Activator.CreateInstance(server.PluginLoader.GetType(fileName) ) );                
            }
        }

        /// <exception cref="ObjectDisposedException"></exception>
    
        public object GetValue(string key)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(GameObjectScriptCollection) );
            }

            return script[key];
        }

        private Dictionary<ushort, GameObjectScript<Item> > items = new Dictionary<ushort, GameObjectScript<Item>>();

        public GameObjectScript<Item> GetItemGameObjectScript(ushort openTibiaId)
        {
            GameObjectScript<Item> gameObjectScript;

            if (items.TryGetValue(openTibiaId, out gameObjectScript) )
            {
                return gameObjectScript;
            }
            
            return null;
        }

        private Dictionary<string, GameObjectScript<Player> > players = new Dictionary<string, GameObjectScript<Player>>();

        public GameObjectScript<Player> GetPlayerGameObjectScript(string name)
        {
            GameObjectScript<Player> gameObjectScript;

            if (players.TryGetValue(name, out gameObjectScript) )
            {
                return gameObjectScript;
            }

            return null;
        }

        private Dictionary<string, GameObjectScript<Monster> > monsters = new Dictionary<string, GameObjectScript<Monster>>();

        public GameObjectScript<Monster> GetMonsterGameObjectScript(string name)
        {
            GameObjectScript<Monster> gameObjectScript;

            if (monsters.TryGetValue(name, out gameObjectScript) )
            {
                return gameObjectScript;
            }

            return null;
        }

        private Dictionary<string, GameObjectScript< Npc> > npcs = new Dictionary<string, GameObjectScript< Npc>>();

        public GameObjectScript< Npc> GetNpcGameObjectScript(string name)
        {
            GameObjectScript< Npc> gameObjectScript;

            if (npcs.TryGetValue(name, out gameObjectScript) )
            {
                return gameObjectScript;
            }

            return null;
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    if (script != null)
                    {
                        script.Dispose();
                    }
                }
            }
        }
    }
}