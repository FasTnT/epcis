using System;

namespace FasTnT.Domain.Persistence
{
    public class CommitOnDispose : IDisposable
    {
        private IUnitOfWork _unitOfWork;

        public CommitOnDispose(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork.BeginTransaction();
        }

        public void Dispose()
        {
            try
            {
                _unitOfWork.Commit();
            }
            finally
            {
                _unitOfWork = null;
            }
        }
    }
}
