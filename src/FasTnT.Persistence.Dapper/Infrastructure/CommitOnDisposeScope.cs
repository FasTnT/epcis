using System;

namespace FasTnT.Persistence.Dapper
{
    internal class CommitOnDisposeScope : IDisposable
    {
        private IUnitOfWork _unitOfWork;

        public CommitOnDisposeScope(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork.Begin();
        }

        public void Dispose() => _unitOfWork.Commit();
    }
}
