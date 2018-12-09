using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Persistence.Dapper
{
    internal class NamespaceStore
    {
        private readonly IUnitOfWork _unitOfWork;
        Dictionary<string, NamespaceDTO> nameDictionary = new Dictionary<string, NamespaceDTO>();

        public NamespaceStore(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<NamespaceDTO> FindByName(string name)
        {
            NamespaceDTO dto = null;
            if (nameDictionary.TryGetValue(name, out dto)) return dto;
            var result = await _unitOfWork.Query<NamespaceDTO>(SqlRequests.NamespaceByNameQuery,new {Namespace = name});
            dto = result.FirstOrDefault();
            if (dto != null)
            {
                nameDictionary.Add(dto.Namespace, dto);
            }
            return dto;
        }

        public async Task<NamespaceDTO> Create(string name)
        {
            var result = await _unitOfWork.Query<int>(SqlRequests.NamespaceCreate, new { Namespace = name });
            var dto = new NamespaceDTO { Id = result.First(), Namespace = name };
            nameDictionary.Add(dto.Namespace, dto);
            return dto;
        }

    }

    internal class NamespaceDTO
    {
        public int Id;
        public string Namespace;
    }
}
