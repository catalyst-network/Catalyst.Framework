#region LICENSE

/**
* Copyright (c) 2019 Catalyst Network
*
* This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
*
* Catalyst.Node is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* Catalyst.Node is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Catalyst.Common.Interfaces.P2P;
using Catalyst.Common.Interfaces.P2P.ReputationSystem;
using Catalyst.Common.P2P;
using Dawn;
using Serilog;
using SharpRepository.Repository;

namespace Catalyst.Node.Core.P2P.ReputationSystem
{
    public sealed class ReputationManager : IReputationManager, IDisposable
    {
        private readonly ILogger _logger;
        public IRepository<Peer> PeerRepository { get; }
        public readonly ReplaySubject<IPeerReputationChange> ReputationEvent;
        public IObservable<IPeerReputationChange> ReputationEventStream => ReputationEvent.AsObservable();
        public IObservable<IPeerReputationChange> MergedEventStream { get; set; }
        static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        public ReputationManager(IRepository<Peer> peerRepository, ILogger logger)
        {
            _logger = logger;
            PeerRepository = peerRepository;
            ReputationEvent = new ReplaySubject<IPeerReputationChange>(0);
            
            ReputationEventStream
               .SubscribeOn(NewThreadScheduler.Default)
               .Subscribe(OnNext, OnError, OnCompleted);
        }
        
        /// <summary>
        ///     Allows passing a reputation streams to merge with the MasterReputationEventStream
        /// </summary>
        /// <param name="reputationChangeStream"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void MergeReputationStream(IObservable<IPeerReputationChange> reputationChangeStream)
        {
            MergedEventStream = ReputationEventStream.Merge(reputationChangeStream);
        }

        private void OnCompleted()
        {
            _logger.Debug("Message stream ended.");
        }

        private void OnError(Exception obj)
        {
            _logger.Error("Message stream ended.");
        }

        public async void OnNext(IPeerReputationChange peerReputationChange)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                var peer = PeerRepository.GetAll().FirstOrDefault(p => p.PeerIdentifier.Equals(peerReputationChange.PeerIdentifier));
                Guard.Argument(peer, nameof(peer)).NotNull();

                peer.Reputation += peerReputationChange.ReputationEvent.Amount;
                PeerRepository.Update(peer);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }
        }
        
        public void Dispose()
        {
            ReputationEvent?.Dispose();
            PeerRepository?.Dispose();    
        }
    }
}
