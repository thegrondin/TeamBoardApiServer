using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBoardApiServer.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public List<BoardToUser> Boards { get; set; } = new List<BoardToUser>();
    }
}
