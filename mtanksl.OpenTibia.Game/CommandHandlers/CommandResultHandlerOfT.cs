﻿using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Diagnostics;

namespace OpenTibia.Game.CommandHandlers
{
    public abstract class CommandResultHandler<TResult, T> : ICommandResultHandler<TResult, T> where T : CommandResult<TResult>
    {
        public Context Context
        {
            get
            {
                return Context.Current;
            }
        }

        public bool IsDestroyed { get; set; }

        public Guid Token { get; } = Guid.NewGuid();

        [DebuggerStepThrough]
        public PromiseResult<TResult> Handle(Func<PromiseResult<TResult>> next, CommandResult<TResult> command)
        {
            return Handle(next, (T)command);
        }

        public abstract PromiseResult<TResult> Handle(Func<PromiseResult<TResult>> next, T command);
    }
}