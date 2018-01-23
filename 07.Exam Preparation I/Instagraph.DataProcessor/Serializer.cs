using System;

using Instagraph.Data;
using Newtonsoft.Json;
using Instagraph.DataProcessor.DtoModels.Export;
using System.Linq;
using System.Collections.Generic;
using Instagraph.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var uncommentedPosts = context.Posts
            .Where(p => p.Comments.Count == 0)
            .Select(p => new UncommentedPostDto
            {
                Id = p.Id,
                Picture = p.Picture.Path,
                User = p.User.ToString(),
            })
            .OrderBy(p => p.Id)
            .ToArray();

            //UncommentedPostDto[] uncommentedPosts = context.Posts
            //    .Include(p => p.User)
            //    .Include(p => p.Comments)
            //    .Include(p => p.Picture)
            //    .Where(p => p.Comments.Count == 0)
            //    .Select(p => new UncommentedPostDto
            //    {
            //        Id = p.Id,
            //        Picture = p.Picture.Path,
            //        User = p.User.ToString(),
            //    })
            //    .OrderBy(p => p.Id)
            //    .ToArray();


            var json = JsonConvert.SerializeObject(uncommentedPosts, Formatting.Indented);
            return json;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var popularUsers = context.Users
               .Where(u => u.Posts
               .Any(p => p.Comments
               .Any(c => u.Followers
               .Any(uf => uf.FollowerId == c.UserId))))
               .OrderBy(u => u.Id)
               .Select(u => new PopularUsers()
                {
                    Username = u.Username,
                    Followers = u.Followers.Count
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(popularUsers, Formatting.Indented);
            return json;

        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            throw new NotImplementedException();
        }
    }
}
