﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ParseQuestionReportRuleViolationCommand : IncomingCommand
    {
        public ParseQuestionReportRuleViolationCommand(Player player, string message)
        {
            Player = player;

            Message = message;
        }

        public Player Player { get; set; }

        public string Message { get; set; }

        public override Promise Execute()
        {
            RuleViolation ruleViolation = Context.Server.RuleViolations.GetRuleViolationByReporter(Player);

            if (ruleViolation != null && ruleViolation.Assignee != null)
            {
                Context.AddPacket(ruleViolation.Assignee, new ShowTextOutgoingPacket(Context.Server.Channels.GenerateStatementId(Player.DatabasePlayerId, Message), ruleViolation.Reporter.Name, ruleViolation.Reporter.Level, MessageMode.RVRContinue, Message) );

                return Promise.Completed;
            }

            return Promise.Break;
        }
    }
}