using RecordShop_BE.Repositories;
using RecordShop_BE.Tables;

namespace RecordShop_BE.Services
{
    public interface IAlbumService
    {
        List<Albums> GetAllAlbums();
        Albums GetAlbumById(string id);

        Albums PostAlbum(Albums a);

        bool PutAlbum(Albums a);

        Albums DeleteAlbumById(string id);
    }

    public class AlbumService : IAlbumService
    {
        private IAlbumRepository repository;
        public AlbumService(IAlbumRepository r)
        {
            repository = r;
        }
        public List<Albums> GetAllAlbums()
        {
            return repository.GetAllAlbums();
        }

        public Albums GetAlbumById(string id)
        {
            return repository.GetAlbumById(int.Parse(id));
        }

        public Albums PostAlbum(Albums a)
        {
            return repository.PostAlbum(a);
        }

        public bool PutAlbum(Albums a)
        {
            return repository.PutAlbum(a);
        }

        public Albums DeleteAlbumById(string id)
        {
            return repository.DeleteAlbumById(int.Parse(id));
        }
    }
}
