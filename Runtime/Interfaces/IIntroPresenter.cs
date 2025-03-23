using System.Threading;
using Cysharp.Threading.Tasks;

namespace Glotski.Interfaces
{
    public interface IIntroPresenter
    {
        UniTask Show(CancellationToken cancellationToken = default);
    }
}