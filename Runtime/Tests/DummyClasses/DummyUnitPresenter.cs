using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Glotski.Interfaces;

namespace Glotski.Tests.DummyClasses
{
    public class DummyStagePresenter : IStagePresenter
    {
        public UniTask InitializeStageAsync(IEnumerable<Unit> units, int stageWidth, int stageHeight, CancellationToken cancellationToken = default)
        {
            return UniTask.Delay(100, cancellationToken: cancellationToken);
        }

        public UniTask MoveUnitAsync(Unit unit, Point tile, CancellationToken cancellationToken = default)
        {
            return UniTask.Delay(100, cancellationToken: cancellationToken);
        }

        public UniTask FlipUnitAsync(Unit unit, int depth = 0, CancellationToken cancellationToken = default)
        {
            return UniTask.Delay(100, cancellationToken: cancellationToken);
        }

        public UniTask ShowAttackAsync(Unit actor, int comboDepth, CancellationToken cancellationToken = default)
        {
            return UniTask.Delay(100, cancellationToken: cancellationToken);
        }
    }
}