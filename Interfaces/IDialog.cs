using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
    public interface IDialog<T>
    {
        public Task<T> Show();
    }
}
