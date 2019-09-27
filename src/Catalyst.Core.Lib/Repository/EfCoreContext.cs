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
using Catalyst.Core.Lib.DAO;
using Catalyst.Core.Lib.P2P.Models;
using Microsoft.EntityFrameworkCore;
using SharpRepository.EfCoreRepository;
using SharpRepository.Repository.Caching;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Catalyst.Core.Lib.Repository
{
    public class PeerEfCoreRepository : EfCoreRepository<PeerDao, string>
    {
        public PeerEfCoreRepository(IDbContext dbContext, ICachingStrategy<PeerDao, string> cachingStrategy = null) :
            base((Microsoft.EntityFrameworkCore.DbContext) dbContext, cachingStrategy) { }
    }

    public class MempoolEfCoreRepository : EfCoreRepository<TransactionBroadcastDao, string>
    {
        public MempoolEfCoreRepository(IDbContext dbContext,
            ICachingStrategy<TransactionBroadcastDao, string> cachingStrategy = null) :
            base((Microsoft.EntityFrameworkCore.DbContext) dbContext, cachingStrategy) { }
    }

    public interface IDbContext : IDisposable
    {
        Microsoft.EntityFrameworkCore.DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }

    public class EfCoreContext : DbContext, IDbContext
    {
        public EfCoreContext(string connectionString)
            : base(new DbContextOptionsBuilder<EfCoreContext>()
               .UseSqlServer(connectionString).Options) { }

        public Microsoft.EntityFrameworkCore.DbSet<PeerIdDao> PeerIdDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<PeerDao> PeerDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TransactionBroadcastDao> TransactionBroadcastStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<RangeProofDao> RangeProofDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<PublicEntryDao> PublicEntryDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ConfidentialEntryDao> ConfidentialEntryDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ContractEntryDao> ContractEntryDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<BaseEntryDao> BaseEntryDaoStore { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<SignatureDao> SignatureDaoStore { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Required code stub
        }
    }
}

