using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

using Newtonsoft.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Instagraph.Data;
using Instagraph.Models;
using Instagraph.DataProcessor.DtoModels;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.IO;

namespace Instagraph.DataProcessor
{
    public class Deserializer
    {
        private static string errorMsg = "Error: Invalid data.";
        private static string successMsg = "Successfully imported {0}.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var deserializedPictures = JsonConvert.DeserializeObject<PictureDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            var validPictures = new List<Picture>();

            foreach (var pictureDto in deserializedPictures)
            {
                if (!IsValid(pictureDto))
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }
                var pathExists = validPictures.Any(p => p.Path == pictureDto.Path);
                var pictureSize = pictureDto.Size;

                if (pathExists || pictureSize <= 0)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var picture = new Picture()
                {
                    Path = pictureDto.Path,
                    Size = pictureDto.Size
                };

                validPictures.Add(picture);
                sb.AppendLine(string.Format(successMsg, $"Picture {pictureDto.Path}"));
            }

            context.Pictures.AddRange(validPictures);
            context.SaveChanges();

            string result = sb.ToString();
            return result;
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
           
            var deserializedUsers = JsonConvert.DeserializeObject<UserDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            var validUsers = new List<User>();

            foreach (var userDto in deserializedUsers)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var profilePictureExist = context.Pictures.Any(p => p.Path == userDto.ProfilePicture);

                if (!profilePictureExist)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var profilePicture = context.Pictures.SingleOrDefault(p => p.Path == userDto.ProfilePicture);

                var user = new User()
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    ProfilePicture = profilePicture
                };

                validUsers.Add(user);
                sb.AppendLine(String.Format(successMsg, $"User {userDto.Username}"));
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            string result = sb.ToString();
            return result;
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            
            UserFollowerDto[] deserializedFollowers = JsonConvert.DeserializeObject<UserFollowerDto[]>(jsonString);

            var sb = new StringBuilder();

            var userFollowers = new List<UserFollower>();

            foreach (var userFollowerDto in deserializedFollowers)
            {
                if (!IsValid(userFollowerDto))
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var user = context.Users.SingleOrDefault(u => u.Username == userFollowerDto.User);
                var follower = context.Users.SingleOrDefault(u => u.Username == userFollowerDto.Follower);

                if (user == null || follower == null)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                bool alreadyFollowed = userFollowers.Any(uf => uf.User == user && uf.Follower == follower);
                if (alreadyFollowed)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }


                var userFollower = new UserFollower()
                {
                    User = user,
                    Follower = follower
                };

                userFollowers.Add(userFollower);
                sb.AppendLine(String.Format(successMsg, $"Follower {userFollowerDto.Follower} to User {userFollowerDto.User}"));
            }

            context.UsersFollowers.AddRange(userFollowers);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            //var serializer = new XmlSerializer(typeof(PostDto[]), new XmlRootAttribute("Posts"));
            //var deserializedPosts = (PostDto[])serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var xDoc = XDocument.Parse(xmlString);
            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();

            var validPosts = new List<Post>();

            foreach (var e in elements)
            {

                var caption = e.Element("caption").Value;
                var user = e.Element("user").Value;
                var picture = e.Element("picture").Value;

                if (string.IsNullOrWhiteSpace(caption) ||
                                   string.IsNullOrWhiteSpace(user) ||
                                   string.IsNullOrWhiteSpace(picture))
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var existingUser = context.Users.SingleOrDefault(u => u.Username == user);
                var existingPicture = context.Pictures.SingleOrDefault(p => p.Path == picture);

                if (existingUser == null || existingPicture == null)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var post = new Post()
                {
                    Caption = caption,
                    User = existingUser,
                    Picture = existingPicture
                };
                //var userExists = validPosts.Any(p => p.User == user);

                validPosts.Add(post);
                sb.AppendLine(String.Format(successMsg, $"Post {caption}"));
            }

            context.Posts.AddRange(validPosts);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);
            var elements = xDoc.Root.Elements();
            var sb = new StringBuilder();
            var comments = new List<Comment>();

            foreach (var e in elements)
            {
                var content = e.Element("content")?.Value;
                var user = e.Element("user")?.Value;
                var postIdStr = e.Element("post")?.Attribute("id")?.Value;

                if (string.IsNullOrWhiteSpace(content) ||
                    string.IsNullOrWhiteSpace(user) ||
                    string.IsNullOrWhiteSpace(postIdStr))
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                int parsedId;
                bool isParsed = int.TryParse(postIdStr, out parsedId);

                if (!isParsed)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                int? userId = context.Users.SingleOrDefault(u => u.Username == user)?.Id;
                int? postId = context.Posts.SingleOrDefault(p => p.Id == parsedId)?.Id;

                if (userId == null || postId == null)
                {
                    sb.AppendLine(errorMsg);
                    continue;
                }

                var comment = new Comment
                {
                    UserId = userId.Value,
                    PostId = postId.Value,
                    Content = content
                };

                comments.Add(comment);
                sb.AppendLine(string.Format(successMsg, $"Comment {content}"));
            }

            context.Comments.AddRange(comments);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return isValid;
        }
    }
}
