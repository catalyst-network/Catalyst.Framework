using System.Collections.Generic;
using System.Linq;
using Catalyst.Core.Lib.DAO.Ledger;
using Catalyst.Core.Lib.Service;
using SharpRepository.Repository;

namespace Catalyst.Core.Modules.Sync
{
    public class DeltaIndexService : IDeltaIndexService
    {
        private readonly IRepository<DeltaIndexDao, string> _repository;

        public DeltaIndexService(IRepository<DeltaIndexDao, string> repository) { _repository = repository; }

        public void Add(IEnumerable<DeltaIndexDao> deltaIndexes) { _repository.Add(deltaIndexes); }

        public void Add(DeltaIndexDao deltaIndex) { _repository.Add(deltaIndex); }

        public IEnumerable<DeltaIndexDao> GetRange(int start, int count)
        {
            return _repository.FindAll(x => x.Height >= start && x.Height < count).OrderBy(x => x.Height);
        }

        public int Height()
        {
            if (_repository.Count() == 0)
            {
                return 0;
            }

            return _repository.Max(x => x.Height);
        }

        public DeltaIndexDao LatestDeltaIndex() { return _repository.Find(x => x.Height == Height()); }
    }
}
