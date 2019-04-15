using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBoardApiServer.Models
{
    public class Board
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public List<BoardToUser> Members { get; set; } = new List<BoardToUser>();
        public User Owner { get; set; }
    }
}
