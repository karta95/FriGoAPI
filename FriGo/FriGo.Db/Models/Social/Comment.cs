﻿using FriGo.Db.Models.Recipes;

namespace FriGo.Db.Models.Social
{
    public class Comment : Entity
    {
        public string Text { get; set; }

        public virtual User User { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}