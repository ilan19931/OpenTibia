﻿using NLua;
using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;

namespace OpenTibia.Game.Plugins
{
    public class LuaScriptingAmmunitionPlugin : AmmunitionPlugin
    {
        private string fileName;

        private ILuaScope script;

        private LuaTable parameters;

        public LuaScriptingAmmunitionPlugin(string fileName, LuaTable parameters, Ammunition ammunition) : base(ammunition)
        {
            this.fileName = fileName;

            this.parameters = parameters;
        }

        public LuaScriptingAmmunitionPlugin(ILuaScope script, LuaTable parameters, Ammunition ammunition) : base(ammunition)
        {
            this.script = script;

            this.parameters = parameters;
        }

        public override void Start()
        {
            if (fileName != null)
            {
                script = Context.Server.LuaScripts.LoadScript(
                    Context.Server.PathResolver.GetFullPath("data/plugins/ammunitions/" + fileName),
                    Context.Server.PathResolver.GetFullPath("data/plugins/ammunitions/lib.lua"), 
                    Context.Server.PathResolver.GetFullPath("data/plugins/lib.lua"), 
                    Context.Server.PathResolver.GetFullPath("data/lib.lua") );
            }
        }

        public override PromiseResult<bool> OnUsingAmmunition(Player player, Creature target, Item weapon, Item ammunition)
        {
            if (fileName != null)
            {
                return script.CallFunction("onusingammunition", player, target, weapon, ammunition).Then(result =>
                {
                    return (bool)result[0] ? Promise.FromResultAsBooleanTrue : Promise.FromResultAsBooleanFalse;
                } );
            }
            else
            {
                return script.CallFunction( (LuaFunction)parameters["onusingammunition"], player, target, weapon, ammunition).Then(result =>
                {
                    return (bool)result[0] ? Promise.FromResultAsBooleanTrue : Promise.FromResultAsBooleanFalse;
                } );
            }
        }

        public override Promise OnUseAmmunition(Player player, Creature target, Item weapon, Item ammunition)
        {
            if (fileName != null)
            {
                return script.CallFunction("onuseammunition", player, target, weapon, ammunition);
            }
            else
            {
                return script.CallFunction( (LuaFunction)parameters["onuseammunition"], player, target, weapon, ammunition);
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