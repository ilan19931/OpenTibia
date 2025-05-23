﻿using NLua;
using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;

namespace OpenTibia.Game.Plugins
{
    public class LuaScriptingSpellPlugin : SpellPlugin
    {
        private string fileName;

        private ILuaScope script;

        private LuaTable parameters;

        public LuaScriptingSpellPlugin(string fileName, LuaTable parameters, Spell spell) : base(spell)
        {
            this.fileName = fileName;

            this.parameters = parameters;
        }

        public LuaScriptingSpellPlugin(ILuaScope script, LuaTable parameters, Spell spell) : base(spell)
        {
            this.script = script;

            this.parameters = parameters;
        }

        public override void Start()
        {
            if (fileName != null) 
            {
                script = Context.Server.LuaScripts.LoadScript(
                    Context.Server.PathResolver.GetFullPath("data/plugins/spells/" + fileName),
                    Context.Server.PathResolver.GetFullPath("data/plugins/spells/lib.lua"), 
                    Context.Server.PathResolver.GetFullPath("data/plugins/lib.lua"),
                    Context.Server.PathResolver.GetFullPath("data/lib.lua") );
            }
        }

        public override PromiseResult<bool> OnCasting(Player player, Creature target, string message, string parameter)
        {
            if (fileName != null)
            {
                return script.CallFunction("oncasting", player, target, message, parameter).Then(result =>
                {
                    return (bool)result[0] ? Promise.FromResultAsBooleanTrue : Promise.FromResultAsBooleanFalse;
                } );
            }
            else
            {
                return script.CallFunction( (LuaFunction)parameters["oncasting"], player, target, message, parameter).Then(result =>
                {
                    return (bool)result[0] ? Promise.FromResultAsBooleanTrue : Promise.FromResultAsBooleanFalse;
                } );
            }
        }

        public override Promise OnCast(Player player, Creature target, string message, string parameter)
        {
            if (fileName != null)
            {
                return script.CallFunction("oncast", player, target, message, parameter);
            }
            else
            {
                return script.CallFunction( (LuaFunction)parameters["oncast"], player, target, message, parameter);
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