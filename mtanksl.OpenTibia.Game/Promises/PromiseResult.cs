﻿using System;

namespace OpenTibia.Game.Commands
{
    public class PromiseResult<TResult>
    {
        private PromiseStatus status;

        private Context context;

        private TResult result;

        private Exception exception;

        private Action<Context, TResult> continueWithFulfilled;

        private Action<Context, Exception> continueWithRejected;

        public PromiseResult()
        {
            this.status = PromiseStatus.Pending;
        }

        public PromiseResult(Action<Action<Context, TResult>, Action<Context, Exception> > run)
        {
            Action<Context, TResult> resolve = (c, r) =>
            {
                if (this.status == PromiseStatus.Pending)
                {
                    this.status = PromiseStatus.Fulfilled;

                    this.context = c;

                    this.result = r;

                    if (this.continueWithFulfilled != null)
                    {
                        this.continueWithFulfilled(this.context, this.result);
                    }
                }
            };

            Action<Context, Exception> reject = (c, e) =>
            {
                if (this.status == PromiseStatus.Pending)
                {
                    this.status = PromiseStatus.Rejected;

                    this.context = c;

                    this.exception = e;

                    if (this.continueWithRejected != null)
                    {
                        this.continueWithRejected(this.context, this.exception);
                    }
                }
            };

            run(resolve, reject);
        }

        public bool TrySetResult(TResult result)
        {
            if (this.status == PromiseStatus.Pending)
            {
                this.status = PromiseStatus.Fulfilled;

                this.result = result;

                if (this.continueWithFulfilled != null)
                {
                    this.continueWithFulfilled(this.context, this.result);
                }

                return true;
            }

            return false;
        }

        public bool TrySetException(Exception exception)
        {
            if (this.status == PromiseStatus.Pending)
            {
                this.status = PromiseStatus.Rejected;

                this.exception = exception;

                if (this.continueWithRejected != null)
                {
                    this.continueWithRejected(this.context, this.exception);
                }

                return true;
            }

            return false;
        }
                
        /// <exception cref="InvalidOperationException"></exception>

        public TResult Result
        {
            get
            {
                if (status != PromiseStatus.Fulfilled)
                {
                    throw new InvalidOperationException("Promise is not fulfilled.");
                }

                return result;
            }
        }

        public PromiseResult<TResult> Then(Action<Context, TResult> onFullfilled, Action<Context, Exception> onRejected = null)
        {
            return Promise.Run<TResult>( (resolve, reject) =>
            {
                if (this.status == PromiseStatus.Pending)
                {
                    this.continueWithFulfilled = (c, r) =>
                    {
                        onFullfilled(c, r);

                        resolve(c, r);
                    };

                    this.continueWithRejected = (c, e) =>
                    {
                        onRejected(c, e);

                        reject(c, e);
                    };
                }
                else if (this.status == PromiseStatus.Fulfilled)
                {
                    onFullfilled(this.context, this.result);

                    resolve(this.context, this.result);
                }
                else if (this.status == PromiseStatus.Rejected)
                {
                    onRejected(this.context, this.exception);

                    reject(this.context, this.exception);
                }
            } );
        }

        public PromiseResult<TResult> Then(Func<Context, TResult, PromiseResult<TResult> > onFullfilled, Func<Context, Exception, PromiseResult<TResult> > onRejected = null)
        {
            return Promise.Run<TResult>( (resolve, reject) =>
            {
                if (this.status == PromiseStatus.Pending)
                {
                    this.continueWithFulfilled = (c, r) =>
                    {
                        onFullfilled(c, r).Then(resolve, reject);
                    };

                    this.continueWithRejected = (c, e) =>
                    {
                        onRejected(c, e).Then(resolve, reject);
                    };
                }
                else if (this.status == PromiseStatus.Fulfilled)
                {
                    onFullfilled(this.context, this.result).Then(resolve, reject);
                }
                else if (this.status == PromiseStatus.Rejected)
                {
                    onRejected(this.context, this.exception).Then(resolve, reject);
                }
            } );
        }

        public Promise Then(Func<Context, TResult, Promise> onFullfilled, Func<Context, Exception, Promise> onRejected = null)
        {
            return Promise.Run( (resolve, reject) =>
            {
                if (this.status == PromiseStatus.Pending)
                {
                    this.continueWithFulfilled = (c, r) =>
                    {
                        onFullfilled(c, r).Then(resolve, reject);
                    };

                    this.continueWithRejected = (c, e) =>
                    {
                        onRejected(c, e).Then(resolve, reject);
                    };
                }
                else if (this.status == PromiseStatus.Fulfilled)
                {
                    onFullfilled(this.context, this.result).Then(resolve, reject);
                }
                else if (this.status == PromiseStatus.Rejected)
                {
                    onRejected(this.context, this.exception).Then(resolve, reject);
                }
            } );
        }     
        
        public static implicit operator Promise(PromiseResult<TResult> promise)
        {
            return promise.Then( (ctx, result) => 
            {
                return Promise.Completed(ctx);

            }, (ctx, exception) =>
            {
                return Promise.Completed(ctx);
            } );
        }
    }
}