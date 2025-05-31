using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using RecordShop_BE.Tables;

namespace RecordShop_BE.Repositories
{
    public interface IAlbumRepository
    {
        public List<Albums> GetAllAlbums();
        public Albums GetAlbumById(int id);
        public Albums PostAlbum(Albums a);
        public bool PutAlbum(Albums a);
        public Albums DeleteAlbumById(int id);
    }
    public class AlbumRepository : IAlbumRepository
    {
        private MyDbContext context;
        public AlbumRepository(MyDbContext context)
        { this.context = context; }

        public List<Albums> GetAllAlbums()
        {
            //memory-db returns entries as they were added, does not sort by ID or anything implicit!

            return context.AlbumTable.OrderBy(a=>a.Id).ToList();
        }

        public Albums GetAlbumById(int id)
        {
            return context.AlbumTable.ToList().FirstOrDefault(a => a.Id == id);
        }

            //TODO refactor else will block large DBs ??
        private int PlugInAlbumGaps()
        {
            var data = context.AlbumTable.OrderBy(a => a.Id).ToList();

            for(int i = 0; i <= data.Count(); i++){
                try { if (data[i].Id != i +1) { return i +1; } } 
                catch (ArgumentOutOfRangeException e) {  }
            }

            return data.Count()+1;
        }

        public Albums PostAlbum(Albums a)
        {
            

            if (!context.Database.IsInMemory())
            {
                //transact not supported for in-memory
                using (var t = context.Database.BeginTransaction())
                {
                    try
                    {
                        a.Id = PlugInAlbumGaps();//Resets to default 0 (treats as unassigned)

                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [AlbumTable] ON");
                        //TODO TODO TODO for SQL SERVER TODO

                        context.AlbumTable.Add(a);
                        //Replaces existing IDs??

                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [AlbumTable] OFF");

                        context.SaveChanges();


                        //Commits all save changes operations!
                        t.Commit();
                    }
                    catch (Exception e)
                    {
                        t.Rollback(); //Revert any save changes to avoid possible error operations with DB

                        //throw new Exception("Unexpected Error on POST!\n"+e.Message, e);
                        a.Id = -1; a.Title = "Error adding to DB!"; a.Description = "Post method successful but database operation failed!\nAsk admin to check server logs!";
                        Console.WriteLine(DateTime.Now + " | " + e.Message);
                    }
                    finally
                    {
                        t.Dispose();
                    }
                }
            }
            else
            {
                //In mem for testing

                a.Id = PlugInAlbumGaps();
                context.AlbumTable.Add(a);
                context.SaveChanges();
            }

            return a;
        }

        public bool PutAlbum(Albums a)
        {
            //See if any diff / exists ?
            var A = GetAlbumById(a.Id);

            if (A != null)
            {
                //exists
                if( JsonSerializer.Serialize(a).Equals( JsonSerializer.Serialize( A ) ))
                {
                    //Is same!
                    return false;
                }
                //Not same!
                //TODO ID 6 is not treated as PK ??
                //context.AlbumTable.Update(a);
                    
                    //If tracked/grabbed from db.. set values than update it!
                context.Entry(A).CurrentValues.SetValues(a);

                context.SaveChanges();

                return true;
            }
            throw new ArgumentNullException("Given ID not found in DB!");
        }

        public Albums DeleteAlbumById(int id)
        {
            //TODO can be null - need error tests
            var a = GetAlbumById(id);
            if (a == null) { return null; }
            context.AlbumTable.Remove(a); context.SaveChanges();

            return a;
        }
    }
}
