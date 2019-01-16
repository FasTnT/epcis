using FasTnT.Model.MasterDatas;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IMasterDataManager
    {
        Task Store(EpcisMasterData masterData);
    }
}