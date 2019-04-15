using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBoardApiServer.Models
{
    public class BoardToUser
    {
        public Guid BoardId { get; set; }
        public Board Board { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
