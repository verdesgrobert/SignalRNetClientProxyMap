﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace SignalRNetClientProxyMapper
{
    /// <summary>
    ///     A client side strong typed proxy base interface for a server side hub.
    /// </summary>
    public interface IClientHubProxyBase
    {
        /// <summary>
        ///     Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        /// <returns>A task that represents when invocation returned.</returns>
        [Obsolete(
            "You should define the function you wish to call inside your interface strongly. (Task NameOfAction(...))")]
        [NotMapped]
        Task Invoke(string method, params object[] args);

        /// <summary>
        ///     Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of result returned from the hub</typeparam>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        /// <returns>A task that represents when invocation returned.</returns>
        [Obsolete(
            "You should define the function you wish to call inside your interface strongly. (Task<TReturn> NameOfFunction(...))"
            )]
        [NotMapped]
        Task<T> Invoke<T>(string method, params object[] args);

        /// <summary>
        ///     Represents a subscription to a hub method
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <returns>A Microsoft.AspNet.SignalR.Client.Hubs.Subscription.</returns>
        /// <remarks>Subscriptions are not yet supported by the proxy interface and must be done manually.</remarks>
        [Obsolete(
            "You should define the event you wish to observe inside your interface strongly. (IObservable<TReturn> NameOfEvent {get;})"
            )]
        [NotMapped]
        Subscription Subscribe(string eventName);
    }
}