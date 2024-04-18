﻿using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Diagnostics;

namespace OpenTibia.Game.CommandHandlers
{
    public class InlineCommandResultHandler<TResult, T> : CommandResultHandler<TResult, T> where T : CommandResult<TResult>
    {
        private Func<Context, Func<PromiseResult<TResult>>, T, PromiseResult<TResult> > handle;

        public InlineCommandResultHandler(Func<Context, Func<PromiseResult<TResult>>, T, PromiseResult<TResult> > handle)
        {
            this.handle = handle;
        }

        [DebuggerStepThrough]
        public override PromiseResult<TResult> Handle(Func<PromiseResult<TResult>> next, T command)
        {
            return handle(Context, next, command);
        }
    }
}