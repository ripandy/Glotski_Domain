using System.Threading;
using Cysharp.Threading.Tasks;

namespace Glotski.Interfaces
{
    public interface IGameOverPresenter
    {
        UniTask<bool> Show(CancellationToken cancellationToken = default);
    }
}