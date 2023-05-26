using ShoppingMallSys.Models;

namespace ShoppingMallSys.Services
{
    public interface IAuthorRepository
    {
        Task<Author> GetByKey(int key);
    }
}
