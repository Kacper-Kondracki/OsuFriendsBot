using LiteDB;
using OsuFriendsDb.Models;

namespace OsuFriendsDb.Services
{
    public class DbUserDataService
    {
        private readonly LiteDatabase _database;
        public const string collection = "UserData";

        public DbUserDataService(LiteDatabase database)
        {
            _database = database;
        }

        public UserData FindById(ulong user)
        {
            return _database.GetCollection<UserData>(collection).FindOne(x => x.UserId == user) ?? new UserData { UserId = user };
        }

        public bool Upsert(UserData user)
        {
            return _database.GetCollection<UserData>(collection).Upsert(user);
        }
    }
}