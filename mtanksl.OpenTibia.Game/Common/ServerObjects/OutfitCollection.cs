﻿using NLua;
using OpenTibia.Common.Structures;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.Common.ServerObjects
{
    public class OutfitCollection : IOutfitCollection
    {
        private IServer server;

        public OutfitCollection(IServer server)
        {
            this.server = server;
        }

        ~OutfitCollection()
        {
            Dispose(false);
        }

        private ILuaScope script;

        public void Start()
        {
            script = server.LuaScripts.LoadScript(
                server.PathResolver.GetFullPath("data/outfits/config.lua") ,
                server.PathResolver.GetFullPath("data/outfits/lib.lua"),
                server.PathResolver.GetFullPath("data/lib.lua") );

            foreach (LuaTable lOutfit in ( (LuaTable)script["outfits"] ).Values)
            {
                OutfitConfig outfit = new OutfitConfig()
                {
                    Id = (ushort)(long)lOutfit["id"],

                    Name = (string)lOutfit["name"],

                    Gender = (Gender)(int)(long)lOutfit["gender"]
                };

                outfits.Add(outfit.Id, outfit);
            }
        }

        public object GetValue(string key)
        {
            return script[key];
        }

        private Dictionary<ushort, OutfitConfig> outfits = new Dictionary<ushort, OutfitConfig>();

        public OutfitConfig GetOutfitById(ushort id)
        {
            OutfitConfig outfit;

            if (outfits.TryGetValue(id, out outfit) )
            {
                return outfit;
            }

            return null;
        }

        public IEnumerable<OutfitConfig> GetOutfits()
        {
            return outfits.Values;
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