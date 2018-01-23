using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Instagraph.Models
{
    public class User
    {
        public User()
        {
            this.Followers = new HashSet<UserFollower>();
            this.UsersFollowing = new HashSet<UserFollower>();
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }

  
        [MaxLength(30)]
        [Required]
        public string Username { get; set; }

        [MaxLength(20)]
        [Required]
        public string Password { get; set; }

        public int ProfilePictureId { get; set; }

        public Picture ProfilePicture { get; set; }

        public ICollection<UserFollower> Followers { get; set; }
        public ICollection<UserFollower> UsersFollowing { get; set; }
        public ICollection <Post>Posts { get; set; }
        public ICollection<Comment>Comments { get; set; }

    }
}