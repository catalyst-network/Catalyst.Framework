using System;
using Catalyst.Node.Common.Modules;

namespace Catalyst.Node.Core.Modules.Contract
{
    public class Contract : IContract
    {
        public Guid Identity { get; set; }
        public string Address { get; set; }
        public ContractType Type { get; set; }
    }
}