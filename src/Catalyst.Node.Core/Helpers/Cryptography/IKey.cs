using NSec.Cryptography;

namespace Catalyst.Node.Core.Helpers.Cryptography
{
    public interface IKey : IPublicKey{
        
        Key GetNSecFormatKey();
    }
}