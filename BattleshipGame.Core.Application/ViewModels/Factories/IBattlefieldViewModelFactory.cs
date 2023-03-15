using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.ViewModels.Factories
{
    public interface IBattlefieldViewModelFactory
    {
        BattlefieldViewModel Create(Battlefield battlefield, bool showWarships);
    }
}
