﻿using NLua;
using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;

namespace OpenTibia.Game.Plugins
{
    public class LuaScriptingCreatureStepInPlugin : CreatureStepInPlugin
    {
        private string fileName;

        private ILuaScope script;

        private LuaTable parameters;

        public LuaScriptingCreatureStepInPlugin(string fileName, LuaTable parameters)
        {
            this.fileName = fileName;

            this.parameters = parameters;
        }

        public LuaScriptingCreatureStepInPlugin(ILuaScope script, LuaTable parameters)
        {
            this.script = script;

            this.parameters = parameters;
        }

        public override void Start()
        {
            if (fileName != null)
            {
                script = Context.Server.LuaScripts.LoadScript(
                    Context.Server.PathResolver.GetFullPath("data/plugins/movements/" + fileName),
                    Context.Server.PathResolver.GetFullPath("data/plugins/movements/lib.lua"),
                    Context.Server.PathResolver.GetFullPath("data/plugins/lib.lua"),
                    Context.Server.PathResolver.GetFullPath("data/lib.lua") );
            }
        }

        public override Promise OnStepIn(Creature creature, Tile fromTile, Tile toTile)
        {
            if (fileName != null)
            {
                return script.CallFunction("onstepin", creature, fromTile, toTile);
            }
            else
            {
                return script.CallFunction( (LuaFunction)parameters["onstepin"], creature, fromTile, toTile);
            }           
        }

        public override void Stop()
        {
            if (fileName != null)
            {
                script.Dispose();
            }
        }       
    }
}