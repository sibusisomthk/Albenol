using AlbenolOnline.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbenolOnline.Helpers
{
    public static class ExtentionMethods
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPassword());
        }
        public static List<User> WithoutPasswordAndTokken(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPasswordAndTokken()).ToList();
        }

        public static User WithoutPassword(this User user)
        {
            user.Password = null;
            return user;
        }
        public static User WithoutPasswordAndTokken(this User user)
        {
            user.Password = null;
            user.Token = null;
            return user;
        }
    }
}
